
# Power Playwright

Power Playwright makes test automation for Power Apps easier by providing a [page object model](https://playwright.dev/dotnet/docs/pom) built on top of [Playwright](https://playwright.dev/dotnet/). It has been designed with the unique challenges of Power Apps test automation in mind:

- Compile once and run any time against any environment version or region
- Extend with your own custom pages and controls
- Generate early-bound page objects specific to your app

## Table of Contents

- [Power Playwright](#power-playwright)
  - [Table of Contents](#table-of-contents)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Usage](#usage)
    - [Login](#login)
    - [Pages and controls](#pages-and-controls)
      - [Custom pages](#custom-pages)
      - [Custom controls](#custom-controls)
    - [Generate app model](#generate-app-model)
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
await using var app = ModelDrivenApp.Launch(this.Context);
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

### Pages and controls

Writing tests with PowerPlaywright involves interacting with page and control interfaces. Page interfaces have been created to model the different types of pages currently available within a model-driven app. These interfaces map onto the pages documented [here](https://learn.microsoft.com/en-us/power-apps/maker/model-driven-apps/create-remove-pages#create-a-page). These provide you with access to the right controls for the page you are on.

At the time of writing, the supported pages are:

- Dataverse table (`IEntityListPage`)
- Dashboard (`IDashboardPage`)
- Custom page (`ICustomPage`)
- Web resource and Navigation link (`IWebResourcePage`)

Control interfaces are accessible via properties on the pages. For example, an `IEntityListPage` page exposes a `Grid` property that provides access to the grid control found on these pages:

```csharp
var recordPage = await listPage.Grid.OpenRecordAsync(0);
```

All page interfacesa also extend `IModelDrivenAppPage` which provides access to controls common on every page. For example, the site map:

```csharp
var listPage = await homePage.SiteMap.OpenPageAsync<IEntityListPage>("Area", "Group", "Page");
```


#### Custom pages

TBA


#### Custom controls

TBA

### Generate app model

TBA

## Contributing

Please refer to the [CONTRIBUTING.md](./CONTRIBUTING.md)