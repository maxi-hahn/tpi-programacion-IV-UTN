using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dtos.Responses
{
    public class MyInscriptionResponse
    {
        public Guid InscriptionId { get; set; }
        public Guid ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public List<ScheduleResponse> Schedules { get; set; } = new();
        public DateTime InscriptionDate { get; set; }
    }
}
