using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Helpers;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Parsing.Helpers
{
    public static class AssertExtensions
    {

        public static void AssertParsingDiagnostic([CanBeNull] this Diagnostic diagnostic, int errorCode, int start, int length)
        {
            diagnostic.AssertDiagnostic(DiagnosticPhase.Parsing, errorCode, start, length);
        }
    }
}
