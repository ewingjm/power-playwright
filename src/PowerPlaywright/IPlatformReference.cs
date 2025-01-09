namespace PowerPlaywright
{
    /// <summary>
    /// Provides access to string values that are platform versioned.
    /// </summary>
    internal interface IPlatformReference
    {
        /// <summary>
        /// Gets the control name of the grid on the entity list page.
        /// </summary>
        string EntityListPageGridControlName { get; }
    }
}