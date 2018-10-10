using System;
using System.IO;

namespace Adamant.Tools.Compiler.Bootstrap.ConformanceTests.Data
{
    public class RuntimeLibraryFixture
    {
        public RuntimeLibraryFixture()
        {
            Directory.CreateDirectory(GetRuntimeDirectory());

            // TODO File.WriteAllText(GetRuntimeLibraryPath(), CEmitter.RuntimeLibraryCode, Encoding.ASCII);

            // TODO File.WriteAllText(GetRuntimeLibraryHeaderPath(), CEmitter.RuntimeLibraryHeader, Encoding.ASCII);
        }

        public static string GetRuntimeDirectory()
        {
            var testsDirectory = LangTestsDirectory.Get();
            return Path.Combine(testsDirectory, "runtime");
        }
        public static string GetRuntimeLibraryPath()
        {
            // TODO return Path.Combine(GetRuntimeDirectory(), CEmitter.RuntimeLibraryCodeFileName);
            throw new NotImplementedException();
        }
        public static string GetRuntimeLibraryHeaderPath()
        {
            // TODO return Path.Combine(GetRuntimeDirectory(), CEmitter.RuntimeLibraryHeaderFileName);
            throw new NotImplementedException();
        }
    }
}
