namespace PowerPlaywright.UnitTests;

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

/// <summary>
/// A version of the <see cref="TestCaseAttribute"/> that allows generic type parameters to be passed.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class TestCaseGenericAttribute : TestCaseAttribute, ITestBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestCaseGenericAttribute"/> class.
    /// </summary>
    /// <param name="arguments">The test case arguments.</param>
    public TestCaseGenericAttribute(params object[] arguments)
        : base(arguments)
    {
    }

    /// <summary>
    /// Gets or sets the type arguments.
    /// </summary>
    public Type[]? TypeArguments { get; set; }

    /// <summary>
    /// Builds a single test from the specified method and context.
    /// </summary>
    /// <param name="method">The MethodInfo for which tests are to be constructed.</param>
    /// <param name="suite">The suite to which the tests will be added.</param>
    /// <returns>An enumeration of tests.</returns>
    /// <remarks>
    /// If the method is a generic method definition and the <see cref="TypeArguments"/> property is set, the method is instantiated with the type arguments provided.
    /// If the property is not set or does not have the correct length, the test is marked as not runnable.
    /// </remarks>
    public new IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test? suite)
    {
        if (!method.IsGenericMethodDefinition)
        {
            return base.BuildFrom(method, suite);
        }

        if (this.TypeArguments == null || this.TypeArguments.Length != method.GetGenericArguments().Length)
        {
            var parameters = new TestCaseParameters { RunState = RunState.NotRunnable };
            parameters.Properties.Set(PropertyNames.SkipReason, $"{nameof(this.TypeArguments)} should have {method.GetGenericArguments().Length} elements");
            return [new NUnitTestCaseBuilder().BuildTestMethod(method, suite, parameters)];
        }

        var genMethod = method.MakeGenericMethod(this.TypeArguments);

        return base.BuildFrom(genMethod, suite);
    }
}