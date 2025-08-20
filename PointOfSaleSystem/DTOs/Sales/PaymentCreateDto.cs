using PointOfSaleSystem.Enums;
using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.DTOs.Sales
{
    public class PaymentCreateDto
    {
        [Required]
        public PaymentMethod Method { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public string? ReferenceNumber { get; set; }

        // optional: server will set PaymentDate if omitted
        public DateTime? PaymentDate { get; set; }

        // amount of change given back to customer (cash)
        public decimal? ChangeAmount { get; set; }

        // Optional: user id of performer. Server can override with authenticated user.
        public string? UserId { get; set; }

        [MaxLength(50)]
        public string? Terminal { get; set; }
    }

    public class PaymentReadDto
    {
        public int Id { get; set; }
        public PaymentMethod Method { get; set; }
        public decimal Amount { get; set; }
        public string? ReferenceNumber { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? PerformedByUserId { get; set; }
        public string? PerformedByUserName { get; set; }
        public decimal? ChangeAmount { get; set; }
        public string? Terminal { get; set; }
        public PaymentStatus Status { get; set; }
    }
}
