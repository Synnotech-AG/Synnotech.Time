using System;

namespace Synnotech.Time;

/// <summary>
/// Represents a clock that returns the local time.
/// This time depends on the culture settings of the
/// current thread.
/// </summary>
public sealed class LocalClock : IClock
{
    /// <summary>
    /// Gets the local time.
    /// </summary>
    public DateTime GetTime() => DateTime.Now;
}