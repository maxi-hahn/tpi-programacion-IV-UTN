using Application.Dtos.Responses;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Mapper
{
    public static class PlanMapper
    {
        public static PlanResponse ToPlanResponse(this Plan plan)
        {
            return new PlanResponse
            {
                Id = plan.Id,
                Name = plan.Name,
                Value = plan.Value,
                Max_Class = plan.Max_Class,
                IsUnlimited = plan.IsUnlimited
            };
        }
    }
}