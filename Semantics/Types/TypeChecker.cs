using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.Literals;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.Operators;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.Types;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Statements;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    public class TypeChecker
    {
        [NotNull] private readonly NameBinder nameBinder;
        [NotNull] private readonly ExpressionAnalysisBuilder expressionAnalysisBuilder;

        public TypeChecker(
            [NotNull] NameBinder nameBinder,
            [NotNull] ExpressionAnalysisBuilder expressionAnalysisBuilder)
        {
            Requires.NotNull(nameof(nameBinder), nameBinder);
            Requires.NotNull(nameof(expressionAnalysisBuilder), expressionAnalysisBuilder);
            this.nameBinder = nameBinder;
            this.expressionAnalysisBuilder = expressionAnalysisBuilder;
        }

        public void CheckTypes(
            [NotNull] IList<DeclarationAnalysis> analyses)
        {
            foreach (var analysis in analyses)
            {
                switch (analysis)
                {
                    case FunctionDeclarationAnalysis f:
                        CheckTypes(f);
                        break;
                    case TypeDeclarationAnalysis t:
                        CheckTypes(t);
                        break;
                    default:
                        throw NonExhaustiveMatchException.For(analysis);
                }
            }
        }

        private void CheckTypes(
            [NotNull] FunctionDeclarationAnalysis function)
        {
            foreach (var parameter in function.Parameters)
            {
                CheckTypeExpression(parameter.TypeExpression, function.Diagnostics);
                parameter.Type = ResolveType(parameter.TypeExpression.Syntax, function.Context.Scope);
            }

            var returnType = function.ReturnTypeExpression;
            CheckTypeExpression(returnType, function.Diagnostics);
            function.ReturnType = ResolveType(returnType.Syntax, function.Context.Scope);
            foreach (var statement in function.Statements)
                CheckTypes(statement, function.Diagnostics);
        }

        private void CheckTypes(
            [NotNull] StatementAnalysis statement,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            switch (statement)
            {
                case VariableDeclarationStatementAnalysis variableDeclaration:
                    CheckTypes(variableDeclaration, diagnostics);
                    break;
                case ExpressionStatementAnalysis expressionStatement:
                    CheckTypes(expressionStatement, diagnostics);
                    break;
                default:
                    throw NonExhaustiveMatchException.For(statement);
            }
        }

        private void CheckTypes(
            [NotNull] VariableDeclarationStatementAnalysis variableDeclaration,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
        }

        private void CheckTypes(
            [NotNull] ExpressionStatementAnalysis expressionStatement,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            CheckTypes(expressionStatement.Expression, diagnostics);
        }

        private static void CheckTypes(
            [NotNull] TypeDeclarationAnalysis typeDeclaration)
        {
            // TODO
        }

        // Checks the expression is well typed, and that the type of the expression is `type`
        private static void CheckTypeExpression(
            [NotNull] ExpressionAnalysis expression,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            CheckTypes(expression, diagnostics);
            if (expression.Type != ObjectType.Type)
                diagnostics.Publish(TypeError.MustBeATypeExpression(expression.Context.File, expression.Syntax.Span));
        }

        private static void CheckTypes(
            [NotNull] ExpressionAnalysis expression,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            switch (expression)
            {
                case PrimitiveTypeAnalysis _:
                    expression.Type = ObjectType.Type;
                    break;
                case ReturnExpressionAnalysis returnExpression:
                    if (returnExpression.ReturnExpression != null)
                        CheckTypes(returnExpression.ReturnExpression, diagnostics);
                    expression.Type = ObjectType.Never;
                    break;
                case IntegerLiteralExpressionAnalysis integerLiteral:
                    // TODO do proper type checking
                    expression.Type = ObjectType.Int;
                    break;
                case BinaryOperatorExpressionAnalysis binaryOperatorExpression:
                    CheckTypes(binaryOperatorExpression, diagnostics);
                    break;
                case IdentifierNameAnalysis identifierName:
                    // TODO lookup correct type
                    expression.Type = ObjectType.Int;
                    break;
                case UnaryOperatorExpressionAnalysis unaryOperatorExpression:
                    CheckTypes(unaryOperatorExpression, diagnostics);
                    break;
                default:
                    throw NonExhaustiveMatchException.For(expression.Syntax);
            }
        }

        private static void CheckTypes(
            [NotNull] BinaryOperatorExpressionAnalysis binaryOperatorExpression,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            CheckTypes(binaryOperatorExpression.LeftOperand, diagnostics);
            var leftOperand = binaryOperatorExpression.LeftOperand.Type;
            var @operator = binaryOperatorExpression.Syntax.Operator;
            CheckTypes(binaryOperatorExpression.RightOperand, diagnostics);
            var rightOperand = binaryOperatorExpression.RightOperand.Type;

            bool typeError;

            switch (@operator)
            {
                case PlusToken _:
                    typeError = leftOperand != rightOperand || leftOperand == ObjectType.Bool;
                    if (!typeError)
                        binaryOperatorExpression.Type = leftOperand;
                    break;
                case EqualsToken _:
                    typeError = leftOperand != rightOperand;
                    if (!typeError)
                        binaryOperatorExpression.Type = leftOperand;
                    break;
                case AndKeywordToken _:
                case OrKeywordToken _:
                case XorKeywordToken _:
                    typeError = leftOperand != ObjectType.Bool || rightOperand != ObjectType.Bool;

                    binaryOperatorExpression.Type = ObjectType.Bool;
                    break;
                default:
                    throw NonExhaustiveMatchException.For(@operator);
            }
            if (typeError)
                diagnostics.Publish(TypeError.OperatorCannotBeAppliedToOperandsOfType(binaryOperatorExpression.Context.File,
                    binaryOperatorExpression.Syntax.Span, @operator, leftOperand, rightOperand));
        }

        private static void CheckTypes(
            [NotNull] UnaryOperatorExpressionAnalysis unaryOperatorExpression,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            CheckTypes(unaryOperatorExpression.Operand, diagnostics);
            var operand = unaryOperatorExpression.Operand.Type;
            var @operator = unaryOperatorExpression.Syntax.Operator;

            bool typeError;

            switch (@operator)
            {
                case NotKeywordToken _:
                    typeError = operand != ObjectType.Bool;
                    unaryOperatorExpression.Type = ObjectType.Bool;
                    break;
                default:
                    throw NonExhaustiveMatchException.For(@operator);
            }
            if (typeError)
                diagnostics.Publish(TypeError.OperatorCannotBeAppliedToOperandOfType(unaryOperatorExpression.Context.File,
                    unaryOperatorExpression.Syntax.Span, @operator, operand));
        }

        [NotNull]
        private static DataType ResolveType(
            [NotNull] ExpressionSyntax typeExpression,
            [NotNull] LexicalScope scope)
        {
            switch (typeExpression)
            {
                case IdentifierNameSyntax identifier:
                    throw new NotImplementedException();
                case PrimitiveTypeSyntax primitive:
                    switch (primitive.Keyword)
                    {
                        case IntKeywordToken _:
                            return ObjectType.Int;
                        case UIntKeywordToken _:
                            return ObjectType.UInt;
                        case ByteKeywordToken _:
                            return ObjectType.Byte;
                        case SizeKeywordToken _:
                            return ObjectType.Size;
                        case VoidKeywordToken _:
                            return ObjectType.Void;
                        case BoolKeywordToken _:
                            return ObjectType.Bool;
                        case StringKeywordToken _:
                            return ObjectType.String;
                        case NeverKeywordToken _:
                            return ObjectType.Never;
                        case TypeKeywordToken _:
                            return ObjectType.Type;
                        default:
                            throw NonExhaustiveMatchException.For(primitive.Keyword);
                    }
                case LifetimeTypeSyntax lifetimeType:
                    return ResolveType(lifetimeType.TypeName, scope);
                default:
                    throw NonExhaustiveMatchException.For(typeExpression);
            }
        }
    }
}
