namespace PowerPlaywright.UnitTests.Pages;

using Microsoft.Playwright;
using PowerPlaywright.Framework.Pages;
using PowerPlaywright.Pages;

/// <summary>
/// Tests for the <see cref="AppPage"/> page.
/// </summary>
[TestFixture]
public class AppPageTests : AppPageTests<IAppPage>
{
    private new AppPage AppPage => (AppPage)base.AppPage;

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

    /// <inheritdoc/>
    protected override IAppPage InstantiateAppPage()
    {
        return new CustomPage(this.Page, this.ControlFactory);
    }
}