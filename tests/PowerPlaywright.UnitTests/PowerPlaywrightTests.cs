namespace PowerPlaywright.UnitTests;

using Microsoft.Playwright;
using NSubstitute;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using PowerPlaywright.Api;
using PowerPlaywright.Config;
using PowerPlaywright.Framework.Pages;
using PowerPlaywright = PowerPlaywright.Api.PowerPlaywright;

/// <summary>
/// Unit tests for the <see cref="PowerPlaywright"/> class.
/// </summary>
[TestFixture]
public class PowerPlaywrightTests
{
    private const string Password = "password";
    private const string Username = "username";
    private const string AppUniqueName = "pp_UserInterfaceDemo";
    private const string StrategiesPackageId = "PowerPlaywright.Strategies";
    private static readonly Uri EnvironmentUrl = new("https://environment.crm.dynamics.com");

    private IBrowserContext browserContext;
    private INuGetPackageInstaller packageInstaller;
    private PackageIdentity packageIdentity;
    private PowerPlaywright powerPlaywright;

    /// <summary>
    /// Sets up the tests.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this.browserContext = Substitute.For<IBrowserContext>();
        this.packageInstaller = Substitute.For<INuGetPackageInstaller>();
        this.packageIdentity = new PackageIdentity(StrategiesPackageId, new NuGetVersion(1, 0, 0));

        this.MockValidDefaults();

