namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;
    using System.Threading.Tasks;

    /// <summary>
    /// Tests the <see cref="IUpdMSPicklist"/> PCF control class.
    /// </summary>
    public class IUpdMSPicklistTests : IntegrationTests
    {
        /// <summary>
        /// Tests that <see cref="IUpdMSPicklist.SetValueAsync(List{string})"/> sets the value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_ReturnsValue()
        {
            var multiSelectPicklist = await this.SetupMultiChoicesScenarioAsync();

            var expectedValues = new Dictionary<int, string>
            {
                { (int)pp_record_pp_choices.ChoiceA, "Choice A" },
                { (int)pp_record_pp_choices.ChoiceC, "Choice C" },
            };

            // Act
            await multiSelectPicklist.SetValueAsync(expectedValues.Select(x => x.Value).ToList());
            var actualSelectedValues = await multiSelectPicklist.GetValueAsync();

            // Assert
            Assert.That(actualSelectedValues, Is.EquivalentTo(expectedValues));
        }

        /// <summary>
        /// Tests that <see cref="IUpdMSPicklist.SelectAllAsync()"/> sets the value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SelectAllAsync_ReturnsValue()
        {
            var multiSelectPicklist = await this.SetupMultiChoicesScenarioAsync();

            var expectedValues = new Dictionary<int, string>
            {
                { (int)pp_record_pp_choices.ChoiceA, "Choice A" },
                { (int)pp_record_pp_choices.ChoiceB, "Choice B" },
                { (int)pp_record_pp_choices.ChoiceC, "Choice C" },
            };

            // Act
            await multiSelectPicklist.SelectAllAsync();
            var actualSelectedValues = await multiSelectPicklist.GetValueAsync();

            // Assert
            Assert.That(actualSelectedValues, Is.EquivalentTo(expectedValues));
        }

        /// <summary>
        /// Tests that <see cref="IUpdMSPicklist.GetValueAsync"/> returns null when the value has not been set.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_DoesNotContainValue_ReturnsNull()
        {
            var multiSelectPicklist = await this.SetupMultiChoicesScenarioAsync();

            Assert.That(multiSelectPicklist.GetValueAsync, Is.Null);
        }

        /// <summary>
        /// Sets up a Multi Select Picklist control scenario for testing by creating a record with a specified or generated URL.
        /// </summary>
        /// <param name="withChoices">An optional URL string to set in the record. If null, Random choices be generated.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="IUpdMSPicklist"/>.</returns>
        private async Task<IUpdMSPicklist> SetupMultiChoicesScenarioAsync(IEnumerable<pp_record_pp_choices>? withChoices = null)
        {
            var record = new RecordFaker();

            if (withChoices != null)
            {
                record.RuleFor(x => x.pp_choices, withChoices);
            }
            else
            {
                record.Ignore(p => p.pp_choices);
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(record.Generate());

            return recordPage.Form.GetField<IUpdMSPicklist>(nameof(pp_Record.pp_choices)).Control;
        }
    }
}