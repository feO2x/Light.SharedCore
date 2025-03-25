using System;
using FluentAssertions;
using Light.SharedCore.Time;
using Xunit;

namespace Light.SharedCore.Tests.Time;

public static class UtcClockTests
{
    [Fact]
    public static void MustReturnUtcTime() =>
        new UtcClock().GetTime().Kind.Should().Be(DateTimeKind.Utc);

    [Fact]
    public static void ReturnedTimeMustBeCloseToUtcNow() =>
        new UtcClock().GetTime().Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
}
