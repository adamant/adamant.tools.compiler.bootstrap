using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Errors
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
    public static class SemanticError
    {
        public static Diagnostic CantRebindMutableBinding(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 6001,
                $"Variable binding can't rebind previous mutable variable binding");
        }

        public static Diagnostic CantRebindAsMutableBinding(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 6002,
                $"Mutable variable binding can't rebind previous variable binding");
        }

        public static Diagnostic CantShadow(CodeFile file, TextSpan bindingSpan, TextSpan useSpan)
        {
            // TODO that use span needs converted to a line and column
            return new Diagnostic(file, bindingSpan, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 6003,
                $"Variable binding can't shadow. Shadowed binding used at {useSpan}");
        }

        public static Diagnostic CantMove(CodeFile file, ExpressionSyntax expression)
        {
            return new Diagnostic(file, expression.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 6004,
                $"Can't move out of expression");
        }

        public static Diagnostic CantAssignToImmutable(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 6005,
                $"Can't assign to immutable variable");
        }
    }
}
