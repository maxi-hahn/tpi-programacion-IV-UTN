using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;
namespace Domain.Interface
{
    public interface IInscriptionRepository
    {
        List<Inscription> GetAll();
    }
}
