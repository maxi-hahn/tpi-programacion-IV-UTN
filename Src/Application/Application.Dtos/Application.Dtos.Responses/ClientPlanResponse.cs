namespace Application.Dtos.Responses
{
    public class ClientPlanResponse
    {
        public Guid? PlanId { get; set; }
        public string? PlanName { get; set; }
        public bool IsActive { get; set; }
        public DateTime? SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
    }
}
