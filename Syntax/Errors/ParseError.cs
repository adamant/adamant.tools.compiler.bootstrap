using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Errors
{
    /// <summary>
    /// Error Code Ranges:
    /// 1001-1999: Lexical Errors
    /// 2001-2999: Parsing Errors
    /// 3001-3999: Type Errors
    /// 4001-4999: Borrow Checking Errors
    /// </summary>
    internal static class ParseError
    {
        public static Diagnostic SkippedToken(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing, 3002, "Unexpected Token");
        }
    }
}
