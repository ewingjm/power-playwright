namespace PowerPlaywright.Pages
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls.External;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// A login page.
    /// </summary>
    internal class LoginPage : BasePage, ILoginPage
    {
        private ILoginControl loginControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginPage"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="controlFactory">The control factory.</param>
        public LoginPage(IPage page, IControlFactory controlFactory)
            : base(page, controlFactory)
        {
        }

        private ILoginControl LoginControl => this.loginControl ?? (this.loginControl = this.ControlFactory.CreateInstance<ILoginControl>(this.Page));

        /// <inheritdoc/>
        public async Task<IModelDrivenAppPage> LoginAsync(string username, string password)
        {
            var homePage = await this.LoginControl.LoginAsync(username, password);

            return homePage;
        }
    }
}