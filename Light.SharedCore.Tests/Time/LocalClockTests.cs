﻿using System;
using FluentAssertions;
using Light.SharedCore.Time;
using Xunit;

namespace Light.SharedCore.Tests.Time;

public static class LocalClockTests
{
    [Fact]
    public static void MustReturnLocalTime() => new LocalClock().GetTime().Kind.Should().Be(DateTimeKind.Local);

    [Fact]
    public static void TimeMustBeCloseToLocalTime() =>
        new LocalClock().GetTime().Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
}
