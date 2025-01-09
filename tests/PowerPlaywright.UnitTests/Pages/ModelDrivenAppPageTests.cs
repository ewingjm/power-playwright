namespace PowerPlaywright.UnitTests.Pages;

using NUnit.Framework;
using PowerPlaywright.Framework.Controls;
using PowerPlaywright.Framework.Controls.Platform;
using PowerPlaywright.Framework.Pages;
using PowerPlaywright.Pages;
using PowerPlaywright.UnitTests;

/// <summary>
/// Tests for the <see cref="ModelDrivenAppPage"/> class.
/// </summary>
[TestFixture]
public class ModelDrivenAppPageTests : AppPageTests<IModelDrivenAppPage>
{
    /// <summary>
    /// Tests that the control properties return the expected control instances from the control factory.
    /// </summary>
    /// <typeparam name="TControlType">The type of control.</param>
    /// <param name="propertyName">The property.</param>
    /// <param name="controlName">The control name (optional).</param>
    [Test]
    [TestCase<ISiteMapControl>(nameof(IModelDrivenAppPage.SiteMap))]
    [TestCase<IClientApi>(nameof(IModelDrivenAppPage.ClientApi))]
    public new void ControlProperty_Always_ReturnsControlInstantiatedByControlFactory<TControlType>(
        string propertyName, string? controlName = null)
        where TControlType : class, IControl
    {
        base.ControlProperty_Always_ReturnsControlInstantiatedByControlFactory<TControlType>(propertyName, controlName);
    }

    /// <inheritdoc/>
    protected override IModelDrivenAppPage InstantiateAppPage()
    {
        return new CustomPage(this.Page, this.ControlFactory);
    }
}