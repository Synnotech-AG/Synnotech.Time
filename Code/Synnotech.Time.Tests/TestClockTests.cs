using System;
using FluentAssertions;
using Xunit;

namespace Synnotech.Time.Tests
{
    public static class TestClockTests
    {
        [Fact]
        public static void DefaultTimeIsSetToUtcNow()
        {
            var testClock = new TestClock();

            var utcNow = DateTime.UtcNow;
            testClock.InitialTime.Should().BeCloseTo(utcNow);
            testClock.GetTime().Should().BeCloseTo(utcNow);
        }

        [Theory]
        [MemberData(nameof(CustomDateTimes))]
        public static void SpecifyCustomInitialDateTime(DateTime customDateTime)
        {
            var testClock = new TestClock(customDateTime);

            testClock.InitialTime.Should().Be(customDateTime);
            testClock.GetTime().Should().Be(customDateTime);
        }

        public static readonly TheoryData<DateTime> CustomDateTimes =
            new ()
            {
                new DateTime(2018, 12, 4, 9, 22, 15, DateTimeKind.Local),
                new DateTime(2010, 5, 19, 15, 37, 0, DateTimeKind.Unspecified),
                new DateTime(1975, 2, 28, 12, 0, 0, DateTimeKind.Utc)
            };

        [Theory]
        [MemberData(nameof(TimeSpans))]
        public static void AdvanceTime(TimeSpan timeSpan)
        {
            var testClock = new TestClock().AdvanceTime(timeSpan);
            var resultingTime = testClock.GetTime();
            resultingTime.Should().Be(testClock.InitialTime.Add(timeSpan));
        }

        public static readonly TheoryData<TimeSpan> TimeSpans =
            new ()
            {
                TimeSpan.FromSeconds(50),
                TimeSpan.FromDays(14),
                TimeSpan.FromMilliseconds(750)
            };
    }
}