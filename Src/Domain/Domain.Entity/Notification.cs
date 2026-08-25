namespace Domain.Entity
{
    public class Notification
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Discriminator used by the frontend to select icons.
        /// Values: ClassCompleted, PlanExpiring, PlanExpired, EnrollmentSuccess, EnrollmentCancelled
        /// </summary>
        public string Type { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }
}
