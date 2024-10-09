namespace PowerPlaywright.UnitTests.Resolvers;

using Microsoft.Playwright;
using NSubstitute;
using PowerPlaywright.Resolvers;

/// <summary>
/// Tests for the <see cref="AppControlStrategyResolver"/> abstract resolver.
/// </summary>
[TestFixture]
public class AppControlStrategyResolverTests
{
    private AppControlStrategyResolver resolver;
    private IPage page;

    /// <summary>
    /// Sets up the resolver.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this.resolver = new PlatformControlStrategyResolver();
        this.page = Substitute.For<IPage>();
        this.page.EvaluateAsync<string>("Xrm.Utility.getGlobalContext().getVersion()").Returns("1.0.0.0");
    }

    /// <summary>
    /// Tests that the <see cref="AppControlStrategyResolver.IsReady"/> property returns false after the resolver is constructed.
    /// </summary>
    [Test]
    public void IsReady_InitializeAsyncNotCalled_ReturnsFalse()
    {
        Assert.That(this.resolver.IsReady, Is.False);
    }

    /// <summary>
    /// Tests that the <see cref="AppControlStrategyResolver.IsReady"/> property returns true after the resolver is initialised.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task IsReady_InitializeAsyncCalled_ReturnsTrue()
    {
        await this.resolver.InitializeAsync(this.page);

        Assert.That(this.resolver.IsReady, Is.True);
    }

    /// <summary>
    /// Tests that the <see cref="AppControlStrategyResolver.OnReady"/> event is triggered by the <see cref="AppControlStrategyResolver.InitialiseResolverAsync(IPage)"/> method when not already initialised.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task InitialiseAsync_InitializeAsyncNotCalled_TriggersOnReadyEvent()
    {
        this.resolver.OnReady += (sender, args) => Assert.Pass();

        await this.resolver.InitializeAsync(this.page);

        Assert.Fail($"The {nameof(AppControlStrategyResolver.OnReady)} event was not trigged.");
    }

    /// <summary>
    /// Tests that the <see cref="AppControlStrategyResolver.OnReady"/> event is not triggered by the <see cref="AppControlStrategyResolver.InitialiseResolverAsync(IPage)"/> method when already initialised.
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
    /// Tests that the <see cref="AppControlStrategyResolver.OnReady"/> event is triggered by the <see cref="AppControlStrategyResolver.InitialiseResolverAsync(IPage)"/> method.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OnReady_EventTriggered_IncludesResolverInEventArgs()
    {
        this.resolver.OnReady += (sender, args) => Assert.That(args.Resolver, Is.EqualTo(this.resolver));

        await this.resolver.InitializeAsync(this.page);
    }
}
