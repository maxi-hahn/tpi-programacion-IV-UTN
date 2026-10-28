using Application.Exceptions;
using Application.Interfaces;
using System.Text.Json;
using Domain.Entity;
using Domain.Interface;

namespace Application.Services
{
    public class ClassService : IClassService
    {
        private readonly IClassRepository _repo;
        private readonly IInscriptionRepository _inscriptionRepo;
        private readonly IUserRepository _userRepo;
        private readonly INotificationService _notificationService;

        public ClassService(
            IClassRepository repo,
            IInscriptionRepository inscriptionRepo,
            IUserRepository userRepo,
            INotificationService notificationService)
        {
            _repo = repo;
            _inscriptionRepo = inscriptionRepo;
            _userRepo = userRepo;
            _notificationService = notificationService;
        }

        private async Task NotifyAllClients(string title, string message, string type, string? data = null)
        {
            var users = await _userRepo.GetAll();
            var clients = users.OfType<Client>();

            foreach (var client in clients)
            {
                await _notificationService.CreateNotification(client.Id, title, message, type, data);
            }
        }

        public async Task<IEnumerable<Class>> GetAll()
        {
            return await _repo.GetAll();
        }

        public async Task<Class?> GetById(Guid id)
        {
            var gymClass = await _repo.GetById(id);
            if (gymClass == null || gymClass.IsDeleted)
                throw new NotFoundException($"Class with ID {id} not found.");
            return gymClass;
        }

        public async Task<Class> Create(Class gymClass)
        {

            if (gymClass == null)
                throw new BadRequestException("Class cannot be null.");
            if (gymClass.Max_Users <=0 || gymClass.Max_Users >100)
                throw new BadRequestException("Max_Users must be a positive number between 1 and 100.");
            if (string.IsNullOrWhiteSpace(gymClass.Name))
                throw new BadRequestException("Name cannot be empty.");

            var classes = await _repo.GetAll();

            if (classes.Any(c => c.Name.ToLower() == gymClass.Name.ToLower()))
            {
                throw new ConflictException("A class with that name already exists");
            }
            gymClass.Id = Guid.NewGuid();
            await _repo.Add(gymClass);
            await _repo.Save();

            var jsonData = JsonSerializer.Serialize(new { className = gymClass.Name });

            await NotifyAllClients(
                "Nueva clase disponible",
                "Se agregó una nueva clase.",
                "NewClass",
                jsonData);

            return gymClass;
        }

        public async Task<bool> Update(Guid id, Class updatedClass)
        {
            var gymClass = await _repo.GetById(id);

            if (gymClass == null)
                throw new NotFoundException($"Class with ID {id} not found.");

            if (updatedClass.Max_Users <= 0)
                throw new BadRequestException("Max_Users must be a positive number.");

            if (!string.IsNullOrWhiteSpace(updatedClass.Name))
            {
                gymClass.Name = updatedClass.Name;
            }

            var currentInscriptions =
                await _inscriptionRepo.CountActiveByClassId(id);

            if (updatedClass.Max_Users < currentInscriptions)
                throw new ConflictException(
                    $"There are currently {currentInscriptions} registered users. Max_Users cannot be lower."
                );

            gymClass.Max_Users = updatedClass.Max_Users;

            await _repo.Update(gymClass);
            await _repo.Save();

            return true;
        }
        public async Task<bool> UpdateStatus(Guid id, bool isActive)
        {
            var gymClass = await _repo.GetById(id);

            if (gymClass == null)
                throw new NotFoundException($"Class with ID {id} not found.");

            gymClass.IsActive = isActive;

            await _repo.Update(gymClass);
            await _repo.Save();

            var jsonData = JsonSerializer.Serialize(new
            {
                className = gymClass.Name,
                isActive = isActive,
                action = isActive ? "enabled" : "disabled"
            });

            if (!isActive)
            {
                await NotifyAllClients(
                    "Clase deshabilitada",
                    "La clase ha sido deshabilitada.",
                    "ClassDisabled",
                    jsonData);
            }
            else
            {
                await NotifyAllClients(
                    "Clase habilitada",
                    "La clase ha sido habilitada.",
                    "ClassEnabled",
                    jsonData);
            }

            return true;
        }
        public async Task<bool> Delete(Guid id)
        {
            var gymClass = await _repo.GetById(id);

            if (gymClass == null)
                throw new NotFoundException($"Class with ID {id} not found.");

            var hasInscriptions = await _inscriptionRepo.ExistsByClassId(id);

            if (hasInscriptions)
                throw new ConflictException("Cannot delete a class with registered users.");

            // Soft delete en lugar de eliminar físicamente
            gymClass.IsDeleted = true;
            gymClass.IsActive = false;

            await _repo.Update(gymClass);
            await _repo.Save();

            var jsonData = JsonSerializer.Serialize(new { className = gymClass.Name });

            await NotifyAllClients(
                "Clase eliminada",
                "La clase ha sido eliminada.",
                "ClassDeleted",
                jsonData);

            return true;
        }
    }
}