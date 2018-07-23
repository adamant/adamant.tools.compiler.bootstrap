using System.IO;
using System.Text;
using Adamant.Tools.Compiler.Bootstrap.Emit.C;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Data
{
    public class RuntimeLibraryFixture
    {
        public RuntimeLibraryFixture()
        {
            Directory.CreateDirectory(GetRuntimeDirectory());

            File.WriteAllText(GetRuntimeLibraryPath(), CEmitter.RuntimeLibraryCode, Encoding.ASCII);

            File.WriteAllText(GetRuntimeLibraryHeaderPath(), CEmitter.RuntimeLibraryHeader, Encoding.ASCII);
        }

        public static string GetRuntimeDirectory()
        {
            var testsDirectory = TestsDirectory.Get();
            return Path.Combine(testsDirectory, "runtime");
        }
        public static string GetRuntimeLibraryPath()
        {
            return Path.Combine(GetRuntimeDirectory(), CEmitter.RuntimeLibraryCodeFileName);
        }
        public static string GetRuntimeLibraryHeaderPath()
        {
            return Path.Combine(GetRuntimeDirectory(), CEmitter.RuntimeLibraryHeaderFileName);
        }
    }
}
