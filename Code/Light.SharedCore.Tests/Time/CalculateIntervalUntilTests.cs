using System;
using FluentAssertions;
using Light.SharedCore.Time;
using Light.Xunit;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Light.SharedCore.Tests.Time;

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
            { new (2017, 10, 4, 4, 12, 0, DateTimeKind.Local), new (0, 3, 0) }, // Time is on same day
            { new (2017, 10, 4, 4, 16, 0, DateTimeKind.Local), new (23, 59, 0) }, // Time is on next day
            { new (2016, 12, 31, 4, 15, 1, DateTimeKind.Local), new (23, 59, 59) }, // New Year's Eve
            { new (2017, 10, 4, StartTime.Hour, StartTime.Minute, StartTime.Second, DateTimeKind.Local), new (24, 0, 0) }, // Exactly same time
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
            { new (2017, 03, 25, 18, 0, 0, DateTimeKind.Local), new (9, 15, 0) }, // Begin of Daylight Saving Time
            { new (2017, 10, 28, 18, 0, 0, DateTimeKind.Local), new (11, 15, 0) } // End of Daylight Saving Time
        };

    private static void CheckTimeSpan(DateTime now, TimeSpan expected)
    {
        var actualTimeSpan = now.CalculateIntervalUntil(StartTime);
        actualTimeSpan.Should().Be(expected);
    }
}