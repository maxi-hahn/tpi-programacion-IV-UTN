using Domain.Entity;
using Domain.Entity.UsersChild;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interface.UsersInterface
{
    public interface IAdminRepository
    {

        List<Admin> GetAll();

    }
}
