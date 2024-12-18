namespace PowerPlaywright.UnitTests.Redirectors
{
    using System.Text.Json;
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
        /// Tests that the right control is returned based on the environment, app, and user settings.
        /// </summary>
        /// <returns>The redirected type.</returns>
        /// <param name="orgChannel">The release channel configured for the environment.</param>
        /// <param name="appChannel">The release channel configured for the app.</param>
        /// <param name="userOverride">The release channel configured for the user.</param>
        /// <param name="newLookAlwaysOn">The new look always on setting.</param>
        /// <param name="newLookOptOut">The new look opt out setting.</param>
        /// <param name="modernizationOptOut">The modernization opt out user app toggle.</param>
        [TestCaseSource(nameof(GetRedirectionTestCases))]
        public Type Redirect_RedirectionInfoSet_ReturnsCorrectControl(ReleaseChannel orgChannel, ReleaseChannel appChannel, ReleaseChannelOverride userOverride, bool newLookAlwaysOn, bool newLookOptOut, bool? modernizationOptOut)
        {
            var appId = Guid.NewGuid();
            var jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.Converters.Add(new AppTogglesConverter(appId));
            var redirectionInfo = new RedirectionInfo(
                JsonSerializer.Deserialize<OrgSettings>($"{{ \"ReleaseChannel\": {(int)orgChannel} }}"),
                JsonSerializer.Deserialize<AppSettings>($"{{ \"AppChannel\": {(int)appChannel}, \"NewLookAlwaysOn\": {newLookAlwaysOn.ToString().ToLower()}, \"NewLookOptOut\": {newLookOptOut.ToString().ToLower()} }}"),
                JsonSerializer.Deserialize<UserSettings>($"{{ \"ReleaseChannel\": {(int)userOverride}, \"TryToggleSets\": {{ \"ModernizationOptOut\": {modernizationOptOut?.ToString().ToLower() ?? "null"} }} }}"));

            this.redirectionInfoProvider
                .GetRedirectionInfo()
                .Returns(redirectionInfo);

            return this.redirector.Redirect();
        }

        /// <summary>
        /// Gets the test cases for <see cref="Redirect_SemiAnnualChannelWithModernizationOptOutFalse_ReturnsPcfGridControl(ReleaseChannel, ReleaseChannel, ReleaseChannelOverride, bool)"/>.
        /// </summary>
        /// <returns>The test case data.</returns>
        private static IEnumerable<TestCaseData> GetRedirectionTestCases()
        {
            var releaseChannels = Enum.GetValues(typeof(ReleaseChannel)).Cast<ReleaseChannel>();
            var overrides = Enum.GetValues(typeof(ReleaseChannelOverride)).Cast<ReleaseChannelOverride>();
            var settingValues = new[] { false, true };
            var tryToggleSetValues = new bool?[] { null, false, true };

            foreach (var envChannel in releaseChannels)
            {
                foreach (var appChannel in releaseChannels)
                {
                    foreach (var userOverride in overrides)
                    {
                        foreach (var newLookAlwaysOn in settingValues)
                        {
                            foreach (var newLookOptOut in settingValues)
                            {
                                foreach (var modernizationOptOut in tryToggleSetValues)
                                {
                                    yield return new TestCaseData(envChannel, appChannel, userOverride, newLookAlwaysOn, newLookOptOut, modernizationOptOut)
                                    {
                                        ExpectedResult = GetExpectedType(envChannel, appChannel, userOverride, newLookAlwaysOn, newLookOptOut, modernizationOptOut),
                                    };
                                }
                            }
                        }
                    }
                }
            }
        }

        private static Type GetExpectedType(ReleaseChannel environmentChannel, ReleaseChannel appChannel, ReleaseChannelOverride userOverride, bool newLookAlwaysOn, bool newLookOptOut, bool? modernizationOptOut)
        {
            if (IsSemiAnnualChannel(environmentChannel, appChannel, userOverride) && !IsNewLookEnabled(newLookAlwaysOn, newLookOptOut, modernizationOptOut))
            {
                return typeof(IPcfGridControl);
            }

            return typeof(IPowerAppsOneGridControl);
        }

        private static bool IsNewLookEnabled(bool newLookAlwaysOn, bool newLookOptOut, bool? modernizationOptOut)
        {
            if (newLookAlwaysOn)
            {
                return true;
            }

            if (!newLookOptOut)
            {
                return false;
            }

            if (modernizationOptOut.HasValue)
            {
                return modernizationOptOut.Value;
            }

            return true;
        }

        private static bool IsSemiAnnualChannel(ReleaseChannel environmentChannel, ReleaseChannel appChannel, ReleaseChannelOverride userOverride)
        {
            if (userOverride == ReleaseChannelOverride.SemiAnnual)
            {
                return true;
            }

            if (userOverride == ReleaseChannelOverride.None && appChannel == ReleaseChannel.SemiAnnualChannel)
            {
                return true;
            }

            if (userOverride == ReleaseChannelOverride.None && appChannel == ReleaseChannel.Auto && environmentChannel == ReleaseChannel.SemiAnnualChannel)
            {
                return true;
            }

            return false;
        }
    }
}