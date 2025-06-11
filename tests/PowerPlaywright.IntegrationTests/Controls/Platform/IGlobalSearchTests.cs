namespace PowerPlaywright.IntegrationTests.Controls.Platform
{
    using System.Text.RegularExpressions;
    using Bogus;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Strategies.Controls.Platform;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;
    using PowerPlaywright.TestApp.Model.Search;

    /// <summary>
    /// Tests for the <see cref="GlobalSearch"/> control.
    /// </summary>
    public partial class IGlobalSearchTests : IntegrationTests
    {
        private Faker<pp_Record> faker;

        /// <summary>
        /// Sets up the search control grid.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.faker = new RecordFaker();
        }

        /// <summary>
        /// Tests that <see cref="IGlobalSearch.SearchAsync(string)"/> performs a search correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SearchAsync_HasResults_Returns_True()
        {
            var searchRecord = await this.SetupSearchScenarioAsync();
            var searchControl = (await this.LoginAsync()).Search;

            await searchControl.SearchAsync(searchRecord.pp_singlelineoftexttext);

            Assert.That(searchControl.HasResultsAsync, Is.True);
        }

        /// <summary>
        /// Tests that <see cref="IGlobalSearch.SearchAsync(string)"/> performs a search correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SearchAsync_HasNoResults_Returns_False()
        {
            var searchControl = (await this.LoginAsync()).Search;

            await searchControl.SearchAsync("sometotallyandutterlyfakestring");

            Assert.That(searchControl.HasResultsAsync, Is.False);
        }

        /// <summary>
        /// Searches and Opens the record at the given index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task OpenSearchResult_Opens_EntityRecordPage()
        {
            var searchRecord = await this.SetupSearchScenarioAsync();
            var searchControl = (await this.LoginAsync()).Search;

            var page = await searchControl.OpenSearchResultAsync<IEntityRecordPage>(searchRecord.pp_singlelineoftexttext, 0);

            await this.Expect(page.Page).ToHaveURLAsync(new Regex($"etn={pp_Record.EntityLogicalName}"));
        }

        /// <summary>
        /// Sets up a record for testing by creating a record with a specified or generated values.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="ISingleLineUrl"/>.</returns>
        private async Task<pp_Record> SetupSearchScenarioAsync()
        {
            var testRecord = await this.CreateRecordAsync(this.faker.Generate());

            await this.WaitForSearchResultAsync(testRecord.pp_singlelineoftexttext);

            return testRecord;
        }

        /// <summary>
        /// Waits for a search result to become available.
        /// </summary>
        /// <param name="query">The wildcared search query.</param>
        /// <param name="maxRetries">The number of retries.</param>
        /// <param name="initialDelayMs">An initial delay to not over exhaust the api.</param>
        /// <returns>SearchQueryApiResponse.</returns>
        /// <exception cref="TimeoutException">Gives a timeout exception on maximum threshold.</exception>
        private async Task<SearchQueryApiResponse> WaitForSearchResultAsync(string query, int maxRetries = 100, int initialDelayMs = 2000)
        {
            int attempt = 0;

            while (attempt < maxRetries)
            {
                var searchResults = await this.SearchAsync(query);

                if (searchResults?.ParsedResponse?.Count >= 1)
                {
                    return searchResults;
                }

                await Task.Delay(initialDelayMs);

                attempt++;
            }

            throw new TimeoutException("Search results were not returned after multiple retries.");
        }
    }
}