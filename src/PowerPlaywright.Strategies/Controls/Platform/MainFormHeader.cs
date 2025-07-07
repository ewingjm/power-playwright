namespace PowerPlaywright.Strategies.Controls.Platform
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// A main form header.
    /// </summary>
    [PlatformControlStrategy(0, 0, 0, 0)]
    public class MainFormHeader : FormControl, IMainFormHeader
    {
        private readonly IControlFactory controlFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainFormHeader"/> class.
        /// </summary>
        /// <param name="appPage">The app page.</param>
        /// <param name="controlFactory">The control factory.</param>
        /// <param name="parent">The parent control.</param>
        public MainFormHeader(IAppPage appPage, IControlFactory controlFactory, IControl parent = null)
            : base(appPage, controlFactory, parent)
        {
            this.controlFactory = controlFactory;
        }

        /// <inheritdoc/>
        public IField<TControl> GetField<TControl>(string name)
            where TControl : IPcfControl
        {
            return this.controlFactory.CreateCachedInstance<IField<TControl>>(this.AppPage, $"header_{name}", this);
        }

        /// <inheritdoc/>
        public IField GetField(string name)
        {
            return this.controlFactory.CreateCachedInstance<IField>(this.AppPage, $"header_{name}", this);
        }

        /// <inheritdoc/>
        public new async Task<IEnumerable<IField>> GetFieldsAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            return await base.GetFieldsAsync();
        }

        /// <inheritdoc/>
        public async ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);

            var form = this.Parent as IMainForm;
            await form?.CollapseHeaderAsync();
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return this.Page.Locator("#headerFieldsFlyout");
        }
    }
}
