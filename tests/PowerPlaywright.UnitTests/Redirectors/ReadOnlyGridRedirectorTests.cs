namespace PowerPlaywright.UnitTests.Redirectors
{
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Framework.Redirectors;
    using PowerPlaywright.Strategies.Redirectors;

    /// <summary>
    /// Tests for the <see cref="ReadOnlyGridRedirector"/> class.
    /// </summary>
    public class ReadOnlyGridRedirectorTests
    {
        private IRedirectionEnvironmentInfo environmentInfo;
        private IRedirectionInfoProvider redirectionInfoProvider;

        private ReadOnlyGridRedirector redirector;

        /// <summary>
        /// Sets up each test.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this.environmentInfo = Substitute.For<IRedirectionEnvironmentInfo>();
            this.redirectionInfoProvider = Substitute.For<IRedirectionInfoProvider>();

            this.redirectionInfoProvider.GetRedirectionInfo().Returns(this.environmentInfo);

            this.redirector = new ReadOnlyGridRedirector(this.redirectionInfoProvider, Substitute.For<ILogger<ReadOnlyGridRedirector>>());
        }

        /// <summary>
        /// Tests that the right control is returned based on the environment, app, and user settings.
        /// </summary>
        /// <param name="isNewLookEnabled">A value indicating whether the new look is enabled.</param>
        /// <returns>The redirected type.</returns>
        [TestCase(false, ExpectedResult = typeof(IPcfGridControl))]
        [TestCase(true, ExpectedResult = typeof(IPowerAppsOneGrid))]
        public Type Redirect_RedirectionInfoSet_ReturnsCorrectControl(bool isNewLookEnabled)
        {
            this.environmentInfo.IsNewLookEnabled.Returns(isNewLookEnabled);

            return this.redirector.Redirect(new RedirectionControlInfo(Substitute.For<IAppPage>()));
        }
    }
}