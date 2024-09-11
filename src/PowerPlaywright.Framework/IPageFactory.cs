namespace PowerPlaywright.Framework
{
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// A page factory.
    /// </summary>
    public interface IPageFactory
    {
        /// <summary>
        /// Create an instance of a <see cref="IBasePage"/> from a page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>The typed page.</returns>
        Task<IBasePage> CreateInstanceAsync(IPage page);

        /// <summary>
        /// Creates an instance of a <see cref="IBasePage"/> of type <typeparamref name="TPage"/>.
        /// </summary>
        /// <typeparam name="TPage">The type of <see cref="IModelDrivenAppPage"/>.</typeparam>
        /// <param name="page">The page.</param>
        /// <returns>The model-driven app page.</returns>
        Task<TPage> CreateInstanceAsync<TPage>(IPage page)
            where TPage : IBasePage;
    }
}