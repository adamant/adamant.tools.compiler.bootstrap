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
    public static class BorrowError
    {
        public static Diagnostic BorrowedValueDoesNotLiveLongEnough(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 4001,
                $"Borrowed value does not live long enough");
        }

        public static Diagnostic CantBorrowMutablyWhileBorrowedImmutably(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 4002,
                $"Can't borrow reference mutably while it is immutably borrowed.");
        }

        public static Diagnostic CantBorrowMutablyWhileBorrowedMutably(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 4003,
                $"Can't borrow reference mutably while it is mutably borrowed by something else.");
        }

        public static Diagnostic CantBorrowImmutablyWhileBorrowedMutably(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 4004,
                $"Can't borrow reference immutably while it is mutably borrowed by something else.");
        }
    }
}
