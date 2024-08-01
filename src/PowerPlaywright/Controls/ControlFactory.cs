namespace PowerPlaywright.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.Json;
    using System.Text.Json.Nodes;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Playwright;
    using PowerPlaywright;
    using PowerPlaywright.Events;
    using PowerPlaywright.Model;
    using PowerPlaywright.Model.Controls;
    using PowerPlaywright.Model.Redirectors;
    using PowerPlaywright.Notifications;

    /// <summary>
    /// A control factory that dynamically discovers controls at runtime from a remote source.
    /// </summary>
    internal class ControlFactory : IControlFactory
    {
        private static readonly Type ControlInterfaceType = typeof(IControl);

        private readonly IList<IControlStrategyAssemblyProvider> assemblyProviders;
        private readonly IList<IControlStrategyResolver> strategyResolvers;
        private readonly ISet<IControlStrategyResolver> processedStrategyResolvers;
        private readonly IServiceProvider serviceProvider;
        private readonly IEventAggregator eventAggregator;
        private readonly ILogger<ControlFactory> logger;

        private IEnumerable<Type> assemblyTypes;
        private IEnumerable<Type> controlTypes;
        private IDictionary<Type, IControlRedirector<IControl>> redirectorsMap;
        private IDictionary<Type, Type> strategyMap;
        private ControlRedirectionInfo redirectionInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlFactory"/> class.
        /// </summary>
        /// <param name="assemblyProviders">The control assembly provider(s).</param>
        /// <param name="strategyResolvers">The control strategy resolver(s).</param>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="logger">The logger.</param>
        public ControlFactory(IEnumerable<IControlStrategyAssemblyProvider> assemblyProviders, IEnumerable<IControlStrategyResolver> strategyResolvers, IServiceProvider serviceProvider, IEventAggregator eventAggregator, ILogger<ControlFactory> logger = null)
        {
            this.assemblyProviders = assemblyProviders?.ToList() ?? throw new ArgumentNullException(nameof(assemblyProviders));
            this.strategyResolvers = strategyResolvers?.ToList() ?? throw new ArgumentNullException(nameof(strategyResolvers));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            this.logger = logger;

            this.processedStrategyResolvers = new HashSet<IControlStrategyResolver>();

            this.eventAggregator.Subscribe<AppInitializedEvent>(this.OnAppInitialize);
            this.eventAggregator.Subscribe<ResolverReadyEvent>(this.OnResolverReady);
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
                            .First();

                        if (map.TryGetValue(typeRedirected, out IControlRedirector<IControl> value))
                        {
                            this.logger.LogWarning("Redirector of type {redirector} will not be used as {existingRedirector} is already registered for type {type}.", redirectorType.Name, value.GetType().Name, typeRedirected.Name);
                            continue;
                        }

                        map.Add(typeRedirected, (IControlRedirector<IControl>)ActivatorUtilities.CreateInstance(this.serviceProvider, redirectorType));
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
                    var readyResolvers = this.strategyResolvers.Where(r => r.IsReady);

                    foreach (var resolver in readyResolvers)
                    {
                        this.ProcessControlStrategyResolver(resolver);
                    }
                }

                return this.strategyMap;
            }
        }

        /// <inheritdoc/>
        public TControl CreateInstance<TControl>(IPage page, string name = null)
            where TControl : IControl
        {
            var type = typeof(TControl);

            this.logger?.LogTrace("Creating an instance of {type}.", type);

            if (this.RedirectorsMap.TryGetValue(type, out var redirectors))
            {
                type = redirectors.Redirect(this.redirectionInfo, this.ControlTypes);
            }

            if (!this.StrategyMap.TryGetValue(type, out var strategyType) || strategyType == null)
            {
                throw new PowerPlaywrightException($"Unable to find a control strategy for type {type.Name}.");
            }

            var parameters = new List<object> { page };

            if (name != null)
            {
                parameters.Add(name);
            }

            return (TControl)ActivatorUtilities.CreateInstance(this.serviceProvider, strategyType, parameters.ToArray());
        }

        private Task OnResolverReady(ResolverReadyEvent notification)
        {
            this.ProcessControlStrategyResolver(notification.Resolver);

            return Task.CompletedTask;
        }

        private async Task OnAppInitialize(AppInitializedEvent @event)
        {
            this.redirectionInfo = await this.GetControlRedirectionInfoAsync(@event.HomePage.Page);
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

            this.processedStrategyResolvers.Add(resolver);
        }

        private async Task<ControlRedirectionInfo> GetControlRedirectionInfoAsync(IPage page)
        {
            var appId = await page.EvaluateAsync<Guid>("async () => { " +
                "   var properties = await Xrm.Utility.getGlobalContext().getCurrentAppProperties();" +
                "   return properties.appId" +
                "}");
            var userId = await page.EvaluateAsync<Guid>("Xrm.Utility.getGlobalContext().userSettings.userId");
            var userSettings = await page.EvaluateAsync("async (userId) => Xrm.WebApi.online.retrieveRecord('usersettings', userId, '?$select=trytogglesets')", userId);
            var toggleSetsString = userSettings.Value.GetProperty("trytogglesets").GetString();

            if (JsonNode.Parse(toggleSetsString).AsObject().TryGetPropertyValue(appId.ToString(), out var appTogglesJson))
            {
                return new ControlRedirectionInfo(appTogglesJson.Deserialize<AppToggles>());
            }

            return new ControlRedirectionInfo(null);
        }
    }
}
