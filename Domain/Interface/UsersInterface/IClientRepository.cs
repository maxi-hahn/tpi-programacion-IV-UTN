using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity.UsersChild;
using Domain.Entity;

namespace Domain.Interface.UsersInterface
{
    public interface IClientRepository
    {

        Task<IEnumerable<Client>> GetAll();
        Task<Client?> GetById(Guid id);
        Task Add(Client client);
        Task Update(Client client);
        Task Delete(Client client);
        Task Save();
    }
}
