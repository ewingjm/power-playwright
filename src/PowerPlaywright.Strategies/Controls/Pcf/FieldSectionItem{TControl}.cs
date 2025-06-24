namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Model;
    using PowerPlaywright.Framework.Pages;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// A control strategy for the <see cref="IFieldSectionItem"/> interface.
    /// </summary>
    [PcfControlStrategy(0, 0, 0)]
    public class FieldSectionItem<TControl> : PcfControlInternal, IFieldSectionItem<TControl>
        where TControl : IPcfControl
    {
        private readonly IFieldSectionItem field;
        private readonly IControlFactory controlFactory;

        /// <inheritdoc/>
        public TControl Control => this.controlFactory.CreateInstance<TControl>(this.AppPage, this.Name, this);

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldSectionItem"/> class.
        /// </summary>
        /// <param name="name">The control name.</param>
        /// <param name="appPage">The app page.</param>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="controlFactory">The control factory.</param>
        /// <param name="parent">The parent control</param>
        public FieldSectionItem(string name, IAppPage appPage, IEnvironmentInfoProvider infoProvider, IControlFactory controlFactory, IControl parent = null)
            : base(name, appPage, infoProvider, parent)
        {
            this.controlFactory = controlFactory ?? throw new ArgumentNullException(nameof(controlFactory));
            this.field = this.controlFactory.CreateInstance<IFieldSectionItem>(this.AppPage, this.Name, this.Parent);
        }

        /// <inheritdoc/>
        public TPcfControl GetControl<TPcfControl>()
            where TPcfControl : IPcfControl
        {
            return this.controlFactory.CreateInstance<TPcfControl>(this.AppPage, this.Name, this);
        }

        /// <inheritdoc/>
        public Task<string> GetLabelAsync()
        {
            return this.field.GetLabelAsync();
        }

        /// <inheritdoc/>
        public Task<bool> IsDisabledAsync()
        {
            return this.field.IsDisabledAsync();
        }

        /// <inheritdoc/>
        public Task<FieldRequirementLevel> GetRequirementLevelAsync()
        {
            return this.field.GetRequirementLevelAsync();
        }

        /// <inheritdoc/>
        public Task<bool> IsVisibleAsync()
        {
            return this.field.IsVisibleAsync();
        }
    }
}
