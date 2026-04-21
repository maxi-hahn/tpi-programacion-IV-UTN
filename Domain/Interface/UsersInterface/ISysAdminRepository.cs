using Domain.Entity.UsersChild;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interface.UsersInterface
{
    public interface ISysAdminRepository
    {
        List<SysAdmin> GetAll();  
    }
}