        this.powerPlaywright = new PowerPlaywright(new PowerPlaywrightConfiguration { PackageIdentity = this.packageIdentity });
    }

    /// <summary>
    /// Tears down the tests.
    /// </summary>
    /// <remarks>
    /// Disposes of the <see cref="IBrowserContext"/> instance.
    /// </remarks>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [TearDown]
    public async Task TearDown()
    {
        await this.browserContext.DisposeAsync();
    }

    /// <summary>
    /// Tests that the <see cref="PowerPlaywright"/> constructor throws an <see cref="ArgumentNullException"/> when the packageIdentity parameter is <see langword="null"/>.
    /// </summary>
    [Test]
    public void Constructor_NullPackageIdentity_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new PowerPlaywright(null));
    }

    /// <summary>
    /// Tests that the <see cref="PowerPlaywright.LaunchAppAsync(IBrowserContext, Uri, string, string, string)"/> method throws an <see cref="ArgumentNullException"/> when the environmentUrl parameter is <see langword="null"/>.
    /// </summary>
    [Test]
    public void LaunchAppAsync_NullEnvironmentUrl_ThrowsArgumentNullException()
    {
        Assert.ThrowsAsync<ArgumentNullException>(
            () => this.powerPlaywright.LaunchAppAsync(this.browserContext, null, AppUniqueName, Username, Password));
    }

    /// <summary>
    /// Tests that the <see cref="PowerPlaywright.LaunchAppAsync(IBrowserContext, Uri, string, string, string)"/> method throws an <see cref="ArgumentException"/> when the uniqueName parameter is <see langword="null"/>.
    /// </summary>
    [Test]
    public void LaunchAppAsync_NullUniqueName_ThrowsArgumentException()
    {
        Assert.ThrowsAsync<ArgumentException>(
            () => this.powerPlaywright.LaunchAppAsync(this.browserContext, EnvironmentUrl, null, Username, Password));
    }

    /// <summary>
    /// /// Tests that the <see cref="PowerPlaywright.LaunchAppAsync(IBrowserContext, Uri, string, string, string)"/> method throws an <see cref="ArgumentException"/> when the username parameter is <see langword="null"/>.
    /// </summary>
    [Test]
    public void LaunchAppAsync_NullUsername_ThrowsArgumentException()
    {
        Assert.ThrowsAsync<ArgumentException>(
            () => this.powerPlaywright.LaunchAppAsync(this.browserContext, EnvironmentUrl, AppUniqueName, null, Password));
    }

    /// <summary>
    /// Tests that the <see cref="PowerPlaywright.LaunchAppAsync(IBrowserContext, Uri, string, string, string)"/> method throws an <see cref="ArgumentException"/> when the password parameter is <see langword="null"/>.
    /// </summary>
    [Test]
    public void LaunchAppAsync_NullPassword_ThrowsArgumentException()
    {
        Assert.ThrowsAsync<ArgumentException>(
            () => this.powerPlaywright.LaunchAppAsync(this.browserContext, EnvironmentUrl, AppUniqueName, Username, null));
    }

    /// <summary>
    /// Tests that the <see cref="PowerPlaywright.LaunchAppAsync{TModelDrivenAppPage}(IBrowserContext, Uri, string, string, string)"/> method throws an <see cref="ArgumentNullException"/> when the environmentUrl parameter is <see langword="null"/>.
    /// </summary>
    [Test]
    public void LaunchAppAsyncTModelDrivenAppPage_NullEnvironmentUrl_ThrowsArgumentNullException()
    {
        Assert.ThrowsAsync<ArgumentNullException>(
            () => this.powerPlaywright.LaunchAppAsync<IEntityListPage>(this.browserContext, null, AppUniqueName, Username, Password));
    }

    /// <summary>
    /// Tests that the <see cref="PowerPlaywright.LaunchAppAsync{TModelDrivenAppPage}(IBrowserContext, Uri, string, string, string)"/> method throws an <see cref="ArgumentException"/> when the uniqueName parameter is <see langword="null"/>.
    /// </summary>
    [Test]
    public void LaunchAppAsyncTModelDrivenAppPage_NullUniqueName_ThrowsArgumentException()
    {
        Assert.ThrowsAsync<ArgumentException>(
            () => this.powerPlaywright.LaunchAppAsync<IEntityListPage>(this.browserContext, EnvironmentUrl, null, Username, Password));
    }

    /// <summary>
    /// Tests that the <see cref="PowerPlaywright.LaunchAppAsync{TModelDrivenAppPage}(IBrowserContext, Uri, string, string, string)"/> method throws an <see cref="ArgumentException"/> when the username parameter is <see langword="null"/>.
    /// </summary>
    [Test]
    public void LaunchAppAsyncTModelDrivenAppPage_NullUsername_ThrowsArgumentException()
    {
        Assert.ThrowsAsync<ArgumentException>(
            () => this.powerPlaywright.LaunchAppAsync<IEntityListPage>(this.browserContext, EnvironmentUrl, AppUniqueName, null, Password));
    }

    /// <summary>
    /// Tests that the <see cref="PowerPlaywright.LaunchAppAsync{TModelDrivenAppPage}(IBrowserContext, Uri, string, string, string)"/> method throws an <see cref="ArgumentException"/> when the password parameter is <see langword="null"/>.
    /// </summary>
    [Test]
    public void LaunchAppAsyncTModelDrivenAppPage_NullPassword_ThrowsArgumentException()
    {
        Assert.ThrowsAsync<ArgumentException>(
            () => this.powerPlaywright.LaunchAppAsync<IEntityListPage>(this.browserContext, EnvironmentUrl, AppUniqueName, Username, null));
    }

    /// <summary>
    /// Tests that the <see cref="PowerPlaywright.CreateInternalAsync(INuGetPackageInstaller, PowerPlaywrightConfiguration)"/> method throws an <see cref="ArgumentNullException"/> when the packageInstaller parameter is <see langword="null"/>.
    /// </summary>
    [Test]
    public void CreateInternalAsync_NullPackageInstaller_ThrowsArgumentNullException()
    {
        Assert.ThrowsAsync<ArgumentNullException>(
            () => PowerPlaywright.CreateInternalAsync(null));
    }

    /// <summary>
    /// Tests that the <see cref="PowerPlaywright.CreateInternalAsync(INuGetPackageInstaller, PowerPlaywrightConfiguration)"/> method returns a non-null <see cref="IPowerPlaywright"/> instance.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateInternalAsync_Always_ReturnsPowerPlaywrightInstance()
    {
        var powerPlaywright = await PowerPlaywright.CreateInternalAsync(this.packageInstaller);

        Assert.That(powerPlaywright, Is.Not.Null);
    }

    /// <summary>
    /// Tests that the <see cref="PowerPlaywright.CreateInternalAsync(INuGetPackageInstaller, PowerPlaywrightConfiguration)"/> method selects and installs the highest package version with the same major version as the current PowerPlaywright major version when a package with a higher major version is also available.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    [Ignore("This test can only be ran individually due to the static nature of the cache.")]
    public async Task CreateInternalAsync_StrategiesPackageWithHigherMajorVersionExists_UsesHighestPackageVersionWithSameMajorVersion()
    {
        var powerPlaywrightMajorVersion = typeof(IPowerPlaywright).Assembly.GetName().Version!.Major;
        this.packageInstaller.GetAllVersionsAsync(StrategiesPackageId)
            .Returns(
                [
                    new NuGetVersion(powerPlaywrightMajorVersion, 0, 0),
                    new NuGetVersion(powerPlaywrightMajorVersion + 1, 0, 0),
                ]);

        await PowerPlaywright.CreateInternalAsync(this.packageInstaller);

        await this.packageInstaller
            .Received(1)
            .InstallPackageAsync(Arg.Is<PackageIdentity>(p => p.Version.Major == powerPlaywrightMajorVersion));
    }

    private void MockValidDefaults()
    {
        var powerPlaywrightVersion = typeof(IPowerPlaywright).Assembly.GetName().Version!;

        this.packageInstaller.GetAllVersionsAsync(StrategiesPackageId)
            .Returns([new NuGetVersion(powerPlaywrightVersion.Major, powerPlaywrightVersion.Minor, powerPlaywrightVersion.Revision)]);
    }
}