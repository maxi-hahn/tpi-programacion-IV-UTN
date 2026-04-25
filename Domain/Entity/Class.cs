using System;
using System.Collections.Generic;
using System.Text;
using static Domain.Entity.Schedule;


namespace Domain.Entity
{
    public class Class
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Max_Users { get; set; }
        public List<Schedule> schedules { get; set; } = new();

    }

    
}
