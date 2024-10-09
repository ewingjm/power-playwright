namespace PowerPlaywright.IntegrationTests;

using System.Reflection;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Packaging;
using NuGet.Packaging.Signing;
using NuGet.Protocol.Core.Types;

/// <summary>
/// Global test setup.
/// </summary>
[SetUpFixture]
public class GlobalSetup
{
    /// <summary>
    /// One time setup before any tests run.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        var localFeedPath = Path.Join(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "packages");

        if (Directory.Exists(localFeedPath))
        {
            Directory.Delete(localFeedPath, true);
        }

        Directory.CreateDirectory(localFeedPath);

        var packagePath = Directory.GetFiles("./", "PowerPlaywright.Strategies.*.nupkg").OrderDescending().First();

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
    }
}