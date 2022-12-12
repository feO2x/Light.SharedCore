using System;
using Light.GuardClauses;
using Microsoft.Extensions.DependencyInjection;

namespace Light.SharedCore.DependencyInjection;

/// <summary>
/// Provides extension methods for service collections to configure additional properties.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds additional properties to the services that are configured for LightInject.
    /// </summary>
    /// <param name="services">The collection that holds all registrations for the DI container.</param>
    /// <param name="defaultSessionLifetime">
    /// The default session lifetime for sessions. The default value is <see cref="ServiceLifetime.Transient" />.
    /// This value is used when calling AddSessionFactoryFor to register a session abstraction, session implementation,
    /// and a corresponding factory to asynchronously initialize the session.
    /// </param>
    /// <param name="defaultFactoryLifetime">
    /// The default session lifetime for factories. The default value is <see cref="ServiceLifetime.Singleton" />.
    /// This value is used when calling AddSessionFactoryFor to register a session abstraction, session implementation,
    /// and a corresponding factory to asynchronously initialize the session.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services" /> is null.</exception>
    public static IServiceCollection ConfigureAdditionalPropertiesForLightInject(this IServiceCollection services,
                                                                                 ServiceLifetime defaultSessionLifetime = ServiceLifetime.Transient,
                                                                                 ServiceLifetime defaultFactoryLifetime = ServiceLifetime.Singleton)
    {
        services.MustNotBeNull();
        
        var additionalProperties = new AdditionalServiceCollectionProperties
        {
            RegisterFunc = false,
            DefaultSessionLifetime = defaultSessionLifetime,
            DefaultFactoryLifetime = defaultFactoryLifetime
        };
        services.AddAdditionalProperties(additionalProperties);
        return services;
    }
}