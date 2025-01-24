namespace PowerPlaywright.UnitTests.Helpers;

using System.Reflection;
using System.Reflection.Emit;

/// <summary>
/// A builder for dynamic types.
/// </summary>
public class DynamicTypeBuilder
{
    private const string AssemblyName = "TypeHelperAssembly";
    private const string ModuleName = "MainModule";

    private readonly TypeBuilder typeBuilder;

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicTypeBuilder"/> class.
    /// </summary>
    public DynamicTypeBuilder()
    {
        this.typeBuilder = AssemblyBuilder
            .DefineDynamicAssembly(new AssemblyName(AssemblyName), AssemblyBuilderAccess.Run)
            .DefineDynamicModule(ModuleName)
            .DefineType(Guid.NewGuid().ToString(), TypeAttributes.Public);
    }

    /// <summary>
    /// Adds an attribute to the type.
    /// </summary>
    /// <typeparam name="TAttribute">The type of attribute.</typeparam>
    /// <param name="constructorArgs">The constructor args.</param>
    /// <returns>The builder.</returns>
    /// <exception cref="Exception">Thrown if unable to find a constructor.</exception>
    public DynamicTypeBuilder AddAttribute<TAttribute>(object[]? constructorArgs)
    {
        var constructor = constructorArgs is null ?
            typeof(TAttribute).GetConstructor(Type.EmptyTypes)
            :
            typeof(TAttribute).GetConstructor(constructorArgs.Select(a => a.GetType()).ToArray());

        if (constructor is null)
        {
            throw new Exception($"Unable to find a constructor for attribute {typeof(TAttribute).Name} using the provided args");
        }

        this.typeBuilder.SetCustomAttribute(
            new CustomAttributeBuilder(constructor, constructorArgs ?? []));

        return this;
    }

    /// <summary>
    /// Implements an interface on the type.
    /// </summary>
    /// <typeparam name="TInterface">The type of interface.</typeparam>
    /// <returns>The builder.</returns>
    public DynamicTypeBuilder AddInterfaceImplementation<TInterface>()
    {
        return this.AddInterfaceImplementation(typeof(TInterface));
    }

    /// <summary>
    /// Implements an interface on the type.
    /// </summary>
    /// <param name="interfaceType">The type of interface.</param>
    /// <returns>The builder.</returns>
    public DynamicTypeBuilder AddInterfaceImplementation(Type interfaceType)
    {
        if (this.typeBuilder.ImplementedInterfaces.Contains(interfaceType))
        {
            return this;
        }

        this.typeBuilder.AddInterfaceImplementation(interfaceType);

        foreach (var method in interfaceType.GetMethods())
        {
            var methodBuilder = this.typeBuilder.DefineMethod(
                method.Name,
                MethodAttributes.Public | MethodAttributes.Virtual,
                method.ReturnType,
                Array.ConvertAll(method.GetParameters(), p => p.ParameterType));

            var il = methodBuilder.GetILGenerator();

            if (method.ReturnType == typeof(void))
            {
                il.Emit(OpCodes.Ret);
            }
            else if (method.ReturnType.IsValueType)
            {
                var local = il.DeclareLocal(method.ReturnType);
                il.Emit(OpCodes.Ldloca_S, local);
                il.Emit(OpCodes.Initobj, method.ReturnType);
                il.Emit(OpCodes.Ldloc_0);
            }
            else
            {
                il.Emit(OpCodes.Ldnull);
            }

            il.Emit(OpCodes.Ret);

            this.typeBuilder.DefineMethodOverride(methodBuilder, method);
        }

        foreach (var inheritedInterface in interfaceType.GetInterfaces())
        {
            this.AddInterfaceImplementation(inheritedInterface);
        }

        return this;
    }

    /// <summary>
    /// Builds the type.
    /// </summary>
    /// <returns>The type.</returns>
    public Type Build()
    {
        return this.typeBuilder.CreateType();
    }
}