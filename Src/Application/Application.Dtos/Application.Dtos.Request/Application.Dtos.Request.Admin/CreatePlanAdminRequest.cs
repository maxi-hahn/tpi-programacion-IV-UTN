namespace Application.Dtos.Request.Admin
{
    public class CreatePlanAdminRequest
    {
        public string Name { get; set; } = string.Empty;
        public int Max_Class { get; set; }  // Corregido
        public float Value { get; set; }
        public bool IsUnlimited { get; set; } = false;  // Agregado
        public string Benefits { get; set; } = string.Empty;  // Agregado
    }
}