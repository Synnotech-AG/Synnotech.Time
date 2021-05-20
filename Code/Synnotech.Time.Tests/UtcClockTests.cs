using System;
using FluentAssertions;
using Xunit;

namespace Synnotech.Time.Tests
{
    public static class UtcClockTests
    {
        [Fact]
        public static void MustReturnUtcTime() => 
            new UtcClock().GetTime().Kind.Should().Be(DateTimeKind.Utc);

        [Fact]
        public static void ReturnedTimeMustBeCloseToUtcNow() => 
            new UtcClock().GetTime().Should().BeCloseTo(DateTime.UtcNow);
    }
}