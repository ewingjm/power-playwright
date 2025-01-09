namespace PowerPlaywright.UnitTests.Pages;

using Microsoft.Playwright;
using NSubstitute;
using NUnit.Framework;
using PowerPlaywright.Framework;
using PowerPlaywright.Framework.Controls.External;
using PowerPlaywright.Framework.Pages;
using PowerPlaywright.Pages;

/// <summary>
/// Tests for the <see cref="LoginPage"/> class.
/// </summary>
[TestFixture]
public class LoginPageTests : AppPageTests<ILoginPage>
{
    /// <summary>
    /// Tests that an <see cref="ArgumentNullException"/> is thrown if a null <see cref="IPage"/> is passed to the constructor.
    /// </summary>
    [Test]
    public void Constructor_NullPage_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new LoginPage(null, this.ControlFactory));
    }

    /// <summary>
    /// Tests that an <see cref="ArgumentNullException"/> is thrown if a null <see cref="IControlFactory"/> is passed to the constructor.
    /// </summary>
    [Test]
    public void Constructor_NullControlFactory_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new LoginPage(this.Page, null));
    }

    /// <summary>
    /// Tests that calling the constructor with valid arguments does not throw an exception.
    /// </summary>
    [Test]
    public void Constructor_ValidArguments_DoesNotThrow()
    {
        Assert.DoesNotThrow(() => new LoginPage(this.Page, this.ControlFactory));
    }

    /// <summary>
    /// Tests that calling <see cref="LoginPage.LoginAsync(string, string)"/> with a non-empty username and password will return the page returned by <see cref="ILoginControl.LoginAsync(string, string)"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task LoginAsync_NonEmptyUsernameAndPassword_ReturnsHomePageReturnedByLoginControl()
    {
        var username = "username@domain.com";
        var password = "hunter2";

        var loginControl = Substitute.For<ILoginControl>();
        var expectedPage = Substitute.For<IModelDrivenAppPage>();

        loginControl
            .LoginAsync(username, password)
            .Returns(expectedPage);

        this.ControlFactory
            .CreateInstance<ILoginControl>(this.AppPage)
            .Returns(loginControl);

        var actualPage = await this.AppPage.LoginAsync(username, password);

        Assert.That(actualPage, Is.EqualTo(expectedPage));
    }

    /// <summary>
    /// Tests that calling <see cref="LoginPage.LoginAsync(string, string)"/> with a null username will throw a <see cref="ArgumentException"/>.
    /// </summary>
    [Test]
    public void LoginAsync_NullUsername_ThrowsArgumentException()
    {
        Assert.ThrowsAsync<ArgumentException>(() => this.AppPage.LoginAsync(null, "hunter2"));
    }

    /// <summary>
    /// Tests that calling <see cref="LoginPage.LoginAsync(string, string)"/> with a null username will throw a <see cref="ArgumentException"/>.
    /// </summary>
    [Test]
    public void LoginAsync_EmptyUsername_ThrowsArgumentException()
    {
        Assert.ThrowsAsync<ArgumentException>(() => this.AppPage.LoginAsync(string.Empty, "hunter2"));
    }

    /// <summary>
    /// Tests that calling <see cref="LoginPage.LoginAsync(string, string)"/> with a null password will throw a <see cref="ArgumentException"/>.
    /// </summary>
    [Test]
    public void LoginAsync_NullPassword_ThrowsArgumentException()
    {
        Assert.ThrowsAsync<ArgumentException>(() => this.AppPage.LoginAsync("username@domain.com", null));
    }

    /// <summary>
    /// Tests that calling <see cref="LoginPage.LoginAsync(string, string)"/> with an empty password will throw a <see cref="ArgumentException"/>.
    /// </summary>
    [Test]
    public void LoginAsync_EmptyPassword_ThrowsArgumentException()
    {
        Assert.ThrowsAsync<ArgumentException>(() => this.AppPage.LoginAsync("username@domain.com", string.Empty));
    }

    /// <inheritdoc/>
    protected override ILoginPage InstantiateAppPage()
    {
        return new LoginPage(this.Page, this.ControlFactory);
    }
}