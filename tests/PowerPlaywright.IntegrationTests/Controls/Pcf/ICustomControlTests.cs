﻿namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;
    using PowerPlaywright.TestApp.PageObjects;

    /// <summary>
    /// Tests for controls add through a custom assembly.
    /// </summary>
    public class ICustomControlTests : IntegrationTests
    {
        /// <summary>
        /// Tests that <see cref="ICustomControlClass.SetValueAsync(bool)"/> checks the toggle when the toggle is unchecked.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetValueAsync_Always_ReturnsValue()
        {
            var control = await this.SetupCustomControlScenarioAsync(withToggle: false);

            Assert.That(control.GetValueAsync, Is.False);
        }

        /// <summary>
        /// Tests that <see cref="ICustomControlClass.SetValueAsync(bool)"/> checks the toggle when the toggle is unchecked.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SetValueAsync_True_ChecksToggle()
        {
            var control = await this.SetupCustomControlScenarioAsync(withToggle: false);

            await control.SetValueAsync(true);

            Assert.That(control.GetValueAsync, Is.True);
        }

        private async Task<ICustomControlClass> SetupCustomControlScenarioAsync(bool withToggle = false)
        {
            var faker = new RecordFaker();
            faker.RuleFor(f => f.pp_yesno, f => withToggle);

            var recordPage = await this.LoginAndNavigateToRecordAsync(faker.Generate());

            return recordPage.Form.GetField<ICustomControlClass>(pp_Record.Forms.Information.Toggle).Control;
        }
    }
}