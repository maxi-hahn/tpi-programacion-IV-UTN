using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity.UsersChild;
using Domain.Entity;

namespace Domain.Interface.UsersInterface
{
    public interface IClientRepository
    {
        List<Client> GetAll();
    }
}
