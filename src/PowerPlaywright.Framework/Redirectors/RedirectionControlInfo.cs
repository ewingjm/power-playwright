namespace PowerPlaywright.Framework.Redirectors
{
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// The information about a specific redirection request.
    /// </summary>
    public class RedirectionControlInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectionControlInfo"/> class.
        /// </summary>
        /// <param name="appPage">The app page.</param>
        /// <param name="name">The control name.</param>
        /// <param name="parent">The parent.</param>
        public RedirectionControlInfo(IAppPage appPage, string name = null, IControl parent = null)
        {
            this.AppPage = appPage;
            this.Name = name;
            this.Parent = parent;
        }

        /// <summary>
        /// Gets the app page of the control being redirected.
        /// </summary>
        public IAppPage AppPage { get; private set; }

        /// <summary>
        /// Gets the name of the control being redirected.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the parent of the control being redirected.
        /// </summary>
        public IControl Parent { get; private set; }
    }
}
