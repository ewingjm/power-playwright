namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// A control strategy for the <see cref="IToggleControl"/>.
    /// </summary>
    [PcfControlStrategy(1, 0, 23)]
    public class ToggleControl : PcfControlInternal, IToggleControl
    {
        private readonly ILocator toggleBtn;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToggleControl"/> class.
        /// </summary>
        /// <param name="name">The name given to the control.</param>
        /// <param name="appPage">The app page.</param>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="parent">The parent control.</param>
        public ToggleControl(string name, IAppPage appPage, IEnvironmentInfoProvider infoProvider, IControl parent = null)
            : base(name, appPage, infoProvider, parent)
        {
            this.toggleBtn = this.Parent.Parent.Container.GetByRole(AriaRole.Switch);
        }

        /// <inheritdoc/>
        public async Task<bool> GetValueAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            return await this.toggleBtn.IsCheckedAsync();
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(bool value)
        {
            await this.Page.WaitForAppIdleAsync();

            if (await this.toggleBtn.IsVisibleAsync())
            {
                if (await this.GetValueAsync() == value)
                {
                    return;
                }

                await this.toggleBtn.ClickAsync();
            }
        }
    }
}