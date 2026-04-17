using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Dni { get; set; }

        public int Id_Plan { get; set; }

        public bool IsActive { get; set; }

        public string Password { get; set; } = string.Empty;

    }
}
