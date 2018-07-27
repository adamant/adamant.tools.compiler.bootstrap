using System;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Framework.Tests
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

        [Fact]
        [Exploratory]
        public void LazyCycleThrowsInvalidOperation()
        {
            Lazy<int> l1 = null, l2 = null;
            l1 = new Lazy<int>(() => l2.Value);
            l2 = new Lazy<int>(() => l1.Value);
            Assert.Throws<InvalidOperationException>(() => l1.Value);
        }

        [Fact]
        [Exploratory]
        public async Task TaskCycleDeadlocks()
        {
            Task<int> task = Task.FromResult(1);
            task = GetValueAsync(() => task);

            Task delayTask = Task.Delay((int)TimeSpan.FromSeconds(2).TotalMilliseconds);
            var completedTask = await Task.WhenAny(task, delayTask);
            Assert.Equal(delayTask, completedTask);
        }

        private async Task<int> GetValueAsync(Func<Task<int>> task)
        {
            await Task.Yield();
            return await task();
        }
    }
}
