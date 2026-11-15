using System.Reflection;
using System.Text.Json;
using Consumer.PageObjects;
using Microsoft.Playwright;
using NSubstitute;
using PowerPlaywright.Config;
using PowerPlaywright.Framework;
using PowerPlaywright.Framework.Controls.Pcf.Attributes;
using PowerPlaywright.Framework.Pages;
using Api = PowerPlaywright.Api.PowerPlaywright;

// Exercise the public entry point and built-in strategies without a browser,
// Dataverse credentials, or any real HTTP requests from Playwright.
var context = Substitute.For<IBrowserContext>();
var page = Substitute.For<IPage>();
context.NewPageAsync().Returns(page);
page.Url.Returns("https://example.invalid/main.aspx?pagetype=custom");
page.EvaluateAsync<string>(Arg.Any<string>(), Arg.Any<object>()).Returns("9.2.0.0");
page.EvaluateAsync(Arg.Any<string>(), Arg.Any<object>())
    .Returns((JsonElement?)JsonSerializer.SerializeToElement(new { }));
var response = Substitute.For<IAPIResponse>();
response.Ok.Returns(true);
var controls = typeof(IControlFactory).Assembly.GetTypes()
    .Select(t => t.GetCustomAttribute<PcfControlAttribute>())
    .Where(a => a != null)
    .Select(a => a!.Name).Distinct()
    .Select(name => new { name, version = "999.0.0", customcontrolid = Guid.NewGuid() });
response.JsonAsync().Returns((JsonElement?)JsonSerializer.SerializeToElement(new { value = controls }));
page.APIRequest.GetAsync(Arg.Any<string>(), Arg.Any<APIRequestContextOptions>()).Returns(response);

var api = await Api.CreateAsync(new PowerPlaywrightConfiguration
{
    PageObjectAssemblies = [new() { Path = "PageObjects.dll" }],
});
var home = await api.LaunchAppAsync(context, new Uri("https://example.invalid"), "test", "test", "test");
var dialog = home.AlertDialog;
Check(dialog.GetType().Assembly.GetName().Name == "PowerPlaywright.Strategies", "Built-in control assembly");
var customPage = await dialog.ConfirmAsync<ConsumerPage>();
Check(customPage.CustomControl.Read() == "consumer control", "Registered custom control/page");

var apiAssembly = typeof(Api).Assembly;
var strategies = dialog.GetType().Assembly;
var framework = typeof(IModelDrivenAppPage).Assembly;
foreach (var assembly in new[] { apiAssembly, framework, strategies })
{
    Check(assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion == args[0], "Matching release: " + assembly.FullName);
    Check(Path.GetDirectoryName(assembly.Location) == AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar), "Assembly in application output");
    Check(!assembly.GetReferencedAssemblies().Any(a => a.Name!.StartsWith("NuGet.")), "No runtime NuGet dependency");
    _ = assembly.GetTypes(); // Detect missing interfaces/types from mixed contracts.
}

// The runner adds a compatible contract and implementation to each test release.
var probeType = strategies.GetType("PowerPlaywright.Strategies.PackageTestStrategy", throwOnError: true)!;
var probe = Activator.CreateInstance(probeType);
Check((string)probeType.GetMethod("GetRelease")!.Invoke(probe, null)! == args[0], "Updated implementation and additive contract");

// PlatformReference is internal, so inspect it here to verify the actual asset
// resolver against the packaged assembly, rather than a project reference.
var environment = Substitute.For<IEnvironmentInfoProvider>();
environment.PlatformVersion.Returns(new Version(9, 2, 0, 0));
var referenceType = apiAssembly.GetType("PowerPlaywright.PlatformReference", throwOnError: true)!;
var reference = Activator.CreateInstance(referenceType, environment);
Check((string)referenceType.GetProperty("EntityListPageGridControlName")!.GetValue(reference)! == "entity_control", "Packaged platform-reference.json");
Console.WriteLine("Consumer passed: " + args[0]);

static void Check(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}
