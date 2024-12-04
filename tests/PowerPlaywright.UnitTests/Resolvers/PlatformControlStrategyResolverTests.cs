namespace PowerPlaywright.UnitTests.Resolvers;

using Bogus;
using Microsoft.Playwright;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using PowerPlaywright.Framework;
using PowerPlaywright.Framework.Controls.External;
using PowerPlaywright.Framework.Controls.Platform;
using PowerPlaywright.Framework.Controls.Platform.Attributes;
using PowerPlaywright.Resolvers;
using PowerPlaywright.Strategies.Controls.External;
using PowerPlaywright.Strategies.Controls.Platform;
using PowerPlaywright.UnitTests.Helpers;

/// <summary>
/// Tests for the <see cref="PlatformControlStrategyResolver"/> resolver.
/// </summary>
[TestFixture]
public class PlatformControlStrategyResolverTests
{
    private Faker faker;

    private IPage page;
    private IEnvironmentInfoProvider environmentInfoProvider;
    private Version platformVersion;

    private PlatformControlStrategyResolver resolver;

    /// <summary>
    /// Sets up the resolver.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this.faker = new Faker();
        this.page = Substitute.For<IPage>();
        this.environmentInfoProvider = Substitute.For<IEnvironmentInfoProvider>();
        this.resolver = new PlatformControlStrategyResolver(this.environmentInfoProvider);

