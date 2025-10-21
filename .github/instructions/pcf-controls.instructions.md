---
applyTo: "src/PowerPlaywright.Framework/Controls/Pcf/**/*.cs,src/PowerPlaywright.Strategies/Controls/Pcf/**/*.cs,src/PowerPlaywright.Strategies/Redirectors/**/*.cs"
---

# Adding a PCF Control

PCF (Power Apps Component Framework) controls are field/data controls that are individually versioned via PCF. They require a control class, specific control interface, redirector, and strategy.

## Step-by-Step Process

### 1. Inspect the Control
**Using Playwright Browser or Form XML:**
- Find control name in DOM via `data-lp-id` attribute or in form XML's `<controlDescription>` element
- Document structure, buttons, input elements, flyout behavior
- Note: Search DevTools for `div[data-lp-id*="fieldControl"]` to find control container

**Example DOM:**
```html
<div data-lp-id="MscrmControls.FieldControls.SimpleLookupControl|fieldname.fieldControl|formname">
```
**Control name:** `MscrmControls.FieldControls.SimpleLookupControl`

### 2. Get Control Version
**From Web API:**
```
GET /api/data/v9.2/customcontrols?$filter=name eq 'MscrmControls.FieldControls.SimpleLookupControl'&$select=name,version
```
Returns: `{"name":"...", "version":"1.0.470"}`

### 3. Create Control Class Interface (if new class)
**Location:** `src/PowerPlaywright.Framework/Controls/Pcf/Classes/I{ClassName}.cs`
**Example:** `ILookup.cs`

```csharp
namespace PowerPlaywright.Framework.Controls.Pcf.Classes
{
    /// <summary>
    /// Lookup control class.
    /// </summary>
    public interface ILookup : IPcfControl
    {
        /// <summary>
        /// Sets the value of the lookup.
        /// </summary>
        Task SetValueAsync(string value);
        
        /// <summary>
        /// Gets the value of the lookup.
        /// </summary>
        Task<string> GetValueAsync();
    }
}
```

**Skip if class already exists.** Common classes: `ILookup`, `IChoice`, `IReadOnlyGrid`, `IField`

### 4. Create Specific Control Interface
**Location:** `src/PowerPlaywright.Framework/Controls/Pcf/I{ControlName}.cs`
**Example:** `ISimpleLookupControl.cs`

```csharp
namespace PowerPlaywright.Framework.Controls.Pcf
{
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    
    /// <summary>
    /// An interface for the MscrmControls.FieldControls.SimpleLookupControl control.
    /// </summary>
    [PcfControl("MscrmControls.FieldControls.SimpleLookupControl")]
    public interface ISimpleLookupControl : ILookup
    {
        // Add control-specific methods only (if any)
        // Most controls don't add additional methods
    }
}
```

**Key Points:**
- Use full qualified name in `[PcfControl("...")]` attribute
- Inherit from control class interface
- Usually empty (inherits all methods from class)

### 5. Create or Update Redirector
**Location:** `src/PowerPlaywright.Strategies/Redirectors/{ClassName}Redirector.cs`
**Example:** `LookupRedirector.cs`

```csharp
namespace PowerPlaywright.Strategies.Redirectors
{
    using System;
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Redirectors;
    
    /// <summary>
    /// A redirector for the <see cref="ILookup"/> control class.
    /// </summary>
    public class LookupRedirector : ControlRedirector<ILookup>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LookupRedirector"/> class.
        /// </summary>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="logger">the logger.</param>
        public LookupRedirector(IRedirectionInfoProvider infoProvider, ILogger<LookupRedirector> logger)
            : base(infoProvider, logger)
        {
        }
        
        /// <inheritdoc/>
        protected override Type GetTargetControlType(IRedirectionEnvironmentInfo environmentInfo, RedirectionControlInfo controlInfo)
        {
            // Logic to select correct control based on environment
            return typeof(ISimpleLookupControl);
        }
    }
}
```

**Purpose:** Routes control class requests (e.g., `ILookup`) to specific control type (e.g., `ISimpleLookupControl`) based on environment settings. There may be different specific control types based on the values found in the redirection info e.g. if new look is enabled. Use the old look and new look environments to check if the rendered controls are different.

**If redirector exists:** Update `GetTargetControlType()` logic to handle new control type.

### 6. Create Control Strategy
**Location:** `src/PowerPlaywright.Strategies/Controls/Pcf/{ControlName}.cs`
**Example:** `SimpleLookupControl.cs`

