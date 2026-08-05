namespace Application.Dtos.Responses
{
    public class UserListResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}