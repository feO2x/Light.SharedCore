using System;
using FluentAssertions;
using Light.SharedCore.Entities;
using Light.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace Light.SharedCore.Tests.Entities;

public sealed class Int32EntityTests
{
    public Int32EntityTests(ITestOutputHelper output) => Output = output;

    private ITestOutputHelper Output { get; }

    public static TheoryData<int> NegativeIds { get; } =
        new () { -1, -34501, int.MinValue };

    [Fact]
    public static void MustImplementIEntityOfInt32() => typeof(Int32Entity<>).Should().Implement<IEntity<int>>();

    [Fact]
    public static void TheSameInstanceShouldBeConsideredEqual()
    {
        var entity = new Entity(42);

        // ReSharper disable EqualExpressionComparison
#pragma warning disable CS1718 // We explicitly want to test the equality operators here
        (entity == entity).Should().BeTrue();
        (entity != entity).Should().BeFalse();
#pragma warning restore CS1718
        // ReSharper restore EqualExpressionComparison
        entity.GetHashCode().Should().Be(entity.Id);
    }

    [Fact]
    public static void TwoInstancesWithTheSameIdShouldBeEqual()
    {
        var x = new Entity(80);
        var y = new Entity(80);

#pragma warning disable CS1718 // We explicitly want to test the equality operators here
        (x == y).Should().BeTrue();
        (x != y).Should().BeFalse();
#pragma warning restore CS1718
        x.GetHashCode().Should().Be(y.GetHashCode());
    }

    [Fact]
    public static void TwoInstancesWithDifferentIdsShouldNotBeEqual()
    {
        var x = new Entity(42);
        var y = new Entity(80);

        (x == y).Should().BeFalse();
        (x != y).Should().BeTrue();
        x.GetHashCode().Should().NotBe(y.GetHashCode());
    }

    [Fact]
    public static void ComparingWithNullMustReturnFalse()
    {
        var x = new Entity(1);
        var y = default(Entity);

        (x == y).Should().BeFalse();
        (x != y).Should().BeTrue();
        (y == x).Should().BeFalse();
        (y != x).Should().BeTrue();
    }

    [Fact]
    public static void TwoNullReferencesAreEqual()
    {
        var @null = default(Entity);

        // ReSharper disable EqualExpressionComparison
#pragma warning disable CS1718 // We explicitly want to test the equality operators here
        (@null == @null).Should().BeTrue();
        (@null != @null).Should().BeFalse();
#pragma warning restore CS1718
        // ReSharper restore EqualExpressionComparison
    }

    [Theory]
    [InlineData(1)]
    [InlineData(12)]
    [InlineData(5000)]
    public static void ToStringShouldReturnSimpleTypeNameAndId(int id) => new Entity(id).ToString().Should().Be("Entity " + id);

    [Theory]
    [MemberData(nameof(NegativeIds))]
    public void ConstructorShouldThrowWhenPassingNegativeId(int negativeId)
    {
        void Act() => _ = new Entity(negativeId);

        ExpectArgumentOutOfRangeException(Act);
    }

    [Theory]
    [MemberData(nameof(NegativeIds))]
    public void PropertyInitializerShouldThrowWhenPassingNegativeId(int negativeId)
    {
        void Act() => _ = new Entity { Id = negativeId };

        ExpectArgumentOutOfRangeException(Act, "value");
    }

    [Theory]
    [MemberData(nameof(NegativeIds))]
    public static void AllowNegativeIdViaConstructor(int negativeId)
    {
        try
        {
            Entity.AllowNegativeIds = true;
            var entity = new Entity(negativeId);
            entity.Id.Should().Be(negativeId);
        }
        finally
        {
            Entity.AllowNegativeIds = false;
        }
    }

    [Theory]
    [MemberData(nameof(NegativeIds))]
    public static void AllowNegativeIdViaPropertyInitializer(int negativeId)
    {
        try
        {
            Entity.AllowNegativeIds = true;
            var entity = new Entity { Id = negativeId };
            entity.Id.Should().Be(negativeId);
        }
        finally
        {
            Entity.AllowNegativeIds = false;
        }
    }

    [Fact]
    public void ConstructorShouldThrowWhenZeroIsPassed()
    {
        void Act() => _ = new Entity(0);

        ExpectArgumentOutOfRangeException(Act);
    }

    [Fact]
    public void PropertyInitializerShouldThrowWhenZeroIsPassed()
    {
        void Act() => _ = new Entity { Id = 0 };

        ExpectArgumentOutOfRangeException(Act, "value");
    }

    [Fact]
    public static void AllowZeroIdViaConstructor()
    {
        try
        {
            Entity.AllowIdZero = true;
            var entity = new Entity(0);
            entity.Id.Should().Be(0);
        }
        finally
        {
            Entity.AllowIdZero = false;
        }
    }

    [Fact]
    public static void AllowZeroViaPropertyInitialization()
    {
        try
        {
            Entity.AllowIdZero = true;
            var entity = new Entity { Id = 0 };
            entity.Id.Should().Be(0);
        }
        finally
        {
            Entity.AllowIdZero = false;
        }
    }

    [Fact]
    public static void SetAllowZeroAndAllowNegativeInOneCall()
    {
        try
        {
            Entity.AllowZeroAndNegativeIds();
            Entity.AllowIdZero.Should().BeTrue();
            Entity.AllowNegativeIds.Should().BeTrue();
        }
        finally
        {
            Entity.AllowNegativeIds = false;
            Entity.AllowIdZero = false;
        }
    }

    [Fact]
    public static void SetIdAfterInitialization()
    {
        var entity = new Entity();

        entity.ToMutable().SetId(42);

        entity.Id.Should().Be(42);
    }

    [Fact]
    public void SetIdAfterInitializationShouldThrowWhenIdIsZero()
    {
        void Act() => new Entity().ToMutable().SetId(0);

        ExpectArgumentOutOfRangeException(Act);
    }

    [Theory]
    [MemberData(nameof(NegativeIds))]
    public void SetIdAfterInitializationShouldThrowWhenIdIsNegative(int negativeId)
    {
        void Act() => new Entity().ToMutable().SetId(negativeId);

        ExpectArgumentOutOfRangeException(Act);
    }

    [Fact]
    public static void AllowZeroOnSetIdAfterInitialization()
    {
        try
        {
            Entity.AllowIdZero = true;
            var entity = new Entity();
            entity.ToMutable().SetId(0);
            entity.Id.Should().Be(0);
        }
        finally
        {
            Entity.AllowIdZero = false;
        }
    }

    [Theory]
    [MemberData(nameof(NegativeIds))]
    public static void AllowNegativeIdsOnSetIdAfterInitialization(int negativeId)
    {
        try
        {
            Entity.AllowNegativeIds = true;
            var entity = new Entity();
            entity.ToMutable().SetId(negativeId);
            entity.Id.Should().Be(negativeId);
        }
        finally
        {
            Entity.AllowNegativeIds = false;
        }
    }

    private void ExpectArgumentOutOfRangeException(Action act, string parameterName = "id")
    {
        var exception = act.Should().Throw<ArgumentOutOfRangeException>().Which;
        exception.ShouldBeWrittenTo(Output);
        exception.ParamName.Should().Be(parameterName);
    }

    private sealed class Entity : Int32Entity<Entity>
    {
        public Entity() { }

        public Entity(int id) : base(id) { }
    }
}