using System.IO;
using System.Text;
using Adamant.Tools.Compiler.Bootstrap.Emit.C;

namespace Adamant.Tools.Compiler.Bootstrap.ConformanceTests.Data
{
    public class RuntimeLibraryFixture
    {
        public RuntimeLibraryFixture()
        {
            Directory.CreateDirectory(GetRuntimeDirectory());

            File.WriteAllText(GetRuntimeLibraryPath(), CodeEmitter.RuntimeLibraryCode, Encoding.UTF8);

            File.WriteAllText(GetRuntimeLibraryHeaderPath(), CodeEmitter.RuntimeLibraryHeader, Encoding.UTF8);
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
