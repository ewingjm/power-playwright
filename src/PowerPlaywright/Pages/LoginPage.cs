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
    internal class LoginPage : AppPage, ILoginPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginPage"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="controlFactory">The control factory.</param>
        public LoginPage(IPage page, IControlFactory controlFactory)
            : base(page, controlFactory)
        {
        }

        private ILoginControl LoginControl => this.ControlFactory.CreateInstance<ILoginControl>(this);

        /// <inheritdoc/>
        public async Task<IModelDrivenAppPage> LoginAsync(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException($"'{nameof(username)}' cannot be null or empty.", nameof(username));
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException($"'{nameof(password)}' cannot be null or empty.", nameof(password));
            }

            var homePage = await this.LoginControl.LoginAsync(username, password);

            return homePage;
        }
    }
}