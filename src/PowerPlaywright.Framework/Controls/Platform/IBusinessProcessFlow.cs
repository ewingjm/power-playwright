namespace PowerPlaywright.Framework.Controls.Platform
{
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;

    /// <summary>
    /// Business process flow (BPF) control class.
    /// </summary>
    public interface IBusinessProcessFlow : IPlatformControl
    {
        /// <summary>
        /// Gets the value of the bp control.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<string> GetActiveStageAsync();

        /// <summary>
        /// Gets the value of the bp control.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<bool> IsCompleteAsync();

        /// <summary>
        /// Moves the bpf to the next stage the value of the bp control.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<bool> MoveToNextStageAsync();

        /// <summary>
        /// Moves the bpf to the previous stage the value of the bp control.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<bool> MoveToPreviousStageAsync();

        /// <summary>
        /// Gets a field on the form with a known child control type.
        /// </summary>
        /// <typeparam name="TControl">The child control type.</typeparam>
        /// <param name="name">The field name.</param>
        /// <returns>The field.</returns>
        IField<TControl> GetField<TControl>(string name)
            where TControl : IPcfControl;
    }
}