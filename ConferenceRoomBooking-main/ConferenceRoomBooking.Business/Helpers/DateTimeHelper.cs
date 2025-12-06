namespace ConferenceRoomBooking.Business.Helpers
{
    public static class DateTimeHelper
    {
        private static readonly TimeZoneInfo IstTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        /// <summary>
        /// Converts UTC DateTime to IST
        /// </summary>
        public static DateTime ConvertUtcToIst(DateTime utcDateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, IstTimeZone);
        }

        /// <summary>
        /// Converts IST DateTime to UTC
        /// </summary>
        public static DateTime ConvertIstToUtc(DateTime istDateTime)
        {
            // Ensure the DateTime is treated as IST
            var istTime = DateTime.SpecifyKind(istDateTime, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(istTime, IstTimeZone);
        }

        /// <summary>
        /// Gets current IST time
        /// </summary>
        public static DateTime GetCurrentIstTime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IstTimeZone);
        }

        /// <summary>
        /// Gets current UTC time
        /// </summary>
        public static DateTime GetCurrentUtcTime()
        {
            return DateTime.UtcNow;
        }

        /// <summary>
        /// Converts DateTime to UTC for storage (handles different DateTime kinds)
        /// </summary>
        public static DateTime ConvertToUtcForStorage(DateTime dateTime)
        {
            // If already UTC, return as is
            if (dateTime.Kind == DateTimeKind.Utc)
            {
                return dateTime;
            }
            
            // If local, convert to UTC
            if (dateTime.Kind == DateTimeKind.Local)
            {
                return dateTime.ToUniversalTime();
            }
            
            // If unspecified, treat as IST and convert to UTC
            var istTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
            var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(istTime, IstTimeZone);
            return DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
        }
        
        /// <summary>
        /// Converts IST DateTime to UTC and ensures it's marked as UTC
        /// </summary>
        public static DateTime ConvertIstToUtcForStorage(DateTime istDateTime)
        {
            var istTime = DateTime.SpecifyKind(istDateTime, DateTimeKind.Unspecified);
            var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(istTime, IstTimeZone);
            return DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
        }

        /// <summary>
        /// Checks if a booking time is within notification window (IST comparison)
        /// </summary>
        public static bool IsWithinNotificationWindow(DateTime bookingStartTimeUtc, int minutesBefore)
        {
            var currentIst = GetCurrentIstTime();
            var bookingStartIst = ConvertUtcToIst(bookingStartTimeUtc);
            var notificationTime = bookingStartIst.AddMinutes(-minutesBefore);
            
            return currentIst >= notificationTime && currentIst <= bookingStartIst;
        }

        /// <summary>
        /// Checks if booking is overdue (IST comparison)
        /// </summary>
        public static bool IsBookingOverdue(DateTime bookingEndTimeUtc)
        {
            var currentIst = GetCurrentIstTime();
            var bookingEndIst = ConvertUtcToIst(bookingEndTimeUtc);
            
            return currentIst > bookingEndIst;
        }
    }
}








