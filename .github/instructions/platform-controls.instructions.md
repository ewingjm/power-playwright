---
applyTo: "src/PowerPlaywright.Framework/Controls/Platform/**/*.cs,src/PowerPlaywright.Strategies/Controls/Platform/**/*.cs"
---

# Adding a Platform Control

Platform controls are UI structure elements (dialogs, command bar, site map, form) that are versioned with the environment.

## Step-by-Step Process

### 1. Inspect the Control
**Using Playwright Browser (see copilot-instructions.md for test URLs):**
- Find dialog/control selector (role, aria-label, data-id)
- Document buttons, child controls, flyout elements
- Note any PCF controls within the platform control (will need `IControlFactory`)

### 2. Get Environment Version
**From browser console:**
```javascript
Xrm.Utility.getGlobalContext().getVersion();
```
Returns format: `major.minor.patch.build` (e.g., `9.2.23114.00174`)

### 3. Create Platform Control Interface
**Location:** `src/PowerPlaywright.Framework/Controls/Platform/I{ControlName}.cs`
**Example:** `IAssignDialog.cs`

```csharp
namespace PowerPlaywright.Framework.Controls.Platform
{
    using PowerPlaywright.Framework.Controls.Platform.Attributes;
    
    /// <summary>
    /// An assign dialog.
    /// </summary>
    [PlatformControl]
    public interface IAssignDialog : IPlatformControl
    {
        /// <summary>
        /// Semantic operation descriptions.
        /// </summary>
        Task AssignToMeAsync();
        Task AssignToUserAsync(string userName);
        Task CancelAsync();
    }
}
```

**Guidelines:**
- Methods should be semantic operations, not UI steps (e.g., `AssignToMeAsync()` not `ClickAssignButton()`)
- Use XML docs on all members
- Inherit from `IPlatformControl`
- Add `[PlatformControl]` attribute

### 4. Create Control Strategy
**Location:** `src/PowerPlaywright.Strategies/Controls/Platform/{ControlName}.cs`
**Example:** `AssignDialog.cs`

```csharp
namespace PowerPlaywright.Strategies.Controls.Platform
{
    using PowerPlaywright.Framework.Controls.Platform.Attributes;
    
    /// <summary>
    /// An assign dialog.
    /// </summary>
    [PlatformControlStrategy(9, 2, 23114, 174)] // From step 2
    public class AssignDialog : Control, IAssignDialog
    {
        private readonly ILookup userOrTeam;
        private readonly ILocator assignButton;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AssignDialog"/> class.
        /// </summary>
        /// <param name="appPage">The app page.</param>
        /// <param name="controlFactory">The control factory.</param>
        /// <param name="parent">The parent control.</param>
        public AssignDialog(IAppPage appPage, IControlFactory controlFactory, IControl parent = null)
            : base(appPage, parent)
        {
            // Child PCF controls (inside dialog)
            this.userOrTeam = controlFactory.CreateCachedInstance<ILookup>(appPage, "fieldName", this);
            
            // Buttons/elements inside control
            this.assignButton = this.Container.GetByRole(AriaRole.Button, new() { Name = "Assign" });
        }
        
        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Page.Locator("[role='dialog'][aria-label='Assign']:not([aria-hidden='true'])");
        }
        
        // Implement interface methods...
    }
}
```

**Key Points:**
- Inherit from `Control` and interface
- Use `[PlatformControlStrategy(major, minor, patch, build)]` with 4 numbers
- Implement `GetRoot()` to identify top-level element
- Elements INSIDE control: use `this.Container.Locator()` or `this.Container.GetByRole()`
- Elements OUTSIDE control: use `this.Page.Locator()`
- Always call `await this.Page.WaitForAppIdleAsync()` before/after interactions

**Constructor Parameters:**
- **Required:** `IAppPage appPage, IControl parent = null`
- **Optional:** `IControlFactory controlFactory` (if child controls exist - including PCF controls)
- **Optional:** `IPageFactory pageFactory` (if navigation occurs)

### 5. Create Integration Tests
**Location:** `tests/PowerPlaywright.IntegrationTests/Controls/Platform/I{ControlName}Tests.cs`
**Example:** See integration-tests.instructions.md

## Common Patterns

### Dialog with Child Controls
```csharp
public DialogControl(IAppPage appPage, IControlFactory controlFactory, IControl parent = null)
    : base(appPage, parent)
{
    this.childLookup = controlFactory.CreateCachedInstance<ILookup>(appPage, "fieldName", this);
    this.okButton = this.Container.GetByRole(AriaRole.Button, new() { Name = "OK" });
}
```

### Control with Flyout (Outside Container)
```csharp
public ControlWithFlyout(IAppPage appPage, IControl parent = null)
    : base(appPage, parent)
{
    // Inside control
    this.triggerButton = this.Container.GetByRole(AriaRole.Button);
    
    // Outside control (flyout)
    this.flyoutMenu = this.Page.Locator("[role='menu']");
}
```

### GetRoot Patterns
```csharp
// Dialog
return context.Page.Locator("[role='dialog'][aria-label='Title']:not([aria-hidden='true'])");

// Section on page
return context.Locator("[data-id='sectionName']");

// Command bar
return context.Locator("[data-id='CommandBar']");
```

## Checklist
- [ ] Interface in Framework/Controls/Platform with `[PlatformControl]`
- [ ] Strategy in Strategies/Controls/Platform with `[PlatformControlStrategy(maj,min,patch,build)]`
- [ ] Constructor accepts `IAppPage, IControl parent`
- [ ] `GetRoot()` implemented
- [ ] Locators use `Container` for inside, `Page` for outside
- [ ] All methods call `WaitForAppIdleAsync()`
- [ ] XML docs on all public members
- [ ] Integration tests in tests/PowerPlaywright.IntegrationTests/Controls/Platform
- [ ] Tests validate side effects, not just actions
- [ ] ❌ NO redirector (platform controls don't need them)