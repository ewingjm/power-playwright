namespace PowerPlaywright.UnitTests.Pages;

using Microsoft.Playwright;
using NSubstitute;
using PowerPlaywright.Framework.Controls.Platform;
using PowerPlaywright.Framework.Pages;
using PowerPlaywright.Pages;

/// <summary>
/// Tests for the <see cref="AppPage"/> page.
/// </summary>
[TestFixture]
public class AppPageTests : AppPageTests<IAppPage>
{
    private new EntityRecordPage AppPage => (EntityRecordPage)base.AppPage;

    /// <summary>
    /// Tests that an <see cref="ArgumentNullException"/> is thrown if a null <see cref="IPage"/> is passed to the constructor.
    /// </summary>
    [Test]
    public void Constructor_NullPage_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new CustomPage(null, this.ControlFactory));
    }

    /// <summary>
    /// Tests that an <see cref="ArgumentNullException"/> is thrown if a null <see cref="IControlFactory"/> is passed to the constructor.
    /// </summary>
    [Test]
    public void Constructor_NullControlFactory_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new CustomPage(this.Page, null));
    }

    /// <summary>
    /// Tests that the <see cref="AppPage.Destroy"/> method always invokes the <see cref="AppPage.OnDestroy"/> event.
    /// </summary>
    [Test]
    public void Destroy_Always_TriggersOnDestroyEvent()
    {
        this.AppPage.OnDestroy += (sender, args) => Assert.Pass();

        this.AppPage.Destroy();

        Assert.Fail($"The {nameof(this.AppPage.OnDestroy)} event was not invoked.");
    }

    /// <summary>
    /// Tests that the <see cref="AppPage.Page"/> property always returns the page passed to the constructor.
    /// </summary>
    [Test]
    public void Page_Always_ReturnsPagePassedToConstructor()
    {
        Assert.That(this.AppPage.Page, Is.EqualTo(this.Page));
    }

    /// <summary>
    /// Tests that the <see cref="AppPage.GetControl{TControl}"/> method always calls <see cref="IControlFactory.CreateInstance{TControl}(IAppPage, string)"/> if the control has not been previously created.
    /// </summary>
    [Test]
    public void GetControl_GetControlNotPreviouslyCalledWithSameTypeAndName_ReturnsControlFromControlFactory()
    {
        _ = this.AppPage.Form;

        this.ControlFactory.Received(1).CreateInstance<IMainFormControl>(this.AppPage);
    }

    /// <summary>
    /// Tests that the <see cref="AppPage.GetControl{TControl}"/> method returns the same control instance when called multiple times with the same type and name.
    /// </summary>
    [Test]
    public void GetControl_GetControlPreviouslyCalledWithSameTypeAndName_ReturnsControlFromCache()
    {
        var firstControl = this.AppPage.Form;
        var secondControl = this.AppPage.Form;

        this.ControlFactory.Received(1).CreateInstance<IMainFormControl>(this.AppPage);
        Assert.That(firstControl, Is.EqualTo(secondControl));
    }

    /// <inheritdoc/>
    protected override IAppPage InstantiateAppPage()
    {
        return new EntityRecordPage(this.Page, this.ControlFactory);
    }
}