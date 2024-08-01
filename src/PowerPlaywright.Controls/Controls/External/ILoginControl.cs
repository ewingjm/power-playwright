namespace PowerPlaywright.Model.Controls.External
{
    using System.Threading.Tasks;
    using PowerPlaywright.Model.Controls;
    using PowerPlaywright.Pages;

    /// <summary>
    /// A login control.
    /// </summary>
    [ExternalControl]
    public interface ILoginControl : IControl
    {
        /// <summary>
        /// Logs in.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        Task<IModelDrivenAppPage> LoginAsync(string username, string password);
    }
}