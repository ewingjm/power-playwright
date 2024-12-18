namespace PowerPlaywright.UnitTests.Extensions;

using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using NSubstitute;
using PowerPlaywright.Extensions;
using PowerPlaywright.Framework;
using PowerPlaywright.Framework.Redirectors;
using PowerPlaywright.Strategies.Redirectors;
using PowerPlaywright.UnitTests.Helpers;

/// <summary>
/// Tests for the <see cref="ServiceCollectionExtensions"/> class.
/// </summary>
[TestFixture]
public class ServiceCollectionExtensionsTests
{
    private IServiceCollection services;

    /// <summary>
    /// Dummy initializable service.
    /// </summary>
    private interface IService : IAppLoadInitializable
    {
    }

    /// <summary>
    /// Sets up a new <see cref="ServiceCollection"/> before each unit test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this.services = new ServiceCollection();
    }

    /// <summary>
    /// Tests that the <see cref="ServiceCollectionExtensions.AddControlRedirectionInfoProvider(IServiceCollection)"/> method registers an implementation factory that throws a <see cref="PowerPlaywrightException"/> when no type implementing <see cref="IRedirectionInfoProvider{TControlRedirectorInfo}"/> is found in the strategies assembly.
    /// </summary>
    [Test]
    public void AddControlRedirectionInfoProvider_TypeImplementingInterfaceNotFoundInStrategiesAssembly_ThrowsPowerPlaywrightException()
    {
        this.AddAssemblyProvider(typeof(string).Assembly);

        this.services.AddControlRedirectionInfoProvider();

        Assert.Throws<PowerPlaywrightException>(() => this.services.BuildServiceProvider().GetService<IRedirectionInfoProvider<object>>());
    }

    /// <summary>
    /// Tests that the <see cref="ServiceCollectionExtensions.AddControlRedirectionInfoProvider(IServiceCollection)"/> method registers an implementation factory that returns an instance when a type implementing <see cref="IRedirectionInfoProvider{TControlRedirectorInfo}"/> is found in the strategies assembly.
    /// </summary>
    [Test]
    public void AddControlRedirectionInfoProvider_TypeImplementingInterfaceFoundInStrategiesAssembly_ImplementationFactoryReturnsInstance()
    {
        this.AddAssemblyProvider();

        var redirectionInfoProvider = this.services
            .AddControlRedirectionInfoProvider()
            .BuildServiceProvider()
            .GetService<IRedirectionInfoProvider<object>>();

        Assert.That(redirectionInfoProvider, Is.InstanceOf<IRedirectionInfoProvider<RedirectionInfo>>());
    }

    /// <summary>
    /// Tests that the <see cref="ServiceCollectionExtensions.AddControlRedirectionInfoProvider(IServiceCollection)"/> method registers the singleton instance returned by the implementation factory as <see cref="IAppLoadInitializable"/>.
    /// </summary>
    [Test]
    public void AddControlRedirectionInfoProvider_TypeImplementingInterfaceFoundInStrategiesAssembly_RegistersTypeAsAppLoadInitializable()
    {
        this.AddAssemblyProvider();

        var initializable = this.services
            .AddControlRedirectionInfoProvider()
            .BuildServiceProvider()
            .GetService<IAppLoadInitializable>();

        Assert.That(initializable, Is.InstanceOf<IRedirectionInfoProvider<RedirectionInfo>>());
    }

    /// <summary>
    /// Tests that the <see cref="ServiceCollectionExtensions.AddControlRedirectionInfoProvider(IServiceCollection)"/> method registers the singleton instance returned by the implementation factory as both <see cref="IAppLoadInitializable"/> and <see cref="IRedirectionInfoProvider{TControlRedirectorInfo}"/>, ensuring that both interfaces reference the same instance.
    /// </summary>
    [Test]
    public void AddControlRedirectionInfoProvider_TypeImplementingInterfaceFoundInStrategiesAssembly_RegistersTypeAsAppLoadInitializableWithSingletonReferenceToService()
    {
        this.AddAssemblyProvider();

        var serviceProvider = this.services
            .AddControlRedirectionInfoProvider()
            .BuildServiceProvider();

        var firstInstance = serviceProvider.GetService<IAppLoadInitializable>();
        var secondInstance = serviceProvider.GetService<IRedirectionInfoProvider<object>>();

        Assert.That(firstInstance, Is.EqualTo(secondInstance));
    }

    /// <summary>
    /// Tests that the <see cref="ServiceCollectionExtensions.AddControlRedirectionInfoProvider(IServiceCollection)"/> method registers the implementation factory as a singleton.
    /// </summary>
    [Test]
    public void AddControlRedirectionInfoProvider_TypeImplementingInterfaceFoundInStrategiesAssembly_RegistersInstanceAsSingleton()
    {
        this.AddAssemblyProvider();
        var serviceProvider = this.services
            .AddControlRedirectionInfoProvider()
            .BuildServiceProvider();

        var firstInstance = serviceProvider.GetService<IRedirectionInfoProvider<object>>();
        var secondInstance = serviceProvider.GetService<IRedirectionInfoProvider<object>>();

        Assert.That(firstInstance, Is.EqualTo(secondInstance));
    }

    /// <summary>
    /// Tests that the <see cref="ServiceCollectionExtensions.AddAppLoadInitializedSingleton{TService}(IServiceCollection, Func{IServiceProvider, TService})"/> method registers an implementation factory that returns the instance provided by the factory.
    /// </summary>
    [Test]
    public void AddAppLoadInitializedSingleton_ImplementationFactoryProvided_ImplementationFactoryReturnsInstance()
    {
        var instance = new object();

        var returnedInstance = this.services
            .AddAppLoadInitializedSingleton((sp) => instance)
            .BuildServiceProvider()
            .GetService<object>();

        Assert.That(returnedInstance, Is.EqualTo(instance));
    }

    /// <summary>
    /// /// Tests that the <see cref="ServiceCollectionExtensions.AddAppLoadInitializedSingleton{TService}(IServiceCollection, Func{IServiceProvider, TService})"/> method registers the implementation factory as a singleton.
    /// </summary>
    [Test]
    public void AddAppLoadInitializedSingleton_ImplementationFactoryProvided_RegistersInstanceAsSingleton()
    {
        var instance = new object();
        var serviceProvider = this.services
            .AddAppLoadInitializedSingleton((sp) => instance)
            .BuildServiceProvider();

        var firstInstance = serviceProvider.GetService<object>();
        var secondInstance = serviceProvider.GetService<object>();

        Assert.That(firstInstance, Is.EqualTo(secondInstance));
    }

    /// <summary>
    /// Tests that the <see cref="ServiceCollectionExtensions.AddAppLoadInitializedSingleton{TService}(IServiceCollection, Func{IServiceProvider, TService})"/> method registers the type returned by the implementation factory as <see cref="IAppLoadInitializable"/>.
    /// </summary>
    [Test]
    public void AddAppLoadInitializedSingleton_ImplementationFactoryProvided_RegistersTypeAsAppLoadInitializable()
    {
        var instance = Substitute.For<IAppLoadInitializable>();

        var initializable = this.services
            .AddAppLoadInitializedSingleton((sp) => (object)instance)
            .BuildServiceProvider()
            .GetService<IAppLoadInitializable>();

        Assert.That(initializable, Is.EqualTo(instance));
    }

    /// <summary>
    /// Tests that the <see cref="ServiceCollectionExtensions.AddAppLoadInitializedSingleton{TService}(IServiceCollection, Func{IServiceProvider, TService})"/> method registers the type returned by the implementation factory as <see cref="IAppLoadInitializable"/> with a singleton reference to the service type.
    /// </summary>
    [Test]
    public void AddAppLoadInitializedSingleton_ImplementationFactoryProvided_RegistersTypeAsAppLoadInitializableWithSingletonReferenceToService()
    {
        var instance = Substitute.For<IAppLoadInitializable>();
        var serviceProvider = this.services
            .AddAppLoadInitializedSingleton((sp) => (object)instance)
            .BuildServiceProvider();

        var firstInstance = serviceProvider.GetService<object>();
        var secondInstance = serviceProvider.GetService<IAppLoadInitializable>();

        Assert.That(firstInstance, Is.EqualTo(secondInstance));
    }

    /// <summary>
    /// Tests that the <see cref="ServiceCollectionExtensions.AddAppLoadInitializedSingleton{TService, TImplementation}(IServiceCollection)"/> method registers the implementation type as a singleton for the service type.
    /// </summary>
    [Test]
    public void AddAppLoadInitializedSingleton_ServiceAndImplementationTypesProvided_ReturnsInstanceOfImplementationTypeForServiceType()
    {
        var returnedInstance = this.services
            .AddAppLoadInitializedSingleton<IService, Implementation>()
            .BuildServiceProvider()
            .GetService<IService>();

        Assert.That(returnedInstance, Is.InstanceOf<Implementation>());
    }

    /// <summary>
    /// Tests that the <see cref="ServiceCollectionExtensions.AddAppLoadInitializedSingleton{TService, TImplementation}(IServiceCollection)"/> method registers the implementation type as a singleton for the service type.
    /// </summary>
    [Test]
    public void AddAppLoadInitializedSingleton_ServiceAndImplementationTypesProvided_RegistersInstanceAsSingleton()
    {
        var serviceProvider = this.services
            .AddAppLoadInitializedSingleton<IService, Implementation>()
            .BuildServiceProvider();

        var firstInstance = serviceProvider.GetService<IService>();
        var secondInstance = serviceProvider.GetService<IService>();

        Assert.That(firstInstance, Is.EqualTo(secondInstance));
    }

    /// <summary>
    /// Tests that the <see cref="ServiceCollectionExtensions.AddAppLoadInitializedSingleton{TService, TImplementation}(IServiceCollection)"/> method registers the implementation type as <see cref="IAppLoadInitializable"/> when the service and implementation types are provided.
    /// </summary>
    [Test]
    public void AddAppLoadInitializedSingleton_ServiceAndImplementationTypesProvided_RegistersTypeAsAppLoadInitializable()
    {
        var instance = this.services
            .AddAppLoadInitializedSingleton<IService, Implementation>()
            .BuildServiceProvider()
            .GetService<IAppLoadInitializable>();

        Assert.That(instance, Is.InstanceOf<Implementation>());
    }

    /// <summary>
    /// Tests that the <see cref="ServiceCollectionExtensions.AddAppLoadInitializedSingleton{TService, TImplementation}(IServiceCollection)"/> method registers the implementation type as <see cref="IAppLoadInitializable"/> with a singleton reference to the service type when the service and implementation types are provided.
    /// </summary>
    [Test]
    public void AddAppLoadInitializedSingleton_ServiceAndImplementationTypesProvided_RegistersTypeAsAppLoadInitializableWithSingletonReferenceToService()
    {
        var serviceProvider = this.services
            .AddAppLoadInitializedSingleton<IService, Implementation>()
            .BuildServiceProvider();

        var firstInstance = serviceProvider.GetService<IService>();
        var secondInstance = serviceProvider.GetService<IAppLoadInitializable>();

        Assert.That(firstInstance, Is.EqualTo(secondInstance));
    }

    private static Type GetRedirectionInfoProviderType()
    {
        return new DynamicTypeBuilder()
            .AddInterfaceImplementation<IRedirectionInfoProvider<RedirectionInfo>>()
            .AddInterfaceImplementation<IAppLoadInitializable>()
            .Build();
    }

    private void AddAssemblyProvider(Assembly? assembly = null)
    {
        var assemblyProvider = Substitute.For<IAssemblyProvider>();

        assemblyProvider.GetAssembly().Returns(assembly ?? GetRedirectionInfoProviderType().Assembly);

        this.services.AddSingleton(assemblyProvider);
    }

    /// <summary>
    /// Dummy implementation of <see cref="IService"/>.
    /// </summary>
    private class Implementation : IService
    {
        public Task InitializeAsync(IPage page)
        {
            throw new NotImplementedException();
        }
    }
}