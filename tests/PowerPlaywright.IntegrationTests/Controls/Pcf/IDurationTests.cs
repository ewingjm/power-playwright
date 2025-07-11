﻿namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using Bogus;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.IntegrationTests.Extensions;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests for the <see cref="IDuration"/> control class.
    /// </summary>
    public class IDurationTests : IntegrationTests
    {
        private Faker faker;

        /// <summary>
        /// Sets up the test dependencies.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.faker = new Faker();
        }

        /// <summary>
        /// Tests that <see cref="IDuration.GetValueAsync"/> returns null when the value has not been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_DoesNotContainValue_ReturnsNull()
        {
            var durationControl = await this.SetupDurationScenarioAsync(withNoValue: true);

            Assert.That(durationControl.GetValueAsync, Is.Null);
        }

        /// <summary>
        /// Tests that <see cref="IDuration.GetValueAsync"/> returns the value when the value has been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_ContainsValue_ReturnsValue()
        {
            var expectedValue = this.faker.Random.Int(min: 1);

            var durationControl = await this.SetupDurationScenarioAsync(withValue: expectedValue);

            Assert.That(durationControl.GetValueAsync, Is.EqualTo($"{expectedValue.ToDurationString()}"));
        }

        /// <summary>
        /// Tests that <see cref="IDuration.SetValueAsync(string)"/> sets the value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_DoesNotContainValue_SetsValue()
        {
            var durationControl = await this.SetupDurationScenarioAsync(withNoValue: true);
            var expectedValue = this.GetRandomDurationString();

            await durationControl.SetValueAsync(expectedValue);

            Assert.That(durationControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Tests that <see cref="IDuration.SetValueAsync(string)"/> replaces the value when the control already contains a value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_ContainsValue_ReplacesValue()
        {
            var durationControl = await this.SetupDurationScenarioAsync();
            var expectedValue = this.GetRandomDurationString();

            await durationControl.SetValueAsync(expectedValue);

            Assert.That(durationControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        private string GetRandomDurationString()
        {
            return this.faker.Random.Int(min: 1).ToDurationString();
        }

        /// <summary>
        /// Sets up a control scenario for testing by creating a record with a specified or generated value.
        /// </summary>
        /// <param name="withValue">An optional value to set in the record. If null, a random value will be generated.</param>
        /// <param name="withNoValue">Whether to set the choice to null. Defaults to false.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="IDuration"/>.</returns>
        private async Task<IDuration> SetupDurationScenarioAsync(int? withValue = null, bool withNoValue = false)
        {
            var record = new RecordFaker();

            if (withNoValue)
            {
                record.RuleFor(x => x.pp_wholenumberduration, f => null!);
            }
            else if (withValue is not null)
            {
                record.RuleFor(x => x.pp_wholenumberduration, withValue);
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(record.Generate());
            return recordPage.Form.GetField<IDuration>(nameof(pp_Record.pp_wholenumberduration)).Control;
        }
    }
}