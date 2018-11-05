using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyzers
{
    public class TypeChecker
    {
        public void CheckDeclarations(
            [NotNull] IList<MemberDeclarationAnalysis> analyses)
        {
            foreach (var analysis in analyses)
                switch (analysis)
                {
                    case FunctionDeclarationAnalysis f:
                        CheckFunction(f);
                        break;
                    case TypeDeclarationAnalysis t:
                        CheckTypeDeclaration(t);
                        break;
                    default:
                        throw NonExhaustiveMatchException.For(analysis);
                }
        }

        private void CheckFunction([NotNull] FunctionDeclarationAnalysis function)
        {
            // Check the signature first
            function.Type = DataType.BeingChecked;
            function.ReturnType = DataType.BeingChecked;
            CheckGenericParameters(function.GenericParameters, function.Diagnostics);
            CheckParameters(function.Parameters, function.Diagnostics);

            function.ReturnType = EvaluateTypeExpression(function.ReturnTypeExpression, function.Diagnostics);
            function.Type = new FunctionType(function.Parameters.Select(p => p.Type), function.ReturnType);

            // Now that the signature is done, we can check the body
            foreach (var statement in function.Statements)
                CheckStatement(statement, function.Diagnostics);
        }

        private void CheckGenericParameters(
            [NotNull][ItemNotNull] IReadOnlyList<GenericParameterAnalysis> genericParameters,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            foreach (var parameter in genericParameters)
            {
                parameter.Type = DataType.BeingChecked;
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
                parameter.Type = DataType.BeingChecked;
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
                variableDeclaration.Type =
                    EvaluateTypeExpression(variableDeclaration.TypeExpression, diagnostics);
            else
            {
                diagnostics.Publish(TypeError.NotImplemented(variableDeclaration.Context.File,
                    variableDeclaration.Syntax.Name.Span,
                    "Inference of local variable types not implemented"));
                variableDeclaration.Type = DataType.Unknown;
            }

            if (variableDeclaration.Initializer != null)
                CheckExpression(variableDeclaration.Initializer, diagnostics);
            // TODO check that the initializer type is compatible with the variable type
        }

        /// <summary>
        /// If the type has not been checked, this checks it and returns it.
        /// Also watches for type cycles
        /// </summary>
        [NotNull]
        private DataType GetTypeDeclarationType(
            [NotNull] TypeDeclarationAnalysis typeDeclaration,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            if (typeDeclaration.Type == null)
                CheckTypeDeclaration(typeDeclaration);

            if (typeDeclaration.Type == DataType.BeingChecked)
            {
                diagnostics.Publish(TypeError.CircularDefinition(typeDeclaration.Context.File, typeDeclaration.Syntax.SignatureSpan, typeDeclaration.Name));
                return DataType.Unknown;
            }

            return typeDeclaration.Type.AssertChecked();
        }

        private void CheckTypeDeclaration(
            [NotNull] TypeDeclarationAnalysis typeDeclaration)
        {
            if (typeDeclaration.Type != null)
                return;   // We have already checked it

            typeDeclaration.Type = DataType.BeingChecked;
            var genericParameters = typeDeclaration.GenericParameters;
            CheckGenericParameters(genericParameters, typeDeclaration.Diagnostics);
            var genericParameterTypes = genericParameters.Select(p => p.Type.AssertNotNull());
            switch (typeDeclaration.Syntax)
            {
                case ClassDeclarationSyntax classDeclaration:
                    var classType = new ObjectType(typeDeclaration.Name, true,
                        classDeclaration.Modifiers.Any(m => m is MutableModifierSyntax),
                        genericParameterTypes);
                    typeDeclaration.Type = new Metatype(classType);
                    break;
                case StructDeclarationSyntax structDeclaration:
                    var structType = new ObjectType(typeDeclaration.Name, false,
                        structDeclaration.Modifiers.Any(m => m is MutableModifierSyntax),
                        genericParameterTypes);
                    typeDeclaration.Type = new Metatype(structType);
                    break;
                case EnumStructDeclarationSyntax enumStructDeclaration:
                    var enumStructType = new ObjectType(typeDeclaration.Name, false,
                        enumStructDeclaration.Modifiers.Any(m => m is MutableModifierSyntax),
                        genericParameterTypes);
                    typeDeclaration.Type = new Metatype(enumStructType);
                    break;
                case EnumClassDeclarationSyntax enumStructDeclaration:
                    var enumClassType = new ObjectType(typeDeclaration.Name, true,
                        enumStructDeclaration.Modifiers.Any(m => m is MutableModifierSyntax),
                        genericParameterTypes);
                    typeDeclaration.Type = new Metatype(enumClassType);
                    break;
                case TypeDeclarationSyntax declarationSyntax:
                    var type = new ObjectType(typeDeclaration.Name, true,
                        declarationSyntax.Modifiers.Any(m => m is MutableModifierSyntax),
                        genericParameterTypes);
                    typeDeclaration.Type = new Metatype(type);
                    break;
                default:
                    throw NonExhaustiveMatchException.For(typeDeclaration.Syntax);
            }
        }

        // Checks the expression is well typed, and that the type of the expression is `bool`
        private void CheckExpressionTypeIsBool(
            [NotNull] ExpressionAnalysis expression,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
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
                case PrimitiveTypeAnalysis primitive:
                    expression.Type = new Metatype(GetPrimitiveType(primitive));
                    break;
                case ReturnExpressionAnalysis returnExpression:
                    if (returnExpression.ReturnExpression != null)
                        CheckExpression(returnExpression.ReturnExpression, diagnostics);
                    // TODO check that expression type matches function return type
                    expression.Type = ObjectType.Never;
                    break;
                case IntegerLiteralExpressionAnalysis _:
                    // TODO do proper type inference
                    expression.Type = ObjectType.Int;
                    break;
                case StringLiteralExpressionAnalysis _:
                    // TODO what about interpolated expressions?
                    expression.Type = ObjectType.String;
                    break;
                case BooleanLiteralExpressionAnalysis _:
                    expression.Type = ObjectType.Bool;
                    break;
                case BinaryOperatorExpressionAnalysis binaryOperatorExpression:
                    CheckBinaryOperator(binaryOperatorExpression, diagnostics);
                    break;
                case IdentifierNameAnalysis identifierName:
                    identifierName.Type = CheckName(expression.Context, identifierName.Syntax.Name, diagnostics);
                    break;
                case UnaryOperatorExpressionAnalysis unaryOperatorExpression:
                    CheckUnaryOperator(unaryOperatorExpression, diagnostics);
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
                    // TODO assign correct type to the expression
                    foreachExpression.Type = DataType.Unknown;
                    break;
                case InvocationAnalysis invocation:
                    CheckExpression(invocation.Callee, diagnostics);
                    // TODO the callee needs to be something callable
                    foreach (var argument in invocation.Arguments)
                        CheckExpression(argument.Value, diagnostics);
                    // TODO assign correct return type
                    invocation.Type = DataType.Unknown;
                    break;
                case GenericInvocationAnalysis genericInvocation:
                    CheckExpression(genericInvocation.Callee, diagnostics);
                    var calleeType = genericInvocation.Callee.Type;
                    foreach (var argument in genericInvocation.Arguments)
                        CheckExpression(argument.Value, diagnostics);
                    // TODO check that argument types match function type
                    switch (calleeType)
                    {
                        case Metatype metatype:
                            genericInvocation.Type = metatype.WithGenericArguments(genericInvocation.Arguments.Select(a => a.Value.Type));
                            break;
                        case UnknownType _:
                            genericInvocation.Type = DataType.Unknown;
                            break;
                        default:
                            throw NonExhaustiveMatchException.For(calleeType);
                    }
                    break;
                case GenericNameAnalysis genericName:
                    genericName.NameType = CheckName(genericName.Context, genericName.Syntax.Name, diagnostics);
                    foreach (var argument in genericName.Arguments)
                        CheckExpression(argument.Value, diagnostics);
                    // TODO check that argument types match function type
                    switch (genericName.NameType)
                    {
                        case Metatype metatype:
                            genericName.Type = metatype.WithGenericArguments(genericName.Arguments.Select(a => a.Value.Type));
                            break;
                        case UnknownType _:
                            genericName.Type = DataType.Unknown;
                            break;
                        default:
                            throw NonExhaustiveMatchException.For(genericName.NameType);
                    }
                    break;
                case RefTypeAnalysis refType:
                    EvaluateTypeExpression(refType.ReferencedType, diagnostics);
                    refType.Type = ObjectType.Type;
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

        [NotNull]
        private DataType CheckName(
            [NotNull] AnalysisContext context,
            [CanBeNull] IIdentifierToken name,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            if (name == null)
            {
                // Missing name, just use unknown
                // Error should already be emitted
                return DataType.Unknown;
            }
            else
            {
                var declaration = context.Scope.Lookup(name.Value);
                switch (declaration)
                {
                    case TypeDeclarationAnalysis typeDeclaration:
                        return GetTypeDeclarationType(typeDeclaration, diagnostics);
                    case ParameterAnalysis parameter:
                        return parameter.Type.AssertChecked();
                    case VariableDeclarationStatementAnalysis variableDeclaration:
                        return variableDeclaration.Type.AssertChecked();
                    case GenericParameterAnalysis genericParameter:
                        return genericParameter.Type.AssertChecked();
                    case ForeachExpressionAnalysis foreachExpression:
                        return foreachExpression.VariableType.AssertChecked();
                    case null:
                        diagnostics.Publish(NameBindingError.CouldNotBindName(context.File, name));
                        return DataType.Unknown; // unknown
                    case TypeDeclaration typeDeclaration:
                        return typeDeclaration.Type;
                    default:
                        throw NonExhaustiveMatchException.For(declaration);
                }
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
            if (!(typeExpression.Type is Metatype) && typeExpression.Type != ObjectType.Type)
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
                    var identifierType = identifier.Type;
                    switch (identifierType)
                    {
                        case Metatype metatype:
                            return metatype.Instance;
                        case UnknownType _:
                        case ObjectType t when t == ObjectType.Type: // It is a variable holding a type?
                            return DataType.Unknown;
                        default:
                            throw NonExhaustiveMatchException.For(identifierType);
                    }
                case PrimitiveTypeAnalysis primitive:
                    return GetPrimitiveType(primitive);
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

        [NotNull]
        private static ObjectType GetPrimitiveType([NotNull] PrimitiveTypeAnalysis primitive)
        {
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
        }
    }
}
