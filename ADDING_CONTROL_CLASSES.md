# Adding Control Classes to Power Playwright - Code Generation Guide

## CRITICAL: Always Inspect Controls in Browser First

**YOU MUST USE PLAYWRIGHT BROWSER MCP TO INSPECT BOTH OLD AND NEW UI ENVIRONMENTS BEFORE WRITING ANY CODE.**

Different UI versions often use completely different controls for the same field type. Never assume the control type without verification.

## Quick Start Checklist

When asked to add a control class, follow this exact sequence:

1. ✅ **Inspect new UI** → Get exact control `data-lp-id`
2. ✅ **Inspect old UI** → Get exact control `data-lp-id` (may be different!)
3. ✅ **Create control class interface** → `Framework/Controls/Pcf/Classes/I{Type}.cs`
4. ✅ **Create/update PCF control interfaces** → Use exact control names from inspection
5. ✅ **Create control strategies** → Only for new PCF control interfaces
6. ✅ **Create redirector** → Route based on `IsNewLookEnabled`
7. ✅ **Write tests** → Minimum 7 test cases
8. ✅ **Build and test** → Must pass on both UIs

## Browser Inspection Process

### Step 1: Navigate with Test Flags

**New UI:**
```
https://{env}.crm.dynamics.com/main.aspx?appid={appid}&pagetype=entityrecord&etn={entity}&flags=easyreproautomation%3Dtrue%2Ctestmode%3Dtrue
```

**Old UI:**
```
https://{env}-oldui.crm4.dynamics.com/main.aspx?appid={appid}&pagetype=entityrecord&etn={entity}&flags=easyreproautomation%3Dtrue%2Ctestmode%3Dtrue
```

### Step 2: Find Control Name

Use Playwright Browser MCP `evaluate` tool:

```javascript
// Find the field container
const container = document.querySelector('[data-lp-id*="{fieldname}"]');

// Get the control within it
const control = container.querySelector('[data-lp-id*="Controls"]') || 
                container.querySelector('[data-lp-id*="FieldControls"]');

// Return the exact control name
control?.getAttribute('data-lp-id');
```

**Example Results:**
- New UI: `PowerApps.CoreControls.OptionSetControl|pp_wholenumbertimezone.fieldControl|pp_record`
- Old UI: `MscrmControls.FieldControls.TimeZonePickListControl|pp_wholenumbertimezone.fieldControl|pp_record`

**Extract control type:** The part between namespace and first `|` is what you need for `[PcfControl]` attribute:
- New UI: `PowerApps.CoreControls.OptionSetControl`
- Old UI: `MscrmControls.FieldControls.TimeZonePickListControl`

## Architecture Overview

```
User Code → IWholeNumberTimezone (Control Class)
              ↓
          WholeNumberTimezoneRedirector
              ↓                    ↓
      IOptionSetControl    ITimeZonePickListControl
      (New UI - Reuse)     (Old UI - Create New)
              ↓                    ↓
      OptionSetControl     TimeZonePickListControl
      (Exists)             (Create New)
```

## Component Naming Conventions

- **Control Class:** `I{FieldType}{DataType}` → `IWholeNumberTimezone`
- **PCF Control Interface:** `I{ControlName}Control` → `ITimeZonePickListControl`
- **Strategy:** `{ControlName}Control` → `TimeZonePickListControl`
- **Redirector:** `{FieldType}{DataType}Redirector` → `WholeNumberTimezoneRedirector`
- **Tests:** `I{FieldType}{DataType}Tests` → `IWholeNumberTimezoneTests`

## File Structure Reference

```
Framework/Controls/Pcf/Classes/I{Type}.cs          → Control class interface
Framework/Controls/Pcf/I{Control}Control.cs        → PCF control interface(s)
Strategies/Controls/Pcf/{Control}Control.cs        → Strategy implementation(s)
Strategies/Redirectors/{Type}Redirector.cs         → Redirector
IntegrationTests/Controls/Pcf/I{Type}Tests.cs      → Tests
```

