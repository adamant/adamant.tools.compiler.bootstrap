using System;
using System.Reflection;
using System.Threading.Tasks;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit
{
    [UnitTest]
    public class DotNetFrameworkTests
    {
        [NotNull]
        private readonly ITestOutputHelper output;

        public DotNetFrameworkTests([NotNull] ITestOutputHelper output)
        {
            Requires.NotNull(nameof(output), output);
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

        [NotNull]
        private static MethodInfo GetMethodInfo([NotNull] Func<int, int> f)
        {
            return f.Method.NotNull();
        }

        [Fact]
        [Exploratory]
        public void LazyCycleThrowsInvalidOperation()
        {
            Lazy<int> l2 = null;
            var l1 = new Lazy<int>(() => l2.Value);
            l2 = new Lazy<int>(() => l1.Value);
            Assert.Throws<InvalidOperationException>(() => l1.Value);
        }

        [Fact]
        [Exploratory]
        public async Task TaskSelfReferenceDeadlocks()
        {
            var task = Task.FromResult(1);
            task = GetValueAsync(() => task);

            var delayTask = Task.Delay((int)TimeSpan.FromSeconds(2).TotalMilliseconds);
            var completedTask = await Task.WhenAny(task, delayTask);
            Assert.Equal(delayTask, completedTask);
        }

        [Fact]
        [Exploratory]
        public async Task TaskCycleDeadlocks()
        {
            var t2 = Task.FromResult(2);
            var t1 = GetValueAsync(() => t2);
            t2 = GetValueAsync(() => t1);

            var delayTask = Task.Delay((int)TimeSpan.FromSeconds(2).TotalMilliseconds);
            var completedTask = await Task.WhenAny(t1, delayTask);
            Assert.Equal(delayTask, completedTask);
        }

        private static async Task<int> GetValueAsync([NotNull] Func<Task<int>> task)
        {
            await Task.Yield();
            return await task();
        }
    }
}
