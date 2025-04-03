namespace PowerPlaywright.UnitTests;

using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using NSubstitute;
using PowerPlaywright.Framework;
using PowerPlaywright.Framework.Controls;
using PowerPlaywright.Framework.Controls.External;
using PowerPlaywright.Framework.Controls.Pcf;
using PowerPlaywright.Framework.Controls.Pcf.Classes;
using PowerPlaywright.Framework.Pages;
using PowerPlaywright.Framework.Redirectors;
using PowerPlaywright.Resolvers;
using PowerPlaywright.Strategies.Controls.External;
using PowerPlaywright.Strategies.Controls.Pcf;
using PowerPlaywright.Strategies.Redirectors;

/// <summary>
/// Tests for the <see cref="ControlFactory"/> class.
/// </summary>
[TestFixture]
public class ControlFactoryTests
{
    private Dictionary<Type, Type?> resolvedTypes;
    private RedirectionInfo redirectionInfo;
    private IAssemblyProvider assemblyProvider;
    private IControlStrategyResolver strategyResolver;
    private IRedirectionInfoProvider redirectionInfoProvider;
    private IServiceProvider serviceProvider;
    private ILogger<ControlFactory> logger;

    private ControlFactory controlFactory;

    /// <summary>
    /// Sets up the test by creating a new instance of <see cref="ControlFactory"/> with the mocked dependencies.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this.resolvedTypes = [];
        this.redirectionInfo = new RedirectionInfo(new OrgSettings(), new AppSettings(), new UserSettings());

        this.assemblyProvider = Substitute.For<IAssemblyProvider>();
        this.strategyResolver = Substitute.For<IControlStrategyResolver>();
        this.redirectionInfoProvider = Substitute.For<IRedirectionInfoProvider>();

        this.serviceProvider = Substitute.For<IServiceProvider>();
        this.logger = Substitute.For<ILogger<ControlFactory>>();

        this.controlFactory = new ControlFactory(
            [this.assemblyProvider], [this.strategyResolver], this.redirectionInfoProvider, this.serviceProvider, this.logger);

