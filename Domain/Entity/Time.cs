using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity
{
    public class Time
    {
        public class HorarioClase
        {
            public int Id { get; set; }
            public DayOfWeek Dia { get; set; } // Lunes, Martes, etc.

            // Usamos TimeOnly (C# 10+) o TimeSpan para la hora pura
            public TimeOnly HoraInicio { get; set; }
            public TimeOnly HoraFin { get; set; }

            // Relación con la disciplina
            public int DisciplinaId { get; set; }
        }
    }
}
