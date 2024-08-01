namespace PowerPlaywright.Events
{
    using PowerPlaywright.Pages;

    /// <summary>
    /// A notification that is published when the app is first initialised.
    /// </summary>
    internal class AppInitializedEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppInitializedEvent"/> class.
        /// </summary>
        /// <param name="homePage">The home page.</param>
        public AppInitializedEvent(IModelDrivenAppPage homePage)
        {
            this.HomePage = homePage;
        }

        /// <summary>
        /// Gets the home page.
        /// </summary>
        public IModelDrivenAppPage HomePage { get; }
    }
}
