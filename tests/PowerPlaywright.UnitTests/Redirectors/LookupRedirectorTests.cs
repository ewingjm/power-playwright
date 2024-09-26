namespace PowerPlaywright.UnitTests.Redirectors
{
    using Moq;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Redirectors;
    using PowerPlaywright.Strategies.Redirectors;

    /// <summary>
    /// Tests for the <see cref="ReadOnlyGridRedirector"/> class.
    /// </summary>
    public class LookupRedirectorTests
    {
        private Mock<IRedirectionInfoProvider<RedirectionInfo>> redirectionInfoProvider;
        private LookupRedirector redirector;

        /// <summary>
        /// Sets up each test.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this.redirectionInfoProvider = new Mock<IRedirectionInfoProvider<RedirectionInfo>>();
            this.redirector = new LookupRedirector(this.redirectionInfoProvider.Object);
        }

        /// <summary>
        /// Tests that the right control is returned based on the <see cref="AppToggles.ModernizationOptOut"/> flag.
        /// </summary>
        /// <param name="modernizationOptOut">The <see cref="AppToggles.ModernizationOptOut"/> flag.</param>
        /// <returns>The redirected type.</returns>
        [TestCase(false, ExpectedResult = typeof(ISimpleLookupControl))]
        [TestCase(true, ExpectedResult = typeof(ISimpleLookupControl))]
        public Type Redirect_ModernizationOptOutSet_ReturnsCorrespondingLookupInterface(bool modernizationOptOut)
        {
            this.redirectionInfoProvider.Setup(p => p.GetRedirectionInfo()).Returns(
                 new RedirectionInfo(
                     new AppToggles
                     {
                         ModernizationOptOut = modernizationOptOut,
                     }));

            return this.redirector.Redirect();
        }
    }
}