## Code Templates

### Template 1: Control Class Interface

**File:** `src/PowerPlaywright.Framework/Controls/Pcf/Classes/I{Type}.cs`

**When to create:** Always create first

**Template:**

```csharp
namespace PowerPlaywright.Framework.Controls.Pcf.Classes
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// {Description} control class.
    /// </summary>
    public interface I{Type} : IPcfControl
    {
        Task SetValueAsync(string value);
        Task<string> GetValueAsync();
        Task<IEnumerable<string>> GetAllOptionsAsync();  // If applicable
    }
}
```

**Rules:**
- ✅ Always inherit from `IPcfControl`
- ✅ Use `Task<string>` for values, NEVER enums
- ✅ Return `null` for empty values, not empty strings
- ✅ Name: `I{FieldType}{DataType}` (e.g., `IWholeNumberTimezone`)

---

### Template 2A: PCF Control Interface (Reuse Existing)

**File:** Modify existing file like `src/PowerPlaywright.Framework/Controls/Pcf/IOptionSetControl.cs`

**When to use:** Browser inspection shows a control that already has an interface

**Template:**

```csharp
// Just add your new interface to the existing control's inheritance list
[PcfControl("PowerApps.CoreControls.OptionSetControl")]
public interface IOptionSetControl : IChoice, IYesNo, I{YourNewType}
{
}
```

**Rules:**
- ✅ `[PcfControl]` value must EXACTLY match control name from browser inspection
- ✅ Add your interface to the inheritance list
- ❌ Do NOT create a new strategy if the control already exists

---

### Template 2B: PCF Control Interface (Create New)

**File:** `src/PowerPlaywright.Framework/Controls/Pcf/I{Control}Control.cs`

**When to use:** Browser inspection shows a new control that doesn't have an interface

**Template:**

```csharp
namespace PowerPlaywright.Framework.Controls.Pcf
{
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;

    /// <summary>
    /// An interface for the {ControlName} control.
    /// </summary>
    [PcfControl("{ExactControlNameFromBrowser}")]
    public interface I{Control}Control : I{YourType}
    {
    }
}
```

**Rules:**
- ✅ `[PcfControl]` must be EXACT string from browser `data-lp-id` (between namespace and first `|`)
- ✅ Inherit from your control class interface
- ⚠️ You MUST create a corresponding strategy (Template 3)

**Examples:**
```csharp
[PcfControl("MscrmControls.FieldControls.TimeZonePickListControl")]
public interface ITimeZonePickListControl : IWholeNumberTimezone { }

[PcfControl("PowerApps.CoreControls.OptionSetControl")]
public interface IOptionSetControl : IChoice, IYesNo { }
```

---

### Template 3: Control Strategy (Old UI Pattern)

**File:** `src/PowerPlaywright.Strategies/Controls/Pcf/{Control}Control.cs`

**When to use:** You created a NEW PCF control interface (Template 2B)

**Template:**

```csharp
namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Pages;

    [PcfControlStrategy(0, 0, 0)]
    public class {Control}Control : PcfControlInternal, I{Control}Control
    {
        private readonly ILocator toggleMenu;
        private readonly ILocator selectedOption;

        public {Control}Control(string name, IAppPage appPage, IEnvironmentInfoProvider infoProvider, IControl parent = null)
            : base(name, appPage, infoProvider, parent)
        {
            // Initialize locators using this.Container
            this.toggleMenu = this.Container.GetByRole(AriaRole.Combobox);
            this.selectedOption = this.toggleMenu.GetByRole(AriaRole.Option, new LocatorGetByRoleOptions { Selected = true });
        }

        public async Task<string> GetValueAsync()
        {
            await this.AppPage.Page.WaitForAppIdleAsync();
            var optionText = await this.selectedOption.TextContentAsync();
            return optionText != "---" ? optionText : null;
        }

        public async Task SetValueAsync(string value)
        {
            await this.toggleMenu.SelectOptionAsync(value);
        }

        public async Task<IEnumerable<string>> GetAllOptionsAsync()
        {
            await this.AppPage.Page.WaitForAppIdleAsync();
            var isEditable = await this.toggleMenu.IsEditableAsync();
            if (!isEditable)
            {
                throw new PowerPlaywrightException("Unable to retrieve the available options because the control is not editable.");
            }

            var options = this.toggleMenu.GetByRole(AriaRole.Option);
            var allOptions = await options.AllTextContentsAsync();
            return allOptions.Where(o => !string.IsNullOrWhiteSpace(o) && o != "---").Select(o => o.Trim());
        }
    }
}
```

