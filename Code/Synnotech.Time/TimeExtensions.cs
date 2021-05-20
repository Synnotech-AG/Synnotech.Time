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
    }
}