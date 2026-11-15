namespace PowerPlaywright.UnitTests;

using Microsoft.Playwright;
using NSubstitute;
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
    private static readonly Uri EnvironmentUrl = new("https://environment.crm.dynamics.com");

    private IBrowserContext browserContext;
    private PowerPlaywright powerPlaywright;

    /// <summary>
    /// Sets up the tests.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this.browserContext = Substitute.For<IBrowserContext>();
        this.powerPlaywright = new PowerPlaywright(new PowerPlaywrightConfiguration());
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
    /// Tests that the <see cref="PowerPlaywright"/> constructor throws an <see cref="ArgumentNullException"/> when the configuration parameter is <see langword="null"/>.
    /// </summary>
    [Test]
    public void Constructor_NullConfiguration_ThrowsArgumentNullException()
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
    /// Creates the API with the default configuration without acquiring packages.
    /// </summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task CreateAsync_DefaultConfiguration_ReturnsPowerPlaywrightInstance()
    {
        Assert.That(await PowerPlaywright.CreateAsync(), Is.InstanceOf<IPowerPlaywright>());
    }
}
