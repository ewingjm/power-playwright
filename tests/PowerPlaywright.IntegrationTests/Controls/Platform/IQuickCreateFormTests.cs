namespace PowerPlaywright.IntegrationTests.Controls.Platform
{
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Model;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    /// <summary>
    /// Tests the <see cref="IQuickCreateForm"/> platform control.
    /// </summary>
    public class IQuickCreateFormTests : IntegrationTests
    {
        /// <summary>
        /// Tests that <see cref="IQuickCreateForm.GetFieldsAsync"/> always returns fields on the quick create form.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetFieldsAsync_QuickCreateOpen_ReturnsQuickCreateFormFields()
        {
            var quickCreate = await this.SetupQuickCreateFormScenarioAsync();

            var fields = await quickCreate.GetFieldsAsync();

            Assert.That(fields.ToList(), Has.Count.EqualTo(1).And.All.Property(nameof(IField.Location)).EqualTo(FieldLocation.Body));
        }

        /// <summary>
        /// Tests that <see cref="IQuickCreateForm.CancelAsync"/> always cancels the quick create.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task Cancel_QuickCreateOpen_CancelsQuickCreate()
        {
            var quickCreate = await this.SetupQuickCreateFormScenarioAsync();

            await quickCreate.CancelAsync();

            Assert.That(await quickCreate.Container.IsVisibleAsync(), Is.False);
        }

        /// <summary>
        /// Tests that <see cref="IQuickCreateForm.GetFormNotificationsAsync"/> returns the notification when there is a single notification.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetFormNotificationsAsync_SingleNotification_ReturnsNotification()
        {
            var expectedNotifications = new FormNotification[] { new(FormNotificationLevel.Info, "This is a notification") };
            var quickCreate = await this.SetupQuickCreateFormScenarioAsync(withNotifications: expectedNotifications);

            var notifications = await quickCreate.GetFormNotificationsAsync();

            Assert.That(notifications, Is.EquivalentTo(expectedNotifications));
        }

        /// <summary>
        /// Tests that <see cref="IQuickCreateForm.GetFormNotificationsAsync"/> returns the notifications when there is multiple notifications.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task GetFormNotificationsAsync_MultipleNotification_ReturnsAllNotifications()
        {
            var expectedNotifications = new FormNotification[]
            {
                new(FormNotificationLevel.Info, "This is an info notification"),
                new(FormNotificationLevel.Warning, "This is an warning notification"),
                new(FormNotificationLevel.Error, "This is an error notification"),
            };
            var quickCreate = await this.SetupQuickCreateFormScenarioAsync(withNotifications: expectedNotifications);

            var notifications = await quickCreate.GetFormNotificationsAsync();

            Assert.That(notifications, Is.EquivalentTo(expectedNotifications));
        }

        /// <summary>
        /// Tests that <see cref="IQuickCreateForm.CancelAsync"/> cancels the quick create form when it is open.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SaveAndClose_QuickCreateOpen_PopulatesLookup()
        {
            var recordPage = await this.SetupRecordPageScenarioAsync();
            var lookup = recordPage.Form.GetField<ILookup>(pp_Record.Forms.Information.RelatedRecord);
            var quickCreate = await lookup.Control.NewViaQuickCreateAsync();
            var expectedName = nameof(this.SaveAndClose_QuickCreateOpen_PopulatesLookup);

            await quickCreate
                .GetField<ISingleLineText>(pp_RelatedRecord.Forms.QuickViewInformation.Name).Control
                .SetValueAsync(expectedName);
            await quickCreate.SaveAndCloseAsync();

            Assert.That(await lookup.Control.GetValueAsync(), Is.EqualTo(expectedName));
        }

        /// <summary>
        /// Sets up a quick create form scenario.
        /// </summary>
        /// <param name="withNotifications">Optional notifications to add to the form.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the initialized <see cref="IUrlControl"/>.</returns>
        private async Task<IQuickCreateForm> SetupQuickCreateFormScenarioAsync(FormNotification[]? withNotifications = null)
        {
            var recordPage = await this.SetupRecordPageScenarioAsync();

            var quickCreate = await recordPage.Form.GetField<ILookup>(pp_Record.Forms.Information.RelatedRecord).Control.NewViaQuickCreateAsync();

            if (withNotifications != null)
            {
                foreach (var notification in withNotifications)
                {
                    await recordPage.Page.EvaluateAsync(
                        "notification => Xrm.Page.ui.setFormNotification(notification.message, notification.level, notification.level);",
                        new
                        {
                            message = notification.Message,
                            level = notification.Level.ToString().ToUpper(),
                        });
                }

                await recordPage.Page.WaitForAppIdleAsync();
            }

            return quickCreate;
        }

        private Task<IEntityRecordPage> SetupRecordPageScenarioAsync()
        {
            var recordfaker = new RecordFaker();

            return this.LoginAndNavigateToRecordAsync(recordfaker.Generate());
        }
    }
}