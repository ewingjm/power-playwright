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
        var appPage = await this.ModelDrivenApp
            .LoginAsync(Configuration.Url, TestAppUniqueName, this.User.Username, this.User.Password);

        await this.Expect(appPage.Page).ToHaveURLAsync(MainPageRegex());
    }

    [GeneratedRegex(@".*\/main\.aspx.*")]
    private static partial Regex MainPageRegex();
}