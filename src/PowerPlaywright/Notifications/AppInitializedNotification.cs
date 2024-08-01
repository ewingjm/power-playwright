namespace PowerPlaywright.Notifications
{
    using MediatR;
    using PowerPlaywright.Pages;

    /// <summary>
    /// A notification that is published when the app is first initialised.
    /// </summary>
    public class AppInitializedNotification : INotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppInitializedNotification"/> class.
        /// </summary>
        /// <param name="homePage">The home page.</param>
        public AppInitializedNotification(IModelDrivenAppPage homePage)
        {
            this.HomePage = homePage;
        }

        /// <summary>
        /// Gets the home page.
        /// </summary>
        public IModelDrivenAppPage HomePage { get; }
    }
}
