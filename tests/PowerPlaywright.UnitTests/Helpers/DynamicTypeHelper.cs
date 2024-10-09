namespace PowerPlaywright.UnitTests.Helpers;

using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

/// <summary>
/// Helper used to dynamically create types.
/// </summary>
internal class DynamicTypeHelper
{
    private const string AssemblyName = "TypeHelperAssembly";
    private const string ModuleName = "MainModule";

    private static ModuleBuilder moduleBuilder;

    static DynamicTypeHelper()
    {
        moduleBuilder = AssemblyBuilder
            .DefineDynamicAssembly(new AssemblyName(AssemblyName), AssemblyBuilderAccess.Run)
            .DefineDynamicModule(ModuleName);
    }

    /// <summary>
    /// Creates a unique type.
    /// </summary>
    /// <param name="methodName">The namme of the calling method.</param>
    /// <returns>The type.</returns>
    public Type CreateType([CallerMemberName] string methodName = "")
    {
        return GetTypeBuilder(methodName).CreateType();
    }

    /// <summary>
    /// Creates a type that implements an interface and has an attribute using the provided attribute constructor and constructor args.
    /// </summary>
    /// <param name="interfaceType">The interface type.</param>
    /// <param name="attributeConstructor">The attribute constructor.</param>
    /// <param name="constructorArgs">The attribute constructor args.</param>
    /// <param name="methodName">The namme of the calling method.</param>
    /// <returns>The type.</returns>
    public Type CreateType(Type interfaceType, ConstructorInfo attributeConstructor, object[]? constructorArgs = null, [CallerMemberName] string methodName = "")
    {
        if (!interfaceType.IsInterface)
        {
            throw new ArgumentException("Provided type must be an interface");
        }

        var typeBuilder = GetTypeBuilder(methodName);
        AddInterfaceImplementation(interfaceType, typeBuilder);
        AddAttribute(attributeConstructor, constructorArgs, typeBuilder);

        return typeBuilder.CreateType();
    }

    /// <summary>
    /// Creates a type with an attribute using the provided attribute constructor and constructor args.
    /// </summary>
    /// <param name="attributeConstructor">The attribute constructor.</param>
    /// <param name="constructorArgs">The attribute constructor args.</param>
    /// <param name="methodName">The namme of the calling method.</param>
    /// <returns>The type.</returns>
    public Type CreateType(ConstructorInfo attributeConstructor, object[]? constructorArgs = null, [CallerMemberName] string methodName = "")
    {
        var typeBuilder = GetTypeBuilder(methodName);
        AddAttribute(attributeConstructor, constructorArgs, typeBuilder);

        return typeBuilder.CreateType();
    }

    private static void AddAttribute(ConstructorInfo attributeConstructor, object[]? constructorArgs, TypeBuilder typeBuilder)
    {
        typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(attributeConstructor, constructorArgs ?? []));
    }

    private static void AddInterfaceImplementation(Type interfaceType, TypeBuilder typeBuilder)
    {
        if (typeBuilder.ImplementedInterfaces.Contains(interfaceType))
        {
            return;
        }

        typeBuilder.AddInterfaceImplementation(interfaceType);

        foreach (var method in interfaceType.GetMethods())
        {
            var methodBuilder = typeBuilder.DefineMethod(
                method.Name,
                MethodAttributes.Public | MethodAttributes.Virtual,
                method.ReturnType,
                Array.ConvertAll(method.GetParameters(), p => p.ParameterType));

            var ilGenerator = methodBuilder.GetILGenerator();
            ilGenerator.Emit(OpCodes.Newobj, typeof(NotImplementedException).GetConstructor(Type.EmptyTypes)!);
            ilGenerator.Emit(OpCodes.Throw);

            typeBuilder.DefineMethodOverride(methodBuilder, method);
        }

        foreach (var inheritedInterface in interfaceType.GetInterfaces())
        {
            AddInterfaceImplementation(inheritedInterface, typeBuilder);
        }
    }

    private static TypeBuilder GetTypeBuilder(string methodName)
    {
        return moduleBuilder.DefineType(GenerateTypeName(methodName), TypeAttributes.Public);
    }

    private static string GenerateTypeName(string methodName)
    {
        return $"{methodName}_{Guid.NewGuid():N}";
    }
}