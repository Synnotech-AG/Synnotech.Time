using System;
using FluentAssertions;
using Xunit;

namespace Synnotech.Time.Tests
{
    public static class LocalClockTests
    {
        [Fact]
        public static void MustReturnLocalTime() =>
            new LocalClock().GetTime().Kind.Should().Be(DateTimeKind.Local);

        [Fact]
        public static void TimeMustBeCloseToLocalTime() =>
            new LocalClock().GetTime().Should().BeCloseTo(DateTime.Now);
    }
}