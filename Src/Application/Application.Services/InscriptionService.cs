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
        private readonly IPlanRepository _planRepo;
        private readonly IScheduleRepository _scheduleRepo;

        public InscriptionService(
            IInscriptionRepository inscriptionRepo,
            IClassRepository classRepo,
            IUserRepository userRepo,
            IPlanRepository planRepo,
            IScheduleRepository scheduleRepo)
        {
            _inscriptionRepo = inscriptionRepo;
            _classRepo = classRepo;
            _userRepo = userRepo;
            _planRepo = planRepo;
            _scheduleRepo = scheduleRepo;
        }

        public async Task<InscriptionResult> Inscribe(Guid userId, InscriptionRequest request)
        {
            // 1. Obtener usuario
            var user = await _userRepo.GetById(userId);

            if (user == null || user is not Client client)
                return new InscriptionResult
                {
                    Success = false,
                    code = "USER_NOT_FOUND",
                    ErrorMessage = "El usuario no existe o no es un cliente."
                };

            // 2. Obtener horario
            var selectedSchedule = await _scheduleRepo.GetById(request.ScheduleId);

            if (selectedSchedule == null)
                return new InscriptionResult
                {
                    Success = false,
                    code = "SCHEDULE_NOT_FOUND",
                    ErrorMessage = "El horario no existe."
                };

            if (!selectedSchedule.IsActive)
                return new InscriptionResult
                {
                    Success = false,
                    code = "SCHEDULE_INACTIVE",
                    ErrorMessage = "El horario está temporalmente deshabilitado."
                };

            // 3. Obtener clase
            var gymClass = await _classRepo.GetById(selectedSchedule.Id_Class);
          
            if (!gymClass.IsActive)
            {
                return new InscriptionResult
                {
                    Success = false,
                    code = "CLASS_DISABLED",
                    ErrorMessage = "La clase está momentáneamente deshabilitada."
                };
            }

            if (gymClass == null)
                return new InscriptionResult
                {
                    Success = false,
                    code = "CLASS_NOT_FOUND",
                    ErrorMessage = "La clase asociada al horario no existe."
                };

            if (!selectedSchedule.IsActive)
            {
                return new InscriptionResult
                {
                    Success = false,
                    code = "SCHEDULE_DISABLED",
                    ErrorMessage = "El horario está momentáneamente deshabilitado."
                };
            }
            // 4. Validar superposición
            var activeInscriptions = await _inscriptionRepo.GetByUserId(userId);

            foreach (var inscription in activeInscriptions.Where(i => i.IsActive))
            {
                var existingSchedule = inscription.Schedule;

                if (existingSchedule == null)
                    continue;

                bool overlaps =
                    selectedSchedule.DayOfWeek == existingSchedule.DayOfWeek &&
                    selectedSchedule.StartTime < existingSchedule.EndTime &&
                    existingSchedule.StartTime < selectedSchedule.EndTime;

                if (overlaps)
                {
                    return new InscriptionResult
                    {
                        Success = false,
                        code = "SCHEDULE_OVERLAP",
                        ErrorMessage = "El horario se superpone con otra inscripción existente."
                    };
                }
            }

            // 5. Ya inscripto
            var existing = await _inscriptionRepo.GetByUserAndSchedule(userId, request.ScheduleId);

            if (existing != null && existing.IsActive)
                return new InscriptionResult
                {
                    Success = false,
                    code = "ALREADY_ENROLLED",
                    ErrorMessage = "El cliente ya está inscripto en este horario."
                };

            // 6. Cupos
            var inscriptions = await _inscriptionRepo.GetByScheduleId(request.ScheduleId);

            if (inscriptions.Count(i => i.IsActive) >= gymClass.Max_Users)
                return new InscriptionResult
                {
                    Success = false,
                    code = "CLASS_FULL",
                    ErrorMessage = "La clase no tiene cupos disponibles."
                };

            // 7. Plan
            if (client.Id_Plan == null)
                return new InscriptionResult
                {
                    Success = false,
                    code = "PLAN_REQUIRED",
                    ErrorMessage = "El cliente nunca tuvo un plan."
                };

            if (!client.IsActive)
                return new InscriptionResult
                {
                    Success = false,
                    code = "PLAN_INACTIVE",
                    ErrorMessage = "El cliente no tiene una suscripción activa."
                };

            var plan = await _planRepo.GetById(client.Id_Plan.Value);

            if (plan == null)
                return new InscriptionResult
                {
                    Success = false,
                    code = "PLAN_NOT_FOUND",
                    ErrorMessage = "El plan del cliente no existe."
                };

            if (!plan.IsUnlimited)
            {
                var activeCount = activeInscriptions.Count(i => i.IsActive);

                if (activeCount >= plan.Max_Class)
                    return new InscriptionResult
                    {
                        Success = false,
                        code = "PLAN_LIMIT_REACHED",
                        ErrorMessage = $"El cliente alcanzó el límite de clases de su plan ({plan.Max_Class})."
                    };
            }

            // 8. Email
            if (!client.EmailVerified)
                return new InscriptionResult
                {
                    Success = false,
                    code = "EMAIL_NOT_VERIFIED",
                    ErrorMessage = "El cliente debe tener el email verificado para poder inscribirse."
                };

            // 9. Crear inscripción
            var inscriptionToCreate = new Inscription
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ClassId = gymClass.Id,
                ScheduleId = request.ScheduleId,
                InscriptionDate = DateTime.UtcNow,
                IsActive = true
            };
            await _inscriptionRepo.Add(inscriptionToCreate);
            await _inscriptionRepo.Save();

            return new InscriptionResult
            {
                Success = true,
                code = "ENROLLMENT_SUCCESS",
                Data = inscriptionToCreate.ToInscriptionResponse()
            };
        }
        public async Task<InscriptionResult> Unsubscribe(Guid userId, Guid scheduleId)
        {
            var inscription = await _inscriptionRepo.GetByUserAndSchedule(userId, scheduleId);

            if (inscription == null)
            {
                return new InscriptionResult
                {
                    Success = false,
                    code = "NOT_ENROLLED",
                    ErrorMessage = "El cliente no está inscripto en este horario."
                };
            }

            await _inscriptionRepo.Unsubscribe(inscription);
            await _inscriptionRepo.Save();

            return new InscriptionResult
            {
                Success = true,
                Data = inscription.ToInscriptionResponse()
            };
        }
        public async Task<IEnumerable<MyInscriptionResponse>> GetMyInscriptions(Guid userId)
        {
            var inscriptions = await _inscriptionRepo.GetByUserIdWithClass(userId);
            return inscriptions.Select(i => i.ToMyInscriptionResponse());
        }
    }
}
    
