using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;
namespace Domain.Interface
{
    public interface IInscriptionRepository
    {
        Task<IEnumerable<Inscription>> GetAll();
        Task<Inscription?> GetById(Guid id);
        Task<IEnumerable<Inscription>> GetByClassId(Guid classId);
        Task<Inscription?> GetByUserAndClass(Guid userId, Guid classId);
        Task Add(Inscription inscription);
        Task Save();
    }
}
