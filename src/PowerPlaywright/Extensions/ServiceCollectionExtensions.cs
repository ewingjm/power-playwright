﻿namespace PowerPlaywright.Extensions
{
    using System;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Redirectors;

    /// <summary>
    /// Extensions to the <see cref="IServiceCollection"/> interface.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a singleton that provides control redirection info.
        /// The type of the singleton is resolved at runtime from the assembly provided by the <see cref="IAssemblyProvider"/>.
        /// </summary>
        /// <param name="services">The service collection to add the singleton to.</param>
        /// <returns>The service collection with the added singleton.</returns>
        public static IServiceCollection AddControlRedirectionInfoProvider(this IServiceCollection services)
        {
            services
                .AddAppLoadInitializedSingleton(sp =>
                {
                    var resolvedType = sp.GetRequiredService<IAssemblyProvider>()
                        .GetAssembly()
                        .GetTypes()
                        .FirstOrDefault(t => typeof(IRedirectionInfoProvider<object>).IsAssignableFrom(t) && t.IsClass && t.IsVisible && !t.IsAbstract)
                    ?? throw new PowerPlaywrightException("A redirection info provider type was not found.");

                    return (IRedirectionInfoProvider<object>)ActivatorUtilities.CreateInstance(sp, resolvedType);
                });

            return services;
        }

        /// <summary>
        /// Adds a singleton that is initialised on app load.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the service to add.</typeparam>
        /// <param name="services">The service collection to add the service to.</param>
        /// <param name="implementationFactory">A factory which will be used to create the instance of the service.</param>
        /// <returns>The service collection with the added service.</returns>
        public static IServiceCollection AddAppLoadInitializedSingleton<TImplementation>(this IServiceCollection services, Func<IServiceProvider, TImplementation> implementationFactory)
            where TImplementation : class
        {
            services
                .AddSingleton(sp => implementationFactory(sp))
                .AddSingleton(sp => (IAppLoadInitializable)sp.GetRequiredService<TImplementation>());

            return services;
        }

        /// <summary>
        /// Adds a singleton that is initialised on app load.
        /// </summary>
        /// <typeparam name="TService">The service.</typeparam>
        /// <typeparam name="TImplementation">The implementation.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddAppLoadInitializedSingleton<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService, IAppLoadInitializable
        {
            services
                .AddSingleton<TImplementation>()
                .AddSingleton<TService>(sp => sp.GetRequiredService<TImplementation>())
                .AddSingleton<IAppLoadInitializable>(sp => sp.GetRequiredService<TImplementation>());

            return services;
        }
    }
}