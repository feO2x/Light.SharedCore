using System;
using Microsoft.Extensions.DependencyInjection;

namespace Light.SharedCore.Time;

/// <summary>
/// Provides extension methods to add the clocks to a service collection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the <see cref="UtcClock" /> as a singleton to the service collection, mapped to <see cref="IClock" />.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services" /> is null.</exception>
    public static IServiceCollection AddUtcClock(this IServiceCollection services) => services.AddSingleton<IClock>(new UtcClock());

    /// <summary>
    /// Adds the <see cref="LocalClock" /> as a singleton to the service collection, mapped to <see cref="IClock" />.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services" /> is null.</exception>
    public static IServiceCollection AddLocalClock(this IServiceCollection services) => services.AddSingleton<IClock>(new LocalClock());

    /// <summary>
    /// Adds the <see cref="TestClock" /> as a singleton to the service collection, mapped to <see cref="IClock" />.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services" /> is null.</exception>
    public static IServiceCollection AddTestClock(this IServiceCollection services) => services.AddSingleton<IClock>(new TestClock());
}