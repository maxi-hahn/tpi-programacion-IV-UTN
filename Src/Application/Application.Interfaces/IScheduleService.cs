using Domain.Entity;

namespace Application.Interfaces
{
    public interface IScheduleService
    {
        Task<List<Schedule>> GetAll();
        Task<Schedule?> GetById(Guid id);
        Task<List<Schedule>> GetByClassId(Guid classId);
        Task<Schedule> Create(Schedule schedule);

        Task<bool> Update(Guid id, Schedule updatedSchedule);

        Task<bool> Delete(Guid id);
    }
}