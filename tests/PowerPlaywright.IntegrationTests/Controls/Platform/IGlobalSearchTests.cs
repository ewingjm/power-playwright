namespace PowerPlaywright.IntegrationTests.Controls.Platform
{
    using System.Text.RegularExpressions;
    using Bogus;
    using Microsoft.Xrm.Sdk.Query;
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
        /// Tests that <see cref="IGlobalSearch.SuggestAsync(string, bool)"/> returns suggestions when it exists.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SuggestAsync_HasSuggestedResults_Returns_True()
        {
            var sampleRecord = await this.GetRandomIndexedRecord();
            var searchControl = await this.SetupSearchScenarioAsync(sampleRecord);

            await searchControl.SuggestAsync(sampleRecord.pp_singlelineoftexttext);

            Assert.That(searchControl.HasSuggestedResultsAsync, Is.True);
        }

        /// <summary>
        /// Tests that <see cref="IGlobalSearch.SuggestAsync(string, bool)"/> does not return suggestions when it does not exist.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SuggestAsync_HasSuggestedResults_Returns_False()
        {
            var searchControl = await this.SetupSearchScenarioAsync();
            await searchControl.SuggestAsync(this.faker.Random.String(100));

            Assert.That(searchControl.HasSuggestedResultsAsync, Is.False);
        }

        /// <summary>
        /// Searches and Opens the record at the given index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task OpenSuggestion_Opens_EntityRecordPage()
        {
            var sampleRecord = await this.GetRandomIndexedRecord();
            var searchControl = await this.SetupSearchScenarioAsync(sampleRecord);

            var page = await searchControl.OpenSuggestionAsync<IEntityRecordPage>(sampleRecord.pp_singlelineoftexttext, 0);

            await this.Expect(page.Page).ToHaveURLAsync(RecordPageRegex());
        }

        /// <summary>
        /// Searches and Opens the record at the given index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SearchAsync_Opens_SearchPage()
        {
            var sampleRecord = await this.GetRandomIndexedRecord();
            var searchControl = await this.SetupSearchScenarioAsync(sampleRecord);

            var page = await searchControl.SearchAsync<ISearchPage>(sampleRecord.pp_singlelineoftexttext);

            await this.Expect(page.Page).ToHaveURLAsync(SearchPageRegex());
        }

        [GeneratedRegex(".*pagetype=entityrecord&etn=pp_record.*")]
        private static partial Regex RecordPageRegex();

        [GeneratedRegex(".*pagetype=search.*")]
        private static partial Regex SearchPageRegex();

        /// <summary>
        /// Sets up a record for testing by creating a record with a specified or generated values.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="ISingleLineUrl"/>.</returns>
        private async Task<IGlobalSearch> SetupSearchScenarioAsync(pp_Record? withRecord = null)
        {
            if (withRecord != null)
            {
                await this.WaitForSearchResultAsync(withRecord.pp_singlelineoftexttext);
            }

            return (await this.LoginAsync()).Search;
        }

        /// <summary>
        /// Waits for a search result to become available.
        /// </summary>
        /// <param name="query">The wildcared search query.</param>
        /// <param name="maxRetries">The number of retries.</param>
        /// <returns>SearchQueryApiResponse.</returns>
        /// <exception cref="TimeoutException">Gives a timeout exception on maximum threshold.</exception>
        private async Task<SearchQueryApiResponse> WaitForSearchResultAsync(string query, int maxRetries = 200)
        {
            int attempt = 0;

            while (attempt < maxRetries)
            {
                var searchResults = await this.SearchAsync(query, [pp_Record.EntityLogicalName], 500);

                if (searchResults?.ParsedResponse?.Count >= 1)
                {
                    return searchResults;
                }

                attempt++;
            }

            throw new TimeoutException("Search results were not returned after multiple retries.");
        }

        /// <summary>
        /// Creates a record for indexing if no records or uses a previously indexed random one to speed the tests up.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        private async Task<pp_Record> GetRandomIndexedRecord()
        {
            var query = new QueryExpression(pp_Record.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true),
                TopCount = 100,
                Criteria = new FilterExpression
                {
                    FilterOperator = LogicalOperator.And,
                    Conditions =
                    {
                        new ConditionExpression("statecode", ConditionOperator.Equal, 0),
                        new ConditionExpression(nameof(pp_Record.pp_singlelineoftexttext), ConditionOperator.NotNull),
                        new ConditionExpression(nameof(pp_Record.pp_singlelineoftexttext), ConditionOperator.NotEqual, string.Empty),
                        new ConditionExpression("createdon", ConditionOperator.LessThan, DateTime.UtcNow.AddMinutes(-10)),
                    },
                },
                Orders =
                    {
                        new OrderExpression("createdon", OrderType.Descending),
                    },
            };

            var records = await this.RetrieveRecordsAsync(query);

            if (!records.Entities.Any())
            {
                return await this.CreateAsync(this.recordFaker.Generate());
            }

            var random = new Random();
            int index = random.Next(records.Entities.Count);
            return records.Entities[index].ToEntity<pp_Record>();
        }
    }
}