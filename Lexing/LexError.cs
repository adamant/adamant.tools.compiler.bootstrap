using Adamant.Tools.Compiler.Bootstrap.Core;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Lexing
{
    /// <summary>
    /// Error Code Ranges:
    /// 1001-1999: Lexical Errors
    /// 2001-2999: Parsing Errors
    /// 3001-3999: Type Errors
    /// 4001-4999: Borrow Checking Errors
    /// </summary>
    internal static class LexError
    {
        [NotNull]
        public static Diagnostic UnclosedBlockComment([NotNull] CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Lexing, 1001, "End-of-file found, expected `*/`");
        }

        [NotNull]
        public static Diagnostic UnclosedStringLiteral([NotNull] CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Lexing, 1002, "End-of-file in string constant");
        }

        [NotNull]
        public static Diagnostic InvalidEscapeSequence([NotNull] CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Lexing, 1003, "Unrecognized escape sequence");
        }

        [NotNull]
        public static Diagnostic CStyleNotEquals([NotNull] CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Lexing, 1004, "Use `≠` or `=/=` for not equal instead of `!=`");
        }

        [NotNull]
        public static Diagnostic UnexpectedCharacter([NotNull] CodeFile file, TextSpan span, char character)
        {
            return new Diagnostic(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Lexing, 1005, $"Unexpected character `{character}`");
        }
    }
}
