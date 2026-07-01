using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.Entity
{
    public class Client : User
    {
        public Guid? Id_Plan { get; set; }

        [ForeignKey("Id_Plan")]
        public Plan? Plan { get; set; }
        public DateTime? SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }

    }
}
