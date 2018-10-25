using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
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
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    public class TypeChecker
    {
        [NotNull] private readonly NameBinder nameBinder;

        public TypeChecker([NotNull] NameBinder nameBinder)
        {
            Requires.NotNull(nameof(nameBinder), nameBinder);
            this.nameBinder = nameBinder;
        }

        public void CheckTypes(
            [NotNull] IList<MemberDeclarationAnalysis> analyses)
        {
            foreach (var analysis in analyses)
            {
                switch (analysis)
                {
                    case FunctionDeclarationAnalysis f:
                        CheckTypes(f, f.Diagnostics);
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
            [NotNull] FunctionDeclarationAnalysis function,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            foreach (var parameter in function.Parameters)
            {
                CheckTypeExpression(parameter.TypeExpression, function.Diagnostics);
                parameter.Type = EvaluateType(parameter.TypeExpression, diagnostics);
            }

            var returnType = function.ReturnTypeExpression;
            CheckTypeExpression(returnType, function.Diagnostics);
            function.ReturnType = EvaluateType(returnType, diagnostics);
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
            if (variableDeclaration.TypeExpression != null)
            {
                CheckTypeExpression(variableDeclaration.TypeExpression, diagnostics);
                variableDeclaration.Type = EvaluateType(variableDeclaration.TypeExpression, diagnostics);
            }

            if (variableDeclaration.Initializer != null)
                CheckTypes(variableDeclaration.Initializer, diagnostics);
            // TODO check that the initializer type is compatible with the variable type
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
        private void CheckTypeExpression(
            [NotNull] ExpressionAnalysis expression,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            CheckTypes(expression, diagnostics);
            if (expression.Type != ObjectType.Type)
                diagnostics.Publish(TypeError.MustBeATypeExpression(expression.Context.File, expression.Syntax.Span));
        }

        private void CheckTypes(
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
                case IntegerLiteralExpressionAnalysis _:
                    // TODO do proper type inference
                    expression.Type = ObjectType.Int;
                    break;
                case BooleanLiteralExpressionAnalysis _:
                    expression.Type = ObjectType.Bool;
                    break;
                case BinaryOperatorExpressionAnalysis binaryOperatorExpression:
                    CheckTypes(binaryOperatorExpression, diagnostics);
                    break;
                case IdentifierNameAnalysis identifierName:
                    var name = identifierName.Name;
                    if (name == null)
                    {
                        // Missing name, just use unknown
                        expression.Type = null;
                    }
                    else
                    {
                        var declaration = expression.Context.Scope.Lookup(name);
                        switch (declaration)
                        {
                            case TypeDeclarationAnalysis _:
                                expression.Type = ObjectType.Type;
                                break;
                            case ParameterAnalysis parameter:
                                expression.Type = parameter.Type; // TODO how can we be sure that type is resolved?
                                break;
                            case VariableDeclarationStatementAnalysis variableDeclaration:
                                expression.Type = variableDeclaration.Type; // TODO how can we be sure that type is resolved?
                                break;
                            case null:
                                diagnostics.Publish(NameBindingError.CouldNotBindName(expression.Context.File, identifierName.Syntax.Span, name));
                                expression.Type = null; // unknown
                                break;
                            default:
                                throw NonExhaustiveMatchException.For(declaration);
                        }
                    }
                    break;
                case UnaryOperatorExpressionAnalysis unaryOperatorExpression:
                    CheckTypes(unaryOperatorExpression, diagnostics);
                    // TODO assign a type to this expression
                    break;
                case LifetimeTypeAnalysis lifetimeType:
                    CheckTypes(lifetimeType.TypeName, diagnostics);
                    if (lifetimeType.TypeName.Type != ObjectType.Type)
                        diagnostics.Publish(TypeError.MustBeATypeExpression(expression.Context.File, lifetimeType.TypeName.Syntax.Span));
                    lifetimeType.Type = ObjectType.Type;
                    break;
                case BlockExpressionAnalysis blockExpression:
                    foreach (var statement in blockExpression.Statements)
                        CheckTypes(statement, diagnostics);

                    expression.Type = ObjectType.Void;// TODO assign the correct type to the block
                    break;
                case NewObjectExpressionAnalysis newObjectExpression:
                    foreach (var argument in newObjectExpression.Arguments)
                        CheckTypes(argument, diagnostics);

                    CheckTypeExpression(newObjectExpression.ConstructorExpression, diagnostics);
                    newObjectExpression.Type = EvaluateType(newObjectExpression.ConstructorExpression, diagnostics);

                    // TODO verify argument types against called function
                    break;
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
        }

        private void CheckTypes(
            [NotNull] BinaryOperatorExpressionAnalysis binaryOperatorExpression,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            CheckTypes(binaryOperatorExpression.LeftOperand, diagnostics);
            var leftOperand = binaryOperatorExpression.LeftOperand.Type;
            var leftOperandCore = leftOperand is LifetimeType l ? l.Type : leftOperand;
            var @operator = binaryOperatorExpression.Syntax.Operator;
            CheckTypes(binaryOperatorExpression.RightOperand, diagnostics);
            var rightOperand = binaryOperatorExpression.RightOperand.Type;
            var rightOperandCore = rightOperand is LifetimeType r ? r.Type : rightOperand;

            bool typeError;

            switch (@operator)
            {
                case PlusToken _:
                    typeError = leftOperand != rightOperand || leftOperand == ObjectType.Bool;
                    if (!typeError)
                        binaryOperatorExpression.Type = leftOperand;
                    break;
                case EqualsToken _:
                    typeError = leftOperandCore != rightOperandCore;
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

        private void CheckTypes(
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

        /// <summary>
        /// Evaluates a type expression to the type it identifies
        /// </summary>
        [CanBeNull]
        private static DataType EvaluateType(
            [NotNull] ExpressionAnalysis typeExpression,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            switch (typeExpression)
            {
                case IdentifierNameAnalysis identifier:
                    var name = identifier.Name;
                    if (name == null)
                    {
                        // Missing identifier, error already emitted, just treat as unknown type
                        return null;
                    }
                    else
                    {
                        var declaration = typeExpression.Context.Scope.Lookup(name);
                        switch (declaration)
                        {
                            case TypeDeclarationAnalysis typeDeclaration:
                                return typeDeclaration.Type.Instance;
                            case FunctionDeclarationAnalysis _: // The name doesn't match the name of a type
                                diagnostics.Publish(TypeError.NameRefersToFunctionNotType(typeExpression.Context.File, identifier.Syntax.Span, name));
                                return null;
                            case null: // Name not known
                                diagnostics.Publish(NameBindingError.CouldNotBindName(typeExpression.Context.File, identifier.Syntax.Span, name));
                                return null;
                            default:
                                throw NonExhaustiveMatchException.For(declaration);
                        }
                    }
                case PrimitiveTypeAnalysis primitive:
                    switch (primitive.Syntax.Keyword)
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
                        case MetatypeKeywordToken _:
                            return ObjectType.Metatype;
                        default:
                            throw NonExhaustiveMatchException.For(primitive.Syntax.Keyword);
                    }
                case LifetimeTypeAnalysis lifetimeType:
                    var type = EvaluateType(lifetimeType.TypeName, diagnostics);
                    var lifetimeToken = lifetimeType.Syntax.Lifetime;
                    Lifetime lifetime;
                    switch (lifetimeToken)
                    {
                        case OwnedKeywordToken _:
                            lifetime = OwnedLifetime.Instance;
                            break;
                        case IdentifierToken identifier:
                            lifetime = new NamedLifetime(identifier.Value);
                            break;
                        default:
                            throw NonExhaustiveMatchException.For(lifetimeToken);
                    }
                    return new LifetimeType(type, lifetime);
                default:
                    throw NonExhaustiveMatchException.For(typeExpression);
            }
        }
    }
}
