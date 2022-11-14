using System;
using FluentAssertions;
using Xunit;

namespace Synnotech.Time.Tests;

public static class TestClockTests
{
    [Fact]
    public static void DefaultTimeIsSetToUtcNow()
    {
        var testClock = new TestClock();

        var utcNow = DateTime.UtcNow;
        testClock.InitialTime.Should().BeCloseTo(utcNow, TimeSpan.FromSeconds(1));
        testClock.GetTime().Should().BeCloseTo(utcNow, TimeSpan.FromSeconds(1));
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

    [Fact]
    public static void ProvideSeveralTimes()
    {
        var initialTime = new DateTime(2021, 5, 20, 10, 30, 0, DateTimeKind.Utc);
        var secondTime = initialTime.AddHours(2);
        var thirdTime = initialTime.AddHours(5);
        var testClock = new TestClock(initialTime, secondTime, thirdTime);

        var capturedTimes = new []
        {
            testClock.GetTime(),
            testClock.GetTime(),
            testClock.GetTime(),
            testClock.GetTime(),
            testClock.GetTime()
        };

        var expectedTimes = new[]
        {
            initialTime,
            secondTime,
            thirdTime,
            thirdTime,
            thirdTime
        };
        capturedTimes.Should().Equal(expectedTimes);
    }

    [Fact]
    public static void TimesArrayNull()
    {
        // ReSharper disable once ObjectCreationAsStatement
        Action act = () => new TestClock(null!);

        act.Should().Throw<ArgumentNullException>()
           .And.ParamName.Should().Be("times");
    }

    [Fact]
    public static void TimesArrayEmpty()
    {
        // ReSharper disable once ObjectCreationAsStatement
        Action act = () => new TestClock(Array.Empty<DateTime>());

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public static void MixedMode()
    {
        var initialTime = new DateTime(2021, 5, 30, 11, 15, 0, DateTimeKind.Utc);
        var secondTime = initialTime.AddDays(1);
        var testClock = new TestClock(initialTime, secondTime);

        testClock.InitialTime.Should().Be(initialTime);

        testClock.AdvanceTime(TimeSpan.FromHours(1));
        var time1 = testClock.GetTime();
        time1.Should().Be(initialTime.AddHours(1));

        var time2 = testClock.GetTime();
        time2.Should().Be(secondTime);

        testClock.AdvanceTime(TimeSpan.FromHours(2));
        var time3 = testClock.GetTime();
        time3.Should().Be(secondTime.AddHours(2));
    }
}