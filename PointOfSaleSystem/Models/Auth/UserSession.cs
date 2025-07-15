using System.ComponentModel.DataAnnotations.Schema;

namespace PointOfSaleSystem.Models.Auth
{
    public class UserSession
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }

        public DateTime LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }

        public string? TerminalName { get; set; }
        public string? IPAddress { get; set; }

        public ApplicationUser User { get; set; }
    }
}
