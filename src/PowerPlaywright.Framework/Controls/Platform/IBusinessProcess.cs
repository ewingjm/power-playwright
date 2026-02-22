namespace PowerPlaywright.Framework.Controls.Platform
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// A business process flow.
    /// </summary>
    [PlatformControl]
    public interface IBusinessProcess : IControl
    {
        /// <summary>
        /// Proceeds to the next stage in the process.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task<string> MoveToNextStageAsync();

        /// <summary>
        /// Moves to a previous stage in the process.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task<string> MoveToPreviousStageAsync();

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
        Task<bool> HasAnotherStageAsync();
    }
}