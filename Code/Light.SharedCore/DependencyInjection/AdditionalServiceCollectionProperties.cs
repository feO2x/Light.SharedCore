using System;
using Microsoft.Extensions.DependencyInjection;

namespace Light.SharedCore.DependencyInjection;

/// <summary>
/// Contains properties that should be attached to <see cref="IServiceCollection" /> instances.
/// </summary>
public class AdditionalServiceCollectionProperties
{
    /// <summary>
    /// Gets or initializes the value indicating whether a <see cref="Func{TResult}" /> should also be registered when
    /// adding a service to the services collection. This Func&lt;T&gt; can be used as a factory to dynamically resolve
    /// instances of the specified type without directly referencing the DI container (this avoids the Service Locator anti pattern).
    /// </summary>
    public bool RegisterFunc { get; init; } = true;
    
    /// <summary>
    /// Gets or initializes the default lifetime for sessions. The default value is <see cref="ServiceLifetime.Transient" />. 
    /// </summary>
    public ServiceLifetime DefaultSessionLifetime { get; init; } = ServiceLifetime.Transient;
    
    /// <summary>
    /// Gets or initializes the default lifetime for factories. The default value is <see cref="ServiceLifetime.Singleton" />.
    /// </summary>
    public ServiceLifetime DefaultFactoryLifetime { get; init; } = ServiceLifetime.Singleton;
}