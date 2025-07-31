namespace PowerPlaywright.Framework.Controls.External
{
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls.External.Attributes;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// A login control.
    /// </summary>
    [ExternalControl]
    public interface ILoginControl : IExternalControl
    {
        /// <summary>
        /// Logs in.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="totpSecret">The secret for TOTP if configured.</param>
        Task<IModelDrivenAppPage> LoginAsync(string username, string password, string totpSecret = null);
    }
}