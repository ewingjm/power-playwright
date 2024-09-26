namespace PowerPlaywright.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using PowerPlaywright.Framework;

    /// <summary>
    /// Extensions to the <see cref="IServiceCollection"/> interface.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
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
            services.AddSingleton<TImplementation>();
            services.AddSingleton<TService>(sp => sp.GetRequiredService<TImplementation>());
            services.AddSingleton<IAppLoadInitializable>(sp => sp.GetRequiredService<TImplementation>());

            return services;
        }
    }
}
