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

        public int? UserId { get; set; }
        public string? PerformedBy { get; set; }
    }
}
