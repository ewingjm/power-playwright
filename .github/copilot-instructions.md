# GitHub Copilot Instructions for Power Playwright

## Project Overview

Power Playwright is a C# testing framework for Microsoft Power Platform model-driven apps, built on top of Playwright. It provides strongly-typed interfaces for interacting with Power Apps controls, forms, dialogs, and other UI elements.

**Path-Specific Instructions:**
This file provides general guidance. Path-specific instructions in `.github/instructions/` provide context-aware guidance based on the files you're working on.

**Key Resources:**
- `CONTRIBUTING.md` - Detailed contribution guidelines
- **Playwright Docs** - https://playwright.dev/dotnet/docs/locators

## Architecture

### Three-Assembly Structure

1. **PowerPlaywright.Framework** (`src/PowerPlaywright.Framework/`)
   - Public interfaces and contracts
   - Attribute definitions
   - Shared types used by both core and strategies
   - ⚠️ Changes here must maintain backward compatibility

2. **PowerPlaywright** (`src/PowerPlaywright/`)
   - Internal implementation of framework interfaces
   - Core orchestration logic
   - Code unlikely to break from Power Platform updates
   - ⚠️ Control implementations do NOT go here

3. **PowerPlaywright.Strategies** (`src/PowerPlaywright.Strategies/`)
   - Control implementations (PCF and Platform)
   - Control redirectors
   - Code that may break from Power Platform updates
   - ⚠️ All changes must be non-breaking

## Control Types

### PCF Controls (Power Apps Component Framework)
- Field/data controls (lookup, choice, text input, etc.)
- Individually versioned via PCF
- Located in `Framework/Controls/Pcf/` and `Strategies/Controls/Pcf/`
- Require a "Control Class" interface (e.g., ILookup, IChoice)
- Require a redirector for runtime type selection
- Version from Web API: `/api/data/v9.2/customcontrols?$filter=name eq 'ControlName'`

### Platform Controls
- Structural UI elements d(ialogs, command bar, site map, form, etc.)
- Versioned with environment/platform
- Located in `Framework/Controls/Platform/` and `Strategies/Controls/Platform/`
- No redirector needed
- Set version to `0.0.0.0` for new controls

## Code Style & Conventions

### Naming Conventions
- Interfaces: `I{ControlName}` (e.g., `IAssignDialog`, `ILookupControl`)
- Classes: `{ControlName}` (e.g., `AssignDialog`, `LookupControl`)
- Redirectors: `{ControlClassName}Redirector` (e.g., `LookupRedirector`)
- Test classes: `I{ControlName}Tests` (e.g., `IAssignDialogTests`)

### File Organization
```
src/
├── PowerPlaywright.Framework/
│   └── Controls/
│       ├── Pcf/
│       │   ├── Classes/           # PCF control class interfaces (ILookup, IChoice)
│       │   └── I*.cs              # PCF control specific interfaces
│       └── Platform/
│           └── I*.cs              # Platform control interfaces
├── PowerPlaywright.Strategies/
│   ├── Controls/
│   │   ├── Pcf/                   # PCF control implementations
│   │   └── Platform/              # Platform control implementations
│   └── Redirectors/               # Control class redirectors
tests/
└── PowerPlaywright.IntegrationTests/
    └── Controls/
        ├── Pcf/                   # PCF control tests
        └── Platform/              # Platform control tests
```

### XML Documentation
- All public interfaces, methods, and properties must have XML doc comments

## Testing Requirements

- Always validate side effects, not just successful execution
- Use assertions to verify UI state, data values, etc.
- At least one case per public method in control interfaces

## Playwright Patterns

### Locator Selection
**Inside control (use Container):**
```csharp
this.Container.GetByRole(AriaRole.Button, new() { Name = "Label" })
```

**Outside control (use Page):**
```csharp
this.Page.Locator("selector")
```