        this.MockValidDefaults();
    }

    /// <summary>
    /// Tests that the <see cref="ControlFactory"/> constructor throws an <see cref="ArgumentNullException"/> when the assemblyProviders is null.
    /// </summary>
    [Test]
    public void Constructor_AssemblyProvidersNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ControlFactory(null, [this.strategyResolver], this.redirectionInfoProvider, this.serviceProvider, this.logger));
    }

    /// <summary>
    /// Tests that the <see cref="ControlFactory"/> constructor throws an <see cref="ArgumentNullException"/> when the strategyResolvers is null.
    /// </summary>
    [Test]
    public void Constructor_StrategyResolversNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ControlFactory([this.assemblyProvider], null, this.redirectionInfoProvider, this.serviceProvider, this.logger));
    }

    /// <summary>
    /// Tests that the <see cref="ControlFactory"/> constructor throws an <see cref="ArgumentNullException"/> when the redirectionInfoProvider is null.
    /// </summary>
    [Test]
    public void Constructor_RedirectionInfoProviderNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ControlFactory([this.assemblyProvider], [this.strategyResolver], null, this.serviceProvider, this.logger));
    }

    /// <summary>
    /// Tests that the <see cref="ControlFactory"/> constructor throws an <see cref="ArgumentNullException"/> when the serviceProvider is null.
    /// </summary>
    [Test]
    public void Constructor_ServiceProviderNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ControlFactory([this.assemblyProvider], [this.strategyResolver], this.redirectionInfoProvider, null, this.logger));
    }

    /// <summary>
    /// Tests that the <see cref="ControlFactory.CreateInstance{TControl}(IAppPage, string, IControl)"/> method throws an <see cref="ArgumentNullException"/> when the appPage is null.
    /// </summary>
    [Test]
    public void CreateInstance_NullAppPage_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => this.controlFactory.CreateInstance<ILoginControl>(null));
    }

    /// <summary>
    /// /// Tests that the <see cref="ControlFactory.CreateInstance{TControl}(IAppPage, string, IControl)"/> method throws a <see cref="PowerPlaywrightException"/> when the strategyResolvers have not resolved the control type from the strategy assemblies.
    /// </summary>
    [Test]
    public void CreateInstance_ResolverHasNotResolvedControlTypeFromStrategyAssemblies_ThrowsPowerPlaywrightException()
    {
        this.resolvedTypes = new Dictionary<Type, Type?>()
        {
            { typeof(ILoginControl), null },
        };

        Assert.Throws<PowerPlaywrightException>(() => this.controlFactory.CreateInstance<ILoginControl>(Substitute.For<IAppPage>()));
    }

    /// <summary>
    /// Tests that the <see cref="ControlFactory.CreateInstance{TControl}(IAppPage, string, IControl)"/> method creates an instance of the resolved control type when the strategyResolvers have resolved the control type from the strategy assemblies.
    /// </summary>
    [Test]
    public void CreateInstance_ResolverHasResolvedControlTypeFromStrategyAssemblies_CreatesInstanceForResolvedType()
    {
        var expectedControl = typeof(LoginControl);
        this.resolvedTypes = new Dictionary<Type, Type?>()
        {
            { typeof(ILoginControl), expectedControl },
        };

        var actualControl = this.controlFactory.CreateInstance<ILoginControl>(Substitute.For<IAppPage>());

        Assert.That(actualControl, Is.InstanceOf(expectedControl));
    }

    /// <summary>
    /// Tests that the <see cref="ControlFactory.CreateInstance{TControl}(IAppPage, string, IControl)"/> method creates an instance of the redirected control type when the strategyResolvers have resolved the control type from the strategy assemblies and a control redirection is found for the control type in the strategy assemblies.
    /// </summary>
    [Test]
    public void CreateInstance_RedirectorFoundForControlTypeInStrategyAssemblies_CreatesInstanceForRedirectedType()
    {
        var actualControl = this.controlFactory.CreateInstance<IReadOnlyGrid>(Substitute.For<IAppPage>(), "name");

        Assert.That(actualControl, Is.InstanceOf<IPowerAppsOneGridControl>());
    }

    /// <summary>
    /// Tests that the <see cref="ControlFactory.CreateInstance{TControl}(IAppPage, string, IControl)"/> method throws a <see cref="PowerPlaywrightException"/> when multiple redirectors are found for the control type in the strategy assemblies.
    /// </summary>
    [Test]
    public void CreateInstance_MultipleRedirectorsFoundForControlTypeInStrategyAssemblies_ThrowsPowerPlaywrightException()
    {
        // TODO: Implement this test after refactoring these tests to use the DynamicTypeBuilder.
    }

    /// <summary>
    /// Tests that the <see cref="ControlFactory.CreateInstance{TControl}(IAppPage, string, IControl)"/> method throws a <see cref="PowerPlaywrightException"/> when the strategyResolvers are not ready.
    /// </summary>
    [Test]
    public void CreateInstance_ResolverNotReady_DoesNotResolve()
    {
        this.strategyResolver.IsReady.Returns(false);

        Assert.Throws<PowerPlaywrightException>(
            () => this.controlFactory.CreateInstance<IPowerAppsOneGridControl>(Substitute.For<IAppPage>(), "name"));
    }

    /// <summary>
    /// Tests that the <see cref="ControlFactory.CreateInstance{TControl}(IAppPage, string, IControl)"/> method throws a <see cref="PowerPlaywrightException"/> when the strategyResolvers are not ready.
    /// </summary>
    [Test]
    public void CreateInstance_ResolverReadyPostStrategyInitialisation_CreatesInstanceForResolvedType()
    {
        this.strategyResolver.IsReady.Returns(false);
        try
        {
            this.controlFactory.CreateInstance<IPowerAppsOneGridControl>(Substitute.For<IAppPage>(), "name");
        }
        catch (PowerPlaywrightException)
        {
            this.strategyResolver.OnReady += Raise.Event();
        }

        var actualControl = this.controlFactory.CreateInstance<IPowerAppsOneGridControl>(Substitute.For<IAppPage>(), "name");

        Assert.That(actualControl, Is.InstanceOf(typeof(PowerAppsOneGridControl)));
    }

    /// <summary>
    /// Tests that the <see cref="ControlFactory.CreateInstance{TControl}(IAppPage, string, IControl)"/> method throws a <see cref="PowerPlaywrightException"/> when the control type is not found in the framework assembly.
    /// </summary>
    [Test]
    public void CreateInstance_ControlTypeNotFoundInFrameworkAssemblys_ThrowsPowerPlaywrightException()
    {
        Assert.Throws<PowerPlaywrightException>(() => this.controlFactory.CreateInstance<IUnrecognisedControl>(Substitute.For<IAppPage>()));
    }

    /// <summary>
    /// Tests that the <see cref="ControlFactory.CreateInstance{TControl}(IAppPage, string, IControl)"/> method passes the provided name as a constructor argument to the resolved control type.
    /// </summary>
    [Test]
    public void CreateInstance_NameProvided_PassesNameAsConstructorArgument()
    {
        var expectedName = "ControlName";

        var actualControl = this.controlFactory.CreateInstance<IPowerAppsOneGridControl>(Substitute.For<IAppPage>(), expectedName);

        Assert.That(actualControl.Name, Is.EqualTo(expectedName));
    }

    /// <summary>
    /// Tests that the <see cref="ControlFactory.CreateInstance{TControl}(IAppPage, string, IControl)"/> method passes the provided parent as a constructor argument to the resolved control type.
    /// </summary>
    [Test]
    public void CreateInstance_ParentProvided_PassesParentAsConstructorArgument()
    {
        var expectedParent = Substitute.For<IControl>();

        var actualControl = this.controlFactory.CreateInstance<IPowerAppsOneGridControl>(Substitute.For<IAppPage>(), "name", expectedParent);

        Assert.That(actualControl.Parent, Is.EqualTo(expectedParent));
    }

    private void MockValidDefaults()
    {
        this.resolvedTypes
            .Add(typeof(IPowerAppsOneGridControl), typeof(PowerAppsOneGridControl));

        // TODO: Refactor tests to use DynamicTypeBuilder
        this.assemblyProvider.GetAssembly()
            .Returns(typeof(PowerAppsOneGridControl).Assembly);
        this.serviceProvider.GetService(Arg.Is<Type>(t => t != typeof(string)))
            .Returns(i => Substitute.For(i.Args().Cast<Type>().ToArray(), []));
        this.strategyResolver.IsReady
            .Returns(true);
        this.strategyResolver.IsResolvable(Arg.Any<Type>())
            .Returns(true);
        this.strategyResolver.Resolve(Arg.Any<Type>(), Arg.Any<IEnumerable<Type>>())
            .Returns((i) => this.resolvedTypes.ContainsKey(i.Arg<Type>()) ? this.resolvedTypes[i.Arg<Type>()] : null);
        this.redirectionInfoProvider.GetRedirectionInfo()
            .Returns((i) => this.redirectionInfo);
    }

    private class IUnrecognisedControl : IControl
    {
        public IControl Parent => throw new NotImplementedException();

        public ILocator Container => throw new NotImplementedException();
    }
}