﻿namespace PowerPlaywright.UnitTests.Redirectors
{
    using Bogus;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Redirectors;
    using PowerPlaywright.Strategies.Redirectors;

    /// <summary>
    /// Tests for the <see cref="ReadOnlyGridRedirector"/> class.
    /// </summary>
    public class LookupRedirectorTests
    {
        private IRedirectionInfoProvider redirectionInfoProvider;
        private IRedirectionInfo redirectionInfo;
        private Faker faker;

        private LookupRedirector redirector;

        /// <summary>
        /// Sets up each test.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this.redirectionInfoProvider = Substitute.For<IRedirectionInfoProvider>();
            this.redirectionInfo = Substitute.For<IRedirectionInfo>();
            this.faker = new Faker();

            this.redirectionInfoProvider.GetRedirectionInfo().Returns(this.redirectionInfo);

            this.redirector = new LookupRedirector(this.redirectionInfoProvider, Substitute.For<ILogger<LookupRedirector>>());
        }

        /// <summary>
        /// Tests that the right control is returned based on the <see cref="AppSettings.NewLookOptOut"/> flag.
        /// </summary>
        /// <param name="isNewLookEnabled">A value indicating whether the new look is enabled.</param>
        /// <param name="isSemiAnnualChannel">A value indicating whether the active release channel is 'semi-annual'.</param>
        /// <returns>The redirected type.</returns>
        [TestCase(false, true, ExpectedResult = typeof(ISimpleLookupControl))]
        [TestCase(false, false, ExpectedResult = typeof(ISimpleLookupControl))]
        [TestCase(true, false, ExpectedResult = typeof(ISimpleLookupControl))]
        [TestCase(true, true, ExpectedResult = typeof(ISimpleLookupControl))]
        public Type Redirect_ModernizationOptOutSet_ReturnsCorrespondingLookupInterface(bool isNewLookEnabled, bool isSemiAnnualChannel)
        {
            this.redirectionInfo.IsNewLookEnabled.Returns(isNewLookEnabled);
            this.redirectionInfo.ActiveReleaseChannel.Returns(isSemiAnnualChannel ? (int)ReleaseChannel.SemiAnnualChannel : (int)this.faker.PickRandomWithout(ReleaseChannel.SemiAnnualChannel));

            return this.redirector.Redirect();
        }
    }
}