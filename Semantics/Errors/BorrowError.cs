using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;

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
    public static class BorrowError
    {
        public static Diagnostic SharedValueDoesNotLiveLongEnough(
            CodeFile file,
            TextSpan span,
            SimpleName variable)
        {
            var msg = variable == null ? "Shared value does not live long enough"
                : $"Value shared by `{variable}` does not live long enough";
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 4001, msg);
        }

        public static Diagnostic CantBorrowWhileAliased(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 4002,
                $"Can't borrow from reference while it is aliased.");
        }

        public static Diagnostic CantBorrowWhileBorrowed(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 4003,
                $"Can't borrow from reference while it is borrowed.");
        }

        public static Diagnostic CantAliasWhileBorrowed(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 4004,
                $"Can't alias reference while it is borrowed.");
        }

        public static Diagnostic CantMoveIntoArgumentWhileShared(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 4004,
                $"Can't move ownership into argument while it is shared.");
        }
    }
}
