namespace PowerPlaywright.Pages
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Web;
    using Microsoft.Playwright;
    using PowerPlaywright;
    using PowerPlaywright.Controls;
    using PowerPlaywright.Model.Controls;

    /// <summary>
    /// Instantiates <see cref="IPage"/> objects.
    /// </summary>
    internal class PageFactory : IPageFactory
    {
        private const string QueryStringParamPageType = "pagetype";

        private readonly IControlFactory controlFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageFactory"/> class.
        /// </summary>
        /// <param name="controlFactory">The control factory.</param>
        public PageFactory(IControlFactory controlFactory)
        {
            this.controlFactory = controlFactory ?? throw new ArgumentNullException(nameof(controlFactory));
        }

        /// <inheritdoc/>
        public async Task<IModelDrivenAppPage> CreateInstanceAsync(IPage page)
        {
            if (page is null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            var pageType = await this.GetPageType(page);
            var appPage = this.InstantiateAppPage(page, pageType);

            return appPage;
        }

        /// <inheritdoc/>
        public async Task<TPage> CreateInstanceAsync<TPage>(IPage page)
            where TPage : class, IModelDrivenAppPage
        {
            if (page is null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            var pageInstance = await this.CreateInstanceAsync(page);

            if (pageInstance is TPage result)
            {
                return result;
            }

            throw new PowerPlaywrightException($"Expected page to be of type {typeof(TPage).Name} page but found {pageInstance.GetType().Name}.");
        }

        private async Task<ModelDrivenPageType?> GetPageType(IPage page)
        {
            if (page is null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            if (page.Url.StartsWith("https://login.microsoftonline.com"))
            {
                return ModelDrivenPageType.Login;
            }

            await page.WaitForURLAsync(new Regex(@".*\?.*pagetype=.*"));

            if (Enum.TryParse<ModelDrivenPageType>(HttpUtility.ParseQueryString(new Uri(page.Url).Query).Get(QueryStringParamPageType), true, out var pageType))
            {
                return pageType;
            }

            return null;
        }

        private IModelDrivenAppPage InstantiateAppPage(IPage page, ModelDrivenPageType? pageType)
        {
            switch (pageType)
            {
                case ModelDrivenPageType.EntityList:
                    return new EntityListPage(page, this.controlFactory);
                case ModelDrivenPageType.EntityRecord:
                    return new EntityRecordPage(page, this.controlFactory);
                case ModelDrivenPageType.Login:
                    return new LoginPage(page, this.controlFactory);
                case ModelDrivenPageType.Dashboard:
                    return new DashboardPage(page, this.controlFactory);
                default:
                    throw new PowerPlaywrightException($"Unable to create {nameof(IModelDrivenAppPage)} instance. The page type can't be determined for {page.Url}.");
            }
        }
    }
}