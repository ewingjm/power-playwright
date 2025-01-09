namespace PowerPlaywright.UnitTests;

using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NuGet.Configuration;
using NuGet.Packaging.Core;
using NuGet.Versioning;

/// <summary>
/// Tests for the <see cref="GlobalPackagesAssemblyProvider"/> class.
/// </summary>
[TestFixture]
public class GlobalPackagesAssemblyProviderTests
{
    private const string StrategiesAssemblyName = "PowerPlaywright.Strategies.dll";

    private string globalPackagesFolder;
    private PackageIdentity packageIdentity;
    private ISettings settings;
    private ILogger<GlobalPackagesAssemblyProvider> logger;
    private GlobalPackagesAssemblyProvider provider;

    /// <summary>
    /// Sets up the test fixture by creating a <see cref="GlobalPackagesAssemblyProvider"/> instance.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this.globalPackagesFolder = Path.Combine(
            TestContext.CurrentContext.WorkDirectory,
            TestContext.CurrentContext.Test.Name,
            "packages");

        this.packageIdentity = new PackageIdentity("PowerPlaywright.Strategies", new NuGetVersion("1.0.0"));
        this.settings = Substitute.For<ISettings>();
        this.logger = Substitute.For<ILogger<GlobalPackagesAssemblyProvider>>();

        this.provider = new GlobalPackagesAssemblyProvider(this.packageIdentity, this.settings, this.logger);

        this.CreateAssembly();
        this.MockValidDefaults();
    }

    /// <summary>
    /// Tests that the <see cref="GlobalPackagesAssemblyProvider"/> constructor throws an <see cref="ArgumentNullException"/> when the <paramref name="packageIdentity"/> is null.
    /// </summary>
    [Test]
    public void Constructor_NullPackageIdentity_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => new GlobalPackagesAssemblyProvider(null, this.settings, this.logger));
    }

    /// <summary>
    /// Tests that the <see cref="GlobalPackagesAssemblyProvider"/> constructor throws an <see cref="ArgumentNullException"/> when the <paramref name="nugetSettings"/> is null.
    /// </summary>
    [Test]
    public void Constructor_NullNuGetSettings_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => new GlobalPackagesAssemblyProvider(this.packageIdentity, null, this.logger));
    }

    /// <summary>
    /// Tests that the <see cref="GlobalPackagesAssemblyProvider"/> constructor throws an <see cref="ArgumentNullException"/> when the <paramref name="logger"/> is null.
    /// </summary>
    [Test]
    public void Constructor_NullLogger_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => new GlobalPackagesAssemblyProvider(this.packageIdentity, this.settings, null));
    }

    /// <summary>
    /// Tests that the <see cref="GlobalPackagesAssemblyProvider.GetAssembly"/> method, when called once, loads the strategies assembly from the global packages cache using the package identity.
    /// </summary>
    [Test]
    public void GetAssembly_CalledOnce_LoadsStrategiesAssemblyFromGlobalPackagesCacheUsingPackageIdentity()
    {
        Assert.That(this.provider.GetAssembly(), Is.Not.Null);
    }

    /// <summary>
    /// Tests that the <see cref="GlobalPackagesAssemblyProvider.GetAssembly"/> method returns the same value when called more than once, by caching the assembly.
    /// </summary>
    [Test]
    public void GetAssembly_CalledMoreThanOnce_ReturnsCachedAssembly()
    {
        var assembly = this.provider.GetAssembly();

        Assert.That(this.provider.GetAssembly(), Is.EqualTo(assembly));
    }

    private void CreateAssembly()
    {
        var sourceAssemblyPath = Path.Join(TestContext.CurrentContext.TestDirectory, StrategiesAssemblyName);
        var targetAssemblyPath = Path.Join(
            this.globalPackagesFolder,
            this.packageIdentity.Id,
            this.packageIdentity.Version.ToString(),
            "lib",
            "netstandard2.0",
            StrategiesAssemblyName);

        Directory.CreateDirectory(Path.GetDirectoryName(targetAssemblyPath)!);
        File.Copy(sourceAssemblyPath, targetAssemblyPath, true);

        if (!File.Exists(targetAssemblyPath))
        {
            if (!File.Exists(sourceAssemblyPath))
            {
                throw new Exception($"File at {sourceAssemblyPath} does not exist.");
            }

            throw new Exception($"Failed to copy file from {sourceAssemblyPath} to {targetAssemblyPath}.");
        }
    }

    private void MockValidDefaults()
    {
        var addItem = new AddItem(ConfigurationConstants.GlobalPackagesFolder, this.globalPackagesFolder);
        var settingSection = Substitute.For<SettingSection>(
            ConfigurationConstants.Config,
            new ReadOnlyDictionary<string, string>(new Dictionary<string, string>()),
            new SettingItem[] { addItem });

        this.settings.GetSection(ConfigurationConstants.Config)
            .Returns(settingSection);
    }
}