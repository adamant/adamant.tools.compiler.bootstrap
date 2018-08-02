using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Core.Syntax;

namespace Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics
{
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
            return new DiagnosticInfo(DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 3, $"Operator `{@operator}` cannot be applied to operands of type `{leftOperandType}` and `{rightOperandType}`.");
        }
    }
}
