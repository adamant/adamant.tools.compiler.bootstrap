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
                if (parameter.TypeExpression != null)
                    parameter.Type = EvaluateType(parameter.TypeExpression, diagnostics);
                else
                    diagnostics.Publish(TypeError.NotImplemented(parameter.Context.File,
                        parameter.Syntax.Span, "Self parameters not implemented"));
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
            [CanBeNull] ExpressionAnalysis expression,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            // Omitted types don't need further type checking
            if (expression == null) return;
            CheckTypes(expression, diagnostics);
            if (expression.Type != ObjectType.Type)
                diagnostics.Publish(TypeError.MustBeATypeExpression(expression.Context.File, expression.Syntax.Span));
        }

        private void CheckTypes(
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
                case InitStructExpressionAnalysis initStructExpression:
                    foreach (var argument in initStructExpression.Arguments)
                        CheckTypes(argument, diagnostics);

                    CheckTypeExpression(initStructExpression.ConstructorExpression, diagnostics);
                    initStructExpression.Type = EvaluateType(initStructExpression.ConstructorExpression, diagnostics);

                    // TODO verify argument types against called function
                    break;
                case ForeachExpressionAnalysis foreachExpression:
                    CheckTypes(foreachExpression.InExpression, diagnostics);
                    // TODO check the break types
                    CheckTypes(foreachExpression.Block, diagnostics);
                    break;
                case InvocationAnalysis invocation:
                    CheckTypes(invocation.Callee, diagnostics);
                    // TODO the callee needs to be something callable
                    foreach (var argument in invocation.Arguments)
                        CheckTypeExpression(argument.Value, diagnostics);
                    break;
                case GenericInvocationAnalysis genericInvocation:
                    foreach (var argument in genericInvocation.Arguments)
                        CheckTypeExpression(argument.Value, diagnostics);
                    break;
                case GenericNameAnalysis genericName:
                    foreach (var argument in genericName.Arguments)
                        CheckTypeExpression(argument.Value, diagnostics);
                    break;
                case RefTypeAnalysis refType:
                    CheckTypeExpression(refType.ReferencedType, diagnostics);
                    break;
                case UnsafeExpressionAnalysis unsafeExpression:
                    CheckTypes(unsafeExpression.Expression, diagnostics);
                    unsafeExpression.Type = unsafeExpression.Expression.Type;
                    break;
                case MutableTypeAnalysis mutableType:
                    CheckTypeExpression(mutableType.ReferencedType, diagnostics);
                    mutableType.Type = EvaluateType(mutableType.ReferencedType, diagnostics);// TODO make that type mutable
                    break;
                case null:
                    // Omitted expressions don't need any checking
                    break;
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
        }

        private void CheckTypes(
            [NotNull] ArgumentAnalysis argument,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            CheckTypes(argument.Value, diagnostics);
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
                case AsteriskEqualsToken _:
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
                    if (type == null) return null;
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
                    return new LifetimeType(type, lifetime);
                case RefTypeAnalysis refType:
                    return new RefType(refType.VariableBinding,
                        EvaluateType(refType.ReferencedType, diagnostics));
                case GenericInvocationAnalysis _:
                case GenericNameAnalysis _:
                case BinaryOperatorExpressionAnalysis _:
                case UnaryOperatorExpressionAnalysis _:
                    // TODO evaluate to type
                    return null;
                case MutableTypeAnalysis mutableType:
                    return EvaluateType(mutableType.ReferencedType, diagnostics); // TODO make the type mutable
                case null:
                    // Can't determine type
                    // TODO should this generate an error?
                    return null;
                default:
                    throw NonExhaustiveMatchException.For(typeExpression);
            }
        }
    }
}
