﻿#if !NETFRAMEWORK
using System;
#endif
using FluentAssertions;
using Light.SharedCore.Parsing;
using Xunit;

namespace Light.SharedCore.Tests.Parsing;

public static class DoubleParserTests
{
    private const double Precision = 0.0000001;

    [Theory]
    [MemberData(nameof(NumbersWithDecimalPoint))]
    public static void ParseFloatingPointNumberWithDecimalPoint(string text, double expectedValue) =>
        CheckNumber(text, expectedValue);

#if !NETFRAMEWORK
    [Theory]
    [MemberData(nameof(NumbersWithDecimalPoint))]
    public static void ParseFloatingPointNumberWithDecimalPointAsSpan(string text, double expectedValue) =>
        CheckNumberAsSpan(text, expectedValue);
#endif

    public static readonly TheoryData<string, double> NumbersWithDecimalPoint =
        new ()
        {
            { "0.74", 0.74 },
            { "1.34", 1.34 },
            { "391202.9", 391202.9 },
            { "-20.816", -20.816 },
            { "15,019.33", 15_019.33 }
        };

    [Theory]
    [MemberData(nameof(NumbersWithDecimalComma))]
    public static void ParseFloatingPointNumberWithDecimalComma(string text, double expectedValue) =>
        CheckNumber(text, expectedValue);

#if !NETFRAMEWORK
    [Theory]
    [MemberData(nameof(NumbersWithDecimalComma))]
    public static void ParseFloatingPointNumberWithDecimalCommaAsSpan(string text, double expectedValue) =>
        CheckNumberAsSpan(text, expectedValue);
#endif


    public static readonly TheoryData<string, double> NumbersWithDecimalComma =
        new ()
        {
            { "000,7832", 0.7832 },
            { "-0,499", -0.499 },
            { "40593,84", 40593.84 },
            { "1.943.100,84", 1_943_100.84 }
        };

    [Theory]
    [MemberData(nameof(IntegerNumbers))]
    public static void ParseInteger(string text, double expectedValue) =>
        CheckNumber(text, expectedValue);

#if !NETFRAMEWORK
    [Theory]
    [MemberData(nameof(IntegerNumbers))]
    public static void ParseIntegerAsSpan(string text, double expectedValue) =>
        CheckNumberAsSpan(text, expectedValue);
#endif

    public static readonly TheoryData<string, double> IntegerNumbers =
        new ()
        {
            { "15", 15.0 },
            { "-743923", -743923.0 },
            { "239.482.392.923", 239_482_392_923.0 },
            { "21,500,000", 21_500_000.0 }
        };

    private static void CheckNumber(string text, double expectedValue)
    {
        var result = DoubleParser.TryParse(text, out var parsedValue);

        result.Should().BeTrue();
        parsedValue.Should().BeApproximately(expectedValue, Precision);
    }

#if !NETFRAMEWORK
    private static void CheckNumberAsSpan(ReadOnlySpan<char> text, double expectedValue)
    {
        var result = DoubleParser.TryParse(text, out var parsedValue);

        result.Should().BeTrue();
        parsedValue.Should().BeApproximately(expectedValue, Precision);
    }
#endif

    [Theory]
    [MemberData(nameof(InvalidNumbers))]
    public static void InvalidNumber(string? text)
    {
        var result = DoubleParser.TryParse(text, out var actualValue);

        result.Should().BeFalse();
        actualValue.Should().Be(0);
    }

#if !NETFRAMEWORK
    [Theory]
    [MemberData(nameof(InvalidNumbers))]
    public static void InvalidNumberAsSpan(string? text)
    {
        var result = DoubleParser.TryParse(text.AsSpan(), out var actualValue);

        result.Should().BeFalse();
        actualValue.Should().Be(0);
    }
#endif

    public static readonly TheoryData<string?> InvalidNumbers =
        new ()
        {
            "Foo",
            "Bar",
            "",
            null,
            "9392gk381"
        };
}
