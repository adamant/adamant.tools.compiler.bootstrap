using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    /// <summary>
    /// Error Code Ranges:
    /// 1-999: Lexical Errors
    /// 1000-1999: Parsing Errors
    /// 2000-2999: Type Errors
    /// 3000-3999: Borrow Checking Errors
    /// </summary>
    public static class SemanticError
    {
        public static Diagnostic OperatorCannotBeAppliedToOperandsOfType(CodeFile file, TextSpan span, TokenKind @operator, DataType leftOperandType, DataType rightOperandType)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 2001, $"Operator `{@operator}` cannot be applied to operands of type `{leftOperandType}` and `{rightOperandType}`.");
        }

        public static Diagnostic BorrowedValueDoesNotLiveLongEnough(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 3001, $"Borrowed values does not live long enough");
        }
    }
}