**Critical Rules:**
- ✅ Base class: `PcfControlInternal` (old UI pattern)
- ✅ Constructor: `(string name, IAppPage appPage, IEnvironmentInfoProvider infoProvider, IControl parent = null)`
- ✅ Use `this.Container` for locators
- ✅ Use `this.AppPage.Page.WaitForAppIdleAsync()` for waiting
- ✅ Return `null` for placeholder values like `"---"`
- ⚠️ Look at `OptionSet.cs` for similar patterns

---

### Template 4: Redirector

**File:** `src/PowerPlaywright.Strategies/Redirectors/{Type}Redirector.cs`

**When to create:** Always create

**Template:**

```csharp
namespace PowerPlaywright.Strategies.Redirectors
{
    using System;
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Redirectors;

    public class {Type}Redirector : ControlRedirector<I{Type}>
    {
        public {Type}Redirector(IRedirectionInfoProvider infoProvider, ILogger<{Type}Redirector> logger)
            : base(infoProvider, logger)
        {
        }

        protected override Type GetTargetControlType(IRedirectionEnvironmentInfo environmentInfo, RedirectionControlInfo controlInfo)
        {
            if (!environmentInfo.IsNewLookEnabled)
            {
                return typeof(I{OldUIControl}Control);  // From old UI inspection
            }

            return typeof(I{NewUIControl}Control);  // From new UI inspection
        }
    }
}
```

**Rules:**
- ✅ Inherit from `ControlRedirector<TControlClass>`
- ✅ Return **interface types**, NOT strategy types
- ✅ Use `!environmentInfo.IsNewLookEnabled` for old UI check
- ⚠️ Can add field-specific logic using `controlInfo.Name` if needed

**Example with conditional logic:**
```csharp
protected override Type GetTargetControlType(IRedirectionEnvironmentInfo environmentInfo, RedirectionControlInfo controlInfo)
{
    if (!environmentInfo.IsNewLookEnabled)
    {
        // Special case for statuscode field
        return controlInfo.Name == "statuscode" 
            ? typeof(IPicklistStatusControl)
            : typeof(IOptionSet);
    }
    return typeof(IOptionSetControl);
}
```

---

### Template 5: Integration Tests

**File:** `tests/PowerPlaywright.IntegrationTests/Controls/Pcf/I{Type}Tests.cs`

**When to create:** Always create

**Minimum required tests:** 7 tests covering all scenarios

**Template:**

