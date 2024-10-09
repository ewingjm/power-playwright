namespace PowerPlaywright.Strategies.Controls.Platform
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// A 'control' that provides client API functionality.
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
        protected override ILocator Root => throw new NotSupportedException($"The {nameof(ClientApi)} control does not have a root.");

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

            return await this.pageFactory.CreateInstanceAsync<IEntityRecordPage>(this.Page);
        }
    }
}