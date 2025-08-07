using PointOfSaleSystem.Enums;

namespace PointOfSaleSystem.DTOs.Auth
{
    public class UserReadDto
    {
        public string Id { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;

        public string FullName => string.Join(" ", new[] { FirstName, MiddleName, LastName }.Where(n => !string.IsNullOrWhiteSpace(n)));

        public string Email { get; set; } = string.Empty;

        public UserRole Role { get; set; }

        public bool IsActive { get; set; }

        public DateTime? DateCreated { get; set; }
    }
}