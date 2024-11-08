namespace PowerPlaywright.UnitTests;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using Microsoft.VisualBasic;
using NSubstitute;
using NuGet.Packaging;
using PowerPlaywright;
using PowerPlaywright.Framework;
using PowerPlaywright.Framework.Pages;

/// <summary>
/// Tests for the <see cref="ModelDrivenApp"/> class.
/// /// </summary>
[TestFixture]
public class ModelDrivenAppTests
{
    private const string Username = "username";
    private const string Password = "password";

    private IOptions<ModelDrivenAppOptions> options;
    private IBrowserContext browserContext;
    private IPageFactory pageFactory;
    private IList<IAppLoadInitializable> initializables;
    private ILogger<ModelDrivenApp> logger;
    private IPage page;
    private ILoginPage loginPage;
    private IEntityListPage entityListPage;
    private ModelDrivenApp modelDrivepApp;

    /// <summary>
    /// Sets up the tests.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this.options = Substitute.For<IOptions<ModelDrivenAppOptions>>();
        this.browserContext = Substitute.For<IBrowserContext>();
        this.pageFactory = Substitute.For<IPageFactory>();
        this.initializables = [Substitute.For<IAppLoadInitializable>()];
        this.logger = Substitute.For<ILogger<ModelDrivenApp>>();
        this.page = Substitute.For<IPage>();
        this.loginPage = Substitute.For<ILoginPage>();
        this.entityListPage = Substitute.For<IEntityListPage>();

        this.MockValidDefaults();

