using System.IO;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Emit.C;

namespace Adamant.Tools.Compiler.Bootstrap.ConformanceTests.Data
{
    public class RuntimeLibraryFixture
    {
        public RuntimeLibraryFixture()
        {
            Directory.CreateDirectory(GetRuntimeDirectory());

            // The `CodeFile.Encoding` is UTF-8 without BOM. The default C# one has a BOM.
            File.WriteAllText(GetRuntimeLibraryPath(), CodeEmitter.RuntimeLibraryCode, CodeFile.Encoding);

            File.WriteAllText(GetRuntimeLibraryHeaderPath(), CodeEmitter.RuntimeLibraryHeader, CodeFile.Encoding);
        }

        public static string GetRuntimeDirectory()
        {
            var testsDirectory = LangTestsDirectory.Get();
            return Path.Combine(testsDirectory, "runtime");
        }
        public static string GetRuntimeLibraryPath()
        {
            return Path.Combine(GetRuntimeDirectory(), CodeEmitter.RuntimeLibraryCodeFileName);

        }
        public static string GetRuntimeLibraryHeaderPath()
        {
            return Path.Combine(GetRuntimeDirectory(), CodeEmitter.RuntimeLibraryHeaderFileName);
        }
    }
}
