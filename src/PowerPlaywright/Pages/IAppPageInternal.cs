namespace PowerPlaywright.Pages
{
    using System;

    /// <summary>
    /// An internal interface that represents a page.
    /// </summary>
    internal interface IAppPageInternal
    {
        /// <summary>
        /// Gets the event raised when the page is destroyed (i.e. navigated away from).
        /// </summary>
        event EventHandler OnDestroy;

        /// <summary>
        /// Destroys the page.
        /// </summary>
        void Destroy();
    }
}