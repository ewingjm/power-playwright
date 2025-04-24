namespace PowerPlaywright.Framework.Controls.Pcf
{
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// A PCF control.
    /// </summary>
    public abstract class PcfControl : Control, IPcfControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PcfControl"/> class.
        /// </summary>
        /// <param name="name">The name of the control.</param>
        /// <param name="nameOverride">The name overide when the name does not match the schema name in rare cases.</param>
        /// <param name="appPage">The app page.</param>
        /// <param name="parent">The parent conrol.</param>
        protected PcfControl(string name, IAppPage appPage, IControl parent = null, string nameOverride = null)
            : base(appPage, parent)
        {
            Name = name;
            NameOverride = nameOverride;
        }

        /// <summary>
        /// Gets the name of the control.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the name (override) of the control.
        /// </summary>
        public string NameOverride { get; }
    }
}