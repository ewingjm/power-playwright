namespace PowerPlaywright
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// An exception thrown by Power Playwright.
    /// </summary>
    [Serializable]
    public class PowerPlaywrightException : Exception
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="PowerPlaywrightException"/> class.
        /// </summary>
        public PowerPlaywrightException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PowerPlaywrightException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public PowerPlaywrightException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PowerPlaywrightException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public PowerPlaywrightException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PowerPlaywrightException"/> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected PowerPlaywrightException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}