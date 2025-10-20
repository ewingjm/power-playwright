---
applyTo: "src/PowerPlaywright.Framework/Controls/Platform/**/*.cs,src/PowerPlaywright.Strategies/Controls/Platform/**/*.cs"
---

# Adding a Platform Control

1. **Inspect the control** using Playwright Browser
   - Find dialog/control selector (role, aria-label, data-id)
   - Document buttons, child controls, flyout elements

2. **Get environment version** from browser console:
   ```javascript
   Xrm.Utility.getGlobalContext().getVersion();
   ```

3. **Create platform control interface**
   - Location: `src/PowerPlaywright.Framework/Controls/Platform/`
   - Has `[PlatformControl]` attribute
   - Methods should be semantic operations (not UI steps)

4. **Create control strategy**
   - Location: `src/PowerPlaywright.Strategies/Controls/Platform/`
   - Inherits from `Control`
   - Has `[PlatformControlStrategy(major, minor, patch, build)]` (4 numbers)
   - Constructor: `(IAppPage appPage, IControlFactory controlFactory, IControl parent = null)`
   - No redirector needed

5. **Create integration tests**
   - Location: `tests/PowerPlaywright.IntegrationTests/Controls/Platform/`

# Constructor signature

```csharp
public ControlName(IAppPage appPage, IControl parent = null)
    : base(appPage, parent)
```

Constructor can also accept `IControlFactory` if child controls are needed or `IPageFactory` if page navigations occur.

# Get root

```csharp
protected override ILocator GetRoot(ILocator context)
{
    return context.Page.Locator("[role='dialog'][aria-label='Dialog Title']:not([aria-hidden='true'])");
}
```

`GetRoot` must be implemented for platform controls. Attempt to identify the top-level element for the control.