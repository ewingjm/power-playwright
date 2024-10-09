namespace PowerPlaywright.UnitTests.Pages;

using Microsoft.Playwright;
using NSubstitute;
using NUnit.Framework;
using PowerPlaywright.Framework;
using PowerPlaywright.Pages;

/// <summary>
/// Tests for the <see cref="CustomPage"/> class.
/// </summary>
[TestFixture]
public class CustomPageTests
{
    private IPage page;
    private IControlFactory controlFactory;

    /// <summary>
    /// Sets up the custom page.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this.page = Substitute.For<IPage>();
        this.controlFactory = Substitute.For<IControlFactory>();
    }

    /// <summary>
    /// Tests that an <see cref="ArgumentNullException"/> is thrown if a null <see cref="IPage"/> is passed to the constructor.
    /// </summary>
    [Test]
    public void Constructor_NullPage_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new CustomPage(null, this.controlFactory));
    }

    /// <summary>
    /// Tests that an <see cref="ArgumentNullException"/> is thrown if a null <see cref="IControlFactory"/> is passed to the constructor.
    /// </summary>
    [Test]
    public void Constructor_NullControlFactory_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new CustomPage(this.page, null));
    }

    /// <summary>
    /// Tests that calling the constructor with valid arguments does not throw an exception.
    /// </summary>
    [Test]
    public void Constructor_ValidArguments_DoesNotThrow()
    {
        Assert.DoesNotThrow(() => new CustomPage(this.page, this.controlFactory));
    }
}