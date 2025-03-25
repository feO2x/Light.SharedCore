using FluentAssertions;
using Light.SharedCore.Parsing;
using Xunit;
#if !NETFRAMEWORK
using System;
#endif

namespace Light.SharedCore.Tests.Parsing;

public static class DecimalParserTests
{
    public static readonly TheoryData<string, decimal> NumbersWithDecimalPoint =
        new ()
        {
            { "0.74", 0.74m },
            { "1.34", 1.34m },
            { "391202.9", 391202.9m },
            { "-20.816", -20.816m },
            { "15,019.33", 15_019.33m }
        };

    public static readonly TheoryData<string, decimal> DecimalCommaData =
        new ()
        {
            { "000,7832", 0.7832m },
            { "-0,499", -0.499m },
            { "40593,84", 40593.84m },
            { "1.943.100,84", 1_943_100.84m }
        };

    public static readonly TheoryData<string, decimal> IntegerData =
        new ()
        {
            { "15", 15.0m },
            { "-743923", -743923.0m },
            { "239.482.392.923", 239_482_392_923.0m },
            { "21,500,000", 21_500_000.0m }
        };

    public static readonly TheoryData<string?> InvalidNumbers =
        new ()
        {
            "Foo",
            "Bar",
            "",
            null,
            "9392gk381"
        };

    [Theory]
    [MemberData(nameof(NumbersWithDecimalPoint))]
    public static void ParseFloatingPointNumberWithDecimalPoint(string text, decimal expectedValue) =>
        CheckNumber(text, expectedValue);

#if !NETFRAMEWORK
    [Theory]
    [MemberData(nameof(NumbersWithDecimalPoint))]
    public static void ParseFloatingPointNumberWithDecimalPointAsSpan(string text, decimal expectedValue) =>
        CheckNumberAsSpan(text, expectedValue);
#endif

    [Theory]
    [MemberData(nameof(DecimalCommaData))]
    public static void ParseFloatingPointNumberWithDecimalComma(string text, decimal expectedValue) =>
        CheckNumber(text, expectedValue);

#if !NETFRAMEWORK
    [Theory]
    [MemberData(nameof(DecimalCommaData))]
    public static void ParseFloatingPointNumberWithDecimalCommaAsSpan(string text, decimal expectedValue) =>
        CheckNumberAsSpan(text, expectedValue);
#endif

    [Theory]
    [MemberData(nameof(IntegerData))]
    public static void ParseInteger(string text, decimal expectedValue) =>
        CheckNumber(text, expectedValue);

#if !NETFRAMEWORK
    [Theory]
    [MemberData(nameof(IntegerData))]
    public static void ParseIntegerAsSpan(string text, decimal expectedValue) =>
        CheckNumberAsSpan(text, expectedValue);
#endif

    private static void CheckNumber(string text, decimal expectedValue)
    {
        var result = DecimalParser.TryParse(text, out var parsedValue);

        result.Should().BeTrue();
        parsedValue.Should().Be(expectedValue);
    }

#if !NETFRAMEWORK
    private static void CheckNumberAsSpan(ReadOnlySpan<char> text, decimal expectedValue)
    {
        var result = DecimalParser.TryParse(text, out var parsedValue);

        result.Should().BeTrue();
        parsedValue.Should().Be(expectedValue);
    }
#endif

    [Theory]
    [MemberData(nameof(InvalidNumbers))]
    public static void InvalidNumber(string? text)
    {
        var result = DecimalParser.TryParse(text, out var actualValue);

        result.Should().BeFalse();
        actualValue.Should().Be(0);
    }

#if !NETFRAMEWORK
    [Theory]
    [MemberData(nameof(InvalidNumbers))]
    public static void InvalidNumberAsSpan(string? text)
    {
        var result = DecimalParser.TryParse(text.AsSpan(), out var actualValue);

        result.Should().BeFalse();
        actualValue.Should().Be(0);
    }
#endif
}
