using System;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
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
        [NotNull]
        public static Diagnostic IncompleteDeclaration([NotNull] CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing, 3001, "Incomplete declaration");
        }

        [NotNull]
        public static Diagnostic UnexpectedToken([NotNull] CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing, 3002, $"Unexpected token `{file.Code[span]}`");
        }

        [NotNull]
        public static Diagnostic MissingToken([NotNull] CodeFile file, [NotNull] Type expected, [NotNull] IToken found)
        {
            return new Diagnostic(file, found.Span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing, 3003, $"Expected `{expected.GetFriendlyName()}` found `{found.Text(file.Code)}`");
        }

        [NotNull]
        public static Diagnostic OwnedNotValidAsGenericLifetimeParameter([NotNull] CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing, 3004, "The lifetime `$owned` cannot be used as a generic lifetime parameter.");
        }
    }
}
