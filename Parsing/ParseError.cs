using System;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
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
    internal static class ParseError
    {
        public static Diagnostic IncompleteDeclaration(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing, 3001, "Incomplete declaration");
        }

        public static Diagnostic UnexpectedToken(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing, 3002, $"Unexpected token `{file.Code[span]}`");
        }

        public static Diagnostic MissingToken(CodeFile file, Type expected, IToken found)
        {
            return new Diagnostic(file, found.Span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing, 3003, $"Expected `{expected.GetFriendlyName()}` found `{found.Text(file.Code)}`");
        }

        public static Diagnostic OwnedNotValidAsGenericLifetimeParameter(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing, 3004, "The lifetime `$owned` cannot be used as a generic lifetime parameter.");
        }

        public static Diagnostic DeclarationNotAllowedInExternal(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing, 3004, "Only function declarations are allowed in external blocks");
        }
    }
}
