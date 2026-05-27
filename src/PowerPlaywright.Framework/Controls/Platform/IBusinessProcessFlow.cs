namespace PowerPlaywright.Framework.Controls.Platform
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;

    /// <summary>
    /// A business process flow.
    /// </summary>
    [PlatformControl]
    public interface IBusinessProcessFlow : IControl
    {
        /// <summary>
        /// Proceeds to the next stage in the process.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task<bool> NextAsync();

        /// <summary>
        /// Moves to a previous stage in the process.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task<bool> PreviousAsync();

        /// <summary>
        /// Determies if the process is complete.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task<bool> IsProcessCompleteAsync();

        /// <summary>
        /// Gets the current stage in the business process flow.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task<string> GetCurrentStageAsync();

        /// <summary>
        /// Clicks the finish button if you are on the last stage of the business process flow.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task<bool> CompleteAsync();

        /// <summary>
        /// Gets the stages visible on the business process flow.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task<IEnumerable<string>> GetStagesAsync();

        /// <summary>
        /// Determines if we can Move forward in the process or we have reached the end of the process.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task<bool> IsFinalStageAsync();

        /// <summary>
        /// Executes the action associated with the current stage in the business process flow.
        /// </summary>
        /// <param name="action">The action to execute for the current stage.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task ExecuteStageActionAsync(Func<IEnumerable<IField>, Task> action);
    }
}