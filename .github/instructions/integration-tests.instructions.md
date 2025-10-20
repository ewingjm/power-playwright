---
applyTo: "tests/PowerPlaywright.IntegrationTests/**/*.cs"
---

# Environment variable(s)

When running the integration tests, you must pass the `POWERPLAYWRIGHT__TEST__URL` environment variable (double underscores) to specify the Power Platform environment to run tests against. Tests must pass against both the new UI and old UI environments. 

# Structure
- Inherit from `IntegrationTests` base class
- Use `[SetUp]` for test initialization
- Use `[Test]` for test methods
- Name pattern: `MethodName_Condition_ExpectedResult`

# Patterns

## Setup

```csharp
private async Task<(IControl, IEntityRecordPage)> SetupScenarioAsync(bool returnPage) {
    var record = new RecordFaker().Generate();
    var page = await this.LoginAndNavigateToRecordAsync(record);
    var control = await page.Form.CommandBar.ClickCommandWithDialogAsync<IControl>("Cmd"); // or page.Property
    return returnPage ? (control, page) : (control, null);
}
```

## Assertion
Test side effects of action under test - don't just verify action occurred.