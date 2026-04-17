using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interface
{
    public interface IPlanRepository
    {
        List<Plan> GetAll();
    }
}