        this.MockValidDefaults();
    }

    /// <summary>
    /// Tests that the <see cref="PlatformControlStrategyResolver.IsReady"/> property returns false after the resolver is constructed.
    /// </summary>
    [Test]
    public void IsReady_InitializeAsyncNotCalled_ReturnsFalse()
    {
        Assert.That(this.resolver.IsReady, Is.False);
    }

    /// <summary>
    /// Tests that the <see cref="PlatformControlStrategyResolver.IsReady"/> property returns true after the resolver is initialised.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task IsReady_InitializeAsyncCalled_ReturnsTrue()
    {
        await this.resolver.InitializeAsync(this.page);

        Assert.That(this.resolver.IsReady, Is.True);
    }

    /// <summary>
    /// Tests that the <see cref="PlatformControlStrategyResolver.OnReady"/> event is triggered by the <see cref="PlatformControlStrategyResolver.InitialiseResolverAsync(IPage)"/> method when not already initialised.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task InitialiseAsync_InitializeAsyncNotCalled_TriggersOnReadyEvent()
    {
        this.resolver.OnReady += (sender, args) => Assert.Pass();

        await this.resolver.InitializeAsync(this.page);

        Assert.Fail($"The {nameof(PlatformControlStrategyResolver.OnReady)} event was not trigged.");
    }

    /// <summary>
    /// Tests that the <see cref="PlatformControlStrategyResolver.OnReady"/> event is not triggered by the <see cref="PlatformControlStrategyResolver.InitialiseResolverAsync(IPage)"/> method when already initialised.
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
    /// Tests that the <see cref="PlatformControlStrategyResolver.OnReady"/> event is triggered by the <see cref="PlatformControlStrategyResolver.InitialiseResolverAsync(IPage)"/> method.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OnReady_EventTriggered_SenderIsResolver()
    {
        this.resolver.OnReady += (sender, args) => Assert.That(sender, Is.EqualTo(this.resolver));

        await this.resolver.InitializeAsync(this.page);
    }

    /// <summary>
    /// Tests that the <see cref="PlatformControlStrategyResolver.IsResolvable(Type)"/> method throws an <see cref="ArgumentNullException"/> when the type is null.
    /// </summary>
    [Test]
    public void IsResolvable_NullType_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => this.resolver.IsResolvable(null));
    }

    /// <summary>
    /// Tests that the <see cref="PlatformControlStrategyResolver.IsResolvable(Type)"/> method returns true when the type has the <see cref="PlatformControlAttribute"/> attribute.
    /// </summary>
    [Test]
    public void IsResolvable_TypeHasPlatformControlAttribute_ReturnsTrue()
    {
        var typeWithPlatformControlAttribute = new DynamicTypeBuilder()
                .AddAttribute<PlatformControlAttribute>([])
                .Build();

        var isResolvable = this.resolver.IsResolvable(typeWithPlatformControlAttribute);

        Assert.That(isResolvable, Is.True);
    }

    /// <summary>
    /// Tests that the <see cref="PlatformControlStrategyResolver.IsResolvable(Type)"/> method returns false when the type has the <see cref="PlatformControlAttribute"/> attribute.
    /// </summary>
    [Test]
    public void IsResolvable_TypeDoesNotHavePlatformControlAttribute_ReturnsFalse()
    {
        var typeWithoutPlatformControlAttribute = new DynamicTypeBuilder().Build();

        var isResolvable = this.resolver.IsResolvable(typeWithoutPlatformControlAttribute);

        Assert.That(isResolvable, Is.False);
    }

    /// <summary>
    /// Tests that the <see cref="PlatformControlStrategyResolver.Resolve(Type, IEnumerable{Type})"/> method throws an <see cref="ArgumentNullException"/> when the control type is null.
    /// </summary>
    [Test]
    public void Resolve_NullControlType_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => this.resolver.Resolve(null, [typeof(SiteMapControl)]));
    }

    /// <summary>
    /// Tests that the <see cref="PlatformControlStrategyResolver.Resolve(Type, IEnumerable{Type})"/> method throws a <see cref="PowerPlaywrightException"/> when the control type does not have the <see cref="PlatformControlAttribute"/> attribute.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Resolve_ControlTypeDoesNotHaveExternalControlAttribute_ThrowsPowerPlaywrightException()
    {
        await this.resolver.InitializeAsync(this.page);

        Assert.Throws<PowerPlaywrightException>(() => this.resolver.Resolve(typeof(ILoginControl), [typeof(ClientApi)]));
    }

    /// <summary>
    /// Tests that the <see cref="PlatformControlStrategyResolver.Resolve(Type, IEnumerable{Type})"/> method throws an <see cref="ArgumentNullException"/> when the strategy types are null.
    /// </summary>
    [Test]
    public void Resolve_NullStrategyTypes_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => this.resolver.Resolve(typeof(IClientApi), null));
    }

    /// <summary>
    /// Tests that a <see cref="PowerPlaywrightException"/> is thrown if <see cref="PlatformControlStrategyResolver.Resolve(Type, IEnumerable{Type})"/> is called before the resolver is initialised via <see cref="PlatformControlStrategyResolver.InitialiseResolverAsync(IPage)"/>.
    /// </summary>
    [Test]
    public void Resolve_InitialiseAsyncNotCalled_ThrowsPowerPlaywrightException()
    {
        this.environmentInfoProvider.PlatformVersion.ReturnsNull();

        Assert.Throws<PowerPlaywrightException>(() => this.resolver.Resolve(typeof(IClientApi), []));
    }

    /// <summary>
    /// Tests that the <see cref="PlatformControlStrategyResolver.Resolve(Type, IEnumerable{Type})"/> method returns public, non-abstract, concrete types that implement the control type interface and have the <see cref="PlatformControlStrategyAttribute"/> attribute.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Resolve_StrategyTypesContainsPublicNonAbstractConcreteTypeWithPlatformControlStrategyAttributeLessThanOrEqualToEnvironmentVersion_ReturnsType()
    {
        var strategyVersion = this.faker.System.Version();
        var expectedStrategyType = this.GetPlatformControlStrategyTypeFor<IClientApi>(strategyVersion);
        this.platformVersion = strategyVersion;
        await this.resolver.InitializeAsync(this.page);

        var strategyType = this.resolver.Resolve(typeof(IClientApi), [expectedStrategyType]);

        Assert.That(strategyType, Is.EqualTo(expectedStrategyType));
    }

    /// <summary>
    /// Tests that the <see cref="PlatformControlStrategyResolver.Resolve(Type, IEnumerable{Type})"/> method returns the type with the highest version set in the <see cref="PlatformControlStrategyAttribute"/> attribute when multiple matching strategy types are found.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Resolve_MultipleStrategyTypesContainsPublicNonAbstractConcreteTypeWithPlatformControlStrategyAttribute_ReturnsTypeWithHighestVersionLowerThanEnvironmentVersion()
    {
        var lowerStrategyVersion = this.faker.System.Version();
        var unexpectedStrategyType = this.GetPlatformControlStrategyTypeFor<IClientApi>(lowerStrategyVersion);
        var higherStrategyVersion = new Version(lowerStrategyVersion.Major, lowerStrategyVersion.Minor, lowerStrategyVersion.Build + 1, lowerStrategyVersion.Revision);
        var expectedStrategyType = this.GetPlatformControlStrategyTypeFor<IClientApi>(higherStrategyVersion);
        this.platformVersion = higherStrategyVersion;
        await this.resolver.InitializeAsync(this.page);

        var strategyType = this.resolver.Resolve(typeof(IClientApi), [unexpectedStrategyType, expectedStrategyType]);

        Assert.That(strategyType, Is.EqualTo(expectedStrategyType));
    }

    /// <summary>
    /// Tests that the <see cref="PlatformControlStrategyResolver.Resolve(Type, IEnumerable{Type})"/> method returns null when no public, non-abstract, concrete types that implement the control type interface and have the <see cref="PlatformControlStrategyAttribute"/> attribute are found.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task Resolve_StrategyTypesDoesNotContainConcreteTypeWithPlatformControlStrategyAttribute_ReturnsNull()
    {
        await this.resolver.InitializeAsync(this.page);

        var strategyType = this.resolver.Resolve(typeof(IClientApi), [typeof(LoginControl)]);

        Assert.That(strategyType, Is.Null);
    }

    private Type GetPlatformControlStrategyTypeFor<TControlType>(Version strategyVersion)
        where TControlType : IPlatformControl
    {
        return new DynamicTypeBuilder()
            .AddInterfaceImplementation<TControlType>()
            .AddAttribute<PlatformControlStrategyAttribute>([(uint)strategyVersion.Major, (uint)strategyVersion.Minor, (uint)strategyVersion.Build, (uint)strategyVersion.Revision])
            .Build();
    }

    private void MockValidDefaults()
    {
        this.platformVersion = new Version(9, 2, 0, 10020);
        this.environmentInfoProvider.PlatformVersion.Returns(this.platformVersion);
    }
}