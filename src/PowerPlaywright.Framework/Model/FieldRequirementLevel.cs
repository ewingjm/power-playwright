namespace PowerPlaywright.Framework.Model
{
    /// <summary>
    /// The field requirement level. Refer to https://learn.microsoft.com/en-us/power-apps/developer/model-driven-apps/clientapi/reference/attributes/getrequiredlevel.
    /// </summary>
    public enum FieldRequirementLevel
    {
        /// <summary>
        /// The field is optional.
        /// </summary>
        None = 0,

        /// <summary>
        /// The field is mandatory.
        /// </summary>
        Required = 2,

        /// <summary>
        /// The field is recommended.
        /// </summary>
        Recommended = 3,
    }
}
