namespace Application.Dtos.Responses
{
    public class ClientPlanResponse
    {
        public Guid? PlanId { get; set; }
        public string? PlanName { get; set; }
        public float? PlanValue { get; set; }
        public int? PlanMaxClass { get; set; }
        public bool? PlanIsUnlimited { get; set; }
        public bool IsActive { get; set; }
        public DateTime? SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
    }
}