using Domain.Entity.UsersChild;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public interface IClientService
    {
        Task<IEnumerable<Client>> GetAll();
        Task<Client?> GetById(Guid id);
        Task<Client> Create(Client client);
        Task<bool> Update(Guid id, Client client);
        Task<bool> Delete(Guid id);
    }
}
