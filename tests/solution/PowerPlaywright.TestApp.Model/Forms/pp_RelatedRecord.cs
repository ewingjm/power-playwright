namespace PowerPlaywright.TestApp.Model
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Form related constants for the <see cref="pp_RelatedRecord"/> table.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Generated partial class.")]
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Generated partial class.")]
    public partial class pp_RelatedRecord
    {
        /// <summary>
        /// Constants relating to forms.
        /// </summary>
        public static class Forms
        {
            /// <summary>
            /// The quick view Information form.
            /// </summary>
            public static class QuickViewInformation
            {
                /// <summary>
                /// The name.
                /// </summary>
                public const string Name = "pp_name";

                /// <summary>
                /// The owner.
                /// </summary>
                public const string Owner = "ownerid";
            }
        }
    }
}