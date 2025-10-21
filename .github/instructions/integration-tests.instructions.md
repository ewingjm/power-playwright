---
applyTo: "tests/PowerPlaywright.IntegrationTests/**/*.cs"
---

# Integration Test Guidelines

Integration tests validate control implementations against actual Power Platform environments. Tests MUST pass on both new UI and old UI environments.

## Environment Setup

### Required Environment Variable
```powershell
$env:POWERPLAYWRIGHT__TEST__URL = "https://your-env.crm.dynamics.com/"
```
**IMPORTANT:** Double underscores required: `POWERPLAYWRIGHT__TEST__URL`

### Additional Requirements
- Power Platform environment with System Administrator access
- Imported `pp_PowerPlaywright_Test` solution (managed) from `tests/solution/`
- Application user with PowerPlaywright Tester role
- User credentials configured (see `tests/PowerPlaywright.IntegrationTests/README.md`)

## Test Structure

### Class Structure
```csharp
namespace PowerPlaywright.IntegrationTests.Controls.Platform
{
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.TestApp.Model;
    
    /// <summary>
    /// Tests for the <see cref="IAssignDialog"/> control.
    /// </summary>
    public class IAssignDialogTests : IntegrationTests
    {
        private Faker faker;
        
        [SetUp]
        public void Setup()
        {
            this.faker = new Faker("en_GB");
        }
        
        [Test]
        public async Task MethodName_Condition_ExpectedResult()
        {
            // Arrange
            var (control, page) = await this.SetupScenarioAsync();
            
            // Act
            await control.SomeMethodAsync();
            
            // Assert - Validate side effects
            Assert.That(await control.GetStateAsync(), Is.EqualTo(expected));
        }
    }
}
```

**Key Points:**
- Inherit from `IntegrationTests` base class
- Use `[SetUp]` for test initialization
- Use `[Test]` for test methods
- Naming: `MethodName_Condition_ExpectedResult`
- XML docs on class and methods

## Test Patterns

### Setup Scenario Helper
```csharp
private async Task<(IControl, IEntityRecordPage)> SetupScenarioAsync(bool returnPage = false)
{
    // Create test data
    var record = new RecordFaker().Generate();
    
    // Navigate to page
    var page = await this.LoginAndNavigateToRecordAsync(record);
    
    // Get control
    var control = await page.Form.CommandBar.ClickCommandWithDialogAsync<IControl>("CommandName");
    // OR
    var control = page.Form.GetControl<IControl>("fieldName");
    
    return returnPage ? (control, page) : (control, null);
}
```

**Available Helper Methods (from `IntegrationTests` base):**
- `LoginAndNavigateToRecordAsync(record)` - Login and navigate to record page
- `LoginAndNavigateToListAsync(entityName)` - Login and navigate to entity list
- `CreateRecordAsync(record)` - Create record in environment
- `UpdateRecordAsync(record)` - Update existing record
- `DeleteRecordAsync(entityName, id)` - Delete record

### Assertion Best Practices

**❌ DON'T - Only verify action occurred:**
```csharp
await dialog.AssignToMeAsync();
Assert.That(await dialog.IsVisibleAsync(), Is.False); // Only checks dialog closed
```

**✅ DO - Validate side effects:**
```csharp
var currentUser = await this.GetCurrentUserAsync();
await dialog.AssignToMeAsync();

// Verify dialog closed AND record owner changed
Assert.That(await dialog.IsVisibleAsync(), Is.False);
Assert.That(await recordPage.Form.GetOwnerAsync(), Is.EqualTo(currentUser.Name));
```

### Test Data Using Bogus
```csharp
// Simple faker
private Faker faker = new Faker("en_GB");
var name = this.faker.Name.FullName();

// Custom faker (in TestApp.Model.Fakers)
var record = new pp_recordFaker().Generate();
```

