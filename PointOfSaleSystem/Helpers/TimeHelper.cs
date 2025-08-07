namespace PointOfSaleSystem.Helpers
{
    public static class TimeHelper
    {
        private static readonly TimeZoneInfo PhTimeZone =
        // Windows: "Singapore Standard Time"; Linux: "Asia/Manila"
        TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila");

        public static DateTime NowPhilippines() =>
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, PhTimeZone);
    }
}