### Common Locator Strategies
1. **Prefer semantic selectors:** `GetByRole`, `GetByLabel`, `GetByText`
2. **Use data attributes:** `[data-id='...']`, `[data-lp-id*='...']`
3. **Avoid brittle selectors:** CSS classes, nth-child, etc.

## Common Pitfalls to Avoid

### ❌ Don't Do This
- Expose internal controls as public properties unnecessarily
- Make breaking changes in PowerPlaywright.Strategies

### ✅ Do This
- Combine multiple steps into semantic operations (e.g., `AssignToMeAsync()`)
- Keep interfaces minimal and user-focused
- Wait for app idle before and after every interaction
- Use extension methods like `ClickAndWaitForAppIdleAsync()`
- Follow existing patterns in similar controls

## Commit Message Format

Follow Conventional Commits specification:

```
<type>(<scope>): <subject>

[optional body]

[optional footer]
```

**Types:**
- `feat:` - New feature
- `fix:` - Bug fix
- `refactor:` - Code change that neither fixes a bug nor adds a feature
- `docs:` - Documentation only changes
- `test:` - Adding missing tests or correcting existing tests
- `chore:` - Changes to build process or auxiliary tools

**Examples:**
```
feat: add assign dialog platform control

- Add IAssignDialog interface with assign operations

Related to #62
```

## Test Environment

You can perform exploratory testing when implementing controls in these two environments:

### URLs
- **New UI:** `https://powerplaywright-test.crm.dynamics.com/`
- **Old UI:** `https://powerplaywright-test-oldui.crm4.dynamics.com/`

The app used is `User Interface Demo`

### URL Flags for Testing

After navigating to the app, add this to the URL query string: `&flags=easyreproautomation%3Dtrue%2Ctestmode%3Dtrue`

## Control Decision Tree

1. Is it a field/data control on a form? → PCF Control (note that PCF controls sometimes but rarely appear outside forms, e.g., in dialogs)
2. Is it part of the UI structure? → Platform control

## Common Scenarios

### Control with Child Controls
```csharp
public class ParentControl : Control, IParentControl
{
    private readonly ILookup childLookup;

    public ParentControl(IAppPage appPage, IControlFactory controlFactory, IControl parent = null)
        : base(appPage, parent)
    {
        this.childLookup = controlFactory.CreateCachedInstance<ILookup>(appPage, "fieldname");
    }
}
```

### Control with Flyout/Popup
```csharp
public ControlWithFlyout(IAppPage appPage, IControlFactory controlFactory, IControl parent = null)
    : base(appPage, parent)
{
    // Inside control - use Container
    this.triggerButton = this.Container.GetByRole(AriaRole.Button);
    
    // Outside control - use Page
    this.flyoutOption = this.Page.Locator("[role='menu'] [role='menuitem']");
}
```

## Build & Test Commands

```pwsh
# Build solution
dotnet build PowerPlaywright.sln

# Run all tests
dotnet test PowerPlaywright.sln

# Run specific test class
dotnet test --filter "FullyQualifiedName~IAssignDialogTests"

# Set test environment (double underscores required)
$env:POWERPLAYWRIGHT__TEST__URL = "https://powerplaywright-test.crm.dynamics.com"
```

## Quick Reference Table

| Aspect                  | PCF Control                                 | Platform Control                                        |
| ----------------------- | ------------------------------------------- | ------------------------------------------------------- |
| **Base Class**          | `PcfControlInternal`                        | `Control`                                               |
| **Constructor**         | `(IAppPage, string name, IControl)`         | `(IAppPage, IControlFactory, IControl)`                 |
| **Strategy Attribute**  | `[PcfControlStrategy(major, minor, patch)]` | `[PlatformControlStrategy(major, minor, patch, build)]` |
| **Needs Redirector**    | ✅ Yes                                       | ❌ No                                                    |
| **Has "Name" Property** | ✅ Yes (field name)                          | ❌ No                                                    |
| **Version Source**      | Web API query                               | Browser console                                         |
