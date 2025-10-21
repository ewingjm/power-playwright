# GitHub Copilot Instructions for Power Playwright

## Project Overview
Power Playwright is a C# testing framework for Microsoft Power Platform model-driven apps built on Playwright. It provides strongly-typed interfaces for interacting with Power Apps controls, forms, and dialogs.

**Tech Stack:** C# (.NET Standard 2.0/net8.0 tests), Playwright 1.49.0, NUnit, Microsoft.Extensions.DI/Logging
**Path-Specific Instructions:** See `.github/instructions/` for context-aware guidance
**Key Docs:** `CONTRIBUTING.md`, `tests/PowerPlaywright.IntegrationTests/README.md`, https://playwright.dev/dotnet/docs/locators

## Architecture: Three-Assembly Structure
1. **PowerPlaywright.Framework** (`src/PowerPlaywright.Framework/`) - Public interfaces, attributes, shared types. ⚠️ Breaking changes prohibited
2. **PowerPlaywright** (`src/PowerPlaywright/`) - Internal implementation. ⚠️ No control implementations here
3. **PowerPlaywright.Strategies** (`src/PowerPlaywright.Strategies/`) - Control implementations and redirectors. ⚠️ Non-breaking changes only

## Control Types
**PCF Controls** (field/data): Individually versioned, need redirector, in `Framework/Controls/Pcf/` and `Strategies/Controls/Pcf/`. Version from Web API: `/api/data/v9.2/customcontrols?$filter=name eq 'ControlName'`
**Platform Controls** (UI structure): Environment versioned, no redirector, in `Framework/Controls/Platform/` and `Strategies/Controls/Platform/`. Version from browser console: `Xrm.Utility.getGlobalContext().getVersion()`

## Build & Test (✅ VALIDATED)
```pwsh
# ALWAYS restore first
dotnet restore

# Build (time: ~13s)
dotnet build -c Release --no-restore

# Unit tests (time: ~4s, 1 skipped)
dotnet test tests/PowerPlaywright.UnitTests/PowerPlaywright.UnitTests.csproj -c Release --no-build --logger console

# Format check (CI requirement)
dotnet format --verify-no-changes --no-restore

# Package
dotnet pack --no-restore --output nupkg -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg

# Clean build (if issues)
dotnet clean && dotnet build -c Release
```

**Expected Warnings (safe to ignore):** MSB3277 (Bcl.AsyncInterfaces), SA0001 (XML comments), CS0067 (unused event)

**Integration Tests:** Require `POWERPLAYWRIGHT__TEST__URL` env var (double underscores) and Power Platform environment setup. See `tests/PowerPlaywright.IntegrationTests/README.md`.

## CI/CD Pipeline (`.github/workflows/power-playwright.yml`)
**Build Job (ubuntu, .NET 8.0):** restore → build → unit tests → pack → upload artifacts
**Test Job (matrix: new-look/old-look):** Playwright setup → integration tests → upload results/traces
**Deploy Job:** Push to nuget.org (main branch only)

**Pre-PR Validation:**
1. `dotnet restore` (no errors)
2. `dotnet build -c Release --no-restore` (only expected warnings)
3. `dotnet test tests/PowerPlaywright.UnitTests/*.csproj -c Release --no-build` (all pass)
4. `dotnet format --verify-no-changes --no-restore` (exit 0)

**Versioning:** GitVersion with Conventional Commits (feat=minor, fix=patch, BREAKING CHANGE=major)

## Code Style & Conventions
**Naming:** Interfaces `I{Name}` (IAssignDialog), Classes `{Name}` (AssignDialog), Redirectors `{ClassName}Redirector`, Tests `I{Name}Tests`
**XML Docs:** Required on all public interfaces/methods/properties
**Locators:** Inside control use `Container`, outside use `Page`. Prefer semantic selectors (GetByRole, GetByLabel) over CSS.

## Project Structure
```
src/
├── PowerPlaywright.Framework/     # Public interfaces (netstandard2.0)
│   ├── Controls/Pcf/Classes/      # ILookup, IChoice, IReadOnlyGrid
│   ├── Controls/Pcf/I*.cs         # Specific PCF interfaces
│   └── Controls/Platform/I*.cs    # ISiteMapControl, IAssignDialog
├── PowerPlaywright/               # Internal impl (netstandard2.0, main NuGet)
└── PowerPlaywright.Strategies/    # Control strategies (netstandard2.0, separate NuGet)
    ├── Controls/Pcf/              # SimpleLookupControl, ChoiceControl
    ├── Controls/Platform/         # SiteMapControl, AssignDialog
    └── Redirectors/               # LookupRedirector, ChoiceRedirector
tests/
├── PowerPlaywright.UnitTests/              # Fast tests (net8.0)
├── PowerPlaywright.IntegrationTests/       # Slow tests (net8.0)
│   ├── Controls/Pcf/ & Controls/Platform/  # Test organization
│   └── .runsettings                        # 4 workers, Chromium
└── solution/pp_PowerPlaywright_Test/       # Power Platform test solution
```

**Key Files:** GitVersion.yml (versioning), .editorconfig (StyleCop SA1633 disabled), src/Directory.Build.props (build props)

## Quick Reference
| Aspect         | PCF Control                           | Platform Control                                 |
| -------------- | ------------------------------------- | ------------------------------------------------ |
| Base Class     | `PcfControlInternal`                  | `Control`                                        |
| Constructor    | `(IAppPage, string name, IControl)`   | `(IAppPage, IControl)`                           |
| Attribute      | `[PcfControlStrategy(maj,min,patch)]` | `[PlatformControlStrategy(maj,min,patch,build)]` |
| Redirector     | ✅ Required                            | ❌ Not needed                                     |
| Version Source | Web API                               | Browser console                                  |

## Common Patterns
**Control with children:** Accept `IControlFactory` in constructor, use `controlFactory.CreateCachedInstance<T>()`
**Control with flyout:** Elements inside control use `this.Container.Locator()`, outside use `this.Page.Locator()`
**Wait for app:** Always use `ClickAndWaitForAppIdleAsync()` and similar extension methods

## Testing Requirements
- Validate side effects, not just action completion
- One test per public interface method minimum
- Tests must pass on both new UI and old UI environments

## Common Pitfalls
❌ Don't: Expose internal controls unnecessarily, make breaking changes in Strategies, use brittle CSS selectors
✅ Do: Combine steps into semantic operations, keep interfaces minimal, wait for app idle, follow existing patterns

## Commit Format
```
<type>(<scope>): <subject>
```
Types: feat (minor), fix/perf (patch), docs, test, refactor, chore. Add `BREAKING CHANGE:` in body for major bump.

## Trust These Instructions
These are validated. Minimize exploration - only search codebase for implementation patterns or when encountering contradictions. Don't search for: build commands, project structure, naming conventions, CI validation steps (all documented above).
