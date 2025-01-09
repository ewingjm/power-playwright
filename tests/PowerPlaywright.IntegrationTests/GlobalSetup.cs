namespace PowerPlaywright.IntegrationTests;

using System.Text.RegularExpressions;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Packaging;
using NuGet.Packaging.Signing;
using NuGet.Protocol.Core.Types;

/// <summary>
/// Global test setup.
/// </summary>
[SetUpFixture]
public partial class GlobalSetup
{
    private const string AssemblyName = "PowerPlaywright.Strategies.dll";

    /// <summary>
    /// One time setup before any tests run.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        var localFeedPath = Path.Join(TestContext.CurrentContext.TestDirectory, "packages");

        if (Directory.Exists(localFeedPath))
        {
            Directory.Delete(localFeedPath, true);
        }

        Directory.CreateDirectory(localFeedPath);

        var packagePath = Directory.GetFiles(
            TestContext.CurrentContext.TestDirectory,
            "PowerPlaywright.Strategies.*.nupkg").OrderDescending().First();

        await OfflineFeedUtility.AddPackageToSource(
            new OfflineFeedAddContext(
                packagePath,
                localFeedPath,
                NullLogger.Instance,
                true,
                true,
                true,
                new PackageExtractionContext(
                    PackageSaveMode.Defaultv3,
                    XmlDocFileSaveMode.None,
                    ClientPolicyContext.GetClientPolicy(Settings.LoadDefaultSettings(null), NullLogger.Instance),
                    NullLogger.Instance)),
            CancellationToken.None);

        // The assembly in the test directory is instrumented by coverlet. Copying this to the feed.
        var version = PackageVersionRegex().Match(packagePath).Groups[1].Value;
        File.Copy(
            Path.Join(TestContext.CurrentContext.TestDirectory, AssemblyName),
            Path.Join(localFeedPath, "powerplaywright.strategies", version, "lib", "netstandard2.0", AssemblyName),
            true);
    }

    [GeneratedRegex(@"PowerPlaywright\.Strategies\.((\d\.\d\.\d)(-.*)?)(\.\d+)?\.nupkg")]
    private static partial Regex PackageVersionRegex();
}