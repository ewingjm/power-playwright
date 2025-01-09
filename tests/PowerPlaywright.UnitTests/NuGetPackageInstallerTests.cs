namespace PowerPlaywright.UnitTests;

using System.Reflection;
using NSubstitute;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using PowerPlaywright.Framework;

/// <summary>
/// Unit tests for the <see cref="NuGetPackageInstaller"/> class.
/// </summary>
[TestFixture]
public class NuGetPackageInstallerTests
{
    private const string PackageId = "PowerPlaywright.Strategies";
    private static readonly string Source = Path.Join(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "packages");

    private FindPackageByIdResource findPackageByIdResource;
    private NuGetPackageInstaller packageInstaller;
    private PackageIdentity package;

    /// <summary>
    /// Initializes the test setup by creating a new instance of <see cref="NuGetPackageInstaller"/> with the specified source.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this.findPackageByIdResource = Substitute.For<FindPackageByIdResource>();
        this.packageInstaller = new NuGetPackageInstaller(this.findPackageByIdResource, Source);
        this.package = new(PackageId, new NuGetVersion(1, 0, 0));
    }

    /// <summary>
    /// Tests that the <see cref="NuGetPackageInstaller"/> constructor throws an <see cref="ArgumentNullException"/> when the <paramref name="findPackageByIdResource"/> parameter is null.
    /// </summary>
    [Test]
    public void Constructor_NullFindPackageByIdResource_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new NuGetPackageInstaller(null, Source));
    }

    /// <summary>
    /// Tests that the <see cref="NuGetPackageInstaller"/> constructor throws an <see cref="ArgumentException"/> when the <paramref name="source"/> parameter is null.
    /// </summary>
    [Test]
    public void Constructor_NullSource_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new NuGetPackageInstaller(this.findPackageByIdResource, null));
    }

    /// <summary>
    /// Tests that the <see cref="NuGetPackageInstaller.GetAllVersionsAsync"/> method throws an <see cref="ArgumentException"/> when the <paramref name="packageId"/> parameter is <see langword="null"/>.
    /// </summary>
    [Test]
    public void GetAllVersionsAsync_NullPackageId_ThrowsArgumentException()
    {
        Assert.ThrowsAsync<ArgumentException>(() => this.packageInstaller.GetAllVersionsAsync(null));
    }

    /// <summary>
    /// Tests that the <see cref="NuGetPackageInstaller.GetAllVersionsAsync"/> method returns all versions of the package.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetAllVersionsAsync_Always_ReturnsAllPackageVersions()
    {
        var expectedVersions = new NuGetVersion[] { new(1, 0, 0), new(1, 0, 1) };
        this.findPackageByIdResource.GetAllVersionsAsync(
            PackageId,
            Arg.Any<SourceCacheContext>(),
            Arg.Any<ILogger>(),
            Arg.Any<CancellationToken>())
        .Returns(expectedVersions);

        var versions = await this.packageInstaller.GetAllVersionsAsync(PackageId);

        Assert.That(versions, Is.EqualTo(expectedVersions));
    }

    /// <summary>
    /// Tests that the <see cref="NuGetPackageInstaller.InstallPackageAsync"/> method throws an <see cref="ArgumentNullException"/> when the <paramref name="packageIdentity"/> parameter is <see langword="null"/>.
    /// </summary>
    [Test]
    public void InstallPackageAsync_NullPackageIdentity_ThrowsArgumentNullException()
    {
        Assert.ThrowsAsync<ArgumentNullException>(() => this.packageInstaller.InstallPackageAsync(null));
    }

    /// <summary>
    /// Tests that the <see cref="NuGetPackageInstaller.InstallPackageAsync"/> method throws a <see cref="PowerPlaywrightException"/> when the package can't be copied to a stream.
    /// </summary>
    [Test]
    public void InstallPackageAsync_FailedToCopyToStream_ThrowsPowerPlaywrightException()
    {
        this.findPackageByIdResource.CopyNupkgToStreamAsync(
            PackageId,
            this.package.Version,
            Arg.Any<Stream>(),
            Arg.Any<SourceCacheContext>(),
            Arg.Any<ILogger>(),
            Arg.Any<CancellationToken>())
        .Returns(false);

        Assert.ThrowsAsync<PowerPlaywrightException>(() => this.packageInstaller.InstallPackageAsync(this.package));
    }

    /// <summary>
    /// Tests that the <see cref="NuGetPackageInstaller.InstallPackageAsync"/> method adds the package to the global packages folder.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task InstallPackageAsync_Always_AddsPackageToGlobalPackagesFolder()
    {
        var nupkgBytes = await File.ReadAllBytesAsync(
            Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources/PowerPlaywright.Strategies.1.0.0.nupkg"));
        this.findPackageByIdResource.CopyNupkgToStreamAsync(
            PackageId,
            this.package.Version,
            Arg.Any<Stream>(),
            Arg.Any<SourceCacheContext>(),
            Arg.Any<ILogger>(),
            Arg.Any<CancellationToken>())
        .Returns(true)
        .AndDoes(i => i.ArgAt<Stream>(2).Write(nupkgBytes, 0, nupkgBytes.Length));
        var packageIdentity = new PackageIdentity(PackageId, new NuGetVersion(1, 0, 0));
        var settings = Settings.LoadDefaultSettings(null);

        await this.packageInstaller.InstallPackageAsync(this.package);

        Assert.That(
            GlobalPackagesFolderUtility.GetPackage(packageIdentity, SettingsUtility.GetGlobalPackagesFolder(settings)).Status,
            Is.EqualTo(DownloadResourceResultStatus.Available));
    }
}