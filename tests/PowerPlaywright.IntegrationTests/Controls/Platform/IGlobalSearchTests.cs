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
        private Faker<pp_Record> recordFaker;
        private Faker faker;

        /// <summary>
        /// Sets up the search control grid.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.recordFaker = new RecordFaker();
            this.faker = new Faker();
        }

        /// <summary>
        /// Tests that <see cref="IGlobalSearch.SearchAsync(string)"/> performs a search correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SearchAsync_HasResults_Returns_True()
        {
            var sampleRecord = this.recordFaker.Generate();
            var searchControl = await this.SetupSearchScenarioAsync(sampleRecord);
            await searchControl.SearchAsync(sampleRecord.pp_singlelineoftexttext);

            Assert.That(searchControl.HasResultsAsync, Is.True);
        }

        /// <summary>
        /// Tests that <see cref="IGlobalSearch.SearchAsync(string)"/> performs a search correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SearchAsync_HasNoResults_Returns_False()
        {
            var searchControl = await this.SetupSearchScenarioAsync();
            await searchControl.SearchAsync(this.faker.Random.String(50));

            Assert.That(searchControl.HasResultsAsync, Is.False);
        }

        /// <summary>
        /// Searches and Opens the record at the given index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task OpenSearchResult_Opens_EntityRecordPage()
        {
            var sampleRecord = this.recordFaker.Generate();
            var searchControl = await this.SetupSearchScenarioAsync(sampleRecord);

            var page = await searchControl.OpenSearchResultAsync<IEntityRecordPage>(sampleRecord.pp_singlelineoftexttext, 0);

            await this.Expect(page.Page).ToHaveURLAsync(RecordPageRegex());
        }

        [GeneratedRegex(".*pagetype=entitylist&etn=pp_record.*")]
        private static partial Regex RecordPageRegex();

        /// <summary>
        /// Sets up a record for testing by creating a record with a specified or generated values.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="ISingleLineUrl"/>.</returns>
        private async Task<IGlobalSearch> SetupSearchScenarioAsync(pp_Record? withRecord = null)
        {
            if (withRecord is not null)
            {
                await this.CreateRecordAsync(withRecord);
                await this.WaitForSearchResultAsync(withRecord.pp_singlelineoftexttext);
            }

            return (await this.LoginAsync()).Search;
        }

        /// <summary>
        /// Waits for a search result to become available.
        /// </summary>
        /// <param name="query">The wildcared search query.</param>
        /// <param name="maxRetries">The number of retries.</param>
        /// <param name="initialDelayMs">An initial delay to not over exhaust the api.</param>
        /// <returns>SearchQueryApiResponse.</returns>
        /// <exception cref="TimeoutException">Gives a timeout exception on maximum threshold.</exception>
        private async Task<SearchQueryApiResponse> WaitForSearchResultAsync(string query, int maxRetries = 200, int initialDelayMs = 2000)
        {
            int attempt = 0;

            while (attempt < maxRetries)
            {
                var searchResults = await this.PerformRelevanceSearchAsync(query, 3);

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