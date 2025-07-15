namespace PointOfSaleSystem.Models.Auth
{
    public class LoginAttemptLog
    {
        public int Id { get; set; }

        public string UsernameOrEmail { get; set; } = string.Empty;
        public DateTime AttemptedAt { get; set; } = DateTime.Now;
        public bool WasSuccessful { get; set; }

        public string? IPAddress { get; set; }
        public string? TerminalName { get; set; }
        public string? FailureReason { get; set; } // optional reason for failed attempt
    }
}
