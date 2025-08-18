namespace PointOfSaleSystem.DTOs.Auth
{
    public class SystemLogDto
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }

        public string Module { get; set; } = string.Empty;
        public string ActionType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string? DataBefore { get; set; }
        public string? DataAfter { get; set; }

        public string? IPAddress { get; set; }

        public string? UserId { get; set; }
        public string? PerformedBy { get; set; }

        // New fields to match the model
        public string? ReferenceType { get; set; }
        public int? ReferenceId { get; set; }

        public string? LogLevel { get; set; } // e.g., "INFO", "WARN", "ERROR"
    }
}
