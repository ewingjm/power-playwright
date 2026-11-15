namespace PowerPlaywright.UnitTests;

using NSubstitute;
using PowerPlaywright.Framework;

/// <summary>
/// Verifies assets are read from the installed strategies assembly.
/// </summary>
[TestFixture]
public class PlatformReferenceTests
{
    /// <summary>
    /// Resolves the versioned reference bundled with the strategies.
    /// </summary>
    [Test]
    public void EntityListPageGridControlName_SupportedVersion_ReadsBundledAsset()
    {
        var environment = Substitute.For<IEnvironmentInfoProvider>();
        environment.PlatformVersion.Returns(new Version(9, 2, 0, 0));

        var reference = new PlatformReference(environment);

        Assert.That(reference.EntityListPageGridControlName, Is.EqualTo("entity_control"));
    }

    /// <summary>
    /// Selects the built-in assembly through normal assembly resolution.
    /// </summary>
    [Test]
    public void GetAssembly_InstalledStrategies_UsesApplicationAssembly()
    {
        var assembly = new InstalledStrategiesAssemblyProvider().GetAssembly();

        Assert.That(assembly, Is.SameAs(typeof(Strategies.Redirectors.RedirectionInfoProvider).Assembly));
        Assert.That(assembly.GetManifestResourceNames(), Does.Contain("PowerPlaywright.Strategies.platform-reference.json"));
    }
}
