namespace PointOfSaleSystem.DTOs.Auth
{
    public class UserSessionDto
    {
        public int Id { get; set; }

        public DateTime LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }

        public string? TerminalName { get; set; }
        public string? IPAddress { get; set; }

        public int UserId { get; set; }
        public string? UserFullName { get; set; }
    }
}
