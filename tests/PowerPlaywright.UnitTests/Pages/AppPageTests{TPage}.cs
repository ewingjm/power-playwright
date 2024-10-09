namespace PowerPlaywright.UnitTests.Pages;

using Microsoft.Playwright;
using NSubstitute;
using PowerPlaywright.Framework;
using PowerPlaywright.Framework.Controls;
using PowerPlaywright.Framework.Pages;

/// <summary>
/// A basic class for page tests.
/// </summary>
/// <typeparam name="TPage">The type of page.</typeparam>
public abstract class AppPageTests<TPage>
    where TPage : IAppPage
{
    /// <summary>
    /// Gets the substitute page.
    /// </summary>
    protected IPage Page { get; private set; }

    /// <summary>
    /// Gets the substitute control factory.
    /// </summary>
    protected IControlFactory ControlFactory { get; private set; }

    /// <summary>
    /// Gets the app page.
    /// </summary>
    protected TPage AppPage { get; private set; }

    /// <summary>
    /// Sets up the custom page.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this.Page = Substitute.For<IPage>();
        this.ControlFactory = Substitute.For<IControlFactory>();

        this.AppPage = this.InstantiateAppPage();
    }

    /// <summary>
    /// A factory method for the app page.
    /// </summary>
    /// <returns>The app page.</returns>
    protected abstract TPage InstantiateAppPage();

    /// <summary>
    /// A test for control registration.
    /// </summary>
    /// <param name="propertyName">The control property name.</param>
    /// <param name="name">The control name (if any.).</param>
    /// <typeparam name="TControlType">The control type.</typeparam>
    protected void ControlProperty_Always_ReturnsControlInstantiatedByControlFactory<TControlType>(string propertyName, string? name = null)
        where TControlType : class, IControl
    {
        var expectedControl = Substitute.For<TControlType>();

        this.ControlFactory
            .CreateInstance<TControlType>(this.AppPage, name)
            .Returns(expectedControl);

        var controlProperty = this.AppPage.GetType().GetProperty(propertyName)
            ?? throw new Exception($"Expected a property named {propertyName} on {this.AppPage.GetType().Name} but it was not found.");

        var actualControl = controlProperty.GetValue(this.AppPage);

        Assert.That(actualControl, Is.EqualTo(expectedControl));
    }
}