namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Model;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests the <see cref="IField"/> control.
    /// </summary>
    public class IFieldTests : IntegrationTests
    {
        /// <summary>
        /// Tests that <see cref="IField.GetRequirementLevelAsync"/> returns <see cref="FieldRequirementLevel.Required"/> if the control is required.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetRequirementLevelAsync_RequiredField_ReturnsRequired()
        {
            var form = await this.SetupFormScenarioAsync(withRequiredField: pp_Record.Forms.Information.WholeNumberNone);
            var control = form.GetField(pp_Record.Forms.Information.WholeNumberNone);

            Assert.That(control.GetRequirementLevelAsync, Is.EqualTo(FieldRequirementLevel.Required));
        }

        /// <summary>
        /// Tests that <see cref="IField.GetRequirementLevelAsync"/> returns <see cref="FieldRequirementLevel.None"/> if the control is optional.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetRequirementLevelAsync_OptionalField_ReturnsNone()
        {
            var form = await this.SetupFormScenarioAsync(withOptionalField: pp_Record.Forms.Information.WholeNumberNone);
            var control = form.GetField(pp_Record.Forms.Information.WholeNumberNone);

            Assert.That(control.GetRequirementLevelAsync, Is.EqualTo(FieldRequirementLevel.None));
        }

        /// <summary>
        /// Tests that <see cref="IField.GetRequirementLevelAsync"/> returns <see cref="FieldRequirementLevel.Recommended"/> if the control is recommended.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetRequirementLevelAsync_RecommendedField_ReturnsRecommended()
        {
            var form = await this.SetupFormScenarioAsync(withRecommendedField: pp_Record.Forms.Information.WholeNumberNone);
            var control = form.GetField(pp_Record.Forms.Information.WholeNumberNone);

            Assert.That(control.GetRequirementLevelAsync, Is.EqualTo(FieldRequirementLevel.Recommended));
        }

        /// <summary>
        /// Tests that <see cref="IField.IsDisabledAsync"/> returns true if the control is disabled.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task IsDisabledAsync_DisabledField_ReturnsTrue()
        {
            var form = await this.SetupFormScenarioAsync(withDisabledField: pp_Record.Forms.Information.WholeNumberNone);
            var control = form.GetField(pp_Record.Forms.Information.WholeNumberNone);

            Assert.That(control.IsDisabledAsync, Is.True);
        }

        /// <summary>
        /// Tests that <see cref="IField.IsDisabledAsync"/> returns true if the form is disabled.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task IsDisabledAsync_DisabledForm_ReturnsTrue()
        {
            var form = await this.SetupFormScenarioAsync(withEditableField: pp_Record.Forms.Information.WholeNumberNone, withDisabledRecord: true);
            var control = form.GetField(pp_Record.Forms.Information.WholeNumberNone);

            Assert.That(control.IsDisabledAsync, Is.True);
        }

        /// <summary>
        /// Tests that <see cref="IField.IsDisabledAsync"/> returns false if the control is not mandatory.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task IsDisabledAsync_NonDisabledField_ReturnsFalse()
        {
            var form = await this.SetupFormScenarioAsync(withEditableField: pp_Record.Forms.Information.WholeNumberNone);
            var control = form.GetField(pp_Record.Forms.Information.WholeNumberNone);

            Assert.That(control.IsDisabledAsync, Is.False);
        }

        /// <summary>
        /// Tests that <see cref="IField.IsVisibleAsync"/> returns true if the control is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task IsVisibleAsync_ControlIsVisible_ReturnsTrue()
        {
            var form = await this.SetupFormScenarioAsync(withVisibleField: pp_Record.Forms.Information.WholeNumberNone);
            var control = form.GetField(pp_Record.Forms.Information.WholeNumberNone);

            Assert.That(control.IsVisibleAsync, Is.True);
        }

        /// <summary>
        /// Tests that <see cref="IField.IsVisibleAsync"/> returns false if the control is hidden.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task IsVisibleAsync_ControlIsHidden_ReturnsFalse()
        {
            var form = await this.SetupFormScenarioAsync(withHiddenField: pp_Record.Forms.Information.WholeNumberNone);
            var control = form.GetField(pp_Record.Forms.Information.WholeNumberNone);

            Assert.That(control.IsVisibleAsync, Is.False);
        }

        /// <summary>
        /// Tests that <see cref="IField.GetLabelAsync"/> returns the label text if the label is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetLabelAsync_LabelIsVisible_ReturnsLabelText()
        {
            var form = await this.SetupFormScenarioAsync(withVisibleField: pp_Record.Forms.Information.WholeNumberNone);
            var control = form.GetField(pp_Record.Forms.Information.WholeNumberNone);

            Assert.That(control.GetLabelAsync, Is.EqualTo("Whole number (none)"));
        }

        /// <summary>
        /// Tests that <see cref="IField.GetLabelAsync"/> returns an empty string if the label is hidden.
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
        /// <param name="withEditableField">An optional field to setup as editable on the form.</param>
        /// <param name="withDisabledField">An optional field to set up as disabled on the form.</param>
        /// <param name="withOptionalField">An optional field to set up as non-mandatory on the form.</param>
        /// <param name="withRequiredField">An optional field to set up as mandatory on the form.</param>
        /// <param name="withRecommendedField">An optional field to set up as recommended on the form.</param>
        /// <param name="withHiddenField">An optional field to set up as hidden on the form.</param>
        /// <param name="withVisibleField">An optional field to set up as visible on the form.</param>
        /// <param name="withDisabledRecord">Whether or not to set up the record as disabled (e.g. inactive).</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="IUrlControl"/>.</returns>
        private async Task<IMainForm> SetupFormScenarioAsync(string? withEditableField = null, string? withDisabledField = null, string? withOptionalField = null, string? withRequiredField = null, string? withRecommendedField = null, string? withHiddenField = null, string? withVisibleField = null, bool withDisabledRecord = false)
        {
            var recordfaker = new RecordFaker();

            if (withDisabledRecord)
            {
                recordfaker.RuleFor(r => r.statecode, r => pp_record_statecode.Inactive);
                recordfaker.RuleFor(r => r.statuscode, r => pp_record_statuscode.Inactive);
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(recordfaker.Generate());

            if (withEditableField != null)
            {
                await recordPage.Page.EvaluateAsync("field => Xrm.Page.getControl(field).setDisabled(false)", withEditableField);
            }

            if (withDisabledField != null)
            {
                await recordPage.Page.EvaluateAsync("field => Xrm.Page.getControl(field).setDisabled(true)", withDisabledField);
            }

            if (withOptionalField != null)
            {
                await recordPage.Page.EvaluateAsync("field => Xrm.Page.getControl(field).getAttribute().setRequiredLevel('none')", withOptionalField);
            }

            if (withRequiredField != null)
            {
                await recordPage.Page.EvaluateAsync("field => Xrm.Page.getControl(field).getAttribute().setRequiredLevel('required')", withRequiredField);
            }

            if (withRecommendedField != null)
            {
                await recordPage.Page.EvaluateAsync("field => Xrm.Page.getControl(field).getAttribute().setRequiredLevel('recommended')", withRecommendedField);
            }

            if (withHiddenField != null)
            {
                await recordPage.Page.EvaluateAsync("field => Xrm.Page.getControl(field).setVisible(false)", withHiddenField);
            }

            if (withVisibleField != null)
            {
                await recordPage.Page.EvaluateAsync("field => Xrm.Page.getControl(field).setVisible(true)", withVisibleField);
            }

            await recordPage.Page.WaitForAppIdleAsync();

            return recordPage.Form;
        }
    }
}