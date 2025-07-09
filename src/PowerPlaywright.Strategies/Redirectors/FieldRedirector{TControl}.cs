namespace PowerPlaywright.Strategies.Redirectors
{
    using System;
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Redirectors;

    /// <summary>
    /// Redirects requests for an <see cref="IField{TControl}"/> control.
    /// </summary>
    /// <typeparam name="TControl">The control type.</typeparam>
    public class FieldRedirector<TControl> : ControlRedirector<IField<TControl>>
        where TControl : IPcfControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldRedirector{TControl}"/> class.
        /// </summary>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="logger">The logger.</param>
        public FieldRedirector(IRedirectionInfoProvider infoProvider, ILogger<FieldRedirector> logger)
            : base(infoProvider, logger)
        {
        }

        /// <inheritdoc/>
        protected override Type GetTargetControlType(IRedirectionEnvironmentInfo environmentInfo, RedirectionControlInfo controlInfo)
        {
            return typeof(IFieldSectionItem<TControl>);
        }
    }
}