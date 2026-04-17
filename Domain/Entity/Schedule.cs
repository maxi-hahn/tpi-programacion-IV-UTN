using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity
{
    public class Schedule
    {
        public int Id { get; set; }
        public enum Day{Lunes, Martes, Miercoles, Jueves, Viernes, Sabado, Domingo} 
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int Id_Class { get; set; }

    }
}
