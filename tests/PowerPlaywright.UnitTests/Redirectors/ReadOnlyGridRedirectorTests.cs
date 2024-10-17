namespace PowerPlaywright.UnitTests.Redirectors
{
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Redirectors;
    using PowerPlaywright.Strategies.Redirectors;

    /// <summary>
    /// Tests for the <see cref="ReadOnlyGridRedirector"/> class.
    /// </summary>
    public class ReadOnlyGridRedirectorTests
    {
        private IRedirectionInfoProvider<RedirectionInfo> redirectionInfoProvider;
        private ReadOnlyGridRedirector redirector;

        /// <summary>
        /// Sets up each test.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this.redirectionInfoProvider = Substitute.For<IRedirectionInfoProvider<RedirectionInfo>>();
            this.redirector = new ReadOnlyGridRedirector(this.redirectionInfoProvider, Substitute.For<ILogger<ReadOnlyGridRedirector>>());
        }

        /// <summary>
        /// Tests that the right control is returned based on the <see cref="AppToggles.ModernizationOptOut"/> flag.
        /// </summary>
        /// <param name="modernizationOptOut">The <see cref="AppToggles.ModernizationOptOut"/> flag.</param>
        /// <returns>The redirected type.</returns>
        [TestCase(false, ExpectedResult = typeof(IPcfGridControl))]
        [TestCase(true, ExpectedResult = typeof(IPowerAppsOneGridControl))]
        public Type Redirect_ModernizationOptOutSet_ReturnsCorrespondingReadOnlyGridInterface(bool modernizationOptOut)
        {
            this.redirectionInfoProvider.GetRedirectionInfo().Returns(
                new RedirectionInfo(
                    new AppToggles
                    {
                        ModernizationOptOut = modernizationOptOut,
                    }));

            return this.redirector.Redirect();
        }
    }
}