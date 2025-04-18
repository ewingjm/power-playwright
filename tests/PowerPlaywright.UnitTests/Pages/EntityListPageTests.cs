namespace PowerPlaywright.UnitTests.Pages;

using Microsoft.Playwright;
using NSubstitute;
using NUnit.Framework;
using PowerPlaywright.Framework;
using PowerPlaywright.Framework.Controls;
using PowerPlaywright.Framework.Controls.Pcf.Classes;
using PowerPlaywright.Framework.Pages;
using PowerPlaywright.Pages;

/// <summary>
/// Tests for the <see cref="EntityListPage"/> class.
/// </summary>
[TestFixture]
public class EntityListPageTests : AppPageTests<IEntityListPage>
{
    private const string GridControlName = "entity_control";

    private IPlatformReference? platformReference;

    /// <summary>
    /// Tests that an <see cref="ArgumentNullException"/> is thrown if a null <see cref="IPage"/> is passed to the constructor.
    /// </summary>
    [Test]
    public void Constructor_NullPage_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new EntityListPage(null, this.ControlFactory, this.platformReference));
    }

    /// <summary>
    /// Tests that an <see cref="ArgumentNullException"/> is thrown if a null <see cref="IControlFactory"/> is passed to the constructor.
    /// </summary>
    [Test]
    public void Constructor_NullControlFactory_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new EntityListPage(this.Page, null, this.platformReference));
    }

    /// <summary>
    /// Tests that an <see cref="ArgumentNullException"/> is thrown if a null <see cref="IPlatformReference"/> is passed to the constructor.
    /// </summary>
    [Test]
    public void Constructor_NullPlatformReference_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new EntityListPage(this.Page, this.ControlFactory, null));
    }

    /// <summary>
    /// Tests that calling the constructor with valid arguments does not throw an exception.
    /// </summary>
    [Test]
    public void Constructor_ValidArguments_DoesNotThrow()
    {
        Assert.DoesNotThrow(() => new EntityListPage(this.Page, this.ControlFactory, this.platformReference));
    }

    /// <summary>
    /// Tests that the control properties return the expected control instances from the control factory.
    /// </summary>
    /// <typeparam name="TControlType">The type of control.</typeparam>
    /// <param name="propertyName">The property.</param>
    /// <param name="controlName">The control name (optional).</param>
    [Test]
    [TestCase<IReadOnlyGrid>([nameof(IEntityListPage.Grid), GridControlName])]
    public new void ControlProperty_Always_ReturnsControlInstantiatedByControlFactory<TControlType>(
        string propertyName, string? controlName = null)
        where TControlType : class, IControl
    {
        base.ControlProperty_Always_ReturnsControlInstantiatedByControlFactory<TControlType>(propertyName, controlName);
    }

    /// <inheritdoc/>
    protected override IEntityListPage InstantiateAppPage()
    {
        this.platformReference = Substitute.For<IPlatformReference>();
        this.platformReference.EntityListPageGridControlName.Returns(GridControlName);

        return new EntityListPage(
            this.Page,
            this.ControlFactory,
            this.platformReference);
    }
}