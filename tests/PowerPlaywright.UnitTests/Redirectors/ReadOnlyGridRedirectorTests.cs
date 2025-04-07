namespace PowerPlaywright.UnitTests.Redirectors
{
    using System.Text.Json;
    using Bogus;
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
        private IRedirectionInfo redirectionInfo;
        private IRedirectionInfoProvider redirectionInfoProvider;
        private Faker faker;

        private ReadOnlyGridRedirector redirector;

        /// <summary>
        /// Sets up each test.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this.redirectionInfo = Substitute.For<IRedirectionInfo>();
            this.redirectionInfoProvider = Substitute.For<IRedirectionInfoProvider>();
            this.faker = new Faker();

            this.redirectionInfoProvider.GetRedirectionInfo().Returns(this.redirectionInfo);

            this.redirector = new ReadOnlyGridRedirector(this.redirectionInfoProvider, Substitute.For<ILogger<ReadOnlyGridRedirector>>());
        }

        /// <summary>
        /// Tests that the right control is returned based on the environment, app, and user settings.
        /// </summary>
        /// <param name="isNewLookEnabled">A value indicating whether the new look is enabled.</param>
        /// <param name="isSemiAnnualChannel">A value indicating whether the active release channel is 'semi-annual'.</param>
        /// <returns>The redirected type.</returns>
        [TestCase(false, true, ExpectedResult = typeof(IPcfGridControl))]
        [TestCase(false, false, ExpectedResult = typeof(IPowerAppsOneGrid))]
        [TestCase(true, false, ExpectedResult = typeof(IPowerAppsOneGrid))]
        [TestCase(true, true, ExpectedResult = typeof(IPowerAppsOneGrid))]
        public Type Redirect_RedirectionInfoSet_ReturnsCorrectControl(bool isNewLookEnabled, bool isSemiAnnualChannel)
        {
            this.redirectionInfo.IsNewLookEnabled.Returns(isNewLookEnabled);
            this.redirectionInfo.ActiveReleaseChannel.Returns(isSemiAnnualChannel ? (int)ReleaseChannel.SemiAnnualChannel : (int)this.faker.PickRandomWithout(ReleaseChannel.SemiAnnualChannel));

            return this.redirector.Redirect();
        }
    }
}