```csharp
namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    
    /// <summary>
    /// A control strategy for the <see cref="ISimpleLookupControl"/>.
    /// </summary>
    [PcfControlStrategy(1, 0, 470)] // From step 2 (3 numbers: major, minor, patch)
    public class SimpleLookupControl : PcfControlInternal, ISimpleLookupControl
    {
        private readonly IControlFactory controlFactory;
        private readonly ILocator input;
        private readonly ILocator flyout;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleLookupControl"/> class.
        /// </summary>
        /// <param name="appPage">The app page.</param>
        /// <param name="name">The name of the field.</param>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="controlFactory">The control factory.</param>
        /// <param name="parent">The parent control.</param>
        /// <param name="logger">The logger.</param>
        public SimpleLookupControl(
            IAppPage appPage,
            string name,
            IEnvironmentInfoProvider infoProvider,
            IControlFactory controlFactory,
            IControl parent,
            ILogger<SimpleLookupControl> logger = null)
            : base(name, appPage, infoProvider, parent)
        {
            this.controlFactory = controlFactory;
            
            // Elements inside control
            this.input = this.Container.Locator($"input[data-id='{this.Name}.fieldControl-LookupResultsDropdown']");
            
            // Elements outside control (flyout)
            this.flyout = this.Page.Locator($"div[data-id='{this.Name}.fieldControl|__flyoutRootNode']");
        }
        
        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Locator($"div[data-lp-id*='MscrmControls.FieldControls.SimpleLookupControl|{this.Name}.fieldControl|']");
        }
        
        // Implement interface methods...
    }
}
```

**Key Points:**
- Inherit from `PcfControlInternal` and specific control interface
- Use `[PcfControlStrategy(major, minor, patch)]` with 3 numbers (no build)
- Base constructor requires: `name, appPage, infoProvider, parent`
- `this.Name` contains the field name
- Always use `await this.Page.WaitForAppIdleAsync()` before/after interactions

**Constructor Parameters (Standard Pattern):**
```csharp
IAppPage appPage,
string name,
IEnvironmentInfoProvider infoProvider,
IControlFactory controlFactory,  // If child controls needed
IControl parent,
ILogger<T> logger = null  // Optional
```

### 7. Create Integration Tests
**Location:** `tests/PowerPlaywright.IntegrationTests/Controls/Pcf/I{ControlName}Tests.cs`
**Example:** See integration-tests.instructions.md

## Common Patterns

### GetRoot for PCF Controls
```csharp
// Using data-lp-id (most common)
return context.Locator($"div[data-lp-id*='ControlName|{this.Name}.fieldControl|']");

// Using data-id
return context.Locator($"div[data-id='{this.Name}.fieldControl-container']");

// Using class name
return context.Locator($"div.ControlName[data-id*='{this.Name}']");
```

### Flyout Elements (Outside Container)
```csharp
// Flyout root
this.flyout = this.Page.Locator($"div[data-id='{this.Name}.fieldControl|__flyoutRootNode']");

// Results in flyout
this.results = this.flyout.Locator("[role='option']");
```

### Multiple Control Versions
If control has breaking changes, create new strategy with higher version:
```csharp
[PcfControlStrategy(1, 0, 470)]
public class SimpleLookupControl : PcfControlInternal, ISimpleLookupControl { }

[PcfControlStrategy(2, 0, 0)]
public class SimpleLookupControlV2 : PcfControlInternal, ISimpleLookupControl { }
```
Framework automatically selects highest compatible version.

## Checklist
- [ ] Control class interface in Framework/Controls/Pcf/Classes (if new)
- [ ] Specific control interface in Framework/Controls/Pcf with `[PcfControl("...")]`
- [ ] Redirector in Strategies/Redirectors (created or updated)
- [ ] Strategy in Strategies/Controls/Pcf with `[PcfControlStrategy(maj,min,patch)]`
- [ ] Constructor signature: `(IAppPage, string name, IEnvironmentInfoProvider, IControlFactory, IControl, ILogger)`
- [ ] Base constructor call: `base(name, appPage, infoProvider, parent)`
- [ ] `GetRoot()` implemented using `this.Name`
- [ ] Locators use `Container` for inside, `Page` for outside
- [ ] All methods call `WaitForAppIdleAsync()`
- [ ] XML docs on all public members
- [ ] Integration tests in tests/PowerPlaywright.IntegrationTests/Controls/Pcf
- [ ] Tests validate side effects, not just actions