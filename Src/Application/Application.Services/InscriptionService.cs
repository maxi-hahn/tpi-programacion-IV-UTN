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

            // 3. Obtener clase (corregido: null check primero)
            var gymClass = await _classRepo.GetById(selectedSchedule.Id_Class);

            if (gymClass == null)
                return new InscriptionResult
                {
                    Success = false,
                    code = "CLASS_NOT_FOUND",
                    ErrorMessage = "La clase asociada al horario no existe."
                };

            if (!gymClass.IsActive)
                return new InscriptionResult
                {
                    Success = false,
                    code = "CLASS_DISABLED",
                    ErrorMessage = "La clase está momentáneamente deshabilitada."
                };

            // 4. Calcular próxima ocurrencia
            var classDate = GetNextOccurrence(selectedSchedule.DayOfWeek, selectedSchedule.StartTime);

            // NUEVO: Validar que la clase no haya pasado
            if (classDate < DateTime.Now)
            {
                return new InscriptionResult
                {
                    Success = false,
                    code = "CLASS_ALREADY_PASSED",
                    ErrorMessage = "No se puede inscribir a una clase que ya pasó."
                };
            }

            // 5. Validar superposición (solo para la misma fecha)
            var activeInscriptions = await _inscriptionRepo.GetByUserId(userId);

            foreach (var inscription in activeInscriptions.Where(i => i.IsActive))
            {
                var existingSchedule = inscription.Schedule;

                if (existingSchedule == null)
                    continue;

                // Solo verificar superposición si es la misma fecha de clase
                if (inscription.ClassDate.Date != classDate.Date)
                    continue;

                bool overlaps =
                    selectedSchedule.DayOfWeek == existingSchedule.DayOfWeek &&
                    selectedSchedule.StartTime < existingSchedule.EndTime &&
                    existingSchedule.StartTime < selectedSchedule.EndTime;

                if (overlaps)
                {
                    var overlappingClass = await _classRepo.GetById(existingSchedule.Id_Class);
                    var className = overlappingClass?.Name ?? "otra clase";

                    return new InscriptionResult
                    {
                        Success = false,
                        code = "SCHEDULE_OVERLAP",
                        ErrorMessage = $"El horario se superpone con {className} ({existingSchedule.DayOfWeek} {existingSchedule.StartTime}-{existingSchedule.EndTime})."
                    };
                }
            }

            // 6. Ya inscripto para esa fecha
            var existing = await _inscriptionRepo.GetByUserScheduleAndDate(userId, request.ScheduleId, classDate);

            if (existing != null)
                return new InscriptionResult
                {
                    Success = false,
                    code = "ALREADY_ENROLLED",
                    ErrorMessage = "El cliente ya está inscripto en este horario para la próxima clase."
                };

            // 7. Cupos de la clase por fecha
            var inscriptionsForDate = await _inscriptionRepo.GetByScheduleAndDate(request.ScheduleId, classDate);

            var enrolledForDate = inscriptionsForDate.Count();

            if (enrolledForDate >= gymClass.Max_Users)
                return new InscriptionResult
                {
                    Success = false,
                    code = "CLASS_FULL",
                    ErrorMessage = "La clase no tiene cupos disponibles para esta fecha."
                };

            // 8. Plan
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
                var periodStart = client.SubscriptionStartDate ?? DateTime.UtcNow.AddMonths(-1);
                var periodEnd = client.SubscriptionEndDate ?? DateTime.UtcNow;

                var classesUsedInPeriod = activeInscriptions.Count(i =>
                    i.IsConsumed &&  // Cambio: contar consumidas, no activas
                    i.InscriptionDate >= periodStart &&
                    i.InscriptionDate <= periodEnd);

                if (classesUsedInPeriod >= plan.Max_Class)
                    return new InscriptionResult
                    {
                        Success = false,
                        code = "PLAN_LIMIT_REACHED",
                        ErrorMessage = $"El cliente alcanzó el límite de clases de su plan ({plan.Max_Class})."
                    };
            }

            // 9. Email
            if (!client.EmailVerified)
                return new InscriptionResult
                {
                    Success = false,
                    code = "EMAIL_NOT_VERIFIED",
                    ErrorMessage = "El cliente debe tener el email verificado para poder inscribirse."
                };

            // 10. Crear inscripción con ClassDate
            var inscriptionToCreate = new Inscription
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ClassId = gymClass.Id,
                ScheduleId = request.ScheduleId,
                InscriptionDate = DateTime.UtcNow,
                ClassDate = classDate, // DateTime normal, se guarda bien en BD
                IsActive = true,
                IsConsumed = true  
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
            // Verificar si la clase ya pasó
            if (inscription.ClassDate < DateTime.Now)
            {
                return new InscriptionResult
                {
                    Success = false,
                    code = "CLASS_ALREADY_PASSED",
                    ErrorMessage = "No se puede cancelar una clase que ya pasó."
                };
            }

            // Calcular tiempo desde la inscripción
            var timeSinceEnrollment = DateTime.UtcNow - inscription.InscriptionDate;
            // Calcular días de anticipación
            var daysUntilClass = (inscription.ClassDate.Date - DateTime.Now.Date).Days;

            // Cancelar la inscripción
            inscription.IsActive = false;

            // Regla: Si cancela dentro de 30 minutos, recupera cupo
            if (timeSinceEnrollment.TotalMinutes <= 30)
            {
                inscription.IsConsumed = false;
            }
            // Regla: Más de 3 días de anticipación, recupera cupo
            else if (daysUntilClass > 3)
            {
                inscription.IsConsumed = false;
            }
            // Regla: 3 días o menos, pierde cupo
            else
            {
                inscription.IsConsumed = true;
            }

            await _inscriptionRepo.Update(inscription);
            await _inscriptionRepo.Save();

            return new InscriptionResult
            {
                Success = true,
                code = "UNSUBSCRIBE_SUCCESS",
                Data = inscription.ToInscriptionResponse()
            };
        }

        public async Task<IEnumerable<MyInscriptionResponse>> GetMyInscriptions(Guid userId)
        {
            var inscriptions = await _inscriptionRepo.GetByUserIdWithClass(userId);

            // Devolver TODAS las inscripciones (pasadas y futuras)
            // El frontend decidirá cómo mostrarlas
            return inscriptions.Select(i => i.ToMyInscriptionResponse());
        }

        public async Task<InscriptionResult> UnsubscribeUser(Guid adminUserId, Guid targetUserId, Guid scheduleId)
        {
            var targetUser = await _userRepo.GetById(targetUserId);
            if (targetUser == null)
            {
                return new InscriptionResult
                {
                    Success = false,
                    code = "USER_NOT_FOUND",
                    ErrorMessage = "El usuario no existe."
                };
            }

            var inscription = await _inscriptionRepo.GetByUserAndSchedule(targetUserId, scheduleId);

            if (inscription == null)
            {
                return new InscriptionResult
                {
                    Success = false,
                    code = "NOT_ENROLLED",
                    ErrorMessage = "El usuario no está inscripto en este horario."
                };
            }

            // El admin puede cancelar en cualquier momento
            var timeSinceEnrollment = DateTime.UtcNow - inscription.InscriptionDate;
            var daysUntilClass = (inscription.ClassDate.Date - DateTime.Now.Date).Days;

            inscription.IsActive = false;

            if (timeSinceEnrollment.TotalMinutes <= 30)
            {
                inscription.IsConsumed = false;
            }
            else if (daysUntilClass > 3)
            {
                inscription.IsConsumed = false;
            }
            else
            {
                inscription.IsConsumed = true;
            }

            await _inscriptionRepo.Update(inscription);
            await _inscriptionRepo.Save();

            return new InscriptionResult
            {
                Success = true,
                code = "UNSUBSCRIBE_SUCCESS",
                Data = inscription.ToInscriptionResponse()
            };
        }

        // Método privado para calcular la próxima ocurrencia
        private DateTime GetNextOccurrence(Day dayOfWeek, TimeOnly startTime)
        {
            var now = DateTime.Now;

            DayOfWeek targetDayOfWeek = dayOfWeek switch
            {
                Day.Lunes => DayOfWeek.Monday,
                Day.Martes => DayOfWeek.Tuesday,
                Day.Miercoles => DayOfWeek.Wednesday,
                Day.Jueves => DayOfWeek.Thursday,
                Day.Viernes => DayOfWeek.Friday,
                Day.Sabado => DayOfWeek.Saturday,
                Day.Domingo => DayOfWeek.Sunday,
                _ => DayOfWeek.Monday
            };

            var currentDay = now.DayOfWeek;
            var daysUntilTarget = ((int)targetDayOfWeek - (int)currentDay + 7) % 7;
            var nextDate = now.Date.AddDays(daysUntilTarget);

            if (daysUntilTarget == 0 && now.TimeOfDay > startTime.ToTimeSpan())
            {
                nextDate = nextDate.AddDays(7);
            }

            return nextDate.Add(startTime.ToTimeSpan());
        }
    }
}