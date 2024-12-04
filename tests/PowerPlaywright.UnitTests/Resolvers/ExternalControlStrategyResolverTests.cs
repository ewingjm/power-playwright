namespace PowerPlaywright.UnitTests.Resolvers;

using NUnit.Framework.Internal;
using PowerPlaywright.Framework;
using PowerPlaywright.Framework.Controls.External;
using PowerPlaywright.Framework.Controls.External.Attributes;
using PowerPlaywright.Framework.Controls.Pcf;
using PowerPlaywright.Framework.Controls.Platform.Attributes;
using PowerPlaywright.Resolvers;
using PowerPlaywright.Strategies.Controls.External;
using PowerPlaywright.Strategies.Controls.Pcf;
using PowerPlaywright.UnitTests.Helpers;

/// <summary>
/// Tests for the <see cref="ExternalControlStrategyResolver"/> resolver.
/// </summary>
[TestFixture]
public class ExternalControlStrategyResolverTests
{
    private ExternalControlStrategyResolver resolver;

    /// <summary>
    /// Sets up the resolver.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this.resolver = new ExternalControlStrategyResolver();
    }

    /// <summary>
    /// Tests that the <see cref="ExternalControlStrategyResolver.IsReady"/> property returns true after the resolver is constructed.
    /// </summary>
    [Test]
    public void IsReady_AfterConstruction_ReturnsTrue()
    {
        Assert.That(this.resolver.IsReady, Is.True);
    }

    /// <summary>
    /// Tests that the <see cref="ExternalControlStrategyResolver.IsResolvable(Type)"/> method throws an <see cref="ArgumentNullException"/> when the type is null.
    /// </summary>
    [Test]
    public void IsResolvable_NullType_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => this.resolver.IsResolvable(null));
    }

    /// <summary>
    /// Tests that the <see cref="ExternalControlStrategyResolver.IsResolvable(Type)"/> method returns true when the type has the <see cref="ExternalControlAttribute"/> attribute.
    /// </summary>
    [Test]
    public void IsResolvable_TypeHasExternalControlAttribute_ReturnsTrue()
    {
        var typeWithExternalControlAttribute = new DynamicTypeBuilder()
            .AddAttribute<ExternalControlAttribute>([])
            .Build();

        var isResolvable = this.resolver.IsResolvable(typeWithExternalControlAttribute);

        Assert.That(isResolvable, Is.True);
    }

    /// <summary>
    /// Tests that the <see cref="ExternalControlStrategyResolver.IsResolvable(Type)"/> method returns false when the type has the <see cref="ExternalControlAttribute"/> attribute.
    /// </summary>
    [Test]
    public void IsResolvable_TypeDoesNotHaveExternalControlAttribute_ReturnsFalse()
    {
        var typeWithoutExternalControlAttribute = new DynamicTypeBuilder().Build();

        var isResolvable = this.resolver.IsResolvable(typeWithoutExternalControlAttribute);

        Assert.That(isResolvable, Is.False);
    }

    /// <summary>
    /// Tests that the <see cref="ExternalControlStrategyResolver.Resolve(Type, IEnumerable{Type})"/> method throws an <see cref="ArgumentNullException"/> when the control type is null.
    /// </summary>
    [Test]
    public void Resolve_NullControlType_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => this.resolver.Resolve(null, [typeof(LoginControl)]));
    }

    /// <summary>
    /// Tests that the <see cref="ExternalControlStrategyResolver.Resolve(Type, IEnumerable{Type})"/> method throws an <see cref="ArgumentNullException"/> when the strategy types are null.
    /// </summary>
    [Test]
    public void Resolve_NullStrategyTypes_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => this.resolver.Resolve(typeof(ILoginControl), null));
    }

    /// <summary>
    /// Tests that the <see cref="ExternalControlStrategyResolver.Resolve(Type, IEnumerable{Type})"/> method throws a <see cref="PowerPlaywrightException"/> when the control type does not have the <see cref="ExternalControlAttribute"/> attribute.
    /// </summary>
    [Test]
    public void Resolve_ControlTypeDoesNotHaveExternalControlAttribute_ThrowsPowerPlaywrightException()
    {
        Assert.Throws<PowerPlaywrightException>(() => this.resolver.Resolve(typeof(IPowerAppsOneGridControl), [typeof(ILoginControl)]));
    }

    /// <summary>
    /// Tests that the <see cref="ExternalControlStrategyResolver.Resolve(Type, IEnumerable{Type})"/> method returns public, non-abstract, concrete types that implement the control type interface and have the <see cref="PlatformControlStrategyAttribute"/> attribute.
    /// </summary>
    [Test]
    public void Resolve_StrategyTypesContainsPublicNonAbstractConcreteTypeWithPlatformControlStrategyAttribute_ReturnsType()
    {
        var expectedStrategyType = GetExternalControlStrategyTypeFor<ILoginControl>();

        var strategyType = this.resolver.Resolve(typeof(ILoginControl), [expectedStrategyType]);

        Assert.That(strategyType, Is.EqualTo(expectedStrategyType));
    }

    /// <summary>
    /// Tests that the <see cref="ExternalControlStrategyResolver.Resolve(Type, IEnumerable{Type})"/> method returns the type with the highest version set in the <see cref="PlatformControlStrategyAttribute"/> attribute when multiple matching strategy types are found.
    /// </summary>
    [Test]
    public void Resolve_MultipleStrategyTypesContainsPublicNonAbstractConcreteTypeWithPlatformControlStrategyAttribute_ReturnsTypeWithHighestVersion()
    {
        var unexpectedStrategyType = GetExternalControlStrategyTypeFor<ILoginControl>(1);
        var expectedStrategyType = GetExternalControlStrategyTypeFor<ILoginControl>(2);

        var strategyType = this.resolver.Resolve(typeof(ILoginControl), [unexpectedStrategyType, expectedStrategyType]);

        Assert.That(strategyType, Is.EqualTo(expectedStrategyType));
    }

    /// <summary>
    /// Tests that the <see cref="ExternalControlStrategyResolver.Resolve(Type, IEnumerable{Type})"/> method returns null when no public, non-abstract, concrete types that implement the control type interface and have the <see cref="PlatformControlStrategyAttribute"/> attribute are found.
    /// </summary>
    [Test]
    public void Resolve_StrategyTypesDoesNotContainConcreteTypeWithPlatformControlStrategyAttribute_ReturnsNull()
    {
        var strategyType = this.resolver.Resolve(typeof(ILoginControl), [typeof(PcfGridControl)]);

        Assert.That(strategyType, Is.Null);
    }

    private static Type GetExternalControlStrategyTypeFor<TControlType>(uint version = 1)
    {
        return new DynamicTypeBuilder()
            .AddInterfaceImplementation<TControlType>()
            .AddAttribute<ExternalControlStrategyAttribute>([version])
            .Build();
    }
}