# Contributing

This document details how to contribute to the Power Playwright project. Please ensure you have read all of this document before contributing.

## Table of Contents

- [Contributing](#contributing)
  - [Table of Contents](#table-of-contents)
  - [Issues](#issues)
  - [Overview](#overview)
    - [PowerPlaywright](#powerplaywright)
    - [PowerPlaywright.Framework](#powerplaywrightframework)
    - [PowerPlaywright.Strategies](#powerplaywrightstrategies)
  - [How to](#how-to)
    - [Add a PCF control class](#add-a-pcf-control-class)
    - [Add a PCF control](#add-a-pcf-control)
    - [Add a control redirector](#add-a-control-redirector)
    - [Add a PCF control strategy](#add-a-pcf-control-strategy)
    - [Add a platform control](#add-a-platform-control)
    - [Add a platform control strategy](#add-a-platform-control-strategy)
  - [Branching strategy](#branching-strategy)
  - [Versioning](#versioning)
  - [Testing](#testing)
  - [Pull requests](#pull-requests)

## Issues

Please first discuss the change you wish to make via an issue before making a change.

## Overview

Power Playwright is comprised of three separate assemblies.

### PowerPlaywright

The internal implementation of the public API described in **PowerPlaywright.Framework** along with any other internal classes. 

Anything located in here is deemed unlikely to break as a result of Power Platform updates. This means that control implementations are not found in this assembly.

### PowerPlaywright.Framework

The public interfaces that describe the API used by consumers and anything that is a dependency of both the **PowerPlaywright** and **PowerPlaywright.Strategies** assemblies.

### PowerPlaywright.Strategies

The implementations of the interfaces described in **PowerPlaywright.Framework**. Anything that might be impacted by platform updates (e.g., control implementations) will be located here. 

It is planned that, in future, the latest version of this assembly will be dynamically fetched and loaded at runtime before tests execute. This will ensure that tests can be kept in sync with changes in the platform without needing to be recompiled against a later version of Power Playwright.

Any changes here **must** be non-breaking.

## How to

### Add a PCF control class

To add a PCF control class, you must create a new interface under _src\PowerPlaywright.Framework\Controls\Pcf\Classes_ that inherits from `IPcfControl`.

This interface can now be used to describe the functionality provided by all possible incarnations of that control class. For example, it is safe to assume that it will always be possible to set and retrieve the value of lookup controls as a `string`, so we can add those methods to the interface:

```csharp
namespace PowerPlaywright.Framework.Controls.Pcf.Classes
{
    using System.Threading.Tasks;

    /// <summary>
    /// Lookup control class.
    /// </summary>
    public interface ILookup : IPcfControl
    {
        /// <summary>
        /// Sets the value of the lookup.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SetValueAsync(string value);

        /// <summary>
        /// Gets the value of the lookup.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<string> GetValueAsync();
    }
}
```

In order to start interacting with controls within this PCF control class, you must carry out the tasks under [Add a PCF control](#add-a-pcf-control), [Add a control redirector](#add-a-control-redirector), **and** [Add a PCF control strategy](#add-a-pcf-control-strategy).

### Add a PCF control

The first thing to do is to find the name of the control. If you have chosen the control explicitly on the form then you can find the name of the control by looking at the form XML. It will be located within a `controlDescription` element that is linked to the control. In this example, it is _MscrmControls.OptionSet.OptionSetControl_ for all form factors.

```xml
<controlDescription forControl="{0e0eceb4-770f-4101-9797-b3c4a5184edb}">
  <customControl id="{3EF39988-22BB-4F0B-BBBE-64B5A3748AEE}">
    <parameters>
      <datafieldname>pp_choice</datafieldname>
    </parameters>
  </customControl>
  <customControl id="{3EF39988-22BB-4F0B-BBBE-64B5A3748AEE}">
    <parameters>
      <datafieldname>pp_choice</datafieldname>
    </parameters>
  </customControl>
  <customControl formFactor="2" name="MscrmControls.OptionSet.OptionSetControl">
    <parameters>
      <value type="OptionSet">pp_choice</value>
    </parameters>
  </customControl>
  <customControl formFactor="0" name="MscrmControls.OptionSet.OptionSetControl">
    <parameters>
      <value type="OptionSet">pp_choice</value>
    </parameters>
  </customControl>
  <customControl formFactor="1" name="MscrmControls.OptionSet.OptionSetControl">
    <parameters>
      <value type="OptionSet">pp_choice</value>
    </parameters>
  </customControl>
</controlDescription>
```

If the control has been added as a standard control class then it is a little bit trickier. You will need to inspect the page source to find which control is being rendered. Here we can see the name of the control rendered for the class is _PowerApps.CoreControls.OptionSetControl_ based on the `data-lp-id` property.

```html
<div id="id-8c23a7bf-ad9d-4d41-8969-ec723eee709c-2-pp_choice3ef39988-22bb-4f0b-bbbe-64b5a3748aee" role="presentation" class="pa-bx flexbox" data-lp-id="PowerApps.CoreControls.OptionSetControl|pp_choice.fieldControl|pp_record">
```

In many cases, the element that contains the fully qualified name of the control can be found by searching in DevTools with the following selector:

`div[data-lp-id*="fieldControl"]`

In some elements, the control name can be found in the `class` attribute rather than `data-lp-id` attribute. For example, in this case the control name is `MscrmControls.OptionSet.OptionSetControl`:

```html
<div class="customControl MscrmControls OptionSet.OptionSetControl MscrmControls.OptionSet.OptionSetControl" data-id="pp_choice.fieldControl_container" style="width: 100%;" data-lp-id="f99db23a-1e9a-4969-8c05-d466c8a02d28|pp_choice.fieldControl|pp_record">
```

Once the name of the control is determined, create a new interface under _src\PowerPlaywright.Framework\Controls\Pcf_ that inherits from the relevant PCF control class interface. Add the `PcfControl` attribute, passing the name of the control as a parameter. Any interactions that are only applicable to this specific control rather than to all controls of this class can be added to this interface. 

```csharp
namespace PowerPlaywright.Framework.Controls.Pcf
{
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;

    [PcfControl("MscrmControls.FieldControls.SimpleLookupControl")]
    public interface ISimpleLookupControl : ILookup
    {
    }
}
```

To enable this control type to be used when a control for the class is requested, the control class redirector must be created or updated. Continue to [Add a control redirector](#add-a-control-redirector).

### Add a control redirector

Control redirector classes are used to redirect requests for an instance of a PCF control class type (e.g., `ILookup`) to a specific control type (e.g. `ISimpleLookupControl`) based on a combination of organisation, app, and user settings. The specific control type rendered for any given control class is subject to change as a result of Microsoft's platform updates, which means these classes are located in the _PowerPlaywright.Strategies_ project.

Create a new class under _src\PowerPlaywright.Strategies\Redirectors_ that inherits from the abstract `ControlRedirector<TSourceControl>` class, where `TSourceControl` is interface created for the PCF control class. Generate the constructor and method override required by the abstract class and implement the method body. 

```csharp
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
    public LookupRedirector(IRedirectionInfoProvider<RedirectionInfo> infoProvider, ILogger<LookupRedirector> logger = null)
        : base(infoProvider, logger)
    {
    }

    /// <inheritdoc/>
    protected override Type GetTargetControlType(RedirectionInfo redirectionInfo)
    {
        return typeof(ISimpleLookupControl);
    }
}
```

In the above example, there is only one control ever rendered for the control class within the platform (represented by the `ISimpleLookupControl` interface). In this case, the implementation is simple - the `ISimpleLookupControl` interface type is returned immediately. Below is a more complex example in which the `redirectionInfo` object is interrogated before returning the appropriate control type:

```csharp
/// <inheritdoc/>
protected override Type GetTargetControlType(RedirectionInfo redirectionInfo)
{
    if (redirectionInfo.ActiveReleaseChannel == ReleaseChannel.SemiAnnualChannel && !redirectionInfo.IsNewLookEnabled)
    {
        return typeof(IPcfGridControl);
    }

    return typeof(IPowerAppsOneGridControl);
}
```

Now that requests for control class instances are being redirected to the new control type, we can move on to creating the control implementation. Continue to [Add a PCF control strategy](#add-a-pcf-control-strategy).

### Add a PCF control strategy

A control strategy is an implementation of a control interface. More than one control strategy can exist for a given control, meaning that multiple versions of a control can be supported concurrently. This allows tests to run successfully on any environment despite version differences.

For PCF control strategies, the classes are decorated with an attribute that dictates the minimum version of the PCF control that the strategy corresponds to. You can query the Web API to get the version of a control in your environment:

`/api/data/v9.2/customcontrols?$filter=name eq 'MscrmControls.FieldControls.SimpleLookupControl'&$select=name,version` 

Create a new class under _src\PowerPlaywright.Strategies\Controls\Pcf_ that inherits from the `PcfControl` abstract class **and** the interface of the control (not the control class) that you want to implement. Decorate it with the `PcfControlStrategy` attribute and provide the version of the control returned by the API call. In the below example, the control strategy is for version _1.0.470_ of the control and above.

```csharp
/// <summary>
/// A control strategy for the <see cref="ISimpleLookupControl"/>.
/// </summary>
[PcfControlStrategy(1, 0, 470)]
public class SimpleLookupControl : PcfControl, ISimpleLookupControl
{
}
```

Generate a constructor with the parameters required by the abstract class. Additional constructor parameters can be added which can be supplied by the internal service provider. These include:

- `IPageFactory` for controls that can return a new page (e.g. opening a subgrid record)
- `IControlFactory` for controls that can return other controls (e.g. a form control that returns the controls on the form)
- `ILogger<T>` for logging

It is recommended declare `ILocator` fields in the class to capture the elements of the control that are to be interacted with. Instantiate these locators in the constructor of the control. You can use `this.Container.Locator()` to ensure that locators are created within the context of controls root element. If the control has elements that are rendered outside of the root, use `this.Page.Locator()`. Refer to the Playwright [Locators](https://playwright.dev/dotnet/docs/locators) documentation for more info on locators.

You must also provide an implementation for the `GetRoot(ILocator context)` method. This should return a locator which will select the container element of the control.

```csharp
/// <inheritdoc/>
protected override ILocator GetRoot(ILocator context)
{
    return context.Locator($"div[data-lp-id*='MscrmControls.FieldControls.SimpleLookupControl|{this.Name}.fieldControl|']");
}
```

Finally, you can implement the interface methods for the control.

### Add a platform control

A platform control is any control that appears in the user interface that has not been implemented as a PCF control. The process for adding a platform control is similar but not exactly the same. This is due to the fact that platform controls are versioned collectively under the environment version whereas PCF controls are versioned individually. Platform controls also do not havea concept of control class, which means that a redirector is not required.

Create a new interface under _src\PowerPlaywright.Framework\Controls\Platform_ that inherits from the `IPlatformControl` interface, add the `PlatformControl` attribute, and then add the method signatures for the functionality you want to implement for this control. Ensure that the interface methods correspond with interactions that relate to the controls core functions (which are unlikely to change) rather than how it is currently implemented. For example, it will always be possible to open a page from a sitemap by providing the area, group, and page:

```csharp
/// <summary>
/// An interface representing a sitemap.
/// </summary>
[PlatformControl]
public interface ISiteMapControl : IPlatformControl
{
    /// <summary>
    /// Opens a page.
    /// </summary>
    /// <typeparam name="TPage">The type of page to open.</typeparam>
    /// <param name="area">The name of the area.</param>
    /// <param name="group">The name of the group.</param>
    /// <param name="page">The name of the page.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<TPage> OpenPageAsync<TPage>(string area, string group, string page)
        where TPage : IModelDrivenAppPage;
}
```

Once you have created the platform control interface, you can continue on to [Add a platform control strategy](#add-a-platform-control-strategy).

### Add a platform control strategy

Adding a platform control strategy is largely no different to [adding a PCF control strategy](#add-a-pcf-control-strategy). The exceptions are:

- Inherit from `Control` rather than `PcfControl`
- Add the `PlatformControlStrategy` attribute instead of the `PcfControlStrategy` attribute

The version passed to the `PlatformControlStrategy` attribute should relate to the version of the environment that the changes are being tested against. To get the environment version, login to an app and open DevTools. Paste the following script in the console and execute it:

```javascript
Xrm.Utility.getGlobalContext().getVersion();
```

## Branching strategy

We are using a [GitHub Flow](https://githubflow.github.io/) workflow. In short:

- Create a topic branch from master
- Implement changes on your topic branch
- Create a pull request into master
- Address review comments or validation pipeline issues

A new package version will be published when the pull request is merged. 

## Versioning

We use [GitVersion](https://gitversion.net/docs/) to automatically version the NuGet packages within this project based on commit messages. 

GitVersion has been configured to version by commit messages based on the [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/) specification. Please therefore ensure that you are writing commit messages according to this spec.

## Testing

All changes should be covered by automated unit and integration tests. Please ensure that you either update existing tests or write additional tests if required.

Refer to the [README.md](./tests/PowerPlaywright.IntegrationTests/README.md) within the integration test project for information on how to run these tests.

## Pull requests

A maintainer will merge your pull request once it meets all of the required checks. Please ensure that you:

- Update the README.md with details of any new or updated functionality
- Create or update unit and integration tests