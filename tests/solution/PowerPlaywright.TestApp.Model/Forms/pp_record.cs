namespace PowerPlaywright.TestApp.Model
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Form related constants for the <see cref="pp_Record"/> table.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Generated partial class.")]
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Generated partial class.")]
    public partial class pp_Record
    {
        /// <summary>
        /// Constants relating to forms.
        /// </summary>
        public static class Forms
        {
            /// <summary>
            /// The Information form.
            /// </summary>
            public static class Information
            {
                /// <summary>
                /// The related records subgrid.
                /// </summary>
                public const string Owner = "ownerid";

                /// <summary>
                /// The related records subgrid.
                /// </summary>
                public const string RelatedRecordsSubgrid = "subgrid_relatedrecords";

                /// <summary>
                /// The related record lookup.
                /// </summary>
                public const string RelatedRecord = "pp_relatedrecord";

                /// <summary>
                /// The toggle control.
                /// </summary>
                public const string Toggle = "pp_yesno1";

                /// <summary>
                /// The Information form tabs.
                /// </summary>
                public static class Tabs
                {
                    /// <summary>
                    /// Tab A.
                    /// </summary>
                    public const string TabA = "Tab A";

                    /// <summary>
                    /// Tab B.
                    /// </summary>
                    public const string TabB = "Tab B";
                }
            }
        }
    }
}