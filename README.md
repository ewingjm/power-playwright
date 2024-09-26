
# Power Playwright

Power Playwright makes test automation for Power Apps easier by providing a [page object model](https://playwright.dev/dotnet/docs/pom) built on top of [Playwright](https://playwright.dev/dotnet/). It has been designed with the unique challenges of Power Apps test automation in mind:

- Support for all regions
- Automatic updates ensure platform changes are non-breaking
- Extensibility for your custom pages and controls

## Table of Contents

- [Power Playwright](#power-playwright)
  - [Table of Contents](#table-of-contents)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Usage](#usage)
    - [Login](#login)
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

## Usage

Instantiate a new `ModelDrivenApp` class by calling the `Launch` method and passing in the Playwright [BrowserContext](https://playwright.dev/dotnet/docs/api/class-browsercontext). If you are using the Playwright NUnit or MSTest base classes, you can use the `ContextTest` class to access an `IBrowserContext` instance.

```csharp
await using var app = await ModelDrivenApp.LaunchAsync(this.Context);
```

### Login

You can login to a specific environment and app (using the unique name) as a given user:

```csharp
var homePage = await app.LoginAsync(Configuration.EnvironmentUrl, Configuration.AppName, Configuration.Username, Configuration.Password);
```

This will return a generic `IModelDrivenAppPage`. If you know the type of home page, you can pass the type as a generic type argument:

```csharp
var listPage = await app.LoginAsync<IEntityListPage>(Configuration.EnvironmentUrl, Configuration.AppName, Configuration.Username, Configuration.Password);
```

### Pages

Writing tests with Power Playwright involves interacting with page and control interfaces. Page interfaces model the different pages available within model-driven apps (documented [here](https://learn.microsoft.com/en-us/power-apps/maker/model-driven-apps/create-remove-pages#create-a-page)). These page interfaces provide you with the relevant controls.

At the time of writing, the supported pages are:

- Dataverse table (`IEntityListPage`)
- Dashboard (`IDashboardPage`)
- Custom page (`ICustomPage`)
- Web resource and Navigation link (`IWebResourcePage`)

#### Custom pages

TBA

### Controls

Controls are accessible via properties on the pages. For example, an `IEntityListPage` page exposes a `Grid` property that provides access to the grid control found on these pages:

```csharp
var recordPage = await listPage.Grid.OpenRecordAsync(0);
```

All page interfaces also extend `IModelDrivenAppPage` which provides access to controls common on every page. For example, the site map:

```csharp
var listPage = await homePage.SiteMap.OpenPageAsync<IEntityListPage>("Area", "Group", "Page");
```

#### Control classes

Controls are generally added to forms as control _classes_ rather than specific PCF controls. This means that the controls on your forms may be rendered as different PCF controls over time unless expliclty set.

Power Playwright lets you retrieve form controls by providing an interface for either a control class or a specific control. These can be found in the `PowerPlaywright.Framework.Controls.Pcf.Classes` and `PowerPlaywright.Framework.Controls.Pcf` namespaces respectively.

```csharp
// This subgrid has not had a control explicitly configured on the form.
recordPage.Form.GetControl<IReadOnlyGrid>("subgrid_a");

// This subgrid configured as 'Microsoft.PowerApps.PowerAppsOneGrid' control.
recordPage.Form.GetControl<IPowerAppsOneGridControl>("subgrid_b"); 
```

#### Custom controls

TBA

## Contributing

Please refer to the [CONTRIBUTING.md](./CONTRIBUTING.md)