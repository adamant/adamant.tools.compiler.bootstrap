using System;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyzers
{
    public class ExpressionTypeResolver
    {
        [NotNull] private readonly CodeFile file;
        [CanBeNull] private readonly DataType returnType;
        [NotNull] private readonly Diagnostics diagnostics;

        public ExpressionTypeResolver(
            [NotNull] CodeFile file,
            [NotNull] Diagnostics diagnostics,
            [CanBeNull] DataType returnType = null)
        {
            this.file = file;
            this.returnType = returnType;
            this.diagnostics = diagnostics;
        }

        public void InferStatementType([NotNull] StatementSyntax statement)
        {
            switch (statement)
            {
                case VariableDeclarationStatementSyntax variableDeclaration:
                    ResolveVariableDeclarationType(variableDeclaration);
                    break;
                case ExpressionSyntax expression:
                    InferExpressionType(expression);
                    break;
                default:
                    throw NonExhaustiveMatchException.For(statement);
            }
        }

        private void ResolveVariableDeclarationType(
            [NotNull] VariableDeclarationStatementSyntax variableDeclaration)
        {
            variableDeclaration.Type.BeginFulfilling();
            if (variableDeclaration.Initializer != null)
                InferExpressionType(variableDeclaration.Initializer);

            if (variableDeclaration.TypeExpression != null)
            {
                var type = CheckAndEvaluateTypeExpression(variableDeclaration.TypeExpression);
                variableDeclaration.Type.Fulfill(type);
                if (variableDeclaration.Initializer != null)
                    ImposeIntegerConstantType(type, variableDeclaration.Initializer);
                // TODO check that the initializer type is compatible with the variable type
            }
            else if (variableDeclaration.Initializer != null)
            {
                // We'll assume the expression type is it
                variableDeclaration.Type.Fulfill(variableDeclaration.Initializer.Type.Fulfilled());
            }
            else
            {
                diagnostics.Add(TypeError.NotImplemented(file,
                    variableDeclaration.NameSpan,
                    "Inference of local variable types not implemented"));
                variableDeclaration.Type.Fulfill(DataType.Unknown);
            }
        }

        // Checks the expression is well typed, and that the type of the expression is `bool`
        private void CheckExpressionTypeIsBool([NotNull] ExpressionSyntax expression)
        {
            InferExpressionType(expression);
            if (expression.Type.Fulfilled() != ObjectType.Bool)
                diagnostics.Add(TypeError.MustBeABoolExpression(file, expression.Span));
        }

        [NotNull]
        public DataType CheckExpressionType(
            [NotNull] ExpressionSyntax expression,
            [NotNull] DataType expectedType)
        {
            var actualType = InferExpressionType(expression);
            // TODO check for type compatibility not equality
            if (!expectedType.Equals(actualType))
                diagnostics.Add(TypeError.CannotConvert(file, expression, expectedType));
            return actualType;
        }

        [NotNull]
        public DataType InferExpressionType([CanBeNull] ExpressionSyntax expression)
        {
            if (expression == null) return DataType.Unknown;

            expression.Type.BeginFulfilling();
            switch (expression)
            {
                case PrimitiveTypeSyntax primitive:
                    return expression.Type.Fulfill(new Metatype(GetPrimitiveType(primitive)));
                case ReturnExpressionSyntax returnExpression:
                    if (returnExpression.ReturnValue != null)
                    {
                        InferExpressionType(returnExpression.ReturnValue);
                        if (returnType != null) // TODO report an error?
                        {
                            ImposeIntegerConstantType(returnType, returnExpression.ReturnValue);
                            if (returnType != returnExpression.ReturnValue.Type.Fulfilled())
                                diagnostics.Add(TypeError.CannotConvert(file,
                                    returnExpression.ReturnValue, returnType));
                        }
                    }
                    else
                    {
                        // TODO a void or never function shouldn't have this
                    }
                    return expression.Type.Fulfill(ObjectType.Never);
                case LiteralExpressionSyntax literalExpression:
                    switch (literalExpression.Literal)
                    {
                        case IIntegerLiteralToken _:
                            return expression.Type.Fulfill(DataType.IntegerConstant);
                        case IStringLiteralToken _:
                            throw new NotImplementedException();
                        case IBooleanLiteralToken _:
                            return expression.Type.Fulfill(ObjectType.Bool);
                        default:
                            throw NonExhaustiveMatchException.For(literalExpression.Literal);
                    }
                case BinaryExpressionSyntax binaryOperatorExpression:
                    return InferBinaryExpressionType(binaryOperatorExpression);
                case IdentifierNameSyntax identifierName:
                    return identifierName.Type.Fulfill(identifierName.ReferencedSymbol.NotNull().Type);
                case UnaryExpressionSyntax unaryOperatorExpression:
                    return InferUnaryExpressionType(unaryOperatorExpression);
                case LifetimeTypeSyntax lifetimeType:
                    InferExpressionType(lifetimeType.TypeExpression);
                    if (lifetimeType.TypeExpression.Type.Fulfilled() != ObjectType.Type)
                        diagnostics.Add(TypeError.MustBeATypeExpression(file, lifetimeType.TypeExpression.Span));
                    return expression.Type.Fulfill(ObjectType.Type);
                case BlockSyntax blockExpression:
                    foreach (var statement in blockExpression.Statements)
                        InferStatementType(statement);

                    return expression.Type.Fulfill(ObjectType.Void);// TODO assign the correct type to the block
                case NewObjectExpressionSyntax newObjectExpression:
                    foreach (var argument in newObjectExpression.Arguments)
                        CheckArgument(argument);
                    // TODO verify argument types against called function
                    return expression.Type.Fulfill(CheckAndEvaluateTypeExpression(newObjectExpression.Constructor));
                case PlacementInitExpressionSyntax placementInitExpression:
                    foreach (var argument in placementInitExpression.Arguments)
                        CheckArgument(argument);

                    // TODO verify argument types against called function

                    return placementInitExpression.Type.Fulfill(CheckAndEvaluateTypeExpression(placementInitExpression.Initializer));
                case ForeachExpressionSyntax foreachExpression:
                    foreachExpression.Type.Fulfill(
                        CheckAndEvaluateTypeExpression(foreachExpression.TypeExpression));
                    InferExpressionType(foreachExpression.InExpression);

                    // TODO check the break types
                    InferExpressionType(foreachExpression.Block);
                    // TODO assign correct type to the expression
                    return expression.Type.Fulfill(DataType.Unknown);
                case WhileExpressionSyntax whileExpression:
                    CheckExpressionTypeIsBool(whileExpression.Condition);
                    InferExpressionType(whileExpression.Block);
                    // TODO assign correct type to the expression
                    return expression.Type.Fulfill(DataType.Unknown);
                case LoopExpressionSyntax loopExpression:
                    InferExpressionType(loopExpression.Block);
                    // TODO assign correct type to the expression
                    return expression.Type.Fulfill(DataType.Unknown);
                case InvocationSyntax invocation:
                {
                    var callee = InferExpressionType(invocation.Callee);
                    foreach (var argument in invocation.Arguments) InferExpressionType(argument.Value);
                    if (callee is FunctionType functionType)
                    {
                        // TODO check argument types
                        return expression.Type.Fulfill(functionType.ResultType);
                    }

                    diagnostics.Add(TypeError.MustBeCallable(file, invocation.Callee));
                    return expression.Type.Fulfill(DataType.Unknown);
                }
                case GenericsInvocationSyntax genericInvocation:
                {
                    foreach (var argument in genericInvocation.Arguments)
                        InferExpressionType(argument.Value);

                    InferExpressionType(genericInvocation.Callee);
                    var calleeType = genericInvocation.Callee.Type.Fulfilled();
                    if (calleeType is OverloadedType overloadedType)
                    {
                        genericInvocation.Callee.Type.Resolve(calleeType = overloadedType.Types
                            .OfType<GenericType>()
                            .Single(t => t.GenericArity == genericInvocation.Arity)
                            .NotNull());
                    }

                    // TODO check that argument types match function type
                    switch (calleeType)
                    {
                        // TODO implement
                        //case Metatype metatype:
                        //    genericInvocation.Type.Computed(
                        //        metatype.WithGenericArguments(
                        //            genericInvocation.Arguments.Select(a => a.Value.Type.AssertComputed())));
                        //    break;
                        case MetaFunctionType metaFunctionType:
                            return genericInvocation.Type.Fulfill(metaFunctionType.ResultType);
                        case UnknownType _:
                            return genericInvocation.Type.Fulfill(DataType.Unknown);
                        default:
                            throw NonExhaustiveMatchException.For(calleeType);
                    }
                }
                case GenericNameSyntax genericName:
                {
                    foreach (var argument in genericName.Arguments)
                        InferExpressionType(argument.Value);

                    genericName.NameType.BeginFulfilling();
                    var nameType = InferNameType(genericName.Name, genericName.Span);
                    if (nameType is OverloadedType overloadedType)
                    {
                        nameType = overloadedType.Types.OfType<GenericType>()
                            .Single(t => t.GenericArity == genericName.Arity).NotNull();
                    }

                    // TODO check that argument types match function type
                    genericName.NameType.Fulfill(nameType);

                    switch (nameType)
                    {
                        // TODO implement
                        //case Metatype metatype:
                        //    genericName.Type.Computed(
                        //        metatype.WithGenericArguments(
                        //            genericName.Arguments.Select(a => a.Value.Type.AssertComputed())));
                        //    break;
                        case UnknownType _:
                            return genericName.Type.Fulfill(DataType.Unknown);
                        default:
                            throw NonExhaustiveMatchException.For(genericName.NameType);
                    }
                }
                case RefTypeSyntax refType:
                    CheckAndEvaluateTypeExpression(refType.ReferencedType);
                    return refType.Type.Fulfill(ObjectType.Type);
                case UnsafeExpressionSyntax unsafeExpression:
                    InferExpressionType(unsafeExpression.Expression);
                    return unsafeExpression.Type.Fulfill(unsafeExpression.Expression.Type.Fulfilled());
                case MutableTypeSyntax mutableType:
                    return mutableType.Type.Fulfill(CheckAndEvaluateTypeExpression(mutableType.ReferencedTypeExpression));// TODO make that type mutable
                case IfExpressionSyntax ifExpression:
                    CheckExpressionTypeIsBool(ifExpression.Condition);
                    InferExpressionType(ifExpression.ThenBlock);
                    InferExpressionType(ifExpression.ElseClause);
                    // TODO assign a type to the expression
                    return ifExpression.Type.Fulfill(DataType.Unknown);
                case ResultExpressionSyntax resultExpression:
                    InferExpressionType(resultExpression.Expression);
                    return resultExpression.Type.Fulfill(ObjectType.Never);
                //                case UninitializedExpressionSyntax uninitializedExpression:
                //                    // TODO assign a type to the expression
                //                    return uninitializedExpression.Type.Computed(DataType.Unknown);
                case MemberAccessExpressionSyntax memberAccess:
                    return InferMemberAccessType(memberAccess);
                case BreakExpressionSyntax breakExpression:
                    InferExpressionType(breakExpression.Value);
                    return breakExpression.Type.Fulfill(ObjectType.Never);
                case AssignmentExpressionSyntax assignmentExpression:
                    var left = InferExpressionType(assignmentExpression.LeftOperand);
                    InferExpressionType(assignmentExpression.RightOperand);
                    var right = ImposeIntegerConstantType(left, assignmentExpression.RightOperand);
                    throw new NotImplementedException("Check compability of types");
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
        }

        [NotNull]
        private DataType InferMemberAccessType([NotNull] MemberAccessExpressionSyntax memberAccess)
        {
            var left = InferExpressionType(memberAccess.Expression);
            ISymbol symbol;
            switch (left)
            {
                case UnknownType _:
                    return DataType.Unknown;
                // TODO data types need to be connected to symbols so we can do this lookup
                //    case ObjectType objectType:
                //        symbol = memberAccess.Expression.Context.Scope.LookupGlobal(objectType.Name);
                //        break;
                //    case PrimitiveFixedIntegerType fixedInteger:
                //        symbol = memberAccess.Expression.Context.Scope.LookupGlobal(fixedInteger.Name);
                //        break;
                default:
                    throw NonExhaustiveMatchException.For(left);
            }

            if (symbol == null) return DataType.Unknown;

            DataType type;
            switch (memberAccess.Member)
            {
                case IIdentifierToken identifier:
                    type = symbol.Lookup(new SimpleName(identifier.Value))?.Type;
                    break;
                default:
                    throw NonExhaustiveMatchException.For(memberAccess.Member);
            }
            return memberAccess.Type.Fulfill(type ?? DataType.Unknown);
        }

        [NotNull]
        private DataType InferNameType(
            //[NotNull] AnalysisContext context,
            [NotNull] SimpleName name,
            TextSpan span)
        {
            //var declaration = context.Scope.Lookup(name);
            //switch (declaration)
            //{
            //    case TypeDeclarationSyntax typeDeclaration:
            //        DeclarationTypeChecker.CheckTypeDeclaration(typeDeclaration);
            //        return typeDeclaration.Type.Fulfilled();
            //    case ParameterSyntax parameter:
            //        return parameter.Type.Fulfilled();
            //    case VariableDeclarationStatementSyntax variableDeclaration:
            //        return variableDeclaration.Type.Fulfilled();
            //    case GenericParameterSyntax genericParameter:
            //        return genericParameter.Type.Fulfilled();
            //    case ForeachExpressionSyntax foreachExpression:
            //        return foreachExpression.VariableType.AssertComputed();
            //    case FunctionDeclarationSyntax functionDeclaration:
            //        return functionDeclaration.Type.AssertComputed();
            //    case null:
            //        diagnostics.Add(NameBindingError.CouldNotBindName(context.File, span));
            //        return DataType.Unknown; // unknown
            //    case TypeDeclaration typeDeclaration:
            //        return typeDeclaration.Type.AssertResolved();
            //    case CompositeSymbol composite:
            //        foreach (var typeDeclaration in composite.Symbols.OfType<TypeDeclarationSyntax>())
            //        {
            //            TypeChecker.CheckTypeDeclaration(typeDeclaration);
            //            typeDeclaration.Type.Fulfilled();
            //        }
            //        return new OverloadedType(composite.Symbols.SelectMany(s => s.Types));
            //    default:
            //        throw NonExhaustiveMatchException.For(declaration);
            //}
            throw new NotImplementedException();
        }

        private void CheckArgument(
            [NotNull] ArgumentSyntax argument)
        {
            //            InferExpressionType(argument.Value);
            throw new NotImplementedException();
        }

        [NotNull]
        private DataType InferBinaryExpressionType(
            [NotNull] BinaryExpressionSyntax binaryExpression)
        {
            InferExpressionType(binaryExpression.LeftOperand);
            var leftOperand = binaryExpression.LeftOperand.Type.Fulfilled();
            var leftOperandCore = leftOperand is LifetimeType l ? l.Referent : leftOperand;
            var @operator = binaryExpression.Operator;
            InferExpressionType(binaryExpression.RightOperand);
            var rightOperand = binaryExpression.RightOperand.Type.Fulfilled();
            var rightOperandCore = rightOperand is LifetimeType r ? r.Referent : rightOperand;

            // If either is unknown, then we can't know whether there is a a problem
            // (technically not true, for example, we could know that one arg should
            // be a bool and isn't)
            if (leftOperand == DataType.Unknown
                || rightOperand == DataType.Unknown)
            {
                switch (@operator)
                {
                    case BinaryOperator.EqualsEquals:
                    case BinaryOperator.NotEqual:
                    case BinaryOperator.LessThan:
                    case BinaryOperator.LessThanOrEqual:
                    case BinaryOperator.GreaterThan:
                    case BinaryOperator.GreaterThanOrEqual:
                    case BinaryOperator.And:
                    case BinaryOperator.Or:
                        return binaryExpression.Type.Fulfill(ObjectType.Bool);
                    default:
                        return binaryExpression.Type.Fulfill(DataType.Unknown);
                }
            }

            bool typeError;
            switch (@operator)
            {
                case BinaryOperator.Plus:
                    typeError = CheckNumericOperator(
                        binaryExpression.LeftOperand,
                        binaryExpression.RightOperand,
                        null);
                    binaryExpression.Type.Fulfill(!typeError ? leftOperand : DataType.Unknown);
                    break;
                //case IPlusEqualsToken _:
                //    typeError = CheckNumericOperator(
                //        binaryOperatorExpression.LeftOperand,
                //        binaryOperatorExpression.RightOperand,
                //        binaryOperatorExpression.LeftOperand.Type.AssertComputed());
                //    //typeError = (leftOperand != rightOperand || leftOperand == ObjectType.Bool)
                //    //    // TODO really pointer arithmetic should allow `size` and `offset`, but we don't have constants working correct yet
                //    //    && !(leftOperand is PointerType && (rightOperand == ObjectType.Size || rightOperand == ObjectType.Int));
                //    binaryOperatorExpression.Type.Computed(!typeError ? leftOperand : DataType.Unknown);
                //    break;
                //case IAsteriskEqualsToken _:
                //    typeError = leftOperand != rightOperand || leftOperand == ObjectType.Bool;
                //    binaryOperatorExpression.Type.Computed(!typeError ? leftOperand : DataType.Unknown);
                //    break;
                case BinaryOperator.EqualsEquals:
                case BinaryOperator.NotEqual:
                case BinaryOperator.LessThan:
                case BinaryOperator.LessThanOrEqual:
                case BinaryOperator.GreaterThan:
                case BinaryOperator.GreaterThanOrEqual:
                    typeError = leftOperandCore != rightOperandCore;
                    binaryExpression.Type.Fulfill(ObjectType.Bool);
                    break;
                //case IEqualsToken _:
                //    typeError = leftOperandCore != rightOperandCore;
                //    if (!typeError)
                //        binaryOperatorExpression.Type.Computed(leftOperand);
                //    break;
                case BinaryOperator.And:
                case BinaryOperator.Or:
                    typeError = leftOperand != ObjectType.Bool || rightOperand != ObjectType.Bool;

                    binaryExpression.Type.Fulfill(ObjectType.Bool);
                    break;
                //case IDollarToken _:
                //case IDollarLessThanToken _:
                //case IDollarLessThanNotEqualToken _:
                //case IDollarGreaterThanToken _:
                //case IDollarGreaterThanNotEqualToken _:
                //    typeError = leftOperand != ObjectType.Type;
                //    break;
                //case IAsKeywordToken _:
                //    var asType = EvaluateExpression(binaryOperatorExpression.RightOperand);
                //    // TODO check that left operand can be converted to this
                //    typeError = false;
                //    binaryOperatorExpression.Type.Computed(asType);
                //    break;
                default:
                    throw NonExhaustiveMatchException.For(@operator);
            }
            if (typeError)
                diagnostics.Add(TypeError.OperatorCannotBeAppliedToOperandsOfType(file,
                    binaryExpression.Span, @operator,
                    binaryExpression.LeftOperand.Type.Fulfilled(),
                    binaryExpression.RightOperand.Type.Fulfilled()));

            return binaryExpression.Type.Fulfilled();
        }

        private bool CheckNumericOperator(
            [NotNull] ExpressionSyntax leftOperand,
            [NotNull] ExpressionSyntax rightOperand,
            [CanBeNull] DataType resultType)
        {
            var leftType = leftOperand.Type.Fulfilled();
            var rightType = rightOperand.Type.Fulfilled();
            switch (leftType)
            {
                case PointerType _:
                {
                    // TODO it may need to be size
                    ImposeIntegerConstantType(ObjectType.Offset, rightOperand);

                    return rightType != ObjectType.Size &&
                           rightType != ObjectType.Offset;
                }
                case IntegerConstantType _:
                    // TODO may need to promote based on size
                    ImposeIntegerConstantType(rightType, leftOperand);

                    return !IsIntegerType(rightType);
                case DataType type when IsIntegerType(type):
                    // TODO it may need to be size
                    ImposeIntegerConstantType(leftType, rightOperand);

                    return !IsIntegerType(rightType);
                case ObjectType _:
                    // Other object types can't be used in numeric expressions
                    return false;
                default:
                    throw NonExhaustiveMatchException.For(leftType);
            }
        }

        [NotNull]
        private DataType ImposeIntegerConstantType([NotNull] DataType expectedType, [NotNull] ExpressionSyntax expression)
        {
            var currentType = expression.Type.Fulfilled();

            if (!IsIntegerType(expectedType) // Don't impose a non-integer type
                || currentType != DataType.IntegerConstant) // If it isn't an integer constant, nothing to do
                return currentType;

            switch (expression)
            {
                case LiteralExpressionSyntax _:
                    return expression.Type.Resolve(expectedType);
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
        }

        private static bool IsIntegerType([NotNull] DataType type)
        {
            Requires.NotNull(nameof(type), type);
            return type is PrimitiveFixedIntegerType
                   || type == ObjectType.Size
                   || type == ObjectType.Offset;
        }

        [NotNull]
        private DataType InferUnaryExpressionType(
            [NotNull] UnaryExpressionSyntax unaryExpression)
        {
            InferExpressionType(unaryExpression.Operand);
            var operand = unaryExpression.Operand.Type.Fulfilled();
            var @operator = unaryExpression.Operator;

            // If either is unknown, then we can't know whether there is a a problem
            // (technically not true, for example, we could know that one arg should
            // be a bool and isn't)
            if (operand == DataType.Unknown)
                return unaryExpression.Type.Fulfill(DataType.Unknown);

            bool typeError;
            switch (@operator)
            {
                case INotKeywordToken _:
                    typeError = operand != ObjectType.Bool;
                    unaryExpression.Type.Fulfill(ObjectType.Bool);
                    break;
                case IAtSignToken _:
                    typeError = false; // TODO check that the expression can have a pointer taken
                    if (operand is Metatype)
                        unaryExpression.Type.Fulfill(ObjectType.Type); // constructing a type
                    else
                        unaryExpression.Type.Fulfill(new PointerType(operand)); // taking the address of something
                    break;
                case IQuestionToken _:
                    typeError = false; // TODO check that the expression can have a pointer taken
                    unaryExpression.Type.Fulfill(new PointerType(operand));
                    break;
                case ICaretToken _:
                    switch (operand)
                    {
                        case PointerType pointerType:
                            unaryExpression.Type.Fulfill(pointerType.Referent);
                            typeError = false;
                            break;
                        default:
                            unaryExpression.Type.Fulfill(DataType.Unknown);
                            typeError = true;
                            break;
                    }
                    break;
                default:
                    throw NonExhaustiveMatchException.For(@operator);
            }
            if (typeError)
                diagnostics.Add(TypeError.OperatorCannotBeAppliedToOperandOfType(file,
                    unaryExpression.Span, @operator, operand));

            return unaryExpression.Type.Fulfilled();
        }

        /// <summary>
        /// Evaluates a type expression to the type it identifies
        /// </summary>
        [NotNull]
        public DataType CheckAndEvaluateTypeExpression([CanBeNull] ExpressionSyntax typeExpression)
        {
            if (typeExpression == null)
            {
                // TODO report error?
                return DataType.Unknown;
            }

            var type = InferExpressionType(typeExpression);
            if (!(type is Metatype)
                && type != ObjectType.Type)
            {
                diagnostics.Add(TypeError.MustBeATypeExpression(file, typeExpression.Span));
                return DataType.Unknown;
            }

            return EvaluateExpression(typeExpression);
        }

        [NotNull]
        private DataType EvaluateExpression(
            [NotNull] ExpressionSyntax typeExpression)
        {
            switch (typeExpression)
            {
                case IdentifierNameSyntax identifier:
                {
                    var identifierType = identifier.Type.Fulfilled();
                    switch (identifierType)
                    {
                        case Metatype metatype:
                            return metatype.Instance;
                        case ObjectType t
                            when t == ObjectType.Type: // It is a variable holding a type?
                                                       // for now, return a placeholder type
                            return ObjectType.Any;
                        case UnknownType _:
                            return DataType.Unknown;
                        default:
                            throw NonExhaustiveMatchException.For(identifierType);
                    }
                }
                case PrimitiveTypeSyntax primitive:
                    return GetPrimitiveType(primitive);
                case LifetimeTypeSyntax lifetimeType:
                {
                    var type = EvaluateExpression(lifetimeType.TypeExpression);
                    if (type == DataType.Unknown) return DataType.Unknown;
                    var lifetimeToken = lifetimeType.Lifetime;
                    Lifetime lifetime;
                    switch (lifetimeToken)
                    {
                        case IOwnedKeywordToken _:
                            lifetime = OwnedLifetime.Instance;
                            break;
                        case IRefKeywordToken _:
                            lifetime = RefLifetime.Instance;
                            break;
                        case IIdentifierToken identifier:
                            lifetime = new NamedLifetime(identifier.Value);
                            break;
                        default:
                            throw NonExhaustiveMatchException.For(lifetimeToken);
                    }
                    if (type is ObjectType objectType)
                        return new LifetimeType(objectType, lifetime);
                    return DataType.Unknown;
                }
                case RefTypeSyntax refType:
                {
                    var referent = EvaluateExpression(refType.ReferencedType);
                    if (referent is ObjectType objectType)
                        return new RefType(objectType);
                    return DataType.Unknown;
                }
                case UnaryExpressionSyntax unaryOperatorExpression:
                    switch (unaryOperatorExpression.Operator)
                    {
                        case IAtSignToken _:
                            if (unaryOperatorExpression.Operand.Type.Fulfilled() is Metatype metatype)
                                return new PointerType(metatype.Instance);
                            // TODO evaluate to type
                            return DataType.Unknown;
                        default:
                            // TODO evaluate to type
                            return DataType.Unknown;
                    }
                case GenericsInvocationSyntax _:
                case GenericNameSyntax _:
                {
                    var type = typeExpression.Type.Fulfilled();
                    if (type is Metatype metatype)
                        return metatype.Instance;

                    // TODO evaluate to type
                    return DataType.Unknown;
                }
                case BinaryExpressionSyntax _:
                    // TODO evaluate to type
                    return DataType.Unknown;
                case MutableTypeSyntax mutableType:
                    return EvaluateExpression(mutableType.ReferencedTypeExpression); // TODO make the type mutable
                default:
                    throw NonExhaustiveMatchException.For(typeExpression);
            }
        }

        [NotNull]
        private static DataType GetPrimitiveType([NotNull] PrimitiveTypeSyntax primitive)
        {
            switch (primitive.Keyword)
            {
                case IIntKeywordToken _:
                    return PrimitiveFixedIntegerType.Int;
                case IUIntKeywordToken _:
                    return PrimitiveFixedIntegerType.UInt;
                case IByteKeywordToken _:
                    return PrimitiveFixedIntegerType.Byte;
                case ISizeKeywordToken _:
                    return ObjectType.Size;
                case IVoidKeywordToken _:
                    return ObjectType.Void;
                case IBoolKeywordToken _:
                    return ObjectType.Bool;
                case INeverKeywordToken _:
                    return ObjectType.Never;
                case ITypeKeywordToken _:
                    return ObjectType.Type;
                case IMetatypeKeywordToken _:
                    return ObjectType.Metatype;
                case IAnyKeywordToken _:
                    return ObjectType.Any;
                default:
                    throw NonExhaustiveMatchException.For(primitive.Keyword);
            }
        }
    }
}