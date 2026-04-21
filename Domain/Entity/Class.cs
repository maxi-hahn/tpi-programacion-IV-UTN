using System;
using System.Collections.Generic;
using System.Text;
using static Domain.Entity.Time;


namespace Domain.Entity
{
    public class Class
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Max_Users { get; set; }
        public List<HorarioClase> Horarios { get; set; } = new();

    }

    
}
