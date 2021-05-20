using System;

namespace Synnotech.Time
{
    /// <summary>
    /// Represents a clock that can be used in test scenarios
    /// where time must be controlled programmatically.
    /// </summary>
    public sealed class TestClock : IClock
    {
        /// <summary>
        /// Gets the time that was set when this clock was instantiated.
        /// </summary>
        public readonly DateTime InitialTime;

        private DateTime _currentTime;

        /// <summary>
        /// Initializes a new instance of <see cref="TestClock" />.
        /// <see cref="InitialTime" /> and the current time are set to
        /// <see cref="DateTime.UtcNow" />.
        /// </summary>
        public TestClock() => InitialTime = _currentTime = DateTime.UtcNow;

        /// <summary>
        /// Initializes a new instance of <see cref="TestClock" /> with the specified
        /// initial time.
        /// </summary>
        public TestClock(DateTime initialTime) => InitialTime = _currentTime = initialTime;

        /// <summary>
        /// Gets the current time.
        /// </summary>
        public DateTime GetTime() => _currentTime;

        /// <summary>
        /// Advances the test clock
        /// </summary>
        /// <param name="timeSpan">The amount of time that the clock should advance. This value can also be negative.</param>
        /// <returns>The test clock instance for method chaining.</returns>
        public TestClock AdvanceTime(TimeSpan timeSpan)
        {
            _currentTime = _currentTime.Add(timeSpan);
            return this;
        }
    }
}