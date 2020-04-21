using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using Adamant.Tools.Compiler.Bootstrap.Emit.C;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Helpers;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Conformance.Helpers
{
    [SuppressMessage("Design", "CA1052:Static holder types should be Static or NotInheritable", Justification = "Instantiated by XUnit using reflection")]
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
            return Path.Combine(SolutionDirectory.Get(), "tests", "runtime");
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
