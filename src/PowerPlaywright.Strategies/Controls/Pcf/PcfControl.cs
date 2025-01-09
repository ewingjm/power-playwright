namespace PowerPlaywright.Strategies.Controls
{
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf;
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
        /// <param name="appPage">The app page.</param>
        /// <param name="parent">The parent conrol.</param>
        protected PcfControl(string name, IAppPage appPage, IControl parent = null)
            : base(appPage, parent)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets the name of the control.
        /// </summary>
        public string Name { get; }
    }
}