using System;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Synnotech.Xunit;
using Xunit;

namespace Synnotech.Time.Tests;

public static class CalculateIntervalUntilTests
{
    private static readonly DateTime StartTime = new (1, 1, 1, 4, 15, 0, DateTimeKind.Local);
        
    [Theory]
    [MemberData(nameof(NonSpecificData))]
    public static void CalculateIntervalInNonSpecificCultureScenario(DateTime now, TimeSpan expected) =>
        CheckTimeSpan(now, expected);

    public static readonly TheoryData<DateTime, TimeSpan> NonSpecificData =
        new ()
        {
            { new DateTime(2017, 10, 4, 4, 12, 0, DateTimeKind.Local), new TimeSpan(0, 3, 0) }, // Time is on same day
            { new DateTime(2017, 10, 4, 4, 16, 0, DateTimeKind.Local), new TimeSpan(23, 59, 0) }, // Time is on next day
            { new DateTime(2016, 12, 31, 4, 15, 1, DateTimeKind.Local), new TimeSpan(23, 59, 59) }, // New Year's Eve
            { new DateTime(2017, 10, 4, StartTime.Hour, StartTime.Minute, StartTime.Second, DateTimeKind.Local), new TimeSpan(24, 0, 0) }, // Exactly same time
        };

    [SkippableTheory]
    [MemberData(nameof(GermanSpecificData))]
    public static void CalculateTimeOfNextDayInGermanScenario(DateTime now, TimeSpan expected)
    {
        Skip.IfNot(TestSettings.Configuration.GetValue<bool>("areGermanCultureSpecificTestsEnabled"));
        CheckTimeSpan(now, expected);
    }

    public static readonly TheoryData<DateTime, TimeSpan> GermanSpecificData =
        new ()
        {
            { new DateTime(2017, 03, 25, 18, 0, 0, DateTimeKind.Local), new TimeSpan(9, 15, 0) }, // Begin of Daylight Saving Time
            { new DateTime(2017, 10, 28, 18, 0, 0, DateTimeKind.Local), new TimeSpan(11, 15, 0) } // End of Daylight Saving Time
        };

    private static void CheckTimeSpan(DateTime now, TimeSpan expected)
    {
        var actualTimeSpan = now.CalculateIntervalUntil(StartTime);
        actualTimeSpan.Should().Be(expected);
    }
}