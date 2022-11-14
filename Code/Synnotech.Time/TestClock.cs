using System;
using Light.GuardClauses;

namespace Synnotech.Time;

/// <summary>
/// Represents a clock that can be used in test scenarios
/// where time must be controlled programmatically.
/// </summary>
public sealed class TestClock : IClock
{
    private TestTimesEnumerator _testTimes; // This field MUST NOT be readonly, the struct instance must be able to mutate its state

    /// <summary>
    /// Initializes a new instance of <see cref="TestClock" />.
    /// <see cref="InitialTime" /> and the current time are set to
    /// <see cref="DateTime.UtcNow" />.
    /// </summary>
    public TestClock() => InitialTime = CurrentTime = DateTime.UtcNow;

    /// <summary>
    /// Initializes a new instance of <see cref="TestClock" /> with the specified
    /// initial time.
    /// </summary>
    public TestClock(DateTime initialTime) => InitialTime = CurrentTime = initialTime;

    /// <summary>
    /// Initializes a new instance of <see cref="TestClock" /> with the specified
    /// predefined times. Each call to <see cref="GetTime" /> will return the next
    /// datetime value in the array. If there are more calls to <see cref="GetTime" />
    /// than values in the array, the last item in the array will be returned
    /// several times.
    /// </summary>
    /// <param name="times">The datetime values that will be returned by the test clock.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="times" /> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the specified array is empty.</exception>
    public TestClock(params DateTime[] times)
    {
        times.MustNotBeNullOrEmpty();
        _testTimes = new TestTimesEnumerator(times);
        _testTimes.TryGetNextTime(out var initialTime);
        InitialTime = CurrentTime = initialTime;
    }

    /// <summary>
    /// Gets the time that was set when this clock was instantiated.
    /// </summary>
    public DateTime InitialTime { get; }

    /// <summary>
    /// Gets the value that will be returned by the next call to <see cref="GetTime" />.
    /// </summary>
    public DateTime CurrentTime { get; private set; }

    /// <summary>
    /// Gets the current time of the test clock.
    /// </summary>
    public DateTime GetTime()
    {
        var currentTime = CurrentTime;
        TrySetNextTestTime();
        return currentTime;
    }

    /// <summary>
    /// Advances the <see cref="CurrentTime" /> by the specified time span. This will not affect any
    /// outstanding times that you might have provided to the constructor that takes an array of datetime value.
    /// </summary>
    /// <param name="timeSpan">The amount of time that the clock should advance. This value can also be negative.</param>
    public TestClock AdvanceTime(TimeSpan timeSpan)
    {
        CurrentTime = CurrentTime.Add(timeSpan);
        return this;
    }

    private void TrySetNextTestTime()
    {
        if (_testTimes.TryGetNextTime(out var nextTime))
            CurrentTime = nextTime;
    }

    private struct TestTimesEnumerator
    {
        public TestTimesEnumerator(DateTime[] array)
        {
            Array = array;
            CurrentIndex = 0;
        }

        private DateTime[]? Array { get; }

        private int CurrentIndex { get; set; }

        public bool TryGetNextTime(out DateTime nextTime)
        {
            if (Array == null || CurrentIndex == Array.Length)
            {
                nextTime = default;
                return false;
            }

            nextTime = Array[CurrentIndex++];
            return true;
        }
    }
}