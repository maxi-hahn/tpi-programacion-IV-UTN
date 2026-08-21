namespace Domain.Entity
{
    public class Plan
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public float Value { get; set; }

        public int Max_Class { get; set; }

        public bool IsUnlimited { get; set; } = false;
        public string Benefits { get; set; } = string.Empty; // Separados por coma o JSON

    }
}
