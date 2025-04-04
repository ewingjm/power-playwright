namespace PowerPlaywright.TestApp.PageObjects
{
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// A control strategy for the <see cref="ICustomControl"/>.
    /// </summary>
    [PcfControlStrategy(0, 0, 0)]
    public class CustomControl : PcfControl, ICustomControl
    {
        private readonly ILocator input;
        private readonly IEnvironmentInfoProvider environmentInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomControl"/> class.
        /// </summary>
        /// <param name="environmentInfo">The environment info provider.</param>
        /// <param name="name">The name.</param>
        /// <param name="appPage">The app page.</param>
        /// <param name="parent">The parent control.</param>
        public CustomControl(IEnvironmentInfoProvider environmentInfo, string name, IAppPage appPage, IControl parent = null)
            : base(name, appPage, parent)
        {
            this.environmentInfo = environmentInfo;

            this.input = this.Container.Locator("input");
        }

        /// <inheritdoc/>
        public async Task<bool> GetValueAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            return await this.input.IsCheckedAsync();
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(bool value)
        {
            if (value == await this.GetValueAsync())
            {
                return;
            }

            await this.input.ClickAsync();
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Locator($"div[data-lp-id*='{this.environmentInfo.ControlIds[typeof(ICustomControl).GetCustomAttribute<PcfControlAttribute>().Name]}|{this.Name}.fieldControl']");
        }
    }
}
