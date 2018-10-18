using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
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
        //public static Diagnostic OperatorCannotBeAppliedToOperandsOfType([NotNull] CodeFile file, TextSpan span, [NotNull]  OperatorToken @operator, [NotNull] DataType leftOperandType, [NotNull] DataType rightOperandType)
        //{
        //    return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 3001, $"Operator `{@operator.Text(file.Code)}` cannot be applied to operands of type `{leftOperandType}` and `{rightOperandType}`.");
        //}

        [NotNull]
        public static Diagnostic MustBeATypeExpression([NotNull] CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 3002, "Expression must be of type `type` (i.e. it must evaluate to a type)");
        }
    }
}
