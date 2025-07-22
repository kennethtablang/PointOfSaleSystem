using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.Models.Auth
{
    public class LoginAttemptLog
    {
        public int Id { get; set; }

        [Required]
        public string UsernameOrEmail { get; set; } = string.Empty;

        public DateTime AttemptedAt { get; set; } = DateTime.Now;

        public bool WasSuccessful { get; set; }

        public string? IPAddress { get; set; }

        public string? TerminalName { get; set; }

        [MaxLength(255)]
        public string? FailureReason { get; set; } // optional reason for failed attempt
    }
}
