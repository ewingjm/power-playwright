namespace PowerPlaywright.Strategies.Controls.Platform
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// A base class for all form controls.
    /// </summary>
    public abstract class FormControl : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormControl"/> class.
        /// </summary>
        /// <param name="appPage">The app page.</param>
        /// <param name="controlFactory">The control factory.</param>
        /// <param name="parent">The parent control.</param>
        protected FormControl(IAppPage appPage, IControlFactory controlFactory, IControl parent = null)
            : base(appPage, parent)
        {
            this.ControlFactory = controlFactory;
        }

        /// <summary>
        /// Gets the control factory.
        /// </summary>
        protected IControlFactory ControlFactory { get; }

        /// <summary>
        /// Gets the fields on the form.
        /// </summary>
        /// <returns>All fields on the form.</returns>
        protected async Task<IEnumerable<IField>> GetFieldsAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            var fieldLocators = await this.Container.Locator($"div[data-lp-id*='{this.GetFieldControlName()}']").AllAsync();

            var lpIdTasks = fieldLocators
                .Select(field => field.GetAttributeAsync(Attributes.DataLpId))
                .ToArray();

            var lpIds = await Task.WhenAll(lpIdTasks);

            var fields = lpIds
                .Select(lpId =>
                {
                    var fieldName = lpId.Split('|')[1];
                    return this.ControlFactory.CreateCachedInstance<IField>(this.AppPage, fieldName, this);
                })
                .ToList();

            return fields;
        }

        private string GetFieldControlName()
        {
            return this.ControlFactory
                .GetRedirectedType<IField>(this.AppPage)
                .GetCustomAttribute<PcfControlAttribute>().Name;
        }
    }
}
