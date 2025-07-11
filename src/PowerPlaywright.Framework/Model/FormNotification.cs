namespace PowerPlaywright.Framework.Model
{
    /// <summary>
    /// A form notification.
    /// </summary>
    public class FormNotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormNotification"/> class.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="message">The message.</param>
        public FormNotification(FormNotificationLevel level, string message)
        {
            this.Level = level;
            this.Message = message;
        }

        /// <summary>
        /// Gets the notification level.
        /// </summary>
        public FormNotificationLevel Level { get; }

        /// <summary>
        /// Gets the notification message.
        /// </summary>
        public string Message { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.Level}: {this.Message}";
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is FormNotification otherNotification)
            {
                return this.Level == otherNotification.Level && this.Message == otherNotification.Message;
            }

            return base.Equals(obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }
}
