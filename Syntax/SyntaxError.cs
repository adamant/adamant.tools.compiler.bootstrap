using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public static class SyntaxError
    {
        public static Diagnostic MissingToken(CodeFile file, TextSpan span, TokenKind kind)
        {
            return new Diagnostic(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing, 2, $"Missing {kind} token.");
        }

        // TODO break this into specific errors rather than taking a generic message
        public static Diagnostic LexError(
            CodeFile file,
            TextSpan span,
            DiagnosticLevel level,
            string message)
        {
            return new Diagnostic(file, span, level, DiagnosticPhase.Lexing, 1, message);
        }

        public static Diagnostic SkippedToken(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing, 3002, "Unexpected Token");
        }
    }
}
