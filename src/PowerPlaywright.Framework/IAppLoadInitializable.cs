namespace PowerPlaywright.Framework
{
    using System.Threading.Tasks;
    using Microsoft.Playwright;

    /// <summary>
    /// Initializable when an app is loaded.
    /// </summary>
    public interface IAppLoadInitializable
    {
        /// <summary>
        /// Intializes the object that is dependent on app load.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task InitializeAsync(IPage page);
    }
}
