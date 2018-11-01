using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations.Function;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.Literals;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.Operators;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.Types;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Statements;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens.Identifiers;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    public class TypeChecker
    {
        public void CheckDeclarations(
            [NotNull] IList<MemberDeclarationAnalysis> analyses)
        {
            foreach (var analysis in analyses)
            {
                switch (analysis)
                {
                    case FunctionDeclarationAnalysis f:
                        CheckFunction(f, f.Diagnostics);
                        break;
                    case TypeDeclarationAnalysis t:
                        CheckTypeDeclaration(t);
                        break;
                    default:
                        throw NonExhaustiveMatchException.For(analysis);
                }
            }
        }

        private void CheckFunction(
            [NotNull] FunctionDeclarationAnalysis function,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            CheckGenericParameters(function.GenericParameters, diagnostics);
            CheckParameters(function.Parameters, diagnostics);

            function.ReturnType = EvaluateTypeExpression(function.ReturnTypeExpression, diagnostics);
            foreach (var statement in function.Statements)
                CheckStatement(statement, function.Diagnostics);
        }

        private void CheckGenericParameters(
            [NotNull][ItemNotNull] IReadOnlyList<GenericParameterAnalysis> genericParameters,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            foreach (var parameter in genericParameters)
            {
                parameter.Type = parameter.TypeExpression == null ?
                    ObjectType.Type
                    : EvaluateTypeExpression(parameter.TypeExpression, diagnostics);
            }
        }

        private void CheckParameters(
            [NotNull] [ItemNotNull] IReadOnlyList<ParameterAnalysis> parameters,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            foreach (var parameter in parameters)
            {
                if (parameter.TypeExpression != null)
                    parameter.Type = EvaluateTypeExpression(parameter.TypeExpression, diagnostics);
                else
                {
                    diagnostics.Publish(TypeError.NotImplemented(parameter.Context.File,
                        parameter.Syntax.Span, "Self parameters not implemented"));
                    parameter.Type = DataType.Unknown;
                }
            }
        }

        private void CheckStatement(
            [NotNull] StatementAnalysis statement,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            switch (statement)
            {
                case VariableDeclarationStatementAnalysis variableDeclaration:
                    CheckVariableDeclaration(variableDeclaration, diagnostics);
                    break;
                case ExpressionStatementAnalysis expressionStatement:
                    CheckExpression(expressionStatement.Expression, diagnostics);
                    break;
                default:
                    throw NonExhaustiveMatchException.For(statement);
            }
        }

        private void CheckVariableDeclaration(
            [NotNull] VariableDeclarationStatementAnalysis variableDeclaration,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            if (variableDeclaration.TypeExpression != null)
            {
                variableDeclaration.Type = EvaluateTypeExpression(variableDeclaration.TypeExpression, diagnostics);
            }
            else
            {
                diagnostics.Publish(TypeError.NotImplemented(variableDeclaration.Context.File,
                    variableDeclaration.Syntax.Name.Span, "Inference of local variable types not implemented"));
                variableDeclaration.Type = DataType.Unknown;
            }

            if (variableDeclaration.Initializer != null)
                CheckExpression(variableDeclaration.Initializer, diagnostics);
            // TODO check that the initializer type is compatible with the variable type
        }

        private static void CheckTypeDeclaration(
            [NotNull] TypeDeclarationAnalysis typeDeclaration)
        {
            // TODO
        }

        // Checks the expression is well typed, and that the type of the expression is `bool`
        private void CheckExpressionTypeIsBool(
            [CanBeNull] ExpressionAnalysis expression,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            // Omitted types don't need further type checking
            if (expression == null) return;
            CheckExpression(expression, diagnostics);
            if (expression.Type != ObjectType.Bool)
                diagnostics.Publish(TypeError.MustBeABoolExpression(expression.Context.File, expression.Syntax.Span));
        }

        private void CheckExpression(
            [CanBeNull] ExpressionAnalysis expression,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            switch (expression)
            {
                case PrimitiveTypeAnalysis _:
                    expression.Type = ObjectType.Type;
                    break;
                case ReturnExpressionAnalysis returnExpression:
                    if (returnExpression.ReturnExpression != null)
                        CheckExpression(returnExpression.ReturnExpression, diagnostics);
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
                    CheckBinaryOperator(binaryOperatorExpression, diagnostics);
                    break;
                case IdentifierNameAnalysis identifierName:
                    var name = identifierName.Name;
                    if (name == null)
                    {
                        // Missing name, just use unknown
                        // Error should already be emitted
                        expression.Type = DataType.Unknown;
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
                                expression.Type = parameter.Type.AssertNotNull();
                                break;
                            case VariableDeclarationStatementAnalysis variableDeclaration:
                                expression.Type = variableDeclaration.Type.AssertNotNull();
                                break;
                            case GenericParameterAnalysis genericParameter:
                                expression.Type = genericParameter.Type.AssertNotNull();
                                break;
                            case ForeachExpressionAnalysis foreachExpression:
                                expression.Type = foreachExpression.VariableType.AssertNotNull();
                                break;
                            case null:
                                diagnostics.Publish(NameBindingError.CouldNotBindName(expression.Context.File, identifierName.Syntax.Span, name));
                                expression.Type = DataType.Unknown; // unknown
                                break;
                            default:
                                throw NonExhaustiveMatchException.For(declaration);
                        }
                    }
                    break;
                case UnaryOperatorExpressionAnalysis unaryOperatorExpression:
                    CheckUnaryOperator(unaryOperatorExpression, diagnostics);
                    // TODO assign a type to this expression
                    break;
                case LifetimeTypeAnalysis lifetimeType:
                    CheckExpression(lifetimeType.TypeName, diagnostics);
                    if (lifetimeType.TypeName.Type != ObjectType.Type)
                        diagnostics.Publish(TypeError.MustBeATypeExpression(expression.Context.File, lifetimeType.TypeName.Syntax.Span));
                    lifetimeType.Type = ObjectType.Type;
                    break;
                case BlockAnalysis blockExpression:
                    foreach (var statement in blockExpression.Statements)
                        CheckStatement(statement, diagnostics);

                    expression.Type = ObjectType.Void;// TODO assign the correct type to the block
                    break;
                case NewObjectExpressionAnalysis newObjectExpression:
                    foreach (var argument in newObjectExpression.Arguments)
                        CheckArgument(argument, diagnostics);

                    newObjectExpression.Type = EvaluateTypeExpression(newObjectExpression.ConstructorExpression, diagnostics);

                    // TODO verify argument types against called function
                    break;
                case InitStructExpressionAnalysis initStructExpression:
                    foreach (var argument in initStructExpression.Arguments)
                        CheckArgument(argument, diagnostics);

                    // TODO verify argument types against called function

                    initStructExpression.Type = EvaluateTypeExpression(initStructExpression.ConstructorExpression, diagnostics);
                    break;
                case ForeachExpressionAnalysis foreachExpression:
                    foreachExpression.VariableType =
                        EvaluateTypeExpression(foreachExpression.TypeExpression, diagnostics);
                    CheckExpression(foreachExpression.InExpression, diagnostics);

                    // TODO check the break types
                    CheckExpression(foreachExpression.Block, diagnostics);
                    // TODO assign a type to the expression
                    foreachExpression.Type = DataType.Unknown;
                    break;
                case InvocationAnalysis invocation:
                    CheckExpression(invocation.Callee, diagnostics);
                    // TODO the callee needs to be something callable
                    foreach (var argument in invocation.Arguments)
                        CheckExpression(argument.Value, diagnostics);
                    // TODO assign return type
                    invocation.Type = DataType.Unknown;
                    break;
                case GenericInvocationAnalysis genericInvocation:
                    foreach (var argument in genericInvocation.Arguments)
                        CheckExpression(argument.Value, diagnostics);

                    // TODO assign return type
                    genericInvocation.Type = DataType.Unknown;
                    break;
                case GenericNameAnalysis genericName:
                    foreach (var argument in genericName.Arguments)
                        CheckExpression(argument.Value, diagnostics);
                    // TODO check that argument types match function type
                    // TODO assign type
                    break;
                case RefTypeAnalysis refType:
                    refType.Type = EvaluateTypeExpression(refType.ReferencedType, diagnostics);
                    break;
                case UnsafeExpressionAnalysis unsafeExpression:
                    CheckExpression(unsafeExpression.Expression, diagnostics);
                    unsafeExpression.Type = unsafeExpression.Expression.Type;
                    break;
                case MutableTypeAnalysis mutableType:
                    mutableType.Type = EvaluateTypeExpression(mutableType.ReferencedType, diagnostics);// TODO make that type mutable
                    break;
                case IfExpressionAnalysis ifExpression:
                    CheckExpressionTypeIsBool(ifExpression.Condition, diagnostics);
                    CheckExpression(ifExpression.ThenBlock, diagnostics);
                    CheckExpression(ifExpression.ElseClause, diagnostics);
                    // TODO assign a type to the expression
                    ifExpression.Type = DataType.Unknown;
                    break;
                case ResultExpressionAnalysis resultExpression:
                    CheckExpression(resultExpression.Expression, diagnostics);
                    resultExpression.Type = ObjectType.Never;
                    break;
                case null:
                    // Omitted expressions don't need any checking
                    break;
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
        }

        private void CheckArgument(
            [NotNull] ArgumentAnalysis argument,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            CheckExpression(argument.Value, diagnostics);
        }

        private void CheckBinaryOperator(
            [NotNull] BinaryOperatorExpressionAnalysis binaryOperatorExpression,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            CheckExpression(binaryOperatorExpression.LeftOperand, diagnostics);
            var leftOperand = binaryOperatorExpression.LeftOperand.Type;
            var leftOperandCore = leftOperand is LifetimeType l ? l.Type : leftOperand;
            var @operator = binaryOperatorExpression.Syntax.Operator;
            CheckExpression(binaryOperatorExpression.RightOperand, diagnostics);
            var rightOperand = binaryOperatorExpression.RightOperand.Type;
            var rightOperandCore = rightOperand is LifetimeType r ? r.Type : rightOperand;

            // If either is unknown, then we can't know whether there is a a problem
            // (technically not true, for example, we could know that one arg should
            // be a bool and isn't, also we could know the result is a bool)
            if (leftOperand == DataType.Unknown
                || rightOperand == DataType.Unknown)
            {
                binaryOperatorExpression.Type = DataType.Unknown;
                return;
            }

            bool typeError;
            switch (@operator)
            {
                case PlusToken _:
                case AsteriskEqualsToken _:
                    typeError = leftOperand != rightOperand || leftOperand == ObjectType.Bool;
                    if (!typeError)
                        binaryOperatorExpression.Type = leftOperand;
                    break;
                case EqualsEqualsToken _:
                case LessThanToken _:
                case LessThanOrEqualToken _:
                case GreaterThanToken _:
                case GreaterThanOrEqualToken _:
                    typeError = leftOperandCore != rightOperandCore;
                    binaryOperatorExpression.Type = ObjectType.Bool;
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
                case DotDotToken _:
                case DotToken _:
                case CaretDotToken _:
                    // TODO type check this
                    typeError = false;
                    break;
                case DollarToken _:
                case DollarLessThanToken _:
                case DollarLessThanNotEqualToken _:
                case DollarGreaterThanToken _:
                case DollarGreaterThanNotEqualToken _:
                    typeError = leftOperand != ObjectType.Type;
                    break;
                default:
                    throw NonExhaustiveMatchException.For(@operator);
            }
            if (typeError)
                diagnostics.Publish(TypeError.OperatorCannotBeAppliedToOperandsOfType(binaryOperatorExpression.Context.File,
                    binaryOperatorExpression.Syntax.Span, @operator, leftOperand, rightOperand));
        }

        private void CheckUnaryOperator(
            [NotNull] UnaryOperatorExpressionAnalysis unaryOperatorExpression,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            CheckExpression(unaryOperatorExpression.Operand, diagnostics);
            var operand = unaryOperatorExpression.Operand.Type;
            var @operator = unaryOperatorExpression.Syntax.Operator;

            // If either is unknown, then we can't know whether there is a a problem
            // (technically not true, for example, we could know that one arg should
            // be a bool and isn't)
            if (operand == DataType.Unknown)
            {
                unaryOperatorExpression.Type = DataType.Unknown;
                return;
            }

            bool typeError;
            switch (@operator)
            {
                case NotKeywordToken _:
                    typeError = operand != ObjectType.Bool;
                    unaryOperatorExpression.Type = ObjectType.Bool;
                    break;
                case AtSignToken _:
                    typeError = false; // TODO check that the expression can have a pointer taken
                    unaryOperatorExpression.Type = null; // TODO construct a pointer type
                    break;
                case QuestionToken _:
                    typeError = false; // TODO check that the expression can have a pointer taken
                    unaryOperatorExpression.Type = null; // TODO construct a pointer type
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
        [NotNull]
        private DataType EvaluateTypeExpression(
            [CanBeNull] ExpressionAnalysis typeExpression,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            if (typeExpression == null)
            {
                // TODO report error?
                return DataType.Unknown;
            }

            CheckExpression(typeExpression, diagnostics);
            if (typeExpression.Type != ObjectType.Type)
            {
                diagnostics.Publish(TypeError.MustBeATypeExpression(typeExpression.Context.File,
                    typeExpression.Syntax.Span));
                return DataType.Unknown;
            }

            return EvaluateCheckedTypeExpression(typeExpression, diagnostics);
        }

        [NotNull]
        private DataType EvaluateCheckedTypeExpression(
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
                        return DataType.Unknown;
                    }
                    else
                    {
                        var declaration = typeExpression.Context.Scope.Lookup(name);
                        switch (declaration)
                        {
                            case TypeDeclarationAnalysis typeDeclaration:
                                return typeDeclaration.Type.Instance;
                            case FunctionDeclarationAnalysis _: // The name doesn't match the name of a type
                                diagnostics.Publish(TypeError.NameRefersToFunctionNotType(
                                    typeExpression.Context.File, identifier.Syntax.Span, name));
                                return DataType.Unknown;
                            case GenericParameterAnalysis genericParameter:
                                return genericParameter.Type.AssertKnown();
                            case null: // Name not known
                                diagnostics.Publish(NameBindingError.CouldNotBindName(
                                    typeExpression.Context.File, identifier.Syntax.Span, name));
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
                    var type = EvaluateCheckedTypeExpression(lifetimeType.TypeName, diagnostics);
                    if (type == DataType.Unknown) return DataType.Unknown;
                    var lifetimeToken = lifetimeType.Syntax.Lifetime;
                    Lifetime lifetime;
                    switch (lifetimeToken)
                    {
                        case OwnedKeywordToken _:
                            lifetime = OwnedLifetime.Instance;
                            break;
                        case RefKeywordToken _:
                            lifetime = RefLifetime.Instance;
                            break;
                        case IdentifierToken identifier:
                            lifetime = new NamedLifetime(identifier.Value);
                            break;
                        default:
                            throw NonExhaustiveMatchException.For(lifetimeToken);
                    }

                    return new LifetimeType(type.AssertKnown(), lifetime);
                case RefTypeAnalysis refType:
                    return new RefType(refType.VariableBinding,
                        EvaluateCheckedTypeExpression(refType.ReferencedType, diagnostics).AssertKnown());
                case GenericInvocationAnalysis _:
                case GenericNameAnalysis _:
                case BinaryOperatorExpressionAnalysis _:
                case UnaryOperatorExpressionAnalysis _:
                    // TODO evaluate to type
                    return DataType.Unknown;
                case MutableTypeAnalysis mutableType:
                    return EvaluateCheckedTypeExpression(mutableType.ReferencedType, diagnostics); // TODO make the type mutable
                default:
                    throw NonExhaustiveMatchException.For(typeExpression);
            }
        }
    }
}
