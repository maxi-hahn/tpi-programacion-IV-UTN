namespace Application.Dtos.Responses
{
    public class PlanResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public float Value { get; set; }
        public int Max_Class { get; set; }
        public bool IsUnlimited { get; set; }
        public string Benefits { get; set; } = string.Empty;  
    }
}