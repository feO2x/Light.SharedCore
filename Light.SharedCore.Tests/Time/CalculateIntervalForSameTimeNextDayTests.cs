using System;
using FluentAssertions;
using Light.SharedCore.Time;
using Light.Xunit;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Light.SharedCore.Tests.Time;

public static class CalculateIntervalForSameTimeNextDayTests
{
    public static readonly TheoryData<DateTime, TimeSpan> NonSpecificData =
        new ()
        {
            { new DateTime(2017, 10, 4, 12, 0, 0, DateTimeKind.Local), new TimeSpan(16, 15, 0) }, // Simple example
            { new DateTime(2016, 12, 31, 4, 15, 1, DateTimeKind.Local), new TimeSpan(23, 59, 59) } // New Year's Eve
        };

    public static readonly TheoryData<DateTime, TimeSpan> GermanSpecificData =
        new ()
        {
            {
                new DateTime(2017, 03, 25, 18, 0, 0, DateTimeKind.Local), new TimeSpan(9, 15, 0)
            }, // Begin of Daylight Saving Time
            {
                new DateTime(2017, 10, 28, 18, 0, 0, DateTimeKind.Local), new TimeSpan(11, 15, 0)
            } // End of Daylight Saving Time
        };

    [Theory]
    [MemberData(nameof(NonSpecificData))]
    public static void CalculateTimeOfNextDayInNonSpecificCultureScenario(DateTime now, TimeSpan expected) =>
        CheckTimeSpan(now, expected);

    [SkippableTheory]
    [MemberData(nameof(GermanSpecificData))]
    public static void CalculateTimeOfNextDayInGermanScenario(DateTime now, TimeSpan expected)
    {
        Skip.IfNot(TestSettings.Configuration.GetValue<bool>("areGermanCultureSpecificTestsEnabled"));
        CheckTimeSpan(now, expected);
    }

    private static void CheckTimeSpan(DateTime now, TimeSpan expected)
    {
        var startTime = new DateTime(1, 1, 1, 4, 15, 0, DateTimeKind.Local);
        var actualTimeSpan = now.CalculateIntervalForSameTimeNextDay(startTime);
        actualTimeSpan.Should().Be(expected);
    }
}
