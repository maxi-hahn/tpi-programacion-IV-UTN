using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity
{
    public class Plan
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public float Value { get; set; }

        public int Max_Class { get; set; }


    }
}
