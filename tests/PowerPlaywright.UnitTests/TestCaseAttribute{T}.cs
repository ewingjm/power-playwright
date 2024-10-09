namespace PowerPlaywright.UnitTests;

/// <summary>
/// A version of the <see cref="TestCaseGenericAttribute"/> that allows generic type parameters to be passed.
/// </summary>
/// <typeparam name="T">The type.</typeparam>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class TestCaseAttribute<T> : TestCaseGenericAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestCaseAttribute{T}"/> class.
    /// </summary>
    /// <param name="arguments">The arguments.</param>
    public TestCaseAttribute(params object[] arguments)
        : base(arguments) => this.TypeArguments = [typeof(T)];
}