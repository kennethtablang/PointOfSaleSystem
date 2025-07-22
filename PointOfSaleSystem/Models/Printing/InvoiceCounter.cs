using System.ComponentModel.DataAnnotations;

namespace PointOfSaleSystem.Models.Printing
{
    public class InvoiceCounter
    {
        public int Id { get; set; }

        [Required]
        public int CounterId { get; set; } // Counter that uses this invoice counter

        [Required]
        public long CurrentInvoiceNumber { get; set; } = 1000000000; // Starting from a BIR-registered base

        [Required]
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        // Navigation
        public Settings.Counter? Counter { get; set; }
    }
}
