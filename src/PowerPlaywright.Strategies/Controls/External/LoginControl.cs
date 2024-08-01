namespace PowerPlaywright.Strategies.Controls.External
{
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Model.Controls;
    using PowerPlaywright.Model.Controls.External;

    /// <summary>
    /// Login control strategy.
    /// </summary>
    [ExternalControlStrategy(1)]
    public class LoginControl : Control, ILoginControl
    {
        private readonly ILocator usernameInput;
        private readonly ILocator nextButton;
        private readonly ILocator passwordInput;
        private readonly ILocator staySignedInButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginControl"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="logger">The logger.</param>
        public LoginControl(IPage page)
            : base(page)
        {
            this.usernameInput = this.Page.Locator("input[type=email]");
            this.nextButton = this.Page.Locator("input[type=submit]");
            this.passwordInput = this.Page.Locator("input[type=password]");
            this.staySignedInButton = this.Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions() { Name = "Yes" });
        }

        /// <inheritdoc/>
        public async Task LoginAsync(string username, string password)
        {
            await this.usernameInput.FillAsync(username);
            await this.nextButton.ClickAsync();
            await this.passwordInput.FillAsync(password);
            await this.nextButton.ClickAsync();
            await this.staySignedInButton.WaitForAsync(new LocatorWaitForOptions { Timeout = 5000 });

            if (await this.staySignedInButton.IsVisibleAsync())
            {
                await this.staySignedInButton.ClickAsync();
            }
        }
    }
}