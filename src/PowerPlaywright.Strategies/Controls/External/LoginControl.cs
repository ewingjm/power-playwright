﻿namespace PowerPlaywright.Strategies.Controls.External
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.External;
    using PowerPlaywright.Framework.Controls.External.Attributes;
    using PowerPlaywright.Framework.Pages;

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

            this.usernameInput = this.Container.Locator("input[type=email]");
            this.nextButton = this.Container.Locator("input[type=submit]");
            this.passwordInput = this.Container.Locator("input[type=password]");
            this.workOrSchoolAccount = this.Container.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Work or school account" });
            this.staySignedInButton = this.Container.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Yes" });
        }

        /// <inheritdoc/>
        public async Task<IModelDrivenAppPage> LoginAsync(string username, string password)
        {
            await this.usernameInput.FillAsync(username);
            await this.nextButton.ClickAsync();

            if (await this.workOrSchoolAccount.IsVisibleAsync())
            {
                await this.workOrSchoolAccount.ClickAsync();
            }

            await this.passwordInput.FillAsync(password);
            await this.nextButton.ClickAsync();
            await this.staySignedInButton.WaitForAsync(new LocatorWaitForOptions { Timeout = 5000 });

            if (await this.staySignedInButton.IsVisibleAsync())
            {
                await this.staySignedInButton.ClickAsync();
            }

            await this.Page.WaitForURLAsync("**/main.aspx*");

            return (IModelDrivenAppPage)await this.pageFactory.CreateInstanceAsync(this.Page);
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Locator("div[id='lightbox']");
        }
    }
}