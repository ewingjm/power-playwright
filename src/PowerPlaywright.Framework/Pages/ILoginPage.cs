namespace PowerPlaywright.Framework.Pages
{
    using System.Threading.Tasks;

    /// <summary>
    /// A login page.
    /// </summary>
    public interface ILoginPage : IAppPage
    {
        /// <summary>
        /// Logs in on the login page.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="totpSecret">The secret if TOTP is configured.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IModelDrivenAppPage> LoginAsync(string username, string password, string totpSecret = null);
    }
}