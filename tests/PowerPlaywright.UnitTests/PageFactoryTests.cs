namespace PowerPlaywright.UnitTests;

using Bogus;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using NSubstitute;
using PowerPlaywright.Framework;
using PowerPlaywright.Framework.Pages;
using PowerPlaywright.Pages;

/// <summary>
/// Tests for the <see cref="PageFactory"/> class.
/// </summary>
[TestFixture]
public class PageFactoryTests
{
    private const string PageTypeQueryEntityRecord = "entityrecord";
    private const string PageTypeQueryWebResource = "webresource";
    private const string PageTypeQueryDashboard = "dashboard";
    private const string PageTypeQueryEntityList = "entitylist";

    private Faker faker;
    private ILogger<PageFactory> logger;
    private IServiceProvider serviceProvider;
    private IPage page;

    private PageFactory pageFactory;

    /// <summary>
    /// Sets up the tests.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this.faker = new Faker();
        this.serviceProvider = Substitute.For<IServiceProvider>();
        this.logger = Substitute.For<ILogger<PageFactory>>();
        this.page = Substitute.For<IPage>();

        this.pageFactory = new PageFactory(this.serviceProvider, this.logger);

        this.MockValidDefaults();
    }

    /// <summary>
    /// Tests that the <see cref="PageFactory"/> constructor throws an <see cref="ArgumentNullException"/> when the <paramref name="serviceProvider"/> is null.
    /// </summary>
    [Test]
    public void Constructor_NullServiceProvider_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new PageFactory(null, this.logger));
    }

    /// <summary>
    /// Tests that the <see cref="PageFactory"/> constructor throws an <see cref="ArgumentNullException"/> when the <paramref name="logger"/> is null.
    /// </summary>
    [Test]
    public void Constructor_NullLogger_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new PageFactory(this.serviceProvider, null));
    }

    /// <summary>
    /// Tests that the <see cref="PageFactory.CreateInstanceAsync{TPage}(IPage)"/> method throws an <see cref="ArgumentNullException"/> when the <paramref name="page"/> is null.
    /// </summary>
    [Test]
    public void CreateInstanceAsync_NullPage_ThrowsArgumentNullException()
    {
        Assert.ThrowsAsync<ArgumentNullException>(() => this.pageFactory.CreateInstanceAsync(null));
    }

    /// <summary>
    /// Tests that the <see cref="PageFactory.CreateInstanceAsync{TPage}(IPage)"/> method returns a <see cref="LoginPage"/> when the URL host is "login.microsoftonline.com".
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateInstanceAsync_PageUrlHostIsMicrosoftLogin_ReturnsLoginPage()
    {
        this.page.Url
            .Returns(this.faker.Internet.UrlWithPath(protocol: "https", domain: "login.microsoftonline.com"));

        var page = await this.pageFactory.CreateInstanceAsync(this.page);

        Assert.That(page, Is.InstanceOf<LoginPage>());
    }

    /// <summary>
    /// Tests that the <see cref="PageFactory.CreateInstanceAsync{TPage}(IPage)"/> method returns the correct app page typebased on the "pagetype" query string parameter in the URL.
    /// </summary>
    /// <typeparam name="TExpectedPageType">The expected page type to be returned.</typeparam>
    /// <param name="pageType">The page type query string parameter value.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [TestCase<EntityRecordPage>(PageTypeQueryEntityRecord)]
    [TestCase<EntityListPage>(PageTypeQueryEntityList)]
    [TestCase<DashboardPage>(PageTypeQueryDashboard)]
    [TestCase<WebResourcePage>(PageTypeQueryWebResource)]
    [TestCase<CustomPage>("custom")]
    public async Task CreateInstanceAsync_PageUrlQueryStringParameterPageType_ReturnsCorrectAppPage<TExpectedPageType>(string pageType)
    {
        this.page.Url
            .Returns(this.faker.Internet.UrlWithPath(fileExt: $"main.aspx?pagetype={pageType}"));

        var page = await this.pageFactory.CreateInstanceAsync(this.page);

        Assert.That(page, Is.InstanceOf<TExpectedPageType>());
    }

    /// <summary>
    /// Tests that the <see cref="PageFactory.CreateInstanceAsync{TPage}(IPage)"/> method throws a <see cref="PowerPlaywrightException"/> when the "pagetype" query string parameter in the URL matches no known page type.
    /// </summary>
    [Test]
    public void CreateInstanceAsync_PageUrlAndQueryStringMatchNoKnownPage_ThrowsPowerPlaywrightException()
    {
        this.page.Url
            .Returns(this.faker.Internet.UrlWithPath(fileExt: "main.aspx?pagetype=unknown"));

        Assert.ThrowsAsync<PowerPlaywrightException>(() => this.pageFactory.CreateInstanceAsync(this.page));
    }

    /// <summary>
    /// Tests that the <see cref="PageFactory.CreateInstanceAsync{TPage}(IPage)"/> method throws an <see cref="ArgumentNullException"/> when the <paramref name="page"/> is null.
    /// </summary>
    [Test]
    public void CreateInstanceAsyncTPage_NullPage_ThrowsArgumentNullException()
    {
        Assert.ThrowsAsync<ArgumentNullException>(() => this.pageFactory.CreateInstanceAsync<EntityRecordPage>(null));
    }

    /// <summary>
    /// Tests that the <see cref="PageFactory.CreateInstanceAsync{TPage}(IPage)"/> method instantiates the detected page type when the detected page type is assignable to the requested page type.
    /// </summary>
    /// <typeparam name="TRequestedType">The requested page type.</typeparam>
    /// <typeparam name="TExpectedType">The expected instantiated page type.</typeparam>
    /// <param name="pageType">The page type query string parameter value.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns> unit test.</returns>
    [TestCase<IEntityRecordPage, EntityRecordPage>(PageTypeQueryEntityRecord)]
    [TestCase<IEntityListPage, EntityListPage>(PageTypeQueryEntityList)]
    [TestCase<IDashboardPage, DashboardPage>(PageTypeQueryDashboard)]
    [TestCase<IWebResourcePage, WebResourcePage>(PageTypeQueryWebResource)]
    [TestCase<IAppPage, EntityRecordPage>(PageTypeQueryEntityRecord)]
    [TestCase<IModelDrivenAppPage, EntityRecordPage>(PageTypeQueryEntityRecord)]
    [TestCase<EntityRecordPage, EntityRecordPage>(PageTypeQueryEntityRecord)]
    public async Task CreateInstanceAsyncTPage_DetectedPageTypeIsOrIsAssignableToRequestedPageType_InstantiatesDetectedPageType<TRequestedType, TExpectedType>(string pageType)
        where TRequestedType : IAppPage
    {
        this.page.Url
            .Returns(this.faker.Internet.UrlWithPath(fileExt: $"main.aspx?pagetype={pageType}"));

        var page = await this.pageFactory.CreateInstanceAsync<TRequestedType>(this.page);

        Assert.That(page, Is.InstanceOf<TExpectedType>());
    }

    /// <summary>
    /// Tests that the <see cref="PageFactory.CreateInstanceAsync{TPage}(IPage)"/> method instantiates the requested page type when the requested page type is assignable to the detected page type.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateInstanceAsyncTPage_RequestedPageTypeIsAssignableToDetectedPageType_InstantiatesRequestedPageType()
    {
        this.page.Url
            .Returns(this.faker.Internet.UrlWithPath(fileExt: $"main.aspx?pagetype=custom"));

        var page = await this.pageFactory.CreateInstanceAsync<MyCustomPage>(this.page);

        Assert.That(page, Is.InstanceOf<MyCustomPage>());
    }

    /// <summary>
    /// Tests that the <see cref="PageFactory.CreateInstanceAsync{TPage}(IPage)"/> method throws a <see cref="PowerPlaywrightException"/> when the requested page type is not assignable to or from the detected page type.
    /// </summary>
    [Test]
    public void CreateInstanceAsyncTPage_RequestedPageTypeIsNotAssignableToOrFromDetectedPageType_ThrowsPowerPlaywrightException()
    {
        this.page.Url
            .Returns(this.faker.Internet.UrlWithPath(fileExt: $"main.aspx?pagetype=entityrecord"));

        Assert.ThrowsAsync<PowerPlaywrightException>(() => this.pageFactory.CreateInstanceAsync<MyCustomPage>(this.page));
    }

    private void MockValidDefaults()
    {
        this.serviceProvider.GetService(Arg.Any<Type>())
            .Returns(i => Substitute.For(i.Args().Cast<Type>().ToArray(), []));
        this.page.Url
            .Returns(this.faker.Internet.UrlWithPath(fileExt: "main.aspx?pagetype=entityrecord"));
    }

    private class MyCustomPage(IPage page, IControlFactory controlFactory)
        : CustomPage(page, controlFactory)
    {
    }
}