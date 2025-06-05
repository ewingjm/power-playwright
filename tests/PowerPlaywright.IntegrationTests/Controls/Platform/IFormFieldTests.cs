namespace PowerPlaywright.IntegrationTests.Controls.Platform
{
    using System.Threading.Tasks;
    using Bogus;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests the <see cref="IFormField{TPcfControl}"/> and <see cref="IFormField"/> platform controls.
    /// </summary>
    public class IFormFieldTests : IntegrationTests
    {
        /// <summary>
        /// Tests that the <see cref="IFormField{TPcfControl}.Control"/> property returns an instance of the PCF control type.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task PcfControl_Always_ReturnsPcfControlInstance()
        {
            var form = await this.SetupFormScenarioAsync();
            var control = form.GetField<ISingleLineUrl>(nameof(pp_Record.pp_singlelineoftexturl));

            Assert.That(control.Control, Is.Not.Null);
        }

        /// <summary>
        /// Tests that <see cref="IFormField.IsMandatoryAsync"/> returns true if the control is mandatory.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task IsMandatoryAsync_MandatoryField_ReturnsTrue()
        {
            var form = await this.SetupFormScenarioAsync();
            var control = form.GetField(nameof(pp_Record.pp_singlelineoftexttext));

            Assert.That(control.IsMandatoryAsync, Is.True);
        }

        /// <summary>
        /// Tests that <see cref="IFormField.IsMandatoryAsync"/> returns false if the control is not mandatory.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task IsMandatoryAsync_NonMandatoryField_ReturnsFalse()
        {
            var form = await this.SetupFormScenarioAsync();
            var control = form.GetField(nameof(pp_Record.pp_currency));

            Assert.That(control.IsMandatoryAsync, Is.False);
        }

        /// <summary>
        /// Tests that <see cref="IFormField.IsDisabledAsync"/> returns true if the control is disabled.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task IsDisabledAsync_DisabledField_ReturnsTrue()
        {
            var form = await this.SetupFormScenarioAsync();
            var control = form.GetField(nameof(pp_Record.pp_singlelineoftexttext));

            Assert.That(control.IsDisabledAsync, Is.True);
        }

        /// <summary>
        /// Tests that <see cref="IFormField.IsDisabledAsync"/> returns false if the control is not mandatory.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task IsDisabledAsync_NonDisabledField_ReturnsFalse()
        {
            var form = await this.SetupFormScenarioAsync();
            var control = form.GetField(nameof(pp_Record.pp_currency));

            Assert.That(control.IsMandatoryAsync, Is.False);
        }

        /// <summary>
        /// Tests that <see cref="IFormField.IsVisibleAsync"/> returns true if the control is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task IsVisibleAsync_ControlIsVisible_ReturnsTrue()
        {
            var form = await this.SetupFormScenarioAsync();
            var control = form.GetField(nameof(pp_Record.pp_singlelineoftexturl));

            Assert.That(control.IsVisibleAsync, Is.True);
        }

        /// <summary>
        /// Tests that <see cref="IFormField.IsVisibleAsync"/> returns false if the control is hidden.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task IsVisibleAsync_ControlIsHidden_ReturnsFalse()
        {
            var form = await this.SetupFormScenarioAsync();
            var control = form.GetField(nameof(pp_Record.pp_singlelineoftexturl));

            Assert.That(control.IsVisibleAsync, Is.False);
        }

        /// <summary>
        /// Tests that <see cref="IFormField.GetLabelAsync"/> returns the label text if the label is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetLabelAsync_LabelIsVisible_ReturnsLabelText()
        {
            var form = await this.SetupFormScenarioAsync();
            var control = form.GetField(nameof(pp_Record.pp_singlelineoftexturl));

            Assert.That(control.GetLabelAsync, Is.EqualTo("Single line of text (URL)"));
        }

        /// <summary>
        /// Tests that <see cref="IFormField.GetLabelAsync"/> returns an empty string if the label is hidden.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetLabelAsync_LabelIsHidden_ReturnsEmptyString()
        {
            var form = await this.SetupFormScenarioAsync();
            var control = form.GetField(pp_Record.Forms.Information.RelatedRecordsSubgrid);

            Assert.That(control.GetLabelAsync, Is.Empty);
        }

        /// <summary>
        /// Sets up a form scenario.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="IUrlControl"/>.</returns>
        private async Task<IMainForm> SetupFormScenarioAsync()
        {
            var record = new RecordFaker();

            var recordPage = await this.LoginAndNavigateToRecordAsync(record.Generate());

            return recordPage.Form;
        }
    }
}