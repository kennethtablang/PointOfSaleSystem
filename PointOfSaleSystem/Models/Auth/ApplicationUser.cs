using Microsoft.AspNetCore.Identity;
using PointOfSaleSystem.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PointOfSaleSystem.Models.Auth
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        [NotMapped]
        public string FullName =>
            string.Join(" ", new[] { FirstName, MiddleName, LastName }
                .Where(s => !string.IsNullOrWhiteSpace(s)));

        [Required]
        public UserRole Role { get; set; }

        public ICollection<UserSession>? Sessions { get; set; }
        public ICollection<SystemLog>? Logs { get; set; }
    }
}
