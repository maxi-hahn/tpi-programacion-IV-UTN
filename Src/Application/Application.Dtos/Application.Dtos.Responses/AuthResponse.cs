namespace Application.Dtos.Responses
{
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;

        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public bool EmailVerified { get; set; }

        public string? PlanName { get; set; }
        public bool HasPlan { get; set; }
    }
}
