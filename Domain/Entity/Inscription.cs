using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity
{
    public class Inscription
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid ClassId { get; set; }

        public DateTime InscriptionDate { get; set; }

        public bool IsActive { get; set; }

    }
}
