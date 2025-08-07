using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.DTOs.Settings
{
    public class CounterReadDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = default!;

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public string? TerminalIdentifier { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }

    public class CounterCreateDto
    {
        [Required, MaxLength(50)]
        public string Name { get; set; } = default!;

        [MaxLength(100)]
        public string? Description { get; set; }

        public string? TerminalIdentifier { get; set; }
    }

    public class CounterUpdateDto
    {
        [Required, MaxLength(50)]
        public string Name { get; set; } = default!;

        [MaxLength(100)]
        public string? Description { get; set; }

        public string? TerminalIdentifier { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}
