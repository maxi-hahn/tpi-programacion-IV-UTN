using Application.Application.Interfaces;
using Application.Dtos.Request;
using Application.Dtos.Responses;
using Application.Interfaces;
using Domain.Entity;
using Domain.Interface;

namespace Application.Services
{
    public class InscriptionService : IInscriptionService
    {
        private readonly IInscriptionRepository _inscriptionRepo;
        private readonly IClassRepository _classRepo;
        private readonly IUserRepository _userRepo;
        private readonly IScheduleRepository _scheduleRepo;

        public InscriptionService(
            IInscriptionRepository inscriptionRepo,
            IClassRepository classRepo,
            IUserRepository userRepo,
            IScheduleRepository scheduleRepo
            )
        {
            _inscriptionRepo = inscriptionRepo;
            _classRepo = classRepo;
            _userRepo = userRepo;
            _scheduleRepo = scheduleRepo;
        }

        public async Task<InscriptionResponse?> Inscribe(InscriptionRequest request)
        {
            // 1. Validar que el usuario existe y es un Client
            var user = await _userRepo.GetById(request.UserId);
            if (user == null || user is not Client)
                return null;

            // 2. Validar que la clase existe
            var gymClass = await _classRepo.GetById(request.ClassId);
            if (gymClass == null)
                return null;

            // 3. Validar que el usuario no esté ya inscripto
            var existing = await _inscriptionRepo.GetByUserAndClass(request.UserId, request.ClassId);
            if (existing != null && existing.IsActive)
                return null;

            // 4. Validar cupos disponibles
            var inscriptions = await _inscriptionRepo.GetByClassId(request.ClassId);
            var activeCount = inscriptions.Count(i => i.IsActive);
            if (activeCount >= gymClass.Max_Users)
                return null;

            // 5. Crear la inscripción
            var inscription = new Inscription
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                ClassId = request.ClassId,
                InscriptionDate = DateTime.UtcNow,
                IsActive = true
            };


            await _inscriptionRepo.Add(inscription);
            await _inscriptionRepo.Save();

            return new InscriptionResponse
            {
                Id = inscription.Id,
                UserId = inscription.UserId,
                ClassId = inscription.ClassId,
                InscriptionDate = inscription.InscriptionDate,
                IsActive = inscription.IsActive,
            };
        }
        public async Task<InscriptionResponse?> GetById(Guid id)
        {
            var inscription = await _inscriptionRepo.GetById(id);

            if (inscription == null)
                return null;

            return new InscriptionResponse
            {
                Id = inscription.Id,
                UserId = inscription.UserId,
                ClassId = inscription.ClassId,
                InscriptionDate = inscription.InscriptionDate,
                IsActive = inscription.IsActive
            };
        }
    }
}
