namespace Domain.Interface
{
    public interface IBaseRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();

        Task<T?> GetById(Guid id);

        Task Add(T entity);

        Task Update(T entity);

        Task Delete(T entity);

        Task Save();
    }
}