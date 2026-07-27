using Application.Exceptions;
using Application.Interfaces;
using Domain.Entity;
using Domain.Interface;

namespace Application.Services
{
    public class ClassService : IClassService
    {
        private readonly IClassRepository _repo;
        private readonly IInscriptionRepository _inscriptionRepo;
        public ClassService(IClassRepository repo, IInscriptionRepository inscriptionRepo)
        {
            _repo = repo;
            _inscriptionRepo = inscriptionRepo;
        }

        public async Task<IEnumerable<Class>> GetAll()
        {
            return await _repo.GetAll();
        }

        public async Task<Class?> GetById(Guid id)
        {
            var gymClass = await _repo.GetById(id);
            if (gymClass == null)
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

            return gymClass;
        }

        public async Task<bool> Update(Guid id, Class updatedClass)
        {
            var gymClass = await _repo.GetById(id);

            if (gymClass == null)
                throw new NotFoundException($"Class with ID {id} not found.");

            if (updatedClass.Max_Users <= 0)
                throw new BadRequestException("Max_Users must be a positive number.");

            if (string.IsNullOrWhiteSpace(updatedClass.Name))
                throw new BadRequestException("Name cannot be empty.");

            var currentInscriptions = await _inscriptionRepo.CountActiveByScheduleId(id);

            if (updatedClass.Max_Users < currentInscriptions)
                throw new ConflictException($"There are currently {currentInscriptions} registered users. Max_Users cannot be lower.");

            gymClass.Name = updatedClass.Name;
            gymClass.Max_Users = updatedClass.Max_Users;

            await _repo.Update(gymClass);
            await _repo.Save();

            return true;
        }

        public async Task<bool> Delete(Guid id)
        {
            var gymClass = await _repo.GetById(id);

            if (gymClass == null)
                throw new NotFoundException($"Class with ID {id} not found.");

            var hasInscriptions =
                await _inscriptionRepo.ExistsByScheduleId(id);

            if (hasInscriptions)
                throw new ConflictException("Cannot delete a class with registered users.");

            await _repo.Delete(gymClass);
            await _repo.Save();

            return true;
        }
    }
}