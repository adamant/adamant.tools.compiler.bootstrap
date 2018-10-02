using Adamant.Tools.Compiler.Bootstrap.Core.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics
{
    /// <summary>
    /// Error Code Ranges:
    /// 1-999: Lexical Errors
    /// 1000-1999: Parsing Errors
    /// 2000-2999: Type Errors
    /// 3000-3999: Borrow Checking Errors
    /// </summary>
    public static class Error
    {
        public static DiagnosticInfo MissingToken(TokenKind kind)
        {
            return new DiagnosticInfo(DiagnosticLevel.CompilationError, DiagnosticPhase.Parsing, 2, $"Missing {kind} token.");
        }

        // TODO break this into specific errors rather than taking a generic message
        public static DiagnosticInfo LexError(DiagnosticLevel level, string message)
        {
            return new DiagnosticInfo(level, DiagnosticPhase.Lexing, 1, message);
        }

        public static DiagnosticInfo OperatorCannotBeAppliedToOperandsOfType(TokenKind @operator, DataType leftOperandType, DataType rightOperandType)
        {
            return new DiagnosticInfo(DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 2001, $"Operator `{@operator}` cannot be applied to operands of type `{leftOperandType}` and `{rightOperandType}`.");
        }

        public static DiagnosticInfo BorrowedValueDoesNotLiveLongEnough(TextSpan expression)
        {
            return new DiagnosticInfo(DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 3001, $"Borrowed values does not live long enough");
        }
    }
}
