namespace PowerPlaywright
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Web;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Pages;
    using PowerPlaywright.Pages.Enums;

    /// <summary>
    /// Instantiates <see cref="IPage"/> objects.
    /// </summary>
    internal class PageFactory : IPageFactory
    {
        private const string QueryStringParamPageType = "pagetype";

        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<PageFactory> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageFactory"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="logger">The logger.</param>
        public PageFactory(IServiceProvider serviceProvider, ILogger<PageFactory> logger)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<IAppPage> CreateInstanceAsync(IPage page)
        {
            if (page is null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            var pageType = await this.GetConcreteTypeForPageAsync(page);

            return this.InstantiateAppPage(pageType, page);
        }

        /// <inheritdoc/>
        public async Task<TPage> CreateInstanceAsync<TPage>(IPage page)
            where TPage : IAppPage
        {
            if (page is null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            var requestedType = typeof(TPage);
            var pageType = await this.GetConcreteTypeForPageAsync(page);

            if (requestedType == pageType || requestedType.IsAssignableFrom(pageType))
            {
                return (TPage)this.InstantiateAppPage(pageType, page);
            }

            if (!pageType.IsAssignableFrom(requestedType))
            {
                throw new PowerPlaywrightException($"Expected a page of type {requestedType.Name} but found a page of type {pageType.Name}.");
            }

            return this.InstantiateAppPage<TPage>(page);
        }

        private async Task<Type> GetConcreteTypeForPageAsync(IPage page)
        {
            var pageType = await this.GetPageTypeAsync(page)
                ?? throw new PowerPlaywrightException("Current page type is unrecognized.");

            return this.GetPageMappedType(pageType);
        }

        private async Task<ModelDrivenPageType?> GetPageTypeAsync(IPage page)
        {
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

        private Type GetPageMappedType(ModelDrivenPageType pageType)
        {
            switch (pageType)
            {
                case ModelDrivenPageType.EntityList:
                    return typeof(EntityListPage);

                case ModelDrivenPageType.EntityRecord:
                    return typeof(EntityRecordPage);

                case ModelDrivenPageType.Login:
                    return typeof(LoginPage);

                case ModelDrivenPageType.WebResource:
                    return typeof(WebResourcePage);

                case ModelDrivenPageType.Dashboard:
                    return typeof(DashboardPage);

                case ModelDrivenPageType.Custom:
                    return typeof(CustomPage);

                case ModelDrivenPageType.Search:
                    return typeof(SearchPage);

                default:
                    throw new PowerPlaywrightException($"Page type {pageType} has not been mapped to a concrete type.");
            }
        }

        private IAppPage InstantiateAppPage(Type pageType, IPage page)
        {
            return (IAppPage)ActivatorUtilities.CreateInstance(this.serviceProvider, pageType, page);
        }

        private TPage InstantiateAppPage<TPage>(IPage page)
            where TPage : IAppPage
        {
            return ActivatorUtilities.CreateInstance<TPage>(this.serviceProvider, page);
        }
    }
}