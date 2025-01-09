# PowerPlaywright.IntegrationTests

Integration tests should be written to cover all logic unless it is explicitly stated otherwise. 

As the platform is constantly updated, all Playwright logic (i.e, most of what is in the _PowerPlaywright.Strategies_ library) considered liable to break at any time. The only way to ensure this project is robust is to ensure full automated test coverage and regular execution of these tests against canary environments.

## Table of Contents

- [PowerPlaywright.IntegrationTests](#powerplaywrightintegrationtests)
  - [Table of Contents](#table-of-contents)
  - [Prerequisites](#prerequisites)
    - [Playwright](#playwright)
    - [Environment](#environment)
    - [Solution](#solution)
    - [Application user](#application-user)
    - [Users](#users)
  - [Getting started](#getting-started)
    - [User Interface Demo app](#user-interface-demo-app)
    - [Configuration](#configuration)
    - [Writing tests](#writing-tests)


## Prerequisites

### Playwright

The test project is based on Playwright's documentation. Please ensure you familiarise yourself with NUnit Playwright test projects by reading their [documentation](https://playwright.dev/dotnet/docs/intro) and following the installation step to install the browser(s).

### Environment

You must have a Power Apps environment that you have System Administrator access to. The easiest way to do this at no cost is to create a _Developer_ environment.

It is recommended to toggle _Get new features early_ to _Yes_ when creating your environment. You may need to change your region in order for this to work. The tests are required to pass against all regions, including those that have early opt-in enabled.

>ℹ️ You do not need to run the tests against all regions yourself. This will be handled via the validation pipeline.

### Solution

You must have built the [pp_PowerPlaywright_Test](tests\solution\pp_PowerPlaywright_Test) solution as managed and imported this into the environment above. This contains the _User Interface Demo_ app.

```shell
cd tests/solution/pp_PowerPlaywright_Test
dotnet build -c Release
```

### Application user

You must have an app registration with an associated application user in the above environment that has been granted the _PowerPlaywright Tester_ role. 

### Users

You must have at least one user with the _PowerPlaywright Tester_ role assigned.

## Getting started

### User Interface Demo app

The _pp_PowerPlaywright_Test_ solution introduces the _User Interface Demo_ app. This app is used to provide the tests with all the required permutations of the Power Apps user interface.

If you you need to make changes to this app, please import the solution into a separate environment as unmanaged, make the changes, and extract it back into source control.

### Configuration

You must configure user secrets on the integration test project as follows:

```json
{
  "url": "https://<environment>.<region>.dynamics.com/",
  "users": [
    {
      "username": "<user>@<tenant>.onmicrosoft.com"
      "password": "<password>"
    }
  ],
  "clientId": "<app registration client ID>",
  "clientSecret": "<app registration client secret>"
}
```

You should configure more users here if you wish to increase the parallelism of the tests. Generally, it's recommended to have one user for each worker. The workers can be configured in the [.runsettings](./.runsettings) file (along with Playwright settings).

### Writing tests

Refer to the best practices described [here](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices) by Microsoft when it comes to naming and writing tests.