        this.modelDrivepApp = new ModelDrivenApp(this.options, this.browserContext, this.pageFactory, this.logger, this.initializables);
    }

    /// <summary>
    /// Tears down the test.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [TearDown]
    public async Task TearDown()
    {
        await this.browserContext.DisposeAsync();
    }

    /// <summary>
    /// Tests that the <see cref="ModelDrivenApp"/> constructor throws an <see cref="ArgumentNullException"/> when the <paramref name="options"/> is null.
    /// </summary>
    [Test]
    public void Constructor_NullOptions_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ModelDrivenApp(null, this.browserContext, this.pageFactory, this.logger, this.initializables));
    }

    /// <summary>
    /// Tests that the <see cref="ModelDrivenApp"/> constructor throws an <see cref="ArgumentNullException"/> when the <paramref name="browserContext"/> is null.
    /// </summary>
    [Test]
    public void Constructor_NullBrowserContext_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => new ModelDrivenApp(this.options, null, this.pageFactory, this.logger, this.initializables));
    }

    /// <summary>
    /// Tests that the <see cref="ModelDrivenApp"/> constructor throws an <see cref="ArgumentNullException"/> when the <paramref name="pageFactory"/> is null.
    /// </summary>
    [Test]
    public void Constructor_NullPageFactory_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => new ModelDrivenApp(this.options, this.browserContext, null, this.logger, this.initializables));
    }

    /// <summary>
    /// Tests that the <see cref="ModelDrivenApp"/> constructor throws an <see cref="ArgumentNullException"/> when the <paramref name="logger"/> is null.
    /// </summary>
    [Test]
    public void Constructor_NullLogger_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => new ModelDrivenApp(this.options, this.browserContext, this.pageFactory, null, this.initializables));
    }

    /// <summary>
    /// Tests that the <see cref="ModelDrivenApp"/> constructor does not throw an <see cref="ArgumentNullException"/> when the <paramref name="initializables"/> is null.
    /// </summary>
    [Test]
    public void Constructor_NullAppInitializables_DoesNotThrow()
    {
        Assert.DoesNotThrow(
            () => _ = new ModelDrivenApp(this.options, this.browserContext, this.pageFactory, this.logger, null));
    }

    /// <summary>
    /// Tests that calling <see cref="ModelDrivenApp.LoginAsync(string, string)"/> with a null username throws an <see cref="ArgumentException"/>.
    /// </summary>
    [Test]
    public void LoginAsync_NullUsername_ThrowsArgumentException()
    {
        Assert.ThrowsAsync<ArgumentException>(
            () => this.modelDrivepApp.LoginAsync(null, Password));
    }

    /// <summary>
    /// Tests that calling <see cref="ModelDrivenApp.LoginAsync(string, string)"/> with an empty username throws an <see cref="ArgumentException"/>.
    /// </summary>
    [Test]
    public void LoginAsync_EmptyUsername_ThrowsArgumentException()
    {
        Assert.ThrowsAsync<ArgumentException>(
            () => this.modelDrivepApp.LoginAsync(string.Empty, Password));
    }

    /// <summary>
    /// Tests that calling <see cref="ModelDrivenApp.LoginAsync(string, string)"/> with a null password throws an <see cref="ArgumentException"/>.
    /// </summary>
    [Test]
    public void LoginAsync_NullPassword_ThrowsArgumentException()
    {
        Assert.ThrowsAsync<ArgumentException>(
            () => this.modelDrivepApp.LoginAsync(Username, null));
    }

    /// <summary>
    /// Tests that calling <see cref="ModelDrivenApp.LoginAsync(string, string)"/> with an empty password throws an <see cref="ArgumentException"/>.
    /// </summary>
    [Test]
    public void LoginAsync_EmptyPassword_ThrowsArgumentException()
    {
        Assert.ThrowsAsync<ArgumentException>(
            () => this.modelDrivepApp.LoginAsync(Username, string.Empty));
    }

    /// <summary>
    /// Tests that calling <see cref="ModelDrivenApp.LoginAsync(string, string)"/> more than once with different usernames throws a <see cref="PowerPlaywrightException"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task LoginAsync_CalledMoreThanOnceWithDifferentUsername_ThrowsPowerPlaywrightException()
    {
        await this.modelDrivepApp.LoginAsync(Username, Password);

        Assert.ThrowsAsync<PowerPlaywrightException>(() => this.modelDrivepApp.LoginAsync("username1", Password));
    }

    /// <summary>
    /// Tests that <see cref="ModelDrivenApp.LoginAsync"/> always navigates to the correct app URL.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task LoginAsync_Always_GoesToAppUrl()
    {
        var appUrl = $"{this.options.Value.EnvironmentUrl}Apps/uniquename/{this.options.Value.AppUniqueName}/main.aspx";

        await this.modelDrivepApp.LoginAsync(Username, Password);

        await this.page.Received(1).GotoAsync(appUrl);
    }

    /// <summary>
    /// Tests that calling <see cref="ModelDrivenApp.LoginAsync"/> with the initial page being a <see cref="LoginPage"/> will return the page returned by calling <see cref="LoginPage.LoginAsync(string, string)"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task LoginAsync_InitialPageIsLoginPage_ReturnsPageReturnedByCallingLoginPageLoginAsync()
    {
        this.pageFactory.CreateInstanceAsync(this.page)
            .Returns(this.loginPage);
        this.loginPage.LoginAsync(Username, Password)
            .Returns(this.entityListPage);

        var homePage = await this.modelDrivepApp.LoginAsync(Username, Password);

        Assert.That(homePage, Is.EqualTo(this.entityListPage));
    }

    /// <summary>
    /// Tests that calling <see cref="ModelDrivenApp.LoginAsync"/> with the initial page being a <see cref="ModelDrivenAppPage"/> will return the initial page.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task LoginAsync_InitialPageIsModelDrivenAppPage_ReturnsInitialPage()
    {
        this.pageFactory.CreateInstanceAsync(this.page)
            .Returns(this.entityListPage);

        var homePage = await this.modelDrivepApp.LoginAsync(Username, Password);

        Assert.That(homePage, Is.EqualTo(this.entityListPage));
    }

    /// <summary>
    /// Tests that the <see cref="ModelDrivenApp.LoginAsync"/> method appends specific flags to the home page URL after login.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task LoginAsync_Always_AppendsFlagsToHomePageUrl()
    {
        var homePageUrlWithFlags = this.page.Url + "&flags=easyreproautomation%3Dtrue%2Ctestmode%3Dtrue";

        await this.modelDrivepApp.LoginAsync(Username, Password);

        await this.page.Received(1).GotoAsync(homePageUrlWithFlags);
    }

    /// <summary>
    /// Tests that the <see cref="ModelDrivenApp.LoginAsync"/> method waits for the app to be idle after login.
    /// </summary>
    /// /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task LoginAsync_Always_WaitsForAppIdle()
    {
        await this.modelDrivepApp.LoginAsync(Username, Password);

        await this.page.Received(1).WaitForFunctionAsync("window.UCWorkBlockTracker.isAppIdle()", Arg.Any<object>(), Arg.Any<PageWaitForFunctionOptions>());
    }

    /// <summary>
    /// Tests that the <see cref="ModelDrivenApp.LoginAsync"/> method waits for all initializables passed to the constructor to complete their initialization after app load.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task LoginAsync_InitializablesPassedToConstructor_WaitsForAllToInitialize()
    {
        var tasks = Enumerable.Range(0, 5).Select(i => Task.Delay(500)).ToArray();
        this.initializables.AddRange(
            tasks.Select(t =>
            {
                var initializable = Substitute.For<IAppLoadInitializable>();
                initializable.InitializeAsync(Arg.Any<IPage>())
                    .Returns(t);

                return initializable;
            }));

        await this.modelDrivepApp.LoginAsync(Username, Password);

        Assert.That(tasks.Where(t => t.IsCompleted).Count(), Is.EqualTo(tasks.Count()));
    }

    /// <summary>
    /// Tests that calling <see cref="ModelDrivenApp.LoginAsync{TPage}"/> with a type that matches the home page type will return the home page.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task LoginAsyncTModelDrivenAppPage_ProvidedTypeMatchesHomePageType_ReturnsHomePage()
    {
        var expectedPage = Substitute.For<IEntityRecordPage>();
        this.loginPage.LoginAsync(Username, Password)
            .Returns(expectedPage);

        var homePage = await this.modelDrivepApp.LoginAsync<IEntityRecordPage>(Username, Password);

        Assert.That(homePage, Is.EqualTo(expectedPage));
    }

    /// <summary>
    /// Tests that the <see cref="ModelDrivenApp.LoginAsync{TModelDrivenAppPage}"/> method throws a <see cref="PowerPlaywrightException"/> when the provided page type does not match the home page type.
    /// </summary>
    [Test]
    public async Task LoginAsyncTModelDrivenAppPage_ProvidedTypeDoesNotMatchHomePageType_ThrowsPowerPlaywrightException()
    {
        var entityListPage = Substitute.For<IEntityListPage>();
        this.loginPage.LoginAsync(Username, Password)
            .Returns(entityListPage);

        Assert.ThrowsAsync<PowerPlaywrightException>(() => this.modelDrivepApp.LoginAsync<IEntityRecordPage>(Username, Password));
    }

    private void MockValidDefaults()
    {
        this.options.Value
            .Returns(new ModelDrivenAppOptions
            {
                AppUniqueName = "pp_PowerPlaywright_Test",
                EnvironmentUrl = new Uri("https://environment.crm.dynamics.com"),
            });
        this.browserContext.NewPageAsync()
            .Returns(this.page);
        this.pageFactory.CreateInstanceAsync(this.page)
            .Returns(this.loginPage);
        this.loginPage.LoginAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(this.entityListPage);
        this.entityListPage.Page
            .Returns(this.page);
        this.page.Url
            .Returns("https://environment.crm.dynamics.com/main.aspx?appid=676d5493-1648-ee11-be6d-000d3aaae10a&pagetype=entitylist&etn=contact&viewid=cbae0a36-5446-ed11-bba2-6045bd8f972b&viewType=1039");
    }
}