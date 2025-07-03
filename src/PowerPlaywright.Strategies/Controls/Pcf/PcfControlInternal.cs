namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// Internal abstract class for PCF controls.
    /// </summary>
    public abstract class PcfControlInternal : PcfControl
    {
        private readonly IEnvironmentInfoProvider infoProvider;

        private PcfControlAttribute pcfControlAttribute;

        /// <summary>
        /// Initializes a new instance of the <see cref="PcfControlInternal"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="appPage">The app page.</param>
        /// <param name="infoProvider">The environment info provider.</param>
        /// <param name="parent">The parent control.</param>
        protected PcfControlInternal(string name, IAppPage appPage, IEnvironmentInfoProvider infoProvider, IControl parent = null)
            : base(name, appPage, parent)
        {
            this.infoProvider = infoProvider;
        }

        /// <summary>
        /// Gets the <see cref="PcfControlAttribute"/> applicable to this control.
        /// </summary>
        public PcfControlAttribute PcfControlAttribute
        {
            get
            {
                if (this.pcfControlAttribute == null)
                {
                    this.pcfControlAttribute = this.GetType().GetInterfaces()
                        .FirstOrDefault(i => i.GetInterfaces().Contains(typeof(IPcfControl)) && i.GetCustomAttribute<PcfControlAttribute>() != null)
                        ?.GetCustomAttribute<PcfControlAttribute>();

                    if (this.pcfControlAttribute == null)
                    {
                        throw new InvalidOperationException($"PcfControlAttribute not found on {this.GetType().Name}.");
                    }
                }

                return this.pcfControlAttribute;
            }
        }

        /// <summary>
        /// Returns the control ID for the target environment.
        /// </summary>
        /// <returns>The control ID.</returns>
        protected Guid GetControlId()
        {
            return this.infoProvider.ControlIds.TryGetValue(this.PcfControlAttribute.Name, out var controlId)
                ? controlId
                : throw new InvalidOperationException($"Control ID for {this.PcfControlAttribute.Name} not found in environment info.");
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Locator($"//div[starts-with(@data-lp-id, '{this.PcfControlAttribute.Name}|') or starts-with(@data-lp-id, '{this.GetControlId()}|')]");
        }
    }
}