### Multiple Test Cases
```csharp
[Test]
public async Task SetValue_ValidValue_SetsValue() { }

[Test]
public async Task SetValue_EmptyValue_ClearsValue() { }

[Test]
public async Task SetValue_InvalidValue_ThrowsException() { }
```

## Common Test Scenarios

### Dialog Tests
```csharp
[Test]
public async Task IsVisibleAsync_DialogIsVisible_ReturnsTrue()
{
    var (dialog, _) = await this.SetupDialogScenarioAsync();
    
    Assert.That(await dialog.IsVisibleAsync(), Is.True);
}

[Test]
public async Task CancelAsync_DialogIsVisible_Closes()
{
    var (dialog, _) = await this.SetupDialogScenarioAsync();
    
    await dialog.CancelAsync();
    
    Assert.That(await dialog.IsVisibleAsync(), Is.False);
}
```

### Field Control Tests
```csharp
[Test]
public async Task SetValueAsync_ValidValue_UpdatesField()
{
    var (control, page) = await this.SetupFieldScenarioAsync(returnPage: true);
    var expected = "Test Value";
    
    await control.SetValueAsync(expected);
    await page.Form.SaveAsync();
    
    Assert.That(await control.GetValueAsync(), Is.EqualTo(expected));
}
```

### Navigation Tests
```csharp
[Test]
public async Task OpenRecordAsync_ValidRow_NavigatesToRecord()
{
    var (grid, _) = await this.SetupGridScenarioAsync();
    
    var recordPage = await grid.OpenRecordAsync(0);
    
    Assert.That(recordPage, Is.Not.Null);
    Assert.That(await recordPage.Form.IsVisibleAsync(), Is.True);
}
```

## Test Requirements

### Coverage Requirements
- **Minimum:** One test per public interface method
- **Recommended:** Multiple tests per method covering:
  - Happy path
  - Edge cases
  - Error conditions

### Validation Requirements
- **Always validate side effects**, not just that action completed
- Verify UI state changes (visibility, enabled/disabled)
- Verify data changes (field values, record state)
- Verify navigation occurred (page transitions)

### Environment Requirements
Tests must pass on BOTH:
- New UI environment (latest features)
- Old UI environment (legacy mode)

The CI/CD pipeline runs tests against both environments automatically.

## Running Tests

### All Integration Tests
```powershell
dotnet test tests/PowerPlaywright.IntegrationTests/PowerPlaywright.IntegrationTests.csproj -c Release
```

### Specific Test Class
```powershell
dotnet test --filter "FullyQualifiedName~IAssignDialogTests"
```

### Specific Test Method
```powershell
dotnet test --filter "FullyQualifiedName~IAssignDialogTests.AssignToMeAsync_ValidUser_AssignsToCurrentUser"
```

### Test Configuration
Settings in `tests/PowerPlaywright.IntegrationTests/.runsettings`:
- **NumberOfTestWorkers:** 4 (parallel test execution)
- **BrowserName:** chromium
- **Headless:** true (CI), false (local debugging)

## Debugging Tips

### View Browser During Tests
In `.runsettings`, change:
```xml
<HEADED>1</HEADED>
```

### Enable Playwright Debug
```xml
<PWDEBUG>1</PWDEBUG>
```

### Trace Files
Test failures automatically generate Playwright traces in:
`tests/PowerPlaywright.IntegrationTests/bin/Release/net8.0/playwright-traces/`

View with: `playwright show-trace trace.zip`

## Checklist
- [ ] Test class inherits from `IntegrationTests`
- [ ] Test class has XML doc comment with `<see cref="..."/>`
- [ ] `[SetUp]` method if needed
- [ ] Test methods use `[Test]` attribute
- [ ] Test naming: `MethodName_Condition_ExpectedResult`
- [ ] Setup helper creates scenario efficiently
- [ ] Assertions validate side effects (not just actions)
- [ ] Tests pass on both new UI and old UI environments
- [ ] One test minimum per public interface method
- [ ] Test data uses Faker or TestApp.Model.Fakers
- [ ] No hardcoded GUIDs or environment-specific data