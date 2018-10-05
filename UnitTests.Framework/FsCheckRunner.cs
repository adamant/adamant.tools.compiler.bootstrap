using System;
using FsCheck;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using Xunit.Abstractions;

namespace Adamant.Tools.Compiler.Bootstrap.UnitTests.Framework
{
    public class FsCheckRunner : IRunner
    {
        private readonly ITestOutputHelper testOutput;

        public FsCheckRunner(ITestOutputHelper testOutput)
        {
            this.testOutput = testOutput;
        }

        public void OnStartFixture(Type t)
        {
            testOutput.WriteLine(Runner.onStartFixtureToString(t));
        }

        public void OnArguments(int ntest, FSharpList<object> args, FSharpFunc<int, FSharpFunc<FSharpList<object>, string>> every)
        {
            testOutput.WriteLine(every.Invoke(ntest).Invoke(args));
        }

        public void OnShrink(FSharpList<object> args, FSharpFunc<FSharpList<object>, string> everyShrink)
        {
            testOutput.WriteLine(everyShrink.Invoke(args));
        }

        public void OnFinished(string name, TestResult result)
        {
            testOutput.WriteLine(Runner.onFinishedToString(name, result));
        }
    }
}
