using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using UnaryOperator = Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.UnaryOperator;

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
    public static class TypeError
    {
        public static Diagnostic NotImplemented(CodeFile file, TextSpan span, string message)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 3000, message);
        }

        public static Diagnostic OperatorCannotBeAppliedToOperandsOfType(
            CodeFile file,
            TextSpan span,
            BinaryOperator @operator,
            DataType leftOperandType,
            DataType rightOperandType)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 3001,
                $"Operator `{@operator.ToSymbolString()}` cannot be applied to operands of type `{leftOperandType}` and `{rightOperandType}`.");
        }

        public static Diagnostic OperatorCannotBeAppliedToOperandOfType(
            CodeFile file,
            TextSpan span,
            UnaryOperator @operator,
            DataType operandType)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 3002,
                $"Operator `{@operator}` cannot be applied to operand of type `{operandType}`.");
        }

        public static Diagnostic MustBeATypeExpression(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 3003,
                "Expression must be of type `type` (i.e. it must evaluate to a type)");
        }

        public static Diagnostic NameRefersToFunctionNotType(CodeFile file, TextSpan span, string name)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 3004,
                $"The name `{name}` refers to a function not a type.");
        }

        public static Diagnostic MustBeABoolExpression(CodeFile file, TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 3005,
                "Expression must be of type `bool`");
        }

        public static Diagnostic CircularDefinition(CodeFile file, TextSpan span, Name typeDeclarationName)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 3006,
                $"Declaration of type `{typeDeclarationName}` is part of a circular definition");
        }

        public static Diagnostic CannotConvert(CodeFile file, ISyntax expression, DataType ofType, DataType toType)
        {
            return new Diagnostic(file, expression.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 3007,
                $"Cannot convert expression `{file.Code[expression.Span]}` of type `{ofType}` to type `{toType}`");
        }

        public static Diagnostic MustBeCallable(CodeFile file, IExpressionSyntax expression)
        {
            return new Diagnostic(file, expression.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 3008,
                $"Expression must be of callable type to be invoked `{file.Code[expression.Span]}`");
        }

        public static Diagnostic CannotMoveBorrowedValue(CodeFile file, IMoveExpressionSyntax expression)
        {
            return new Diagnostic(file, expression.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 3009,
                $"Cannot move borrowed value `{file.Code[expression.Referent.Span]}`");
        }

        public static Diagnostic TypeDeclaredImmutable(CodeFile file, IExpressionSyntax expression)
        {
            return new Diagnostic(file, expression.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 3010,
                $"Type can't be made mutable because it was declared immutable `{file.Code[expression.Span]}`");
        }

        public static Diagnostic ExpressionCantBeMutable(CodeFile file, IExpressionSyntax expression)
        {
            return new Diagnostic(file, expression.Span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 3011,
                $"Expression can't be made mutable `{file.Code[expression.Span]}`");
        }

        public static Diagnostic ReturnExpressionMustHaveValue(CodeFile file, in TextSpan span, DataType returnType)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 3012,
                $"The return expression must return a value of type `{returnType}`");
        }

        public static Diagnostic CantReturnFromNeverFunction(CodeFile file, in TextSpan span)
        {
            return new Diagnostic(file, span, DiagnosticLevel.FatalCompilationError, DiagnosticPhase.Analysis, 3013,
                $"Functions that never return can't contain return statements");
        }
    }
}
