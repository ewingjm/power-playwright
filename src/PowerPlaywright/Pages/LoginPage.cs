namespace PowerPlaywright.Pages
{
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Model.Controls;
    using PowerPlaywright.Model.Controls.External;

    /// <summary>
    /// A login page.
    /// </summary>
    internal class LoginPage : ModelDrivenAppPage, ILoginPage
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
        public Task<IModelDrivenAppPage> LoginAsync(string username, string password)
        {
            return this.LoginControl.LoginAsync(username, password);
        }
    }
}