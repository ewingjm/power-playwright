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
        public PageFactory(IServiceProvider serviceProvider, ILogger<PageFactory> logger = null)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task<IBasePage> CreateInstanceAsync(IPage page)
        {
            if (page is null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            var pageType = await this.GetPageType(page) ?? throw new PowerPlaywrightException("Current page type is unrecognized.");

            return this.InstantiateAppPage(page, pageType);
        }

        /// <inheritdoc/>
        public async Task<TPage> CreateInstanceAsync<TPage>(IPage page)
            where TPage : IBasePage
        {
            if (page is null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            var pageType = await this.GetPageType(page) ?? throw new PowerPlaywrightException("Current page type is unrecognized.");
            var detectedType = this.GetPageMappedType(pageType);
            var requestedType = typeof(TPage);

            if (requestedType == detectedType || requestedType.IsAssignableFrom(detectedType))
            {
                return (TPage)this.InstantiateAppPage(page, pageType);
            }

            if (!detectedType.IsAssignableFrom(requestedType))
            {
                throw new NotFoundException($"Expected a page of type {requestedType.Name} but found a page of type {detectedType.Name}.");
            }

            return this.InstantiateAppPage<TPage>(page);
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

        private Type GetPageMappedType(ModelDrivenPageType pageType)
        {
            switch (pageType)
            {
                case ModelDrivenPageType.EntityList:
                    return typeof(IEntityListPage);
                case ModelDrivenPageType.EntityRecord:
                    return typeof(IEntityRecordPage);
                case ModelDrivenPageType.Login:
                    return typeof(ILoginPage);
                case ModelDrivenPageType.WebResource:
                    return typeof(IWebResourcePage);
                case ModelDrivenPageType.Dashboard:
                    return typeof(IDashboardPage);
                case ModelDrivenPageType.Custom:
                    return typeof(IModelDrivenAppPage);
                default:
                    return null;
            }
        }

        private IBasePage InstantiateAppPage(IPage page, ModelDrivenPageType? pageType)
        {
            switch (pageType)
            {
                case ModelDrivenPageType.EntityList:
                    return ActivatorUtilities.CreateInstance<EntityListPage>(this.serviceProvider, page);
                case ModelDrivenPageType.EntityRecord:
                    return ActivatorUtilities.CreateInstance<EntityRecordPage>(this.serviceProvider, page);
                case ModelDrivenPageType.Login:
                    return ActivatorUtilities.CreateInstance<LoginPage>(this.serviceProvider, page);
                case ModelDrivenPageType.Dashboard:
                    return ActivatorUtilities.CreateInstance<DashboardPage>(this.serviceProvider, page);
                case ModelDrivenPageType.WebResource:
                    return ActivatorUtilities.CreateInstance<WebResourcePage>(this.serviceProvider, page);
                case ModelDrivenPageType.Custom:
                    return ActivatorUtilities.CreateInstance<CustomPage>(this.serviceProvider, page);
                default:
                    throw new PowerPlaywrightException($"Unable to create {nameof(IModelDrivenAppPage)} instance. The page type can't be determined for {page.Url}.");
            }
        }

        private TPage InstantiateAppPage<TPage>(IPage page)
            where TPage : IBasePage
        {
            return ActivatorUtilities.CreateInstance<TPage>(this.serviceProvider, page);
        }
    }
}