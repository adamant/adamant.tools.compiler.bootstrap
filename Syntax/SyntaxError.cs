using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    /// <summary>
    /// Error Code Ranges:
    /// 1001-1999: Lexical Errors
    /// 2001-2999: Parsing Errors
    /// 3001-3999: Type Errors
    /// 4001-4999: Borrow Checking Errors
    /// </summary>
    public static class SyntaxError
    {
        public static Diagnostic UnclosedBlockComment(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Lexing, 1001, "End-of-file found, expected `*/`");
        }

        public static Diagnostic UnclosedStringLiteral(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Lexing, 1002, "End-of-file in string constant");
        }

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
