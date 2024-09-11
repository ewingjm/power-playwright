namespace PowerPlaywright.Framework
{
    using System;
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
        /// <param name="environmentUrl">The environment URL.</param>
        /// <param name="appUniqueName">The unique name of the app.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IModelDrivenAppPage> LoginAsync(Uri environmentUrl, string appUniqueName, string username, string password);
    }
}