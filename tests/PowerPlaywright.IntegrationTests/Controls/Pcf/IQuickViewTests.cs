namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using Bogus;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests for the <see cref="IReadOnlyGrid"/> control class.
    /// </summary>
    public class IQuickViewTests : IntegrationTests
    {
        private Faker faker;

        /// <summary>
        /// Sets up the lookup control.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.faker = new Faker("en_GB");
        }

        /// <summary>
        /// Tests that <see cref="IQuickView.GetField(string)"/> throws an exception when the quick view is not present on the form.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetField_Always_ReturnsInstanceOfField()
        {
            var quickViewControl = await this.SetupQuickViewScenarioAsync();

            Assert.That(() => quickViewControl.GetField(pp_RelatedRecord.Forms.QuickViewInformation.Name), Is.InstanceOf<IField>());
        }

        /// <summary>
        /// Tests that <see cref="IQuickView.GetField(string)"/> returns a field scoped to the quick view by getting the value of a field with the same name as a field on the parent form.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetField_Always_ReturnsFieldScopedToQuickView()
        {
            var expectedName = this.faker.Random.Words();
            var relatedRecordFaker = new RelatedRecordFaker();
            relatedRecordFaker.RuleFor(r => r.pp_Name, f => expectedName);
            var quickViewControl = await this.SetupQuickViewScenarioAsync(withRelatedRecord: relatedRecordFaker);
            var quickViewField = quickViewControl.GetField<ISingleLineText>(pp_RelatedRecord.Forms.QuickViewInformation.Name);

            Assert.That(quickViewField.Control.GetValueAsync, Is.EqualTo(expectedName));
        }

        /// <summary>
        /// Tests that the fields returns by <see cref="IQuickView.GetField(string)"/> always return true for <see cref="IField.IsDisabledAsync"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetField_FieldVisible_AlwaysDisabled()
        {
            var quickViewControl = await this.SetupQuickViewScenarioAsync();

            Assert.That(quickViewControl.GetField(pp_RelatedRecord.Forms.QuickViewInformation.Name).IsDisabledAsync, Is.True);
        }

        private async Task<IQuickView> SetupQuickViewScenarioAsync(Faker<pp_Record>? withRecord = null, Faker<pp_RelatedRecord>? withRelatedRecord = null)
        {
            withRecord ??= new RecordFaker();

            withRecord.RuleFor(r => r.pp_relatedrecord_record, f => withRelatedRecord?.Generate() ?? new RelatedRecordFaker().Generate());

            var recordPage = await this.LoginAndNavigateToRecordAsync(withRecord.Generate());

            return recordPage.Form.GetQuickView(pp_Record.Forms.Information.QuickView);
        }
    }
}