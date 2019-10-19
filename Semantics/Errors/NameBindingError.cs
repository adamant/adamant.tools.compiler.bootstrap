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
    public static class NameBindingError
    {
        public static Diagnostic CouldNotBindName(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 5001,
                $"The name `{file.Code[span]}` is not defined in this scope.");
        }

        public static Diagnostic AmbiguousName(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 5002,
                $"The name `{file.Code[span]}` is ambiguous.");
        }

        public static Diagnostic CouldNotBindMember(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 5003,
                $"Could not find member `{file.Code[span]}` on object.");
        }

        public static Diagnostic CouldNotBindConstructor(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 5001,
                $"Type doesn't have a constructor with this name and number of arguments.");
        }

        public static Diagnostic AmbiguousConstructorCall(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 5002,
                $"Constructor call is ambiguous.");
        }

        public static Diagnostic CouldNotBindFunction(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 5001,
                $"Could not find function with this name and number of arguments.");
        }

        public static Diagnostic AmbiguousFunctionCall(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 5002,
                $"Function call is ambiguous.");
        }

        public static Diagnostic CouldNotBindMethod(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 5001,
                $"Could not find method with this name and number of arguments.");
        }

        public static Diagnostic AmbiguousMethodCall(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 5002,
                $"Method call is ambiguous.");
        }
    }
}
