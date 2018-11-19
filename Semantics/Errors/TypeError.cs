using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
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
        public static Diagnostic NotImplemented([NotNull] CodeFile file, TextSpan span, [NotNull] string message)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 3000, message);
        }

        [NotNull]
        public static Diagnostic OperatorCannotBeAppliedToOperandsOfType(
            [NotNull] CodeFile file,
            TextSpan span,
            [NotNull] BinaryOperator @operator,
            [CanBeNull] DataType leftOperandType,
            [CanBeNull] DataType rightOperandType)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 3001,
                $"Operator `{@operator}` cannot be applied to operands of type `{leftOperandType}` and `{rightOperandType}`.");
        }

        [NotNull]
        public static Diagnostic OperatorCannotBeAppliedToOperandOfType(
            [NotNull] CodeFile file,
            TextSpan span,
            [NotNull] IOperatorToken @operator,
            [CanBeNull] DataType operandType)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 3002,
                $"Operator `{@operator.Text(file.Code)}` cannot be applied to operand of type `{operandType}`.");
        }

        [NotNull]
        public static Diagnostic MustBeATypeExpression([NotNull] CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 3003,
                "Expression must be of type `type` (i.e. it must evaluate to a type)");
        }

        [NotNull]
        public static Diagnostic NameRefersToFunctionNotType([NotNull] CodeFile file, TextSpan span, string name)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 3004,
                $"The name `{name}` refers to a function not a type.");
        }

        [NotNull]
        public static Diagnostic MustBeABoolExpression([NotNull] CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 3005,
                "Expression must be of type `bool`");
        }

        [NotNull]
        public static Diagnostic CircularDefinition([NotNull] CodeFile file, TextSpan span, [NotNull] Name typeDeclarationName)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 3006,
                $"Declaration of type `{typeDeclarationName}` is part of a circular definition");
        }

        [NotNull]
        public static Diagnostic CannotConvert([NotNull] CodeFile file, [NotNull] ExpressionAnalysis expression, [NotNull] DataType type)
        {
            return new Diagnostic(file, expression.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 3007,
                $"Cannot convert expression `{file.Code[expression.Span]}` to type `{type}`");
        }

        [NotNull]
        public static Diagnostic MustBeCallable([NotNull] CodeFile file, [NotNull] ExpressionAnalysis expression)
        {
            return new Diagnostic(file, expression.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 3008,
                $"Expression must be of callable type to be invoked `{file.Code[expression.Span]}`");
        }
    }
}
