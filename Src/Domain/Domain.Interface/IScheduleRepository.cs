using Domain.Entity;
using Microsoft.VisualBasic;
namespace Domain.Interface
{
    public interface IScheduleRepository : IBaseRepository<Schedule>
    {

        Task<List<Schedule>> GetAll();
        Task<Schedule?> GetById(Guid id);
        Task<List<Schedule>> GetByClassId(Guid classId);
        Task<Schedule> Create(Schedule schedule);
        Task<bool> Update(Guid id, Schedule schedule);
        Task<bool> Delete(Guid id);

        Task Save();
    }
}
