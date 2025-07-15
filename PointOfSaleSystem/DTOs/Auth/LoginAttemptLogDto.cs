namespace PointOfSaleSystem.DTOs.Auth
{
    public class LoginAttemptLogDto
    {
        public int Id { get; set; }

        public string UsernameOrEmail { get; set; } = string.Empty;
        public DateTime AttemptedAt { get; set; }
        public bool WasSuccessful { get; set; }

        public string? IPAddress { get; set; }
        public string? TerminalName { get; set; }
        public string? FailureReason { get; set; }
    }
}
