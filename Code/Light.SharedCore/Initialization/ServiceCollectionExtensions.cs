using System;
using Light.GuardClauses;
using Light.SharedCore.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Light.SharedCore.Initialization;

/// <summary>
/// Provides extension methods for registering async factories with a DI container
/// based on <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// <para>
    /// Registers the specified type as well as a factory to asynchronously initialize instances.
    /// You can inject the async factory into client code to resolve an instance of your
    /// type asynchronously. When <see cref="IAsyncFactory{T}.CreateAsync" /> is called,
    /// an instance of your type is resolved and initialized asynchronously if your type
    /// implements the <see cref="IInitializeAsync" /> interface. Usually this factory is
    /// used for humble objects that perform I/O and need to be initialized asynchronously.
    /// </para>
    /// <para>
    /// <code>
    /// public class MyService
    /// {
    ///     public MyService(IAsyncFactory&lt;IMySession&gt; factory) => Factory = factory;
    /// 
    ///     private IAsyncFactory&lt;IMySession&gt; Factory { get; }
    /// 
    ///     public async Task DoSomethingAsync()
    ///     {
    ///         await using var mySession = await Factory.CreateAsync();
    ///         // Do something useful with your session here
    ///     }
    /// }
    /// </code>
    /// </para>
    /// </summary>
    /// <param name="services">The collection that holds all registrations for the DI container.</param>
    /// <param name="sessionLifetime">
    /// The lifetime of your instance (optional). Should be either <see cref="ServiceLifetime.Transient" /> or
    /// <see cref="ServiceLifetime.Scoped" />. If null is specified, the default session lifetime is obtained via the
    /// <see cref="AdditionalServiceCollectionProperties" /> attached to your <paramref name="services" />.
    /// </param>
    /// <param name="factoryLifetime">
    /// The lifetime for the factory (optional). Should be usually <see cref="ServiceLifetime.Singleton" /> or
    /// <see cref="ServiceLifetime.Transient" />. If null is specified, the default factory lifetime is obtained via
    /// the <see cref="AdditionalServiceCollectionProperties" /> attached to your <paramref name="services" />.
    /// </param>
    /// <param name="registerSessionFunc">
    /// The value indicating whether a Func&lt;TAbstraction&gt; is also registered with the DI container (optional).
    /// This factory delegate is necessary for the <see cref="GenericAsyncFactory{T}" /> to work properly. If null is
    /// specified, the default value will be obtained via the <see cref="AdditionalServiceCollectionProperties" />
    /// attached to your <paramref cref="services" />. You can set this value to false if you use a proper DI container
    /// like LightInject that offers function factories.
    /// </param>
    /// <typeparam name="TAbstraction">The abstraction that your concrete type implements.</typeparam>
    /// <typeparam name="TImplementation">The type that should be instantiated by the async factory.</typeparam>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services" /> is null.</exception>
    public static IServiceCollection AddSessionFactoryFor<TAbstraction, TImplementation>(this IServiceCollection services,
                                                                                         ServiceLifetime? sessionLifetime = null,
                                                                                         ServiceLifetime? factoryLifetime = null,
                                                                                         bool? registerSessionFunc = null)
        where TAbstraction : class
        where TImplementation : TAbstraction
    {
        services.MustNotBeNull();

        if (sessionLifetime is null || factoryLifetime is null || registerSessionFunc is null)
        {
            var properties = services.GetAdditionalProperties();
            sessionLifetime ??= properties.DefaultSessionLifetime;
            factoryLifetime ??= properties.DefaultFactoryLifetime;
            registerSessionFunc ??= properties.RegisterFunc;
        }

        services.Add(new (typeof(TAbstraction), typeof(TImplementation), sessionLifetime.Value));
        services.Add(new (typeof(IAsyncFactory<TAbstraction>), typeof(GenericAsyncFactory<TAbstraction>), factoryLifetime.Value));

        if (registerSessionFunc.Value)
            services.AddSingleton(container => new Func<TAbstraction>(container.GetRequiredService<TAbstraction>));

        return services;
    }
}