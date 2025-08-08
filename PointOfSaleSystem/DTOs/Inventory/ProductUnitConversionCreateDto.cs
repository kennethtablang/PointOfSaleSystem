using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.DTOs.Inventory
{
    public class ProductUnitConversionCreateDto : IValidatableObject
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int FromUnitId { get; set; }

        [Required]
        public int ToUnitId { get; set; }

        [Required]
        public decimal ConversionRate { get; set; }

        [MaxLength(200)]
        public string? Notes { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FromUnitId == ToUnitId)
            {
                yield return new ValidationResult(
                    "FromUnitId and ToUnitId must be different.",
                    new[] { nameof(FromUnitId), nameof(ToUnitId) }
                );
            }

            if (ConversionRate <= 0)
            {
                yield return new ValidationResult(
                    "ConversionRate must be greater than 0.",
                    new[] { nameof(ConversionRate) }
                );
            }
        }
    }

    public class ProductUnitConversionUpdateDto : IValidatableObject
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int FromUnitId { get; set; }

        [Required]
        public int ToUnitId { get; set; }

        [Required]
        public decimal ConversionRate { get; set; }

        [MaxLength(200)]
        public string? Notes { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FromUnitId == ToUnitId)
            {
                yield return new ValidationResult(
                    "FromUnitId and ToUnitId must be different.",
                    new[] { nameof(FromUnitId), nameof(ToUnitId) }
                );
            }

            if (ConversionRate <= 0)
            {
                yield return new ValidationResult(
                    "ConversionRate must be greater than 0.",
                    new[] { nameof(ConversionRate) }
                );
            }
        }
    }

    public class ProductUnitConversionReadDto
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string? ProductName { get; set; }

        public int FromUnitId { get; set; }
        public string? FromUnitName { get; set; }

        public int ToUnitId { get; set; }
        public string? ToUnitName { get; set; }

        public decimal ConversionRate { get; set; }

        public string? Notes { get; set; }
    }
}
