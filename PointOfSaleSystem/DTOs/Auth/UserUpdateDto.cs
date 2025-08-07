using PointOfSaleSystem.Enums;
using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.DTOs.Auth
{
    public class UserUpdateDto
    {
        [Required]
        public string Id { get; set; }

        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }

        public bool? IsActive { get; set; }

        public UserRole? Role { get; set; }
    }
}
