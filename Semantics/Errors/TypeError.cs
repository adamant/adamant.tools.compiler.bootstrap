using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Errors
{
    /// <summary>
    /// Error Code Ranges:
    /// 1001-1999: Lexical Errors
    /// 2001-2999: Parsing Errors
    /// 3001-3999: Type Errors
    /// 4001-4999: Borrow Checking Errors
    /// </summary>
    public static class TypeError
    {
        [NotNull]
        public static Diagnostic OperatorCannotBeAppliedToOperandsOfType(
            [NotNull] CodeFile file,
            TextSpan span,
            [NotNull] OperatorToken @operator,
            [CanBeNull] DataType leftOperandType,
            [CanBeNull] DataType rightOperandType)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 3001, $"Operator `{@operator.Text(file.Code)}` cannot be applied to operands of type `{leftOperandType}` and `{rightOperandType}`.");
        }

        [NotNull]
        public static Diagnostic OperatorCannotBeAppliedToOperandOfType(
            [NotNull] CodeFile file,
            TextSpan span,
            [NotNull] OperatorToken @operator,
            [CanBeNull] DataType operandType)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 3002, $"Operator `{@operator.Text(file.Code)}` cannot be applied to operand of type `{operandType}`.");
        }

        [NotNull]
        public static Diagnostic MustBeATypeExpression([NotNull] CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 3003, "Expression must be of type `type` (i.e. it must evaluate to a type)");
        }
    }
}
