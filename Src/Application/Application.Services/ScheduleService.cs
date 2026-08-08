using Application.Exceptions;
using Application.Interfaces;
using Domain.Entity;
using Domain.Interface;

namespace Application.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IInscriptionRepository _inscriptionRepo;
        private readonly IScheduleRepository _repository;
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
            IInscriptionRepository inscriptionRepo)
        {
            _repository = repository;
            _inscriptionRepo = inscriptionRepo;
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

            return await _repository.Create(schedule);
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

            return await _repository.Update(id, updatedSchedule);
        }
        public async Task<bool> UpdateStatus(Guid id, bool isActive)
        {
            var schedule = await _repository.GetById(id);

            if (schedule == null)
                throw new NotFoundException("Schedule not found");

            schedule.IsActive = isActive;

            return await _repository.Update(id, schedule);
        }
        public async Task<bool> Delete(Guid id)
        {
            var schedule = await _repository.GetById(id);

            if (schedule == null)
                throw new NotFoundException("Schedule not found");

            var hasActiveInscriptions =
                await _inscriptionRepo.ExistsByScheduleId(id);

            if (hasActiveInscriptions)
                throw new ConflictException(
                    "Cannot delete a schedule with registered users.");

            return await _repository.Delete(id);
        }
    }
}