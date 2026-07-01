using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Entity
{
    public class Schedule
    {

        //Cambiar a Guid
        public Guid Id { get; set; }

        //Modificar a lista
        public Day DayOfWeek { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        [ForeignKey("Class")]
        public Guid Id_Class { get; set; }

        public bool IsActive { get; set; } = true;
        [JsonIgnore]
        public Class? Class { get; set; }

        public Schedule() { }

    }



    public enum Day
    {
        Lunes,
        Martes,
        Miercoles,
        Jueves,
        Viernes,
        Sabado,
        Domingo
    }
}