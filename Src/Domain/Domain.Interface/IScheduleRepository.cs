using Domain.Entity;
namespace Domain.Interface
{
    public interface IScheduleRepository : IBaseRepository<Schedule>
    {
        Task<Schedule> Create(Schedule schedule);
        Task<IEnumerable<Schedule>> GetByClassId(Guid classId);
        Task<bool> Update(Guid id, Schedule schedule);
        Task<bool> Delete(Guid id);
    }
}
