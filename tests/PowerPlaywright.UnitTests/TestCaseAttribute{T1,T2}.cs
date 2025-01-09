namespace PowerPlaywright.UnitTests;

/// <summary>
/// A version of the <see cref="TestCaseGenericAttribute"/> that allows generic type parameters to be passed.
/// </summary>
/// <typeparam name="T1">The first type.</typeparam>
/// <typeparam name="T2">The second type.</typeparam>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class TestCaseAttribute<T1, T2> : TestCaseGenericAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestCaseAttribute{T1,T2}"/> class.
    /// </summary>
    /// <param name="arguments">The arguments.</param>
    public TestCaseAttribute(params object[] arguments)
        : base(arguments) => this.TypeArguments = [typeof(T1), typeof(T2)];
}