```csharp
namespace PowerPlaywright.IntegrationTests.Controls.Pcf
{
    using System.Threading.Tasks;
    using Bogus;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.IntegrationTests.Extensions;
    using PowerPlaywright.TestApp.Model;
    using PowerPlaywright.TestApp.Model.Fakers;

    public class I{Type}Tests : IntegrationTests
    {
        private Faker faker;

        [SetUp]
        public void Setup()
        {
            this.faker = new Faker("en_GB");
        }

        [Test]  // Test 1: Null value
        public async Task GetValueAsync_DoesNotContainValue_ReturnsNull()
        {
            var control = await this.Setup{Type}ScenarioAsync(withNoValue: true);
            Assert.That(await control.GetValueAsync(), Is.Null);
        }

        [Test]  // Test 2: Get value when set
        public async Task GetValueAsync_ContainsValue_ReturnsValue()
        {
            var expectedValue = "{SampleValue}";
            var control = await this.Setup{Type}ScenarioAsync(withValue: expectedValue);
            Assert.That(await control.GetValueAsync(), Is.EqualTo(expectedValue));
        }

        [Test]  // Test 3: Get all options
        public async Task GetAllOptionsAsync_ActiveRecord_ReturnsOptions()
        {
            var control = await this.Setup{Type}ScenarioAsync();
            var options = await control.GetAllOptionsAsync();
            Assert.That(options, Is.Not.Empty);
            Assert.That(options, Contains.Item("{SampleOption}"));
        }

        [Test]  // Test 4: Disabled field throws
        public async Task GetAllOptionsAsync_DisabledField_ThrowsException()
        {
            var control = await this.Setup{Type}ScenarioAsync(withDisabledRecord: true);
            Assert.ThrowsAsync<PowerPlaywrightException>(() => control.GetAllOptionsAsync());
        }

        [Test]  // Test 5: Get value on inactive record
        public async Task GetValueAsync_InactiveRecord_ReturnsValue()
        {
            var expectedValue = "{SampleValue}";
            var control = await this.Setup{Type}ScenarioAsync(withValue: expectedValue, withDisabledRecord: true);
            Assert.That(await control.GetValueAsync(), Is.EqualTo(expectedValue));
        }

        [Test]  // Test 6: Set value on empty field
        public async Task SetValueAsync_DoesNotContainValue_SetsValue()
        {
            var control = await this.Setup{Type}ScenarioAsync(withNoValue: true);
            var expectedValue = "{SampleValue}";
            await control.SetValueAsync(expectedValue);
            Assert.That(await control.GetValueAsync(), Is.EqualTo(expectedValue));
        }

        [Test]  // Test 7: Replace existing value
        public async Task SetValueAsync_ContainsValue_ReplacesValue()
        {
            var control = await this.Setup{Type}ScenarioAsync(withValue: "{InitialValue}");
            var expectedValue = "{NewValue}";
            await control.SetValueAsync(expectedValue);
            Assert.That(await control.GetValueAsync(), Is.EqualTo(expectedValue));
        }

        private async Task<I{Type}> Setup{Type}ScenarioAsync(string withValue = null, bool withNoValue = false, bool withDisabledRecord = false)
        {
            var record = new RecordFaker();

            if (withDisabledRecord)
            {
                record.RuleFor(r => r.statecode, r => pp_record_statecode.Inactive);
                record.RuleFor(r => r.statuscode, r => pp_record_statuscode.Inactive);
            }

            if (withNoValue)
            {
                record.RuleFor(x => x.{fieldname}, f => ({type}?)null);
            }
            else if (withValue is not null)
            {
                // Map display value to database value if needed
                record.RuleFor(x => x.{fieldname}, {mappedValue});
            }

            var recordPage = await this.LoginAndNavigateToRecordAsync(record.Generate());
            return recordPage.Form.GetField<I{Type}>(nameof(pp_Record.{fieldname})).Control;
        }
    }
}
```

**Required Test Coverage (Minimum 7 tests):**
1. ✅ GetValueAsync returns null when not set
2. ✅ GetValueAsync returns value when set
3. ✅ GetAllOptionsAsync returns options
4. ✅ GetAllOptionsAsync throws on disabled field
5. ✅ GetValueAsync works on inactive record
6. ✅ SetValueAsync sets value on empty field
7. ✅ SetValueAsync replaces existing value

---

## Build and Test Commands

### Environment Configuration

**CRITICAL:** Tests must pass on BOTH old and new UI environments.

Set the environment variable `POWERPLAYWRIGHT__TEST__URL` to control which UI version tests run against:

```bash
# Test against NEW UI (default)
$env:POWERPLAYWRIGHT__TEST__URL = "https://powerplaywright-test.crm.dynamics.com"

# Test against OLD UI
$env:POWERPLAYWRIGHT__TEST__URL = "https://powerplaywright-test-oldui.crm4.dynamics.com"
```

