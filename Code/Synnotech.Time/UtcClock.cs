using System;

namespace Synnotech.Time
{
    /// <summary>
    /// Represents a clock that returns the UTC time. This time is represented on a
    /// continuous timeline and does not suffer from inconsistencies like switches to
    /// summer time / daylight saving time and other political decisions of local time zones.
    /// It should be your preferred clock to use in applications.
    /// </summary>
    public sealed class UtcClock : IClock
    {
        /// <summary>
        /// Gets the current UTC time.
        /// </summary>
        public DateTime GetTime() => DateTime.UtcNow;
    }
}