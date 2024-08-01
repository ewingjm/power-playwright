﻿namespace PowerPlaywright.Pages
{
    using System.Threading.Tasks;

    /// <summary>
    /// A login page.
    /// </summary>
    public interface ILoginPage : IModelDrivenAppPage
    {
        /// <summary>
        /// Logs in on the login page.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task LoginAsync(string username, string password);
    }
}