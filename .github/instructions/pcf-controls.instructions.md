---
applyTo: "src/PowerPlaywright.Framework/Controls/Pcf/**/*.cs,src/PowerPlaywright.Strategies/Controls/Pcf/**/*.cs,src/PowerPlaywright.Strategies/Redirectors/**/*.cs"
---

# Adding a PCF Control

1. **Inspect the control** using Playwright Browser
   - Find control name in DOM (`data-lp-id`) or form XML
   - Document structure, buttons, input elements

2. **Get control version** from Web API:
   ```
   /api/data/v9.2/customcontrols?$filter=name eq 'ControlName'&$select=name,version
   ```

3. **Create control class interface** (if new class)
   - Location: `src/PowerPlaywright.Framework/Controls/Pcf/Classes/`
   - Inherits from `IPcfControl`

4. **Create specific control interface**
   - Location: `src/PowerPlaywright.Framework/Controls/Pcf/`
   - Has `[PcfControl("Full.Qualified.Name")]` attribute
   - Inherits from control class interface

5. **Create redirector**
   - Location: `src/PowerPlaywright.Strategies/Redirectors/`
   - Inherits from `ControlRedirector<TControlClass>`

6. **Create control strategy**
   - Location: `src/PowerPlaywright.Strategies/Controls/Pcf/`
   - Inherits from `PcfControlInternal`
   - Has `[PcfControlStrategy(major, minor, patch)]` (3 numbers)
   - Constructor: `(IAppPage appPage, string name, IControl parent = null)`

7. **Create integration tests**
   - Location: `tests/PowerPlaywright.IntegrationTests/Controls/Pcf/`

# Constructor signature

```csharp
public ControlName(IAppPage appPage, string name, IControl parent = null)
    : base(appPage, name, parent)
```

Constructor can also accept `IControlFactory` if child controls are needed or `IPageFactory` if page navigations occur.