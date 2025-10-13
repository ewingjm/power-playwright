namespace PowerPlaywright.IntegrationTests;

using System.Text.RegularExpressions;

/// <summary>
/// Tests for the <see cref="ModelDrivenApp"/> class.
/// </summary>
public partial class ModelDrivenAppTests : IntegrationTests
{
    /// <summary>
    /// Calling <see cref="ModelDrivenApp.LoginAsync"/> with valid credentials should login to the app.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task LoginAsync_ValidParameters_LogsInToApp()
    {
        var appPage = await this.PowerPlaywright
            .LaunchAppAsync(this.Context, Configuration.Url, TestAppUniqueName, this.User.Username, this.User.Password, this.User.TOTPSecret);

        await this.Expect(appPage.Page).ToHaveURLAsync(MainPageRegex());
    }

    /// <summary>
    /// Tests that <see cref="ModelDrivenApp.ClientApi.OpenFormAsync"/> opens an entity form in create mode.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task OpenFormAsync_EntityLogicalName_OpensEntityFormInCreateMode()
    {
        var appPage = await this.PowerPlaywright
            .LaunchAppAsync(this.Context, Configuration.Url, TestAppUniqueName, this.User.Username, this.User.Password, this.User.TOTPSecret);

        var newRecordPage = await appPage.ClientApi.OpenFormAsync("pp_record");

        await this.Expect(newRecordPage.Form.Container).ToBeVisibleAsync();
    }

    [GeneratedRegex(@".*\/main\.aspx.*")]
    private static partial Regex MainPageRegex();
}