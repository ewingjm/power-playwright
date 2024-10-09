namespace PowerPlaywright
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Framework.Redirectors;
    using PowerPlaywright.Pages;
    using PowerPlaywright.Resolvers;

    /// <summary>
    /// A control factory that dynamically discovers controls at runtime from a remote source.
    /// </summary>
    internal class ControlFactory : IControlFactory, IAppLoadInitializable
    {
        private static readonly Type ControlInterfaceType = typeof(IControl);

        private readonly IList<IAssemblyProvider> assemblyProviders;
        private readonly IList<IControlStrategyResolver> strategyResolvers;
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<ControlFactory> logger;
        private readonly IDictionary<IAppPage, IDictionary<(Type, string, IControl), IControl>> pageCache;

        private IEnumerable<Type> assemblyTypes;
        private IEnumerable<Type> controlTypes;
        private IDictionary<Type, IControlRedirector<IControl>> redirectorsMap;
        private IDictionary<Type, Type> strategyMap;
        private IRedirectionInfoProvider<object> redirectionInfoProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlFactory"/> class.
        /// </summary>
        /// <param name="assemblyProviders">The control assembly provider(s).</param>
        /// <param name="strategyResolvers">The control strategy resolver(s).</param>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="logger">The logger.</param>
        public ControlFactory(IEnumerable<IAssemblyProvider> assemblyProviders, IEnumerable<IControlStrategyResolver> strategyResolvers, IServiceProvider serviceProvider, ILogger<ControlFactory> logger = null)
        {
            this.assemblyProviders = assemblyProviders?.ToList() ?? throw new ArgumentNullException(nameof(assemblyProviders));
            this.strategyResolvers = strategyResolvers?.ToList() ?? throw new ArgumentNullException(nameof(strategyResolvers));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this.logger = logger;
            this.pageCache = new Dictionary<IAppPage, IDictionary<(Type, string, IControl), IControl>>();
        }

        private IEnumerable<Type> AssemblyTypes
        {
            get
            {
                if (this.assemblyTypes is null)
                {
                    this.assemblyTypes = this.assemblyProviders.SelectMany(p => p.GetAssembly().GetTypes());
                }

                return this.assemblyTypes;
            }
        }

        private IEnumerable<Type> ControlTypes
        {
            get
            {
                if (this.controlTypes is null)
                {
                    this.controlTypes = Assembly
                        .GetAssembly(ControlInterfaceType)
                        .GetTypes()
                        .Where(t => ControlInterfaceType.IsAssignableFrom(t) && t.IsInterface)
                        .ToList();
                }

                return this.controlTypes;
            }
        }

        private IDictionary<Type, IControlRedirector<IControl>> RedirectorsMap
        {
            get
            {
                if (this.redirectorsMap is null)
                {
                    this.logger?.LogInformation("Getting control redirectors.");

                    var redirectorTypes = this.AssemblyTypes.Where(t => typeof(IControlRedirector<IControl>).IsAssignableFrom(t) && t.IsClass && t.IsVisible && !t.IsAbstract);
                    this.logger?.LogTrace("Found {count} redirector types.", redirectorTypes.Count());

                    this.logger?.LogTrace("Instantiating redirectors.");

                    var map = new Dictionary<Type, IControlRedirector<IControl>>();

                    foreach (var redirectorType in redirectorTypes)
                    {
                        var typeRedirected = redirectorType
                            .GetInterfaces()
                            .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IControlRedirector<>))
                            .GetGenericArguments()
                            .Last();

                        if (map.TryGetValue(typeRedirected, out IControlRedirector<IControl> value))
                        {
                            this.logger.LogWarning("Redirector of type {redirector} will not be used as {existingRedirector} is already registered for type {type}.", redirectorType.Name, value.GetType().Name, typeRedirected.Name);
                            continue;
                        }

                        map.Add(typeRedirected, (IControlRedirector<IControl>)ActivatorUtilities.CreateInstance(this.serviceProvider, redirectorType, this.RedirectionInfoProvider));
                    }

                    this.redirectorsMap = map;
                }

                return this.redirectorsMap;
            }
        }

        private IDictionary<Type, Type> StrategyMap
        {
            get
            {
                if (this.strategyMap is null)
                {
                    foreach (var resolver in this.strategyResolvers)
                    {
                        if (resolver.IsReady)
                        {
                            this.ProcessControlStrategyResolver(resolver);
                        }
                        else
                        {
                            resolver.OnReady += this.Resolver_OnReady;
                        }
                    }
                }

                return this.strategyMap;
            }
        }

        private IRedirectionInfoProvider<object> RedirectionInfoProvider
        {
            get
            {
                if (this.redirectionInfoProvider is null)
                {
                    this.logger?.LogInformation("Getting redirection info provider.");

                    var type = this.AssemblyTypes
                        .Where(t => typeof(IRedirectionInfoProvider<object>).IsAssignableFrom(t) && t.IsClass && t.IsVisible && !t.IsAbstract)
                        .FirstOrDefault() ?? throw new PowerPlaywrightException("A redirection info provider type was not found.");

                    this.logger?.LogTrace("Found {type}.", type.Name);

                    this.redirectionInfoProvider = (IRedirectionInfoProvider<object>)ActivatorUtilities.CreateInstance(this.serviceProvider, type);
                }

                return this.redirectionInfoProvider;
            }
        }

        /// <inheritdoc/>
        public TControl CreateInstance<TControl>(IAppPage appPage, string name = null, IControl parent = null)
            where TControl : IControl
        {
            var controlType = typeof(TControl);

            this.logger?.LogTrace("Checking for cached instance of {controlType}.", controlType);
            var controlCacheKey = (controlType, name, parent);
            if (this.pageCache.TryGetValue(appPage, out var controlCache) && controlCache.TryGetValue(controlCacheKey, out IControl cachedControl))
            {
                this.logger?.LogTrace("Control found in cache.");
                return (TControl)cachedControl;
            }

            this.logger?.LogTrace("Creating an instance of {controlType}.", controlType);

            if (this.RedirectorsMap.TryGetValue(controlType, out var redirector))
            {
                controlType = redirector.Redirect();
            }

            if (!this.StrategyMap.TryGetValue(controlType, out var strategyType) || strategyType == null)
            {
                throw new PowerPlaywrightException($"Unable to find a control strategy for type {controlType.Name}.");
            }

            var parameters = new List<object> { appPage };

            if (name != null)
            {
                parameters.Add(name);
            }

            if (parent != null)
            {
                parameters.Add(parent);
            }

            var control = (TControl)ActivatorUtilities.CreateInstance(this.serviceProvider, strategyType, parameters.ToArray());

            if (controlCache is null)
            {
                controlCache = new Dictionary<(Type, string, IControl), IControl>();
            }

            controlCache.Add(controlCacheKey, control);

            if (!this.pageCache.ContainsKey(appPage))
            {
                this.pageCache.Add(appPage, controlCache);
            }

            if (appPage is IAppPageInternal appPageInternal)
            {
                appPageInternal.OnDestroy += this.AppPage_OnDestroy;
            }

            return control;
        }

        /// <inheritdoc/>
        public async Task InitializeAsync(IPage page)
        {
            if (this.RedirectionInfoProvider is IAppLoadInitializable i)
            {
                await i.InitializeAsync(page);
            }
        }

        private void AppPage_OnDestroy(object sender, EventArgs e)
        {
            if (sender is IAppPage appPage && this.pageCache.ContainsKey(appPage))
            {
                this.pageCache.Remove(appPage);
            }
        }

        private void Resolver_OnReady(object sender, ResolverReadyEventArgs e)
        {
            this.ProcessControlStrategyResolver(e.Resolver);
        }

        private void ProcessControlStrategyResolver(IControlStrategyResolver resolver)
        {
            this.logger?.LogTrace("Processing control strategy resolver {resolver}.", resolver.GetType().Name);

            if (this.strategyMap == null)
            {
                this.strategyMap = this.ControlTypes.ToDictionary(c => c, c => (Type)null);
            }

            var controlsToResolve = this.strategyMap.Where(mapping => mapping.Value is null && resolver.IsResolvable(mapping.Key)).Select(mapping => mapping.Key);

            foreach (var control in controlsToResolve)
            {
                this.logger?.LogTrace("Getting control strategy for {control}.", control.Name);
                this.strategyMap[control] = resolver.Resolve(control, this.AssemblyTypes);

                if (this.strategyMap[control] is null)
                {
                    this.logger?.LogTrace("Unable to resolve a strategy for {control}.", control.Name);
                }
            }
        }
    }
}