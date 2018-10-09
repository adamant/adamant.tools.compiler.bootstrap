using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    /// <summary>
    /// Error Code Ranges:
    /// 1001-1999: Lexical Errors
    /// 2001-2999: Parsing Errors
    /// 3001-3999: Type Errors
    /// 4001-4999: Borrow Checking Errors
    /// </summary>
    public static class SemanticError
    {
        public static Diagnostic OperatorCannotBeAppliedToOperandsOfType(CodeFile file, TextSpan span, OperatorToken @operator, DataType leftOperandType, DataType rightOperandType)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 3001, $"Operator `{@operator.Text(file.Code)}` cannot be applied to operands of type `{leftOperandType}` and `{rightOperandType}`.");
        }

        public static Diagnostic BorrowedValueDoesNotLiveLongEnough(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 4001, $"Borrowed values does not live long enough");
        }
    }
}
