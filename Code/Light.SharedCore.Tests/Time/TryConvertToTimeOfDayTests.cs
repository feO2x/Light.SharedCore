using System;
using FluentAssertions;
using Light.SharedCore.Time;
using Xunit;

namespace Light.SharedCore.Tests.Time;

public static class TryConvertToTimeOfDayTests
{
    [Theory]
    [MemberData(nameof(ValidTimeSpans))]
    public static void ConvertValidValueToLocalTimeOfDay(TimeSpan timeSpan)
    {
        var result = timeSpan.TryConvertToTimeOfDay(out var timeOfDay);
        CheckValidResult(timeSpan, result, timeOfDay, DateTimeKind.Local);
    }

    [Theory]
    [MemberData(nameof(ValidTimeSpans))]
    public static void ConvertValidValueToUtcTimeOfDay(TimeSpan timeSpan)
    {
        var result = timeSpan.TryConvertToUtcTimeOfDay(out var timeOfDay);
        CheckValidResult(timeSpan, result, timeOfDay, DateTimeKind.Utc);
    }

    public static readonly TheoryData<TimeSpan> ValidTimeSpans =
        new ()
        {
            new (23, 59, 59),
            TimeSpan.Zero,
            new (5, 23, 59, 59), // The days part is ignored
            new (0, 23, 59, 59, 999)
        };

    private static void CheckValidResult(TimeSpan timeSpan, bool result, DateTime timeOfDay, DateTimeKind expectedKind)
    {
        result.Should().BeTrue();
        var expectedTimeOfDay = new DateTime(1, 1, 1, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds, expectedKind);
        timeOfDay.Should().Be(expectedTimeOfDay);
    }

    [Theory]
    [MemberData(nameof(InvalidTimeSpans))]
    public static void ConversionFailsForInvalidTimeSpans(TimeSpan invalidTimeSpan)
    {
        var result = invalidTimeSpan.TryConvertToUtcTimeOfDay(out var timeOfDay);
        result.Should().BeFalse();
        timeOfDay.Should().Be(default);
    }

    public static readonly TheoryData<TimeSpan> InvalidTimeSpans =
        new ()
        {
            TimeSpan.FromHours(-1),
            new (-1, -23, 0),
            new (-5, 20, 0)
        };
}