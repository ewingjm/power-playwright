namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Controls.Pcf;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.Playwright;
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Framework.Controls;
    using System.Linq;

    /// <summary>
    /// A control strategy for the <see cref="IUpdMSPicklistControl"/>.
    /// </summary>
    [PcfControlStrategy(0, 0, 0)]
    public class UpdMSPicklistControl : PcfControl, IUpdMSPicklistControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdMSPicklistControl"/> class.
        /// </summary>
        /// <param name="name">The name given to the control.</param>
        /// <param name="appPage">The app page.</param>
        /// <param name="parent">The parent control.</param>
        public UpdMSPicklistControl(string name, IAppPage appPage, IControl parent = null) : base(name, appPage, parent)
        {
        }

        /// <inheritdoc/>
        public Task<int?[]> GetValueAsync()
        {
            return null;
            //var fieldValue = Xrm.Page.getAttribute("pp_choices").getValue();
            //console.log(fieldValue);
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(int[] optionValues)
        {
            await AppPage.Page.EvaluateAsync(@"(fieldName, optionValues) => {
        const attribute = Xrm.Page.getAttribute(fieldName);
        if (!attribute) {
            console.error(`Field '${fieldName}' not found.`);
            throw new Error(`Field '${fieldName}' not found.`);
        }
        attribute.setValue(optionValues);  // Must be array of integers for multi-select
        attribute.fireOnChange();
    }", new object[] { this.Name, optionValues });
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Locator($"div[data-lp-id*='MscrmControls.MultiSelectPicklist.UpdMSPicklistControl|{this.Name}.fieldControl|']");
        }
    }
}