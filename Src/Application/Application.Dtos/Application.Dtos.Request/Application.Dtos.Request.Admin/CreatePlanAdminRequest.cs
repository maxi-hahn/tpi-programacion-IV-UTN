using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dtos.Request.Admin
{
    public class CreatePlanAdminRequest
    {
        public string Name { get; set; } = string.Empty;

        public int Max_Clases { get; set; }

        public float Value { get; set; }

    }
}


