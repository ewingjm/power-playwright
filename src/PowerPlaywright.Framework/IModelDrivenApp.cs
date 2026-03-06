namespace PowerPlaywright.Framework
{
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// An interface representing a model-driven app.
    /// </summary>
    public interface IModelDrivenApp
    {
        /// <summary>
        /// Logs in to the model-driven app.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="totpSecret">The secret if TOTP is configured.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IModelDrivenAppPage> LoginAsync(string username, string password, string totpSecret = null);

        /// <summary>
        /// Logs in to the model-driven app.
        /// </summary>
        /// <typeparam name="TModelDrivenAppPage">The type of home page.</typeparam>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="totpSecret">The secret if TOTP is configured.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<TModelDrivenAppPage> LoginAsync<TModelDrivenAppPage>(string username, string password, string totpSecret = null)
            where TModelDrivenAppPage : IModelDrivenAppPage;
    }
}