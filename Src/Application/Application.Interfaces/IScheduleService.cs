using Domain.Entity;

namespace Application.Interfaces
{
    public interface IScheduleService
    {
        Task<List<Schedule>> GetAll();

        Task<Schedule?> GetById(Guid id);
        Task<Schedule> Create(Schedule schedule);

        Task<bool> Update(Guid id, Schedule updatedSchedule);
        Task<bool> UpdateStatus(Guid id, bool isActive);
        Task<bool> Delete(Guid id);

    }
}