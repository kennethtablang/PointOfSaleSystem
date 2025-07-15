using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.DTOs.Auth
{
    public class AdminResetPasswordDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
