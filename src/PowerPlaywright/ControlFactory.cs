namespace PowerPlaywright
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Framework.Redirectors;
    using PowerPlaywright.Resolvers;

    /// <summary>
    /// A control factory that dynamically discovers controls at runtime from a remote source.
    /// </summary>
    internal class ControlFactory : IControlFactory
    {
        private static readonly Type ControlInterfaceType = typeof(IControl);

        private readonly IEnumerable<IAssemblyProvider> assemblyProviders;
        private readonly IEnumerable<IControlStrategyResolver> strategyResolvers;
        private readonly IRedirectionInfoProvider redirectionInfoProvider;
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<ControlFactory> logger;

        private IEnumerable<Type> assemblyTypes;
        private IEnumerable<Type> controlTypes;
        private Dictionary<Type, IControlRedirector<IControl>> redirectorsMap;
        private IDictionary<Type, Type> strategyMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlFactory"/> class.
        /// </summary>
        /// <param name="assemblyProviders">The assembly providers.</param>
        /// <param name="strategyResolvers">The control strategy resolvers.</param>
        /// <param name="redirectionInfoProvider">The redirection info provider.</param>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="logger">The logger.</param>
        public ControlFactory(
            IEnumerable<IAssemblyProvider> assemblyProviders,
            IEnumerable<IControlStrategyResolver> strategyResolvers,
            IRedirectionInfoProvider redirectionInfoProvider,
            IServiceProvider serviceProvider,
            ILogger<ControlFactory> logger = null)
        {
            this.assemblyProviders = assemblyProviders ?? throw new ArgumentNullException(nameof(assemblyProviders));
            this.strategyResolvers = strategyResolvers ?? throw new ArgumentNullException(nameof(strategyResolvers));
            this.redirectionInfoProvider = redirectionInfoProvider ?? throw new ArgumentNullException(nameof(redirectionInfoProvider));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this.logger = logger;
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
                    this.controlTypes = ControlInterfaceType.Assembly.GetTypes().Concat(this.AssemblyTypes).Where(IsControlType).ToList();
                }

                return this.controlTypes;
            }
        }

        private Dictionary<Type, IControlRedirector<IControl>> RedirectorsMap
        {
            get
            {
                if (this.redirectorsMap is null)
                {
                    this.logger.LogInformation("Getting control redirectors.");
                    this.redirectorsMap = new Dictionary<Type, IControlRedirector<IControl>>();

                    var redirectorTypes = this.AssemblyTypes.Where(IsRedirectorType);
                    this.logger.LogTrace("Found {count} redirector types.", redirectorTypes.Count());

                    foreach (var redirectorType in redirectorTypes)
                    {
                        var typeRedirected = GetRedirectedType(redirectorType);

                        if (this.redirectorsMap.TryGetValue(typeRedirected, out IControlRedirector<IControl> value))
                        {
                            throw new PowerPlaywrightException($"A redirector of type {value} is already registered for {typeRedirected}. Unable to register {redirectorType}.");
                        }

                        this.logger.LogTrace("Instantiating redirector of type {redirector}.", redirectorType);
                        var redirector = (IControlRedirector<IControl>)ActivatorUtilities.CreateInstance(
                            this.serviceProvider, redirectorType, this.redirectionInfoProvider);

                        this.redirectorsMap.Add(typeRedirected, redirector);
                    }
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
                    this.strategyMap = this.ControlTypes.ToDictionary(c => c, c => (Type)null);

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

        /// <inheritdoc/>
        public TControl CreateInstance<TControl>(IAppPage appPage, string name = null, IControl parent = null)
            where TControl : IControl
        {
            if (appPage is null)
            {
                throw new ArgumentNullException(nameof(appPage));
            }

            var controlType = typeof(TControl);
            this.logger.LogTrace("Creating an instance of {controlType}.", controlType);

            if (this.TryRedirectControlType(ref controlType))
            {
                this.logger.LogTrace("Type redirected to {controlType}.", controlType);
            }

            var strategyType = this.GetStrategyType(controlType);
            this.logger.LogTrace("Found strategy type {strategyType}.", strategyType);

            var parameters = GetStrategyTypeParameters(appPage, name, parent);
            this.logger.LogTrace("Parameters: {parameters}.", string.Join(", ", parameters));

            var strategy = (TControl)ActivatorUtilities.CreateInstance(
                this.serviceProvider, strategyType, parameters);
            this.logger.LogTrace("Created instance of {controlType}.", strategyType);

            return strategy;
        }

        private static Type GetRedirectedType(Type redirectorType)
        {
            return redirectorType
                .GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IControlRedirector<>))
                .GetGenericArguments()
                .Last();
        }

        private static bool IsControlType(Type t)
        {
            return ControlInterfaceType.IsAssignableFrom(t) && t.IsInterface && t.IsPublic;
        }

        private static bool IsRedirectorType(Type t)
        {
            return typeof(IControlRedirector<IControl>).IsAssignableFrom(t) && t.IsClass && t.IsVisible && !t.IsAbstract;
        }

        private static object[] GetStrategyTypeParameters(IAppPage appPage, string name, IControl parent)
        {
            var parameters = new List<object> { appPage };

            if (name != null)
            {
                parameters.Add(name);
            }

            if (parent != null)
            {
                parameters.Add(parent);
            }

            return parameters.ToArray();
        }

        private Type GetStrategyType(Type controlType)
        {
            var key = controlType.IsConstructedGenericType ?
                controlType.GetGenericTypeDefinition() : controlType;

            if (!this.StrategyMap.TryGetValue(key, out var strategyType))
            {
                throw new PowerPlaywrightException($"Type {controlType.Name} is not a valid control interface type.");
            }

            if (strategyType is null)
            {
                throw new PowerPlaywrightException($"Unable to find a control strategy for type {controlType.Name}.");
            }

            if (strategyType.IsGenericTypeDefinition)
            {
                strategyType = strategyType.MakeGenericType(controlType.GenericTypeArguments);
            }

            return strategyType;
        }

        private bool TryRedirectControlType(ref Type controlType)
        {
            if (this.RedirectorsMap.TryGetValue(controlType, out var redirector))
            {
                controlType = redirector.Redirect();

                return true;
            }

            return false;
        }

        private void Resolver_OnReady(object sender, EventArgs e)
        {
            this.ProcessControlStrategyResolver((IControlStrategyResolver)sender);
        }

        private void ProcessControlStrategyResolver(IControlStrategyResolver resolver)
        {
            this.logger.LogTrace("Processing control strategy resolver {resolver}.", resolver.GetType().Name);

            var controlsToResolve = this.strategyMap
                .Where(mapping => mapping.Value is null && resolver.IsResolvable(mapping.Key))
                .Select(mapping => mapping.Key)
                .ToList();

            foreach (var control in controlsToResolve)
            {
                this.logger.LogTrace("Getting control strategy for {control}.", control.Name);
                this.strategyMap[control] = resolver.Resolve(control, this.AssemblyTypes);

                if (this.strategyMap[control] is null)
                {
                    this.logger.LogTrace("Unable to resolve a strategy for {control}.", control.Name);
                }
            }
        }
    }
}