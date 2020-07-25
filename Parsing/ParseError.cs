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
        /// <summary>
        /// Special temporary error for language features that are not implemented. For that reason
        /// it breaks convention and uses error number 2000
        /// </summary>
        public static Diagnostic NotImplemented(CodeFile file, TextSpan span, string feature)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 2000,
                $"{feature} are not yet implemented");
        }

        public static Diagnostic IncompleteDeclaration(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing, 2001, "Incomplete declaration");
        }

        public static Diagnostic UnexpectedToken(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing, 2002, $"Unexpected token `{file.Code[span]}`");
        }

        public static Diagnostic MissingToken(CodeFile file, Type expected, IToken found)
        {
            return new Diagnostic(file, found.Span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing, 2003, $"Expected `{expected.GetFriendlyName()}` found `{found.Text(file.Code)}`");
        }

        public static Diagnostic DeclarationNotAllowedInExternal(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing, 2004, "Only function declarations are allowed in external blocks");
        }

        public static Diagnostic UnexpectedEndOfExpression(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing, 2005, "Unexpected end of expression");
        }

        public static Diagnostic CantMoveOutOfExpression(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing, 2006,
                "Can't move out of expression. Can only move out of variable.");
        }

        public static Diagnostic ResultStatementInBody(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing, 2007,
                "Result statements can't appear directly in function or method bodies. Must be in block expression.");
        }

        public static Diagnostic ExtraSelfParameter(CodeFile file, in TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing, 2008,
                "There can be only one self parameter to a method.");
        }

        public static Diagnostic SelfParameterMustBeFirst(CodeFile file, in TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing, 2009,
                "Self parameter must be the first parameter.");
        }

        public static Diagnostic CantAssignIntoExpression(CodeFile file, in TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing, 2010,
                "Expression can not appear on the left hand side of an assignment.");
        }
    }
}