**Note:** The URL automatically gets `&flags=easyreproautomation%3Dtrue%2Ctestmode%3Dtrue` query parameters appended during test execution for better form loading and control visibility.

### Build and Test Sequence

```bash
# 1. Build solution
dotnet build PowerPlaywright.sln -c Release --no-incremental

# 2. Test on NEW UI
$env:POWERPLAYWRIGHT__TEST__URL = "https://powerplaywright-test.crm.dynamics.com"
dotnet test --filter "FullyQualifiedName~I{Type}Tests" --configuration Release --no-build
# Expected: Passed! - Failed: 0, Passed: 7, Skipped: 0

# 3. Test on OLD UI
$env:POWERPLAYWRIGHT__TEST__URL = "https://powerplaywright-test-oldui.crm4.dynamics.com"
dotnet test --filter "FullyQualifiedName~I{Type}Tests" --configuration Release --no-build
# Expected: Passed! - Failed: 0, Passed: 7, Skipped: 0
```

**Both test runs MUST pass before committing.**

## Common Pitfalls & Solutions

### ❌ Pitfall 1: Not inspecting both UIs
**Problem:** Assuming same control in both UIs  
**Solution:** ALWAYS inspect both with Playwright Browser MCP  
**Impact:** Tests fail in one UI environment

### ❌ Pitfall 2: Wrong [PcfControl] attribute value
**Problem:** Typo or partial control name  
**Solution:** Copy EXACT string from browser inspection  
**Impact:** Control not found at runtime

### ❌ Pitfall 3: Wrong constructor signature
**Problem:** Copying from wrong control type  
**Solution:** Check similar controls (e.g., `OptionSet.cs` for old UI)  
**Impact:** Build errors or runtime crashes

### ❌ Pitfall 4: Creating unnecessary interfaces
**Problem:** Creating PCF interface when control already exists  
**Solution:** Check if control already has interface first  
**Impact:** Duplicate code, maintenance burden

### ❌ Pitfall 5: Using enums instead of strings
**Problem:** Returning enum types from GetValueAsync  
**Solution:** Always use `Task<string>`, map to codes in tests  
**Impact:** Model assembly dependency, deployment issues

### ❌ Pitfall 6: Field not visible in browser
**Problem:** Can't find control, assume it doesn't exist  
**Solution:** Scroll form with `tabpanel.scrollBy(0, 1000)`  
**Impact:** False assumption about missing field

### ❌ Pitfall 7: Missing test coverage
**Problem:** Only testing happy path  
**Solution:** Use 7-test template (null, inactive, disabled, etc.)  
**Impact:** Edge cases fail in production

## Quick Decision Tree

```
START: Need to add control class support
│
├─ 1. Have you inspected BOTH old and new UI?
│   ├─ NO → STOP. Use Playwright Browser MCP first
│   └─ YES → Continue
│
├─ 2. Do both UIs use the SAME control?
│   ├─ YES → You only need to update existing PCF interface
│   │         (Template 2A, skip Templates 2B & 3)
│   └─ NO → You need separate PCF interfaces
│           (Use Templates 2A AND 2B + 3)
│
├─ 3. Does the control already have an interface?
│   ├─ YES → Just add your interface to inheritance (Template 2A)
│   │         NO strategy needed
│   └─ NO → Create new interface (Template 2B) + strategy (Template 3)
│
└─ 4. Create: Control class (Template 1) + Redirector (Template 4) + Tests (Template 5)
```

## Data Type Reference

| Field Type | C# Return Type | Test Data Example | Notes |
|------------|----------------|-------------------|-------|
| Text | `string` | `"Sample text"` | Return `null` for empty |
| Choice/Optionset | `string` | `"Choice A"` | Display name, never enum |
| Whole Number | `int?` | `42` | Nullable for optional |
| Decimal | `decimal?` | `123.45m` | Nullable for optional |
| Date | `DateTime?` | `DateTime.UtcNow` | Always UTC |
| Yes/No | `bool?` | `true` / `false` / `null` | Tri-state support |
| Timezone | `string` | `"(GMT-08:00) Pacific Time"` | Display string |

