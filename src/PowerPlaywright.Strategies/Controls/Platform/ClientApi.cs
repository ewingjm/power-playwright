namespace PowerPlaywright.Strategies.Controls.Platform
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// A 'control' that provides client API functionality.
    /// TODO: Move to PowerPlaywright and use PlatformReference class for scripts.
    /// </summary>
    [PlatformControlStrategy(0, 0, 0, 0)]
    public class ClientApi : Control, IClientApi
    {
        private readonly IPageFactory pageFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientApi"/> class.
        /// </summary>
        /// <param name="appPage">The app page.</param>
        /// <param name="pageFactory">The page factory.</param>
        public ClientApi(IAppPage appPage, IPageFactory pageFactory)
            : base(appPage, null)
        {
            this.pageFactory = pageFactory;
        }

        /// <inheritdoc/>
        public async Task<IEntityRecordPage> NavigateToRecordAsync(string entityName, Guid entityId)
        {
            await this.Page.EvaluateAsync(
                @"
                    async ({ entityName, entityId } ) => { 
                        await Xrm.Navigation.navigateTo(
                            { 
                                pageType: 'entityrecord',
                                entityName: entityName,
                                entityId: entityId 
                            }
                        ) 
                    } ",
                new { entityName, entityId });

            await this.Page.WaitForAppIdleAsync();

            return await this.pageFactory.CreateInstanceAsync<IEntityRecordPage>(this.Page);
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            throw new NotSupportedException($"The {nameof(ClientApi)} control does not have a root.");
        }
    }
}