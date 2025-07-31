namespace PowerPlaywright.Strategies.Controls.External
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Playwright;
    using OtpNet;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.External;
    using PowerPlaywright.Framework.Controls.External.Attributes;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Strategies.Extensions;

    /// <summary>
    /// Login control strategy.
    /// </summary>
    [ExternalControlStrategy(1)]
    public class LoginControl : Control, ILoginControl
    {
        private readonly IPageFactory pageFactory;
        private readonly ILogger<LoginControl> logger;

        private readonly ILocator usernameInput;
        private readonly ILocator nextButton;
        private readonly ILocator passwordInput;
        private readonly ILocator otpInput;
        private readonly ILocator workOrSchoolAccount;
        private readonly ILocator staySignedInButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginControl"/> class.
        /// </summary>
        /// <param name="appPage">The app page.</param>
        /// <param name="pageFactory">The page factory.</param>
        /// <param name="logger">The logger.</param>
        public LoginControl(IAppPage appPage, IPageFactory pageFactory, ILogger<LoginControl> logger = null)
            : base(appPage)
        {
            this.pageFactory = pageFactory ?? throw new System.ArgumentNullException(nameof(pageFactory));
            this.logger = logger;

            this.usernameInput = this.Container.GetByRole(AriaRole.Textbox).And(this.Container.Locator("input[type=email]"));
            this.nextButton = this.Container.GetByRole(AriaRole.Button).And(this.Container.Locator("input[type=submit]"));
            this.passwordInput = this.Container.GetByRole(AriaRole.Textbox).And(this.Container.Locator("input[type=password]"));
            this.otpInput = this.Container.GetByRole(AriaRole.Textbox).And(this.Container.Locator("input[type=tel]"));
            this.workOrSchoolAccount = this.Container.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Work or school account" });
            this.staySignedInButton = this.Container.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Yes" });
        }

        /// <inheritdoc/>
        public async Task<IModelDrivenAppPage> LoginAsync(string username, string password, string totpSecret = null)
        {
            await this.usernameInput.FocusAsync();
            await this.usernameInput.FillAsync(username);
            await this.nextButton.ClickAsync();

            try
            {
                await this.workOrSchoolAccount.Or(this.passwordInput).WaitForAsync(new LocatorWaitForOptions { Timeout = 10000 });
            }
            catch (TimeoutException)
            {
                if (await this.nextButton.IsVisibleAsync())
                {
                    await this.nextButton.ClickAsync();
                }
            }

            await this.workOrSchoolAccount.Or(this.passwordInput).ClickAsync();

            await this.passwordInput.FocusAsync();
            await this.passwordInput.FillAsync(password);
            await this.nextButton.ClickAsync();

            if (totpSecret != null)
            {
                var totp = new Totp(totpSecret.DecodeAsBase32String());

                try
                {
                    await this.otpInput.ClickAsync(new LocatorClickOptions { Timeout = 10000 });
                    await this.otpInput.FocusAsync();
                    await this.otpInput.FillAsync(totp.ComputeTotp());
                    await this.nextButton.ClickAsync();
                }
                catch (TimeoutException)
                {
                    // Swallow. MFA may not be configured.
                }
            }

            try
            {
                await this.staySignedInButton.ClickAsync(new LocatorClickOptions { Timeout = 10000 });
            }
            catch
            {
                // Ignore.
            }

            await this.Page.WaitForURLAsync("**/main.aspx*", new PageWaitForURLOptions { Timeout = 60000 });

            return (IModelDrivenAppPage)await this.pageFactory.CreateInstanceAsync(this.Page);
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Locator("div[id='lightbox']");
        }
    }
}