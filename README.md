
# Power Playwright

Power Playwright makes test automation for Power Apps easier by providing a [page object model](https://playwright.dev/dotnet/docs/pom) built on top of [Playwright](https://playwright.dev/dotnet/).

- All regions supported
- Resilient to platform updates
- Extensible for custom pages and controls

## Table of Contents

- [Power Playwright](#power-playwright)
  - [Table of Contents](#table-of-contents)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
    - [Updating and reproducible runs](#updating-and-reproducible-runs)
  - [Usage](#usage)
    - [Pages](#pages)
      - [Custom pages](#custom-pages)
    - [Controls](#controls)
      - [Control classes](#control-classes)
      - [Custom controls](#custom-controls)
  - [Contributing](#contributing)

## Prerequisites

You will need a test project setup with [Playwright](https://playwright.dev/dotnet/docs/intro).

## Installation

Install the NuGet package in your test project:

```shell
dotnet add package PowerPlaywright
```

The main package includes the API, Framework, built-in strategies, and their assets as one tested release. Initialization and execution use the installed application output and make no requests to NuGet. Install Playwright browsers separately as described in the Playwright documentation.

### Updating and reproducible runs

To accept stable updates within major version 1 at restore time, use:

```xml
<PackageReference Include="PowerPlaywright" Version="1.*" />
```

When explicitly checking for updates, reevaluate the dependency graph and bypass the HTTP cache, then build and run with that same resolution:

```shell
dotnet restore --force-evaluate --no-http-cache
dotnet build --configuration Release --no-restore
dotnet test --configuration Release --no-build --no-restore
```

On .NET 8, use `--no-cache` in place of `--no-http-cache`.

An ordinary build can use incremental restore and skip feed checks when project inputs have not changed. A floating reference alone does not guarantee that every build contacts the feed. `1.*` selects stable releases; prerelease updates require an explicit prerelease version or floating pattern such as `1.*-*`.

For controlled updates, use an exact reference such as `Version="[1.2.3]"`, or enable `RestorePackagesWithLockFile`, commit `packages.lock.json`, and restore with `--locked-mode` in CI. A locked restore retains the recorded resolution and fails if project dependencies require a lock-file change. To update a floating dependency deliberately, run the refresh command above **without** `--locked-mode`, review the lock-file changes, then rebuild. For an exact reference, also change the reference before refreshing.

Record the resolved versions from `obj/project.assets.json` with your test results, and retain `packages.lock.json` when used. Package fixes take effect after restore and rebuild; publishing a new release does not replace code in an already running or previously built suite.

When migrating from runtime strategy acquisition, keep referencing only `PowerPlaywright`, remove explicit references to the old `PowerPlaywright.Strategies` package, and clean/rebuild the test output. `CreateAsync` and `PageObjectAssemblies` registration are unchanged. Runtime access to package feeds, feed credentials, and the global NuGet cache is no longer needed after restore/build; access to the target Power Apps environment is still required for live tests.

## Usage

Create an instance of `IPowerPlaywright`. You can reuse a single instance across your test suite:

```csharp
var powerPlaywright = await PowerPlaywright.CreateAsync();
```

You can use this to create instances of `IModelDrivenApp` by providing the Playwright `IBrowserContext` (refer to the [Playwright docs](https://playwright.dev/dotnet/docs/intro)) along with the environment url, app unique name, and credentials:

```csharp
var homePage = await powerPlaywright.LaunchAppAsync(this.Context, environmentUrl, appUniqueName, username, passsword)
```

This will return a generic `IModelDrivenAppPage` that provides access to functionality common across all app pages (e.g. the site map). If you know the type of home page then you can pass the type as an argument:

```csharp
var listPage = await powerPlaywright.LaunchAppAsync<IEntityListPage>(this.Context, environmentUrl, appUniqueName, username, passsword)
```

### Pages

Writing tests with Power Playwright involves interacting with page and control interfaces. 

Page interfaces model the different pages available within model-driven apps (documented [here](https://learn.microsoft.com/en-us/power-apps/maker/model-driven-apps/create-remove-pages#create-a-page)). These page interfaces provide you with the relevant controls. 

At the time of writing, the supported pages are:

- Dataverse form (`IEntityRecordPage`)
- Dataverse table (`IEntityListPage`)
- Dashboard (`IDashboardPage`)
- Custom page (`ICustomPage`)
- Web resource (`IWebResourcePage`)

You should pass one of these interfaces as a type argument when calling methods that return a page.

#### Custom pages

You can create a custom page by creating a sub-class of the `CustomPage` class and adding properties with public getters to represent its controls. These getters should be implemented using the inherited `GetControl` method.

At present, there are no canvas app controls included in Power Playwright. This means you will need to implement these controls as [custom controls](#custom-controls).

### Controls

Controls are accessible via properties on the pages. For example, an `IEntityListPage` page exposes a `Grid` control property that provides access to the grid control:

```csharp
var recordPage = await listPage.Grid.OpenRecordAsync(0);
```

In addition, some controls are accessible by interacting with other controls. For example, an `IEntityRecordPage` page exposes a `Form` control property with a `GetControl` method:

```csharp
var readOnlyGrid = recordPage.Form.GetControl<IReadOnlyGrid>(pp_Record.Forms.Information.RelatedRecordsSubgrid);
```

#### Control classes

In most cases, PCF controls are added to forms as control _classes_ rather than specific controls. This means that the actual PCF control that gets rendered on the form can change over time.

You can retrieve form controls by specifying either a control class _or_ a specific control. These can be found in the `PowerPlaywright.Framework.Controls.Pcf.Classes` and `PowerPlaywright.Framework.Controls.Pcf` namespaces respectively.

This subgrid has been added without configuring a specific control:

```csharp
var subGrid = recordPage.Form.GetControl<IReadOnlyGrid>(pp_Record.Forms.Information.RelatedRecordsSubgrid);
```

This subgrid has been added with the PowerAppsOneGrid control explicitly configured:

```csharp
var subGrid = recordPage.Form.GetControl<IPowerAppsOneGridControl>(pp_Record.Forms.Information.RelatedRecordsSubgrid); 
```

#### Custom controls

Power Playwright can be extended by consumers to support their own custom controls or controls that have not yet been implemented by Power Playwright. To add additional controls, pass the names of your assemblies containing the controls to the `PowerPlaywright.CreateAsync` method.

```csharp
PowerPlaywright.CreateAsync(
  new PowerPlaywrightConfiguration 
  {
    PageObjectAssemblies = [new() { Path = "Company.Project.PageObjects.dll" }] 
  });
```

For information on implementing controls, refer to [CONTRIBUTING.md](./CONTRIBUTING.md).

## Contributing

Please refer to the [CONTRIBUTING.md](./CONTRIBUTING.md).
