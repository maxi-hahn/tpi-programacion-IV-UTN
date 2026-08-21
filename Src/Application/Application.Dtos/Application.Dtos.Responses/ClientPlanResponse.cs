public class ClientPlanResponse
{
    public Guid? PlanId { get; set; }
    public string? PlanName { get; set; }
    public float? PlanValue { get; set; }
    public int? PlanMaxClass { get; set; }
    public bool? PlanIsUnlimited { get; set; }
    public string? PlanBenefits { get; set; }
    public int? ClassesRemaining { get; set; }  // NUEVO
    public bool IsActive { get; set; }
    public DateTime? SubscriptionStartDate { get; set; }
    public DateTime? SubscriptionEndDate { get; set; }
}