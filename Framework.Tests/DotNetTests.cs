using System;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests
{
    public class DotNetTests
    {
        private readonly ITestOutputHelper output;

        public DotNetTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        [Exploratory]
        public void ClosureTypes()
        {
            var lambda1 = GetMethodInfo(x => 1);
            var lambda2 = GetMethodInfo(x => 2);
            output.WriteLine(lambda1.Name);

            output.WriteLine(lambda2.Name);
            Assert.NotEqual(lambda1, lambda2);
        }

        private MethodInfo GetMethodInfo(Func<int, int> f)
        {
            return f.Method;
        }
    }
}
