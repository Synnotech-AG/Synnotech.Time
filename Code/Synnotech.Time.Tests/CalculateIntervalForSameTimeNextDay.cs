using System;
using FluentAssertions;
using Xunit;

namespace Synnotech.Time.Tests
{
    public static class CalculateIntervalForSameTimeNextDay
    {
        [Theory]
        [MemberData(nameof(CalculateTimeSpansData))]
        public static void CalculateTimeSpans(DateTime now, TimeSpan expected)
        {
            var startTime = new DateTime(1, 1, 1, 4, 15, 0, DateTimeKind.Local);

            var actualTimeSpan = now.CalculateIntervalForSameTimeNextDay(startTime);

            actualTimeSpan.Should().Be(expected);
        }

        public static readonly TheoryData<DateTime, TimeSpan> CalculateTimeSpansData =
            new ()
            {
                { new DateTime(2017, 10, 4, 12, 0, 0, DateTimeKind.Local), new TimeSpan(16, 15, 0) }, // Simple example
                { new DateTime(2016, 12, 31, 4, 15, 1, DateTimeKind.Local), new TimeSpan(23, 59, 59) }, // New Year's Eve
                { new DateTime(2017, 03, 25, 18, 0, 0, DateTimeKind.Local), new TimeSpan(9, 15, 0) }, // Begin of Daylight Saving Time
                { new DateTime(2017, 10, 28, 18, 0, 0, DateTimeKind.Local), new TimeSpan(11, 15, 0) } // End of Daylight Saving Time
            };
    }
}