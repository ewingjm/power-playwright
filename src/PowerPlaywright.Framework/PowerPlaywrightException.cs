namespace PowerPlaywright.Framework
{
    using System;

    /// <summary>
    /// An exception thrown by Power Playwright.
    /// </summary>
    [Serializable]
    public class PowerPlaywrightException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PowerPlaywrightException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public PowerPlaywrightException(string message)
            : base(message)
        {
        }
    }
}