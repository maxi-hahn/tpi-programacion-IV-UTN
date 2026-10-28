using Application.Dtos.Request;
using Application.Dtos.Request.Admin;
using System.Text.Json;
using Application.Dtos.Responses;
using Application.Exceptions;
using Application.Interfaces;
using Application.Mapper;
using Domain.Entity;
using Domain.Interface;
using System.Numerics;
using System.Xml.Linq;

namespace Application.Services
{

    public class AdminService : UserService, IAdminService
    {
        private readonly IPlanRepository _planRepo;
        private readonly IClassRepository _repo;
        private readonly IScheduleRepository _scheduleRepo;
        private readonly IUserRepository _userRepo;
        private readonly IInscriptionRepository _inscriptionRepo;
        private readonly INotificationService _notificationService;

        public AdminService(
            IUserRepository repo,
            IPasswordHasherService hasher,
            IUserContext userContext,
            IClassRepository classRepo,
            IScheduleRepository scheduleRepo,
            IPlanRepository planRepo,
            IUserRepository userRepo,
            IInscriptionRepository inscriptionRepo,
            INotificationService notificationService)
            : base(repo, hasher, userContext)
        {
            _repo = classRepo;
            _scheduleRepo = scheduleRepo;
            _planRepo = planRepo;
            _userRepo = userRepo;
            _inscriptionRepo = inscriptionRepo;
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



        // -------------------------CRUD for Plan---------------------

        public async Task<Plan?> UpdatePlan(Guid id, CreatePlanAdminRequest request)
        {
            var plan_id = await _planRepo.GetById(id);

            if (plan_id == null)
                throw new NotFoundException("Plan not found");

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ValidationException("Plan name is required");
            if (request.Max_Class < 0 || request.Max_Class > 100)
                throw new ValidationException("Max classes must be between 0 and 100");
            if (request.Value <= 0)
                throw new ValidationException("Plan value must be greater than zero");

            plan_id.Name = request.Name;
            plan_id.Max_Class = request.Max_Class;
            plan_id.Value = request.Value;
            plan_id.IsUnlimited = request.IsUnlimited;
            plan_id.Benefits = request.Benefits ?? string.Empty;

            await _planRepo.Update(plan_id);
            await _planRepo.Save();

            var jsonData = JsonSerializer.Serialize(new
            {
                planName = plan_id.Name,
                planValue = plan_id.Value,
                action = "updated"
            });

            await NotifyAllClients(
                "Planes actualizados",
                "Los planes del gimnasio han sido actualizados. ¡Revisalos!",
                "PlanChanged",
                jsonData);

            return plan_id;
        }

        public async Task<Plan?> CreatePlan(CreatePlanAdminRequest request)
        {
            if (request == null)
                throw new BadRequestException("Request cannot be null");

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ValidationException("Plan name is required");

            if (request.Value <= 0)
                throw new ValidationException("Plan value must be greater than zero");

            if (request.Max_Class < 0)
                throw new ValidationException("Max classes cannot be negative");

            var plan = new Plan
            {
                Name = request.Name,
                Max_Class = request.Max_Class,
                Value = request.Value,
                IsUnlimited = request.IsUnlimited,
                Benefits = request.Benefits ?? string.Empty
            };

            await _planRepo.Add(plan);
            await _planRepo.Save();

            var jsonData = JsonSerializer.Serialize(new
            {
                planName = plan.Name,
                planValue = plan.Value,
                action = "created"
            });

            await NotifyAllClients(
                "Planes actualizados",
                "Los planes del gimnasio han sido actualizados. ¡Revisalos!",
                "PlanChanged",
                jsonData);

            return plan;
        }

        public async Task<Plan?> DeletePlan(Guid id)
        {
            var plan = await _planRepo.GetById(id);
            var clientsUsingPlan = await _userRepo.GetClientsByPlanId(id);
            if (plan == null)
                throw new NotFoundException("Plan not found");
            if (clientsUsingPlan.Any())
            {
                throw new ConflictException(
                    "Cannot delete a plan assigned to clients");
            }

            await _planRepo.Delete(plan);
            await _planRepo.Save();

            var jsonData = JsonSerializer.Serialize(new
            {
                planName = plan.Name,
                action = "deleted"
            });

            await NotifyAllClients(
                "Planes actualizados",
                "Los planes del gimnasio han sido actualizados. ¡Revisalos!",
                "PlanChanged",
                jsonData);

            return plan;
        }

        public async Task<IEnumerable<Plan>> GetPlan()
        {
            return await _planRepo.GetAll();
        }



        // ---------------CRUD for Class -------------------

        public async Task<IEnumerable<Class?>> UpdateClass(Guid id, CreateClassRequest request, List<CreteScheduleAdminRequest> scheduleRequests)
        {

            var gymClass = await _repo.GetById(id);
            if (gymClass == null)
                throw new NotFoundException("Class not found");
            if (request.Max_Users <= 0 || request.Max_Users >= 100)
                throw new ValidationException("Max users must be greater than zero and less than or equal to 100");

            gymClass.Name = request.Name ?? gymClass.Name;
            if (request.Max_Users != 0) gymClass.Max_Users = request.Max_Users;

            if (scheduleRequests != null)
            {
                gymClass.Schedules = (List<Schedule>)await UpdateSchedule(id, scheduleRequests);

            }

            await _repo.Save();

            return await _repo.GetAll(); ;
        }

        public async Task<IEnumerable<Class?>> DeleteClass(Guid id)
        {
            var gymClass = await _repo.GetById(id);
            if (gymClass == null)
                throw new NotFoundException("Class not found");

            var hasActiveInscriptions = await _inscriptionRepo.ExistsByClassId(id);

            if (hasActiveInscriptions)
            {
                throw new ConflictException("Cannot delete a class with active inscriptions");
            }

            // Soft delete permanente
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

            return await _repo.GetAll();
        }

        public async Task<Class?> CreteClass(CreateClassRequest request, List<CreteScheduleAdminRequest> scheduleRequests)
        {
            if (request == null)
                throw new BadRequestException("Request cannot be null");
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ValidationException("Class name is required");

            if (request.Max_Users <= 0)
                throw new ValidationException("Max users must be greater than zero");

            if (scheduleRequests == null || !scheduleRequests.Any())
                throw new ValidationException(
                    "At least one schedule is required");
            var schedules = new List<Schedule>();

            foreach (var scheduleRequest in scheduleRequests)
            {
                var schedule = await CreteSchedule(scheduleRequest);
                schedules.Add(schedule);
            }

            var clase = new Class
            {
                Name = request.Name,
                Max_Users = request.Max_Users,
                Schedules = schedules
            };

            await _repo.Add(clase);
            await _repo.Save();

            var jsonData = JsonSerializer.Serialize(new { className = clase.Name });

            await NotifyAllClients(
                "Nueva clase disponible",
                "Se agregó una nueva clase.",
                "NewClass",
                jsonData);

            return clase;
        }

        public async Task<IEnumerable<Class>> GetClass()
        {
            return await _repo.GetAll();
        }

        public async Task<ClassDetailResponse?> GetClassDetail(Guid id)
        {
            var gymClass = await _repo.GetById(id);
            if (gymClass == null)
                throw new NotFoundException("Class not found");

            var inscriptions = await _inscriptionRepo.GetByClassId(id);
            var activeInscriptions = inscriptions.Where(i => i.IsActive).ToList();

            var clients = new List<ClientInfoResponse>();
            foreach (var inscription in activeInscriptions)
            {
                var user = await _userRepo.GetById(inscription.UserId);
                if (user != null)
                {
                    clients.Add(new ClientInfoResponse
                    {
                        Name = user.Name,
                        Email = user.Email
                    });
                }
            }

            return gymClass.ToClassDetailResponse(activeInscriptions.Count, clients);
        }



        // -------------------------CRUD for Schedule---------------------

        public async Task<Schedule?> CreteSchedule(CreteScheduleAdminRequest request)
        {
            if (request == null)
                throw new BadRequestException("Request cannot be null");

            if (request.StartTime >= request.EndTime)
                throw new ValidationException("Start time must be before end time");

            var schedule = new Schedule
            {
                DayOfWeek = (Day)request.DayOfWeek,
                StartTime = request.StartTime,
                EndTime = request.EndTime,

            };
            return schedule;
        }

        public async Task<Schedule?> DeleteSchedule(Guid id)
        {
            var schedule = await _scheduleRepo.GetById(id);
            if (schedule == null)
                throw new NotFoundException("Schedule not found");

            var hasActiveInscriptions = await _inscriptionRepo.ExistsByScheduleId(id);

            if (hasActiveInscriptions)
            {
                throw new ConflictException("Cannot delete a schedule with active inscriptions");
            }

            var gymClass = await _repo.GetById(schedule.Id_Class);
            var className = gymClass?.Name ?? "la clase";

            // Soft delete
            schedule.IsActive = false;

            await _scheduleRepo.Update(id, schedule);
            await _scheduleRepo.Save();

            var jsonData = JsonSerializer.Serialize(new
            {
                className = className,
                day = schedule.DayOfWeek.ToString(),
                startTime = schedule.StartTime.ToString("HH:mm"),
                endTime = schedule.EndTime.ToString("HH:mm"),
                action = "deleted"
            });

            await NotifyAllClients(
                "Horario eliminado",
                "El horario ha sido eliminado.",
                "ScheduleDeleted",
                jsonData);

            return schedule;
        }


        public async Task<IEnumerable<Schedule?>> UpdateSchedule(Guid id, List<CreteScheduleAdminRequest> scheduleRequests)
        {
            var gymClass = await _repo.GetById(id);

            if (gymClass == null)
                throw new NotFoundException("Class not found");

            var schedules = new List<Schedule>();

            if (scheduleRequests.Any(s => s.StartTime >= s.EndTime))
            {
                throw new ValidationException("Start time must be before end time");
            }
            foreach (var schedule in scheduleRequests)
            {
                var pushOrigin = false;

                foreach (var gymClassSchedule in gymClass.Schedules)
                {
                    if (schedule.DayOfWeek == (int)gymClassSchedule.DayOfWeek && schedule.StartTime == gymClassSchedule.StartTime && schedule.EndTime == gymClassSchedule.EndTime)
                    {
                        pushOrigin = true;
                        schedules.Add(gymClassSchedule);
                        break;
                    }
                }
                if (!pushOrigin)
                {
                    schedules.Add(await CreteSchedule(schedule));
                }
            }
            return schedules;
        }

        // -------------------------Client Inscriptions---------------------

        public async Task<ClientInscriptionsResponse?> GetClientInscriptions(Guid clientId)
        {
            var user = await _userRepo.GetById(clientId);
            if (user == null || user is not Client client)
                throw new NotFoundException("Client not found");

            var inscriptions = await _inscriptionRepo.GetByUserIdWithClass(clientId);

            return new ClientInscriptionsResponse
            {
                ClientId = client.Id,
                ClientName = client.Name,
                ClientEmail = client.Email,
                Inscriptions = inscriptions.Select(i => i.ToMyInscriptionResponse()).ToList()
            };
        }
    }
}