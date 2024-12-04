namespace PowerPlaywright.UnitTests.Resolvers;

using System.Reflection;
using Bogus;
using Microsoft.Playwright;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using PowerPlaywright.Framework;
using PowerPlaywright.Framework.Controls.External;
using PowerPlaywright.Framework.Controls.Pcf;
using PowerPlaywright.Framework.Controls.Pcf.Attributes;
using PowerPlaywright.Resolvers;
using PowerPlaywright.Strategies.Controls.External;
using PowerPlaywright.Strategies.Controls.Pcf;
using PowerPlaywright.UnitTests.Helpers;

/// <summary>
/// Tests for the <see cref="PcfControlStrategyResolver"/> resolver.
/// </summary>
[TestFixture]
public class PcfControlStrategyResolverTests
{
    private Faker faker;

    private IPage page;
    private IEnvironmentInfoProvider environmentInfoProvider;
    private IDictionary<string, Version> controlVersions;

    private PcfControlStrategyResolver resolver;

    /// <summary>
    /// Sets up the resolver.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this.faker = new Faker();
        this.controlVersions = new Dictionary<string, Version>();
        this.page = Substitute.For<IPage>();
        this.environmentInfoProvider = Substitute.For<IEnvironmentInfoProvider>();
        this.resolver = new PcfControlStrategyResolver(this.environmentInfoProvider);