## Validation Checklist

Before committing, verify:

```
✅ Used Playwright Browser MCP to inspect BOTH UIs
✅ Captured exact control names from data-lp-id attributes
✅ Created control class interface (Template 1)
✅ Updated/created PCF control interface(s) with EXACT [PcfControl] values
✅ Created control strategy if new PCF interface (Template 3)
✅ Created redirector (Template 4)
✅ Wrote all 7 required tests (Template 5)
✅ Build succeeds: dotnet build -c Release --no-incremental
✅ All 7 tests pass on NEW UI (POWERPLAYWRIGHT__TEST__URL = new UI)
✅ All 7 tests pass on OLD UI (POWERPLAYWRIGHT__TEST__URL = old UI)
✅ Commit message references issue number
```

**CRITICAL:** Both UI environments must pass all tests.

## Example: Complete Implementation Flow

**Given:** Need to add timezone control support

**Step 1: Browser Inspection**
- New UI → `PowerApps.CoreControls.OptionSetControl`
- Old UI → `MscrmControls.FieldControls.TimeZonePickListControl`
- **Decision:** Need separate PCF interfaces

**Step 2: Files to Create**
```
✅ Framework/Controls/Pcf/Classes/IWholeNumberTimezone.cs (Template 1)
✅ Framework/Controls/Pcf/ITimeZonePickListControl.cs (Template 2B - OLD UI)
✅ Strategies/Controls/Pcf/TimeZonePickListControl.cs (Template 3)
✅ Strategies/Redirectors/WholeNumberTimezoneRedirector.cs (Template 4)
✅ IntegrationTests/Controls/Pcf/IWholeNumberTimezoneTests.cs (Template 5)
```

**Step 3: Files to Modify**
```
✅ Framework/Controls/Pcf/IOptionSetControl.cs
   Add: IWholeNumberTimezone to inheritance list (Template 2A)
```

**Step 4: Build & Test (Both UIs)**
```bash
# Build
dotnet build PowerPlaywright.sln -c Release --no-incremental

# Test NEW UI
$env:POWERPLAYWRIGHT__TEST__URL = "https://powerplaywright-test.crm.dynamics.com"
dotnet test --filter "IWholeNumberTimezoneTests" --configuration Release --no-build
# Result: Passed! - Failed: 0, Passed: 7, Skipped: 0

# Test OLD UI
$env:POWERPLAYWRIGHT__TEST__URL = "https://powerplaywright-test-oldui.crm4.dynamics.com"
dotnet test --filter "IWholeNumberTimezoneTests" --configuration Release --no-build
# Result: Passed! - Failed: 0, Passed: 7, Skipped: 0
```

## Reference: Similar Controls to Study

For your reference when implementing:

| Your Field Type | Study This Control | File Location |
|----------------|-------------------|---------------|
| Choice/Optionset | `IChoice` | `Framework/Controls/Pcf/Classes/IChoice.cs` |
| Old UI control | `IOptionSet` / `OptionSet.cs` | `Strategies/Controls/Pcf/OptionSet.cs` |
| New UI control | `IOptionSetControl` / `OptionSetControl.cs` | `Strategies/Controls/Pcf/OptionSetControl.cs` |
| Complex redirector | `ChoiceRedirector` | `Strategies/Redirectors/ChoiceRedirector.cs` |
| Simple tests | `IDateTests` | `IntegrationTests/Controls/Pcf/IDateTests.cs` |

## Pro Tips for Code Gen

1. **Always start with browser inspection** - Generate inspection code first
2. **Use templates sequentially** - Don't skip ahead
3. **Copy exact strings** - Use string interpolation for control names from inspection
4. **Build incrementally** - Test after each template
5. **Parallel tool calls** - Read multiple reference files at once when examining patterns
6. **Validate early** - Run build after creating interfaces, before strategies
