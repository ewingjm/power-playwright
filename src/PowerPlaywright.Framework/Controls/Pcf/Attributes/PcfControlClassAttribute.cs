namespace PowerPlaywright.Framework.Controls.Pcf.Attributes
{
    using System;
    using PowerPlaywright.Framework.Controls.Pcf.Enums;

    /// <summary>
    /// Denotes an interface as describing the interactions available for a specific PCF control class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public sealed class PcfControlClassAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PcfControlClassAttribute"/> class.
        /// </summary>
        /// <param name="classId">The ID of the control class.</param>
        /// <param name="dataType">The data type of the control class.</param>
        /// <param name="description">The description of the control class.</param>
        public PcfControlClassAttribute(string classId, DataType dataType, string description = null)
        {
            if (string.IsNullOrEmpty(classId))
            {
                throw new ArgumentException($"'{nameof(classId)}' cannot be null or empty.", nameof(classId));
            }

            if (string.IsNullOrEmpty(description))
            {
                throw new ArgumentException($"'{nameof(description)}' cannot be null or empty.", nameof(description));
            }

            this.ClassId = classId;
            this.DataType = dataType;
            this.Description = description;
        }

        /// <summary>
        /// Gets the class ID.
        /// </summary>
        public string ClassId { get; }

        /// <summary>
        /// Gets the data type.
        /// </summary>
        public DataType DataType { get; }

        /// <summary>
        /// Gets the PCF control class description.
        /// </summary>
        public string Description { get; }
    }
}
