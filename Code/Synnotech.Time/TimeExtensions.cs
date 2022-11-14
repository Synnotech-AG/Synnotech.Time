using System;

namespace Synnotech.Time
{
    /// <summary>
    /// Provides extension methods for time-related functionality.
    /// </summary>
    public static class TimeExtensions
    {
        /// <summary>
        /// Calculates the time span from <paramref name="now" /> to the next day with the specified <paramref name="timeOfDay" />.
        /// </summary>
        /// <param name="now">The current time.</param>
        /// <param name="timeOfDay">The target time of day of tomorrow. The date part of this value will be ignored.</param>
        public static TimeSpan CalculateIntervalForSameTimeNextDay(this DateTime now, DateTime timeOfDay)
        {
            var tomorrow = now.AddDays(1.0);
            var targetDateTime = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, timeOfDay.Hour, timeOfDay.Minute, timeOfDay.Second, timeOfDay.Millisecond, timeOfDay.Kind);
            return targetDateTime.ToUniversalTime() - now.ToUniversalTime();
        }

        /// <summary>
        /// Calculates the time span from <paramref name="now"/> until the day time is <paramref name="timeOfDay"/> again.
        /// </summary>
        /// <remarks>
        /// Returns a time span of 24 hours if <paramref name="now"/> is exactly <paramref name="timeOfDay"/>.
        /// </remarks>
        /// <param name="now">The current time.</param>
        /// <param name="timeOfDay">The target time of the day. The date part of this value will be ignored.</param>
        public static TimeSpan CalculateIntervalForSameTime(this DateTime now, DateTime timeOfDay)
        {
            var timeOfToday = new DateTime(now.Year, now.Month, now.Day, timeOfDay.Hour, timeOfDay.Minute, timeOfDay.Second);
            return timeOfToday > now
                ? timeOfToday - now
                : now.CalculateIntervalForSameTimeNextDay(timeOfDay);
        }

        /// <summary>
        /// Tries to convert the specified time span to a date time instance. The hour, minute, second, and millisecond values will
        /// be extracted from the time span. The date part will be set to 0001-01-01 (because it is normally ignored by code that
        /// only relies on the time of day). The conversion will fail when <see cref="TimeSpan.Hours" /> is greater than or equal to 24
        /// or when the time span is less than <see cref="TimeSpan.Zero" />.
        /// </summary>
        /// <param name="timeSpan">The value to convert to a date time. The <see cref="TimeSpan.Days" /> property will be ignored.</param>
        /// <param name="dateTime">The resulting date time value when the conversion was successful.</param>
        /// <param name="kind">The kind of the resulting date time. The default value is <see cref="DateTimeKind.Local" />.</param>
        /// <returns>True if the conversion was successful, else false.</returns>
        public static bool TryConvertToTimeOfDay(this TimeSpan timeSpan, out DateTime dateTime, DateTimeKind kind = DateTimeKind.Local)
        {
            if (timeSpan.Hours >= 24 || timeSpan < TimeSpan.Zero)
            {
                dateTime = default;
                return false;
            }

            dateTime = new DateTime(1, 1, 1, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds, kind);
            return true;
        }

        /// <summary>
        /// Tries to convert the specified time span to a date time instance. The hour, minute, second, and millisecond values will
        /// be extracted from the time span. The date part will be set to 0001-01-01 (because it is normally ignored by code that
        /// only relies on the time of day). The conversion will fail when <see cref="TimeSpan.Hours" /> is greater than or equal to 24
        /// or when the time span is less than <see cref="TimeSpan.Zero" />. The kind value of the date time will be set to <see cref="DateTimeKind.Utc" />.
        /// </summary>
        /// <param name="timeSpan">The value to convert to a date time.</param>
        /// <param name="dateTime">The resulting date time value when the conversion was successful.</param>
        /// <returns>True if the conversion was successful, else false</returns>
        public static bool TryConvertToUtcTimeOfDay(this TimeSpan timeSpan, out DateTime dateTime) =>
            timeSpan.TryConvertToTimeOfDay(out dateTime, DateTimeKind.Utc);
    }
}