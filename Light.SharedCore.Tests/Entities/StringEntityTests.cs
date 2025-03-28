﻿using System;
using FluentAssertions;
using Light.SharedCore.Entities;
using Xunit;

namespace Light.SharedCore.Tests.Entities;

public static class StringEntityTests
{
    public static TheoryData<string> InvalidIds { get; } =
        new ()
        {
            null!,
            string.Empty,
            "\t",
            " ",
            " IdWithWhiteSpaceAtTheBeginning",
            "IdWithWhiteSpaceAtTheEnd\r\n",
            new string('x', 201) // Too long
        };

    [Fact]
    public static void MustImplementIEntityOfString() =>
        typeof(StringEntity<>).Should().Implement<IEntity<string>>();

    [Fact]
    public static void TheSameInstanceShouldBeConsideredEqual()
    {
        var entity = new Entity("Foo");

        // ReSharper disable EqualExpressionComparison
#pragma warning disable CS1718 // We explicitly want to test the equality operators here
        (entity == entity).Should().BeTrue();
        (entity != entity).Should().BeFalse();
#pragma warning restore CS1718
        // ReSharper restore EqualExpressionComparison
        entity.GetHashCode().Should().Be(entity.GetHashCode());
    }

    [Fact]
    public static void TwoInstancesWithTheSameIdShouldBeEqual()
    {
        const string id = "Foo";
        var x = new Entity(id);
        var y = new Entity(id);

        (x == y).Should().BeTrue();
        (x != y).Should().BeFalse();
        x.GetHashCode().Should().Be(y.GetHashCode());
    }

    [Fact]
    public static void DefaultValueIsNull()
    {
        var entity = new Entity();

        entity.Id.Should().BeNull();
    }

    [Fact]
    public static void DefaultValueCanBeSetToEmptyString()
    {
        try
        {
            Entity.IsDefaultValueNull = false;
            var entity = new Entity();
            entity.Id.Should().BeEmpty();
        }
        finally
        {
            Entity.IsDefaultValueNull = true;
        }
    }

    [Fact]
    public static void TwoInstancesWIthDifferentIdsShouldNotBeEqual()
    {
        var x = new Entity("Foo");
        var y = new Entity("Bar");

        (x == y).Should().BeFalse();
        (x != y).Should().BeTrue();
        x.GetHashCode().Should().NotBe(y.GetHashCode());
    }

    [Fact]
    public static void ComparingWithNullMustReturnFalse()
    {
        var x = new Entity("Foo");
        var y = default(Entity);

        // ReSharper disable ConditionIsAlwaysTrueOrFalse
        (x == y).Should().BeFalse();
        (x != y).Should().BeTrue();
        (y == x).Should().BeFalse();
        (y != x).Should().BeTrue();
        // ReSharper restore ConditionIsAlwaysTrueOrFalse
    }

    [Theory]
    [MemberData(nameof(InvalidIds))]
    public static void InvalidStringsShouldThrowViaConstructor(string invalidId)
    {
        var act = () => _ = new Entity(invalidId);

        act.Should().Throw<ArgumentException>()
           .And.ParamName.Should().Be("id");
    }

    [Fact]
    public static void DifferentComparisonMode()
    {
        try
        {
            Entity.ComparisonMode = StringComparison.OrdinalIgnoreCase;
            var x = new Entity("FOO");
            var y = new Entity("foo");
            (x == y).Should().BeTrue();
        }
        finally
        {
            Entity.ComparisonMode = StringComparison.Ordinal;
        }
    }

    [Fact]
    public static void SetIdViaPropertyInitializer()
    {
        var id = Guid.NewGuid().ToString();
        var entity = new Entity { Id = id };
        entity.Id.Should().BeSameAs(id);
    }

    [Theory]
    [MemberData(nameof(InvalidIds))]
    public static void InvalidStringsShouldThrowViaPropertyInitialization(string invalidId)
    {
        var act = () => _ = new Entity { Id = invalidId };

        act.Should().Throw<ArgumentException>()
           .And.ParamName.Should().Be("value");
    }

    [Fact]
    public static void SetIdAfterInitialization()
    {
        var entity = new Entity();
        entity.ToMutable().SetId("42");
        entity.Id.Should().BeSameAs("42");
    }

    [Theory]
    [MemberData(nameof(InvalidIds))]
    public static void InvalidStringShouldThrowViaMutableId(string invalidId)
    {
        var act = () => new Entity().ToMutable().SetId(invalidId);

        act.Should().Throw<ArgumentException>()
           .And.ParamName.Should().Be("id");
    }

    [Theory]
    [MemberData(nameof(InvalidIds))]
    public static void ExchangeValidationMethod(string technicallyInvalidId)
    {
        try
        {
            Entity.ValidateId = (id, _) => id;
            var entity = new Entity(technicallyInvalidId);
            entity.Id.Should().Be(technicallyInvalidId);
        }
        finally
        {
            Entity.ValidateId = Entity.ValidateTrimmedNotWhiteSpaceShorterThanOrEqualTo200;
        }
    }


    private sealed class Entity : StringEntity
    {
        public Entity() { }

        public Entity(string id) : base(id) { }
    }
}
