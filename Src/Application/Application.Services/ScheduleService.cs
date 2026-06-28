using Application.Exceptions;
using Application.Interfaces;
using Domain.Entity;
using Domain.Interface;

namespace Application.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _repository;

        public ScheduleService(IScheduleRepository repository)
        {
            _repository = repository;
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

            return await _repository.Create(schedule);
        }

        public async Task<bool> Update(Guid id, Schedule updatedSchedule)
        {
            var schedule = await _repository.GetById(id); 
            if (schedule == null)
                throw new NotFoundException("Schedule not found");
            if (updatedSchedule.EndTime <= updatedSchedule.StartTime)
                throw new BadRequestException("EndTime must be greater than StartTime.");
           
            return await _repository.Update(id, updatedSchedule);
        }

        public async Task<bool> Delete(Guid id)
        {
            var schedule = await _repository.GetById(id);

            if (schedule == null)
                throw new NotFoundException("Schedule not found");

            return await _repository.Delete(id);
        }
    }
}