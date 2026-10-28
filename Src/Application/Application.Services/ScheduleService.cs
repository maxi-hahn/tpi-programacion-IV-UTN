using Application.Exceptions;
using Application.Interfaces;
using System.Text.Json;
using Domain.Entity;
using Domain.Interface;

namespace Application.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IInscriptionRepository _inscriptionRepo;
        private readonly IScheduleRepository _repository;
        private readonly IUserRepository _userRepo;
        private readonly INotificationService _notificationService;
        private readonly IClassRepository _classRepo;

        private async Task<bool> HasOverlap(
            Guid classId,
            Day day,
            TimeOnly startTime,
            TimeOnly endTime,
            Guid? scheduleIdToIgnore = null)
        {
            var schedules = await _repository.GetByClassId(classId);

            return schedules.Any(s =>
                s.DayOfWeek == day &&
                (!scheduleIdToIgnore.HasValue || s.Id != scheduleIdToIgnore.Value) &&
                startTime < s.EndTime &&
                s.StartTime < endTime);
        }

        public ScheduleService(
            IScheduleRepository repository,
            IInscriptionRepository inscriptionRepo,
            IUserRepository userRepo,
            INotificationService notificationService,
            IClassRepository classRepo)
        {
            _repository = repository;
            _inscriptionRepo = inscriptionRepo;
            _userRepo = userRepo;
            _notificationService = notificationService;
            _classRepo = classRepo;
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

        public async Task<List<Schedule>> GetAll()
        {
            return (List<Schedule>)await _repository.GetAll();
        }

        public async Task<Schedule?> GetById(Guid id)
        {
            var schedule = await _repository.GetById(id);
            if (schedule == null)
                throw new NotFoundException("Schedule not found");

            return schedule;
        }

        public async Task<Schedule> Create(Schedule schedule)
        {

            if (schedule.EndTime <= schedule.StartTime)
                throw new BadRequestException("EndTime must be greater than StartTime.");

            var hasOverlap = await HasOverlap(
                schedule.Id_Class,
                schedule.DayOfWeek,
                schedule.StartTime,
                schedule.EndTime);

            if (hasOverlap)
                throw new ConflictException(
                    "The schedule overlaps with another schedule of the same class.");

            var created = await _repository.Create(schedule);

            var gymClass = await _classRepo.GetById(schedule.Id_Class);
            var className = gymClass?.Name ?? "la clase";

            var jsonData = JsonSerializer.Serialize(new
            {
                className = className,
                day = schedule.DayOfWeek.ToString(),
                startTime = schedule.StartTime.ToString("HH:mm"),
                endTime = schedule.EndTime.ToString("HH:mm"),
                action = "created"
            });

            await NotifyAllClients(
                "Horario agregado",
                "Se agregó un nuevo horario.",
                "ScheduleUpdated",
                jsonData);

            return created;
        }

        public async Task<bool> Update(Guid id, Schedule updatedSchedule)
        {
            var schedule = await _repository.GetById(id);

            if (schedule == null)
                throw new NotFoundException("Schedule not found");

            if (updatedSchedule.EndTime <= updatedSchedule.StartTime)
                throw new BadRequestException(
                    "EndTime must be greater than StartTime.");

            var hasOverlap = await HasOverlap(
                schedule.Id_Class,
                updatedSchedule.DayOfWeek,
                updatedSchedule.StartTime,
                updatedSchedule.EndTime,
                id);

            if (hasOverlap)
                throw new ConflictException(
                    "The schedule overlaps with another schedule of the same class.");

            var result = await _repository.Update(id, updatedSchedule);

            var gymClass = await _classRepo.GetById(schedule.Id_Class);
            var className = gymClass?.Name ?? "la clase";

            var jsonData = JsonSerializer.Serialize(new
            {
                className = className,
                day = updatedSchedule.DayOfWeek.ToString(),
                startTime = updatedSchedule.StartTime.ToString("HH:mm"),
                endTime = updatedSchedule.EndTime.ToString("HH:mm"),
                action = "updated"
            });

            await NotifyAllClients(
                "Horario modificado",
                "El horario ha sido modificado.",
                "ScheduleUpdated",
                jsonData);

            return result;
        }
        public async Task<bool> UpdateStatus(Guid id, bool isActive)
        {
            var schedule = await _repository.GetById(id);

            if (schedule == null)
                throw new NotFoundException("Schedule not found");

            schedule.IsActive = isActive;

            var result = await _repository.Update(id, schedule);

            var gymClass = await _classRepo.GetById(schedule.Id_Class);
            var className = gymClass?.Name ?? "la clase";

            var jsonData = JsonSerializer.Serialize(new
            {
                className = className,
                day = schedule.DayOfWeek.ToString(),
                startTime = schedule.StartTime.ToString("HH:mm"),
                endTime = schedule.EndTime.ToString("HH:mm"),
                isActive = isActive,
                action = isActive ? "enabled" : "disabled"
            });

            await NotifyAllClients(
                "Horario modificado",
                "El horario ha sido modificado.",
                "ScheduleUpdated",
                jsonData);

            return result;
        }
        public async Task<bool> Delete(Guid id)
        {
            var schedule = await _repository.GetById(id);

            if (schedule == null)
                throw new NotFoundException("Schedule not found");

            var hasActiveInscriptions = await _inscriptionRepo.ExistsByScheduleId(id);

            if (hasActiveInscriptions)
                throw new ConflictException(
                    "No se puede eliminar un horario con inscripciones activas.");

            var gymClass = await _classRepo.GetById(schedule.Id_Class);
            var className = gymClass?.Name ?? "la clase";

            // Soft delete permanente
            schedule.IsDeleted = true;
            schedule.IsActive = false;

            await _repository.Update(id, schedule);
            await _repository.Save();

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

            return true;
        }
    }
}