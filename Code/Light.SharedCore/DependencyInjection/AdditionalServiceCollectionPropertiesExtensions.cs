using System;
using System.Runtime.CompilerServices;
using Light.GuardClauses;
using Microsoft.Extensions.DependencyInjection;

namespace Light.SharedCore.DependencyInjection;

/// <summary>
/// Provides extensions methods to add additional properties to a <see cref="IServiceCollection" /> instance.
/// </summary>
public static class AdditionalServiceCollectionPropertiesExtensions
{
    private static ConditionalWeakTable<IServiceCollection, AdditionalServiceCollectionProperties> AdditionalProperties { get; } = new ();

    /// <summary>
    /// Gets the additional properties of the service collection. If no properties have been added to the service collection,
    /// they will be created and attached.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services" /> is null.</exception>
    public static AdditionalServiceCollectionProperties GetAdditionalProperties(this IServiceCollection services) =>
        AdditionalProperties.GetOrCreateValue(services.MustNotBeNull());

    /// <summary>
    /// Gets the additional properties of the service collection, downcast to the specified type. If no properties have been added yet,
    /// an <see cref="InvalidOperationException" /> will be thrown. Use this method when you manually added instances of your own subtype
    /// to the service collection.
    /// </summary>
    /// <typeparam name="T">The subtype of <see cref="AdditionalServiceCollectionProperties" />.</typeparam>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services" /> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no additional properties are registered with the specified services.</exception>
    /// <exception cref="InvalidCastException">
    /// Thrown when the additional properties could be retrieved, but cannot be cast to <typeparamref name="T" />.
    /// </exception>
    public static T GetAdditionalProperties<T>(this IServiceCollection services)
    {
        var additionalProperties = AdditionalProperties.GetValue(
            services.MustNotBeNull(),
            _ => throw new InvalidOperationException($"Cannot instantiate additional service collection properties of type \"{typeof(T)}\".")
        );

        if (additionalProperties is T castValue)
            return castValue;

        throw new InvalidCastException($"\"{additionalProperties.GetType()}\" cannot be cast to type \"{typeof(T)}\".");
    }

    /// <summary>
    /// Adds the specified properties to the services collection
    /// </summary>
    /// <param name="services">The service collection the properties will be attached to.</param>
    /// <param name="properties">The properties to attach. Consider that you can also build your own subtype of properties.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services" /> or <paramref name="properties" /> are null.</exception>
    public static IServiceCollection AddAdditionalProperties(this IServiceCollection services, AdditionalServiceCollectionProperties properties)
    {
        AdditionalProperties.Add(services.MustNotBeNull(), properties.MustNotBeNull());
        return services;
    }
}