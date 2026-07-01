using Application.Dtos.Request;
using Application.Dtos.Responses;
using Application.Interfaces;
using Application.Mapper;
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
        private readonly IPlanRepository _planRepo;

        public InscriptionService(
            IInscriptionRepository inscriptionRepo,
            IClassRepository classRepo,
            IUserRepository userRepo,
            IScheduleRepository scheduleRepo,
            IPlanRepository planRepo)
        {
            _inscriptionRepo = inscriptionRepo;
            _classRepo = classRepo;
            _userRepo = userRepo;
            _scheduleRepo = scheduleRepo;
            _planRepo = planRepo;
        }

        public async Task<InscriptionResult> Inscribe(InscriptionRequest request)
        {
            var user = await _userRepo.GetById(request.UserId);
            if (user == null || user is not Client client)
                return new InscriptionResult
                {
                    Success = false,
                    ErrorMessage = "El usuario no existe o no es un cliente."
                };

            // Validar que la clase existe
            var gymClass = await _classRepo.GetById(request.ClassId);
            if (gymClass == null)
                return new InscriptionResult
                {
                    Success = false,
                    ErrorMessage = "La clase no existe."
                };

            // Validar superposición de horarios
            var clientActiveInscriptions = await _inscriptionRepo.GetByUserId(request.UserId);

            foreach (var activeInscription in clientActiveInscriptions.Where(i => i.IsActive))
            {
                var otherClass = await _classRepo.GetById(activeInscription.ClassId);

                if (otherClass == null)
                    continue;

                foreach (var newSchedule in gymClass.Schedules)
                {
                    foreach (var existingSchedule in otherClass.Schedules)
                    {
                        if (newSchedule.DayOfWeek == existingSchedule.DayOfWeek &&
                            newSchedule.StartTime < existingSchedule.EndTime &&
                            existingSchedule.StartTime < newSchedule.EndTime)
                        {
                            return new InscriptionResult
                            {
                                Success = false,
                                ErrorMessage = $"El horario se superpone con la clase '{otherClass.Name}' a la que ya estás inscripto."
                            };
                        }
                    }
                }
            }

            // Validar que no esté ya inscripto
            var existing = await _inscriptionRepo.GetByUserAndClass(request.UserId, request.ClassId);

            if (existing != null && existing.IsActive)
                return new InscriptionResult
                {
                    Success = false,
                    ErrorMessage = "El cliente ya está inscripto en esta clase."
                };

            // Validar cupos
            var inscriptions = await _inscriptionRepo.GetByClassId(request.ClassId);
            var activeCount = inscriptions.Count(i => i.IsActive);

            if (activeCount >= gymClass.Max_Users)
                return new InscriptionResult
                {
                    Success = false,
                    ErrorMessage = "La clase no tiene cupos disponibles."
                };

            // Validar que tenga un plan
            if (client.Id_Plan == null)
                return new InscriptionResult
                {
                    Success = false,
                    ErrorMessage = "El cliente nunca tuvo un plan."
                };

            if (!client.IsActive)
                return new InscriptionResult
                {
                    Success = false,
                    ErrorMessage = "El cliente no tiene una suscripción activa."
                };

            // Validar límite del plan
            var plan = await _planRepo.GetById(client.Id_Plan.Value);

            if (plan == null)
                return new InscriptionResult
                {
                    Success = false,
                    ErrorMessage = "El plan del cliente no existe."
                };

            if (!plan.IsUnlimited)
            {
                var clientInscriptions = await _inscriptionRepo.GetByUserId(request.UserId);
                var clientActiveCount = clientInscriptions.Count(i => i.IsActive);

                if (clientActiveCount >= plan.Max_Class)
                    return new InscriptionResult
                    {
                        Success = false,
                        ErrorMessage = $"El cliente alcanzó el límite de clases de su plan ({plan.Max_Class})."
                    };
            }

            // Crear inscripción
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

            return new InscriptionResult
            {
                Success = true,
                Data = inscription.ToInscriptionResponse()
            };
        }

        public async Task<InscriptionResult> Unsubscribe(Guid userId, Guid classId)
        {
            // 1. Buscar la inscripción
            var inscription = await _inscriptionRepo.GetByUserAndClass(userId, classId);

            if (inscription == null || !inscription.IsActive)
                return new InscriptionResult { Success = false, ErrorMessage = "El cliente no está inscripto en esta clase." };

            // 2. Desactivar la inscripción
            await _inscriptionRepo.Unsubscribe(inscription);
            await _inscriptionRepo.Save();

            return new InscriptionResult
            {
                Success = true,
                Data = inscription.ToInscriptionResponse()
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

        public async Task<IEnumerable<MyInscriptionResponse>> GetMyInscriptions(Guid userId)
        {
            var inscriptions = await _inscriptionRepo.GetByUserIdWithClass(userId);
            return inscriptions.Select(i => i.ToMyInscriptionResponse());
        }
    }
}