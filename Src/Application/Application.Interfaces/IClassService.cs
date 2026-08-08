using Domain.Entity;

namespace Application.Interfaces
{
    public interface IClassService
    {
        Task<IEnumerable<Class>> GetAll();

        Task<Class?> GetById(Guid id);

        Task<Class> Create(Class gymClass);

        Task<bool> Update(Guid id, Class gymClass);
        Task<bool> UpdateStatus(Guid id, bool isActive);
        Task<bool> Delete(Guid id);
    }
}