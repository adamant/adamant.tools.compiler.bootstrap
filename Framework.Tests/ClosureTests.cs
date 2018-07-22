using System;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests
{
    public class ClosureTest
    {
        private readonly ITestOutputHelper output;

        public ClosureTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        [Exploratory]
        public void ClosureType()
        {
            output.WriteLine(Foo(x => x).Name);
        }

        private MethodInfo Foo(Func<int, int> f)
        {
            return f.Method;
        }
    }
}