        this.MockValidDefaults();
    }

    /// <summary>
    /// Tests that the <see cref="PcfControlStrategyResolver.IsReady"/> property returns false after the resolver is constructed.
    /// </summary>
    [Test]
    public void IsReady_InitializeAsyncNotCalled_ReturnsFalse()
    {
        Assert.That(this.resolver.IsReady, Is.False);
    }

    /// <summary>
    /// Tests that the <see cref="PcfControlStrategyResolver.IsReady"/> property returns true after the resolver is initialised.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task IsReady_InitializeAsyncCalled_ReturnsTrue()
    {
        await this.resolver.InitializeAsync(this.page);

        Assert.That(this.resolver.IsReady, Is.True);
    }

    /// <summary>
    /// Tests that the <see cref="PcfControlStrategyResolver.OnReady"/> event is triggered by the <see cref="PcfControlStrategyResolver.InitialiseResolverAsync(IPage)"/> method when not already initialised.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task InitialiseAsync_InitializeAsyncNotCalled_TriggersOnReadyEvent()
    {
        this.resolver.OnReady += (sender, args) => Assert.Pass();

        await this.resolver.InitializeAsync(this.page);

        Assert.Fail($"The {nameof(PcfControlStrategyResolver.OnReady)} event was not trigged.");
    }

    /// <summary>
    /// Tests that the <see cref="PcfControlStrategyResolver.OnReady"/> event is not triggered by the <see cref="PcfControlStrategyResolver.InitialiseResolverAsync(IPage)"/> method when already initialised.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task InitialiseAsync_InitializeAsyncCalled_DoesNotTriggersOnReadyEvent()
    {
        await this.resolver.InitializeAsync(this.page);
        this.resolver.OnReady += (sender, args) => Assert.Fail();

        await this.resolver.InitializeAsync(this.page);
    }

    /// <summary>
    /// Tests that the <see cref="PcfControlStrategyResolver.OnReady"/> event is triggered by the <see cref="PcfControlStrategyResolver.InitialiseResolverAsync(IPage)"/> method.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OnReady_EventTriggered_SenderIsResolver()
    {
        this.resolver.OnReady += (sender, args) => Assert.That(sender, Is.EqualTo(this.resolver));

        await this.resolver.InitializeAsync(this.page);
    }

    /// <summary>
    /// Tests that the <see cref="PcfControlStrategyResolver.IsResolvable(Type)"/> method throws an <see cref="ArgumentNullException"/> when the type is null.
    /// </summary>
    [Test]
    public void IsResolvable_NullType_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => this.resolver.IsResolvable(null));
    }

    /// <summary>
    /// Tests that the <see cref="PcfControlStrategyResolver.IsResolvable(Type)"/> method returns true when the type has the <see cref="PcfControlAttribute"/> attribute.
    /// </summary>
    [Test]
    public void IsResolvable_TypeHasPcfControlAttribute_ReturnsTrue()
    {
        var typeWithPcfControlAttribute = new DynamicTypeBuilder()
            .AddAttribute<PcfControlAttribute>(["Microsoft.PowerApps.PowerAppsOneGrid"])
            .Build();

        var isResolvable = this.resolver.IsResolvable(typeWithPcfControlAttribute);

        Assert.That(isResolvable, Is.True);
    }

    /// <summary>
    /// Tests that the <see cref="PcfControlStrategyResolver.IsResolvable(Type)"/> method returns false when the type has the <see cref="PcfControlAttribute"/> attribute.
    /// </summary>
    [Test]
    public void IsResolvable_TypeDoesNotHavePcfControlAttribute_ReturnsFalse()
    {
        var typeWithoutPcfControlAttribute = new DynamicTypeBuilder().Build();

        var isResolvable = this.resolver.IsResolvable(typeWithoutPcfControlAttribute);

        Assert.That(isResolvable, Is.False);
    }

    /// <summary>
    /// Tests that the <see cref="PcfControlStrategyResolver.Resolve(Type, IEnumerable{Type})"/> method throws an <see cref="ArgumentNullException"/> when the control type is null.
    /// </summary>
    [Test]
    public void Resolve_NullControlType_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => this.resolver.Resolve(null, [typeof(PowerAppsOneGridControl)]));
    }

    /// <summary>
    /// Tests that the <see cref="PcfControlStrategyResolver.Resolve(Type, IEnumerable{Type})"/> method throws an <see cref="ArgumentNullException"/> when the strategy types are null.
    /// </summary>
    [Test]
    public void Resolve_NullStrategyTypes_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => this.resolver.Resolve(typeof(IPowerAppsOneGridControl), null));
    }

    /// <summary>
    /// Tests that the <see cref="PcfControlStrategyResolver.Resolve(Type, IEnumerable{Type})"/> method throws a <see cref="PowerPlaywrightException"/> when the control type does not have the <see cref="PcfControlAttribute"/> attribute.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Resolve_ControlTypeDoesNotHaveExternalControlAttribute_ThrowsPowerPlaywrightException()
    {
        await this.resolver.InitializeAsync(this.page);

        Assert.Throws<PowerPlaywrightException>(() => this.resolver.Resolve(typeof(ILoginControl), [typeof(PowerAppsOneGridControl)]));
    }

    /// <summary>
    /// Tests that a <see cref="PowerPlaywrightException"/> is thrown if <see cref="PcfControlStrategyResolver.Resolve(Type, IEnumerable{Type})"/> is called before the resolver is initialised via <see cref="PcfControlStrategyResolver.InitialiseResolverAsync(IPage)"/>.
    /// </summary>
    [Test]
    public void Resolve_InitialiseAsyncNotCalled_ThrowsPowerPlaywrightException()
    {
        this.environmentInfoProvider.ControlVersions.ReturnsNull<IDictionary<string, Version>>();

        Assert.Throws<PowerPlaywrightException>(() => this.resolver.Resolve(typeof(IPowerAppsOneGridControl), []));
    }

    /// <summary>
    /// Tests that the <see cref="PcfControlStrategyResolver.Resolve(Type, IEnumerable{Type})"/> method returns public, non-abstract, concrete types that implement the control type interface and have the <see cref="PcfControlStrategyAttribute"/> attribute.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Resolve_StrategyTypesContainsPublicNonAbstractConcreteTypeWithPcfControlStrategyAttributeVersionLessThanOrEqualToEnvironmentControlVersion_ReturnsType()
    {
        var strategyVersion = this.faker.System.Version();
        var expectedStrategyType = this.GetPcfControlStrategyTypeFor<IPowerAppsOneGridControl>(out var controlName, strategyVersion);
        this.controlVersions[controlName] = strategyVersion;
        await this.resolver.InitializeAsync(this.page);

        var strategyType = this.resolver.Resolve(typeof(IPowerAppsOneGridControl), [expectedStrategyType]);

        Assert.That(strategyType, Is.EqualTo(expectedStrategyType));
    }

    /// <summary>
    /// Tests that the <see cref="PcfControlStrategyResolver.Resolve(Type, IEnumerable{Type})"/> method returns the type with the highest version set in the <see cref="PcfControlStrategyAttribute"/> attribute when multiple matching strategy types are found.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Resolve_MultipleStrategyTypesContainsPublicNonAbstractConcreteTypeWithPcfControlStrategyAttribute_ReturnsTypeWithHighestVersionLowerThanEnvironmentVersion()
    {
        var lowerStrategyVersion = this.faker.System.Version();
        var unexpectedStrategyType = this.GetPcfControlStrategyTypeFor<IPowerAppsOneGridControl>(out var controlName, lowerStrategyVersion);
        var higherStrategyVersion = new Version(lowerStrategyVersion.Major, lowerStrategyVersion.Minor, lowerStrategyVersion.Build + 1);
        var expectedStrategyType = this.GetPcfControlStrategyTypeFor<IPowerAppsOneGridControl>(out _, higherStrategyVersion);
        this.controlVersions[controlName] = higherStrategyVersion;
        await this.resolver.InitializeAsync(this.page);

        var strategyType = this.resolver.Resolve(typeof(IPowerAppsOneGridControl), [unexpectedStrategyType, expectedStrategyType]);

        Assert.That(strategyType, Is.EqualTo(expectedStrategyType));
    }

    /// <summary>
    /// Tests that the <see cref="PcfControlStrategyResolver.Resolve(Type, IEnumerable{Type})"/> method returns null when no public, non-abstract, concrete types that implement the control type interface and have the <see cref="PcfControlStrategyAttribute"/> attribute are found.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Resolve_StrategyTypesDoesNotContainConcreteTypeWithPcfControlStrategyAttribute_ReturnsNull()
    {
        await this.resolver.InitializeAsync(this.page);

        var strategyType = this.resolver.Resolve(typeof(IPowerAppsOneGridControl), [typeof(LoginControl)]);

        Assert.That(strategyType, Is.Null);
    }

    private Type GetPcfControlStrategyTypeFor<TControlType>(out string controlName, Version strategyVersion)
        where TControlType : IPcfControl
    {
        var controlAttribute = typeof(TControlType).GetCustomAttribute<PcfControlAttribute>()
            ?? throw new Exception($"The provided type of {typeof(TControlType).Name} is not a valid PCF control");
        controlName = controlAttribute.Name;

        return new DynamicTypeBuilder()
               .AddInterfaceImplementation<TControlType>()
               .AddAttribute<PcfControlStrategyAttribute>([(uint)strategyVersion.Major, (uint)strategyVersion.Minor, (uint)strategyVersion.Build])
               .Build();
    }

    private void MockValidDefaults()
    {
        this.page.Url
            .Returns($"https://{Guid.NewGuid()}.crm.dynamics.com");

        this.MockControlVersion();
    }

    private void MockControlVersion()
    {
        this.environmentInfoProvider.ControlVersions.Returns(this.controlVersions);
    }
}