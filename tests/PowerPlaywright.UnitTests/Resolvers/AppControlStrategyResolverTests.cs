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
    private IEnvironmentInfoProvider environmentInfoProvider;
    private AppControlStrategyResolver resolver;

    /// <summary>
    /// Sets up the resolver.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this.environmentInfoProvider = Substitute.For<IEnvironmentInfoProvider>();
        this.resolver = new PlatformControlStrategyResolver(this.environmentInfoProvider);

        this.MockValidDefaults();
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
    /// Tests that the <see cref="PlatformControlStrategyResolver.IsReady"/> property returns true if the environment info provider is ready.
    /// </summary>
    [Test]
    public void IsReady_EnvironmentInfoProviderReady_ReturnsTrue()
    {
        this.environmentInfoProvider.OnReady += Raise.Event();

        Assert.That(this.resolver.IsReady, Is.True);
    }

    /// <summary>
    /// Tests that the <see cref="PlatformControlStrategyResolver.IsReady"/> property returns false if the environment info provider is not ready.
    /// </summary>
    [Test]
    public void IsReady_EnvironmentInfoProviderNotReady_ReturnsFalse()
    {
        Assert.That(this.resolver.IsReady, Is.False);
    }

    /// <summary>
    /// Tests that the <see cref="AppControlStrategyResolver.OnReady"/> event is triggered by the environment info provider becoming ready.
    /// </summary>
    [Test]
    public void OnReady_EnvironmentInfoProviderOnReady_IsTriggered()
    {
        this.resolver.OnReady += (sender, args) => Assert.Pass();

        this.environmentInfoProvider.OnReady += Raise.Event();

        Assert.Fail($"The {nameof(PcfControlStrategyResolver.OnReady)} event was not trigged.");
    }

    /// <summary>
    /// Tests that the <see cref="AppControlStrategyResolver.OnReady"/> event is triggered with the resolver as the sender.
    /// </summary>
    [Test]
    public void OnReady_EventTriggered_SenderIsResolver()
    {
        this.resolver.OnReady += (sender, args) => Assert.That(sender, Is.EqualTo(this.resolver));

        this.environmentInfoProvider.OnReady += Raise.Event();
    }

    private void MockValidDefaults()
    {
        this.environmentInfoProvider.PlatformVersion.Returns(new Version());
    }
}