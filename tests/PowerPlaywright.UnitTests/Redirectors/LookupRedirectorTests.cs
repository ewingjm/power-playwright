namespace PowerPlaywright.UnitTests.Redirectors
{
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Redirectors;
    using PowerPlaywright.Strategies.Redirectors;

    /// <summary>
    /// Tests for the <see cref="ReadOnlyGridRedirector"/> class.
    /// </summary>
    public class LookupRedirectorTests
    {
        private LookupRedirector redirector;

        /// <summary>
        /// Sets up each test.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this.redirector = new LookupRedirector();
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
            return this.redirector.Redirect(new ControlRedirectionInfo(new AppToggles { ModernizationOptOut = modernizationOptOut }));
        }
    }
}