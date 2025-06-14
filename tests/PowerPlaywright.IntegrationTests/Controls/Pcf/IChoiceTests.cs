﻿namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using System.Threading.Tasks;
    using Bogus;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.IntegrationTests.Extensions;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests the <see cref="IChoice"/> PCF control class.
    /// </summary>
    public class IChoiceTests : IntegrationTests
    {
        private Faker faker;

        /// <summary>
        /// Sets up the test dependencies.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.faker = new Faker("en_GB");
        }

        /// <summary>
        /// Tests that <see cref="IChoice.GetValueAsync"/> returns null when the value has not been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_DoesNotContainValue_ReturnsNull()
        {
            var choiceControl = await this.SetupChoiceScenarioAsync(withNoValue: true);

            Assert.That(choiceControl.GetValueAsync, Is.Null);
        }

        /// <summary>
        /// Tests that <see cref="IChoice.GetValueAsync"/> returns the value when the value has been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_ContainsValue_ReturnsValue()
        {
            var expectedValue = this.faker.PickRandom<pp_record_pp_choice>();
            var choiceControl = await this.SetupChoiceScenarioAsync(expectedValue);

            Assert.That(choiceControl.GetValueAsync, Is.EqualTo(expectedValue.ToDisplayName()));
        }

        /// <summary>
        /// Tests that <see cref="IChoice.SetValueAsync(string)"/> sets the value when the control does not contain a value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_DoesNotContainValue_SetsValue()
        {
            var choiceControl = await this.SetupChoiceScenarioAsync(withNoValue: true);
            var expectedValue = this.faker.PickRandom<pp_record_pp_choice>().ToDisplayName();

            await choiceControl.SetValueAsync(expectedValue);

            Assert.That(choiceControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Tests that <see cref="IChoice.SetValueAsync(string)"/> replaces the value when the control already contains a value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_ContainsValue_ReplacesValue()
        {
            var choiceControl = await this.SetupChoiceScenarioAsync();
            var expectedValue = this.faker.PickRandom<pp_record_pp_choice>().ToDisplayName();

            await choiceControl.SetValueAsync(expectedValue);

            Assert.That(choiceControl.GetValueAsync, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Sets up a choice control scenario for testing by creating a record with a specified or generated choices.
        /// </summary>
        /// <param name="withValue">An optional choice value to set in the record. If null, a random value will be generated.</param>
        /// <param name="withNoValue">Whether to set the choice to null. Defaults to false.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="IChoice"/>.</returns>
        private async Task<IChoice> SetupChoiceScenarioAsync(pp_record_pp_choice? withValue = null, bool withNoValue = false)
        {
            var record = new RecordFaker();

            if (withNoValue)
            {
                record.RuleFor(x => x.pp_choice, f => null);
            }
            else if (withValue is not null)
            {
                record.RuleFor(x => x.pp_choice, withValue);
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(record.Generate());

            return recordPage.Form.GetField<IChoice>(nameof(pp_Record.pp_choice)).Control;
        }
    }
}