using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Lexing
{
    /// <summary>
    /// Error Code Ranges:
    /// 1001-1999: Lexical Errors
    /// 2001-2999: Parsing Errors
    /// 3001-3999: Type Errors
    /// 4001-4999: Borrow Checking Errors
    /// 5001-5999: Name Binding Errors
    /// 6001-6999: Other Semantic Errors
    /// </summary>
    internal static class LexError
    {
        public static Diagnostic UnclosedBlockComment(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Lexing, 1001,
                "End-of-file found, expected `*/`");
        }

        public static Diagnostic UnclosedStringLiteral(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Lexing, 1002,
                "End-of-file in string constant");
        }

        public static Diagnostic InvalidEscapeSequence(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Lexing, 1003,
                "Unrecognized escape sequence");
        }

        public static Diagnostic CStyleNotEquals(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Lexing, 1004,
                "Use `≠` or `=/=` for not equal instead of `!=`");
        }

        public static Diagnostic UnexpectedCharacter(CodeFile file, TextSpan span, char character)
        {
            return new Diagnostic(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Lexing, 1005,
                $"Unexpected character `{character}`");
        }

        public static Diagnostic ReservedWord(CodeFile file, TextSpan span, string word)
        {
            return new Diagnostic(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Lexing, 1006,
                $"Reserved word `{word}` used as an identifier");
        }

        public static Diagnostic ContinueInsteadOfNext(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Lexing, 1007,
                $"The word `continue` is a reserved word. Use the `next` keyword or escape the identifier as `\\continue` instead.");
        }

        public static Diagnostic EscapedIdentifierShouldNotBeEscaped(CodeFile file, TextSpan span, string identifier)
        {
            return new Diagnostic(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Lexing, 1008,
                $"The word `{identifier}` is not a keyword or reserved word, it should not be escaped.");
        }

        public static Diagnostic ReservedOperator(CodeFile file, TextSpan span, string op)
        {
            return new Diagnostic(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Lexing, 1009,
                $"Unexpected character(s) `{op}`, reserved for operator or punctuators");
        }
    }
}
