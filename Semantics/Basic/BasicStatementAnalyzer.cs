using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Lifetimes;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Primitives;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Basic.ImplicitConversions;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic
{
    public class BasicStatementAnalyzer
    {
        private readonly CodeFile file;
        private readonly Diagnostics diagnostics;
        private readonly DataType? selfType;
        private readonly DataType? returnType;
        private readonly BasicTypeAnalyzer typeAnalyzer;

        public BasicStatementAnalyzer(
            CodeFile file,
            Diagnostics diagnostics,
            DataType? selfType = null,
            DataType? returnType = null)
        {
            this.file = file;
            this.diagnostics = diagnostics;
            this.returnType = returnType;
            this.selfType = selfType;
            typeAnalyzer = new BasicTypeAnalyzer(file, diagnostics, this);
        }

        public void ResolveTypesInStatement(IStatementSyntax statement)
        {
            switch (statement)
            {
                default:
                    throw ExhaustiveMatch.Failed(statement);
                case IVariableDeclarationStatementSyntax variableDeclaration:
                    ResolveTypesInVariableDeclaration(variableDeclaration);
                    break;
                case IExpressionStatementSyntax expressionStatement:
                    InferExpressionType(ref expressionStatement.Expression);
                    break;
                case IResultStatementSyntax resultStatement:
                    InferExpressionType(ref resultStatement.Expression);
                    break;
            }
        }

        private void ResolveTypesInVariableDeclaration(IVariableDeclarationStatementSyntax variableDeclaration)
        {
            if (variableDeclaration.Initializer != null)
                InferExpressionType(ref variableDeclaration.Initializer.Expression);

            DataType type;
            if (variableDeclaration.TypeSyntax != null)
                type = typeAnalyzer.Evaluate(variableDeclaration.TypeSyntax);
            else if (variableDeclaration.Initializer != null)
            {
                type = variableDeclaration.Initializer.Expression.Type!;
                // Use the initializer type unless it is constant
                switch (type)
                {
                    case IntegerConstantType _:
                        type = DataType.Int;
                        break;
                    case StringConstantType _:
                        throw new NotImplementedException();
                }
            }
            else
            {
                diagnostics.Add(TypeError.NotImplemented(file, variableDeclaration.NameSpan,
                    "Inference of local variable types not implemented"));
                type = DataType.Unknown;
            }

            if (variableDeclaration.Initializer != null)
            {
                InsertImplicitConversionIfNeeded(ref variableDeclaration.Initializer.Expression, type);
                var initializerType = variableDeclaration.Initializer.Type = variableDeclaration.Initializer.Expression.Type!;
                // If the source is an owned reference, then the declaration is implicitly owned
                if (type is UserObjectType targetType && initializerType is UserObjectType sourceType
                      && sourceType.Lifetime == Lifetime.Owned
                      && targetType.Lifetime == Lifetime.None
                      && IsAssignableFrom(targetType.AsOwned(), sourceType))
                    variableDeclaration.Type = targetType.AsOwned().AsDeclared();
                else
                {
                    if (!IsAssignableFrom(type, initializerType))
                        diagnostics.Add(TypeError.CannotConvert(file, variableDeclaration.Initializer, initializerType, type));

                    variableDeclaration.Type = type.AsDeclared();
                }
            }
            else
                variableDeclaration.Type = type.AsDeclared();
        }

        /// <summary>
        /// Create an implicit conversion if allowed and needed
        /// </summary>
        private static void InsertImplicitConversionIfNeeded(
            ref IExpressionSyntax expression,
            DataType targetType)
        {
            if (targetType is OptionalType optionalType)
            {
                // We may need to do a conversion to optional or from none
                if (expression.Type is OptionalType noneType)
                {
                    if (noneType.Referent is NeverType)
                        expression = new ImplicitNoneConversionExpression(expression, optionalType);
                    // TODO may need to lift an implicit numeric conversion
                }
                else
                {
                    // If needed, convert the type to the referent type of the optional type
                    InsertImplicitConversionIfNeeded(ref expression, optionalType.Referent);
                    if (IsAssignableFrom(optionalType.Referent, expression.Type))
                        expression = new ImplicitOptionalConversionExpression(expression, optionalType);
                }

                return;
            }

            switch (expression.Type)
            {
                case SizedIntegerType expressionType:
                    switch (targetType)
                    {
                        case SizedIntegerType expectedType:
                            if (expectedType.Bits > expressionType.Bits
                                && (!expressionType.IsSigned || expectedType.IsSigned))
                                expression =
                                    new ImplicitNumericConversionExpression(expression,
                                        expectedType);
                            break;
                    }
                    break;
                case IntegerConstantType expressionType:
                    var requireSigned = expressionType.Value < 0;
                    switch (targetType)
                    {
                        case SizedIntegerType expectedType:
                            var bits = expressionType.Value.GetByteCount() * 8;
                            if (expectedType.Bits >= bits
                               && (!requireSigned || expectedType.IsSigned))
                                expression = new ImplicitNumericConversionExpression(expression, expectedType);
                            break;
                        case UnsizedIntegerType expectedType:
                            if (!requireSigned || expectedType.IsSigned)
                                expression = new ImplicitNumericConversionExpression(expression, expectedType);
                            break;
                    }
                    break;
                case StringConstantType _:
                {
                    ISymbol symbol;
                    switch (targetType)
                    {
                        case UserObjectType objectType:
                            symbol = objectType.Symbol;
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    if (symbol.FullName.Equals(Name.From("String")))
                    {
                        var stringConstructor = symbol.ChildSymbols[SpecialName.New]
                            .Cast<IFunctionSymbol>()
                            .Single(c =>
                            {
                                var parameters = c.Parameters.ToFixedList();
                                return parameters.Count == 2
                                       && parameters[0].Type == DataType.Size
                                       && parameters[1].Type == DataType.StringConstant;
                            });
                        expression = new ImplicitStringLiteralConversionExpression((IStringLiteralExpressionSyntax)expression, targetType,
                            stringConstructor);
                    }
                    else
                        throw new NotImplementedException("Trying to use string literal as non-string type");
                }
                break;
                case UserObjectType objectType:
                    if (targetType is UserObjectType targetObjectType
                        && targetObjectType.Mutability == Mutability.Immutable
                        && targetObjectType.EqualExceptLifetimeAndMutability(objectType)
                        && objectType.Mutability != Mutability.Immutable)
                    {
                        // TODO if source type is explicitly mutable, issue warning about using `mut` in immutable context
                        // Take the object type and make it immutable so that we preserve the lifetime of the type
                        expression = new ImplicitImmutabilityConversionExpression(expression, objectType.AsImmutable());
                    }
                    break;
            }

            // No conversion
        }

        public DataType CheckExpressionType(
            ref IExpressionSyntax expression,
            DataType expectedType)
        {
            var actualType = InferExpressionType(ref expression);
            // TODO check for type compatibility not equality
            if (!expectedType.Equals(actualType))
                diagnostics.Add(TypeError.CannotConvert(file, expression, actualType, expectedType));
            return actualType;
        }

        private DataType InferExpressionType([NotNull] ref IExpressionSyntax? expression)
        {
            switch (expression)
            {
                default:
                    throw ExhaustiveMatch.Failed(expression);
                case null:
                    return DataType.Unknown;
                case IReturnExpressionSyntax returnExpression:
                {
                    if (returnExpression.ReturnValue != null)
                    {
                        // TODO report an error instead of throwing
                        if (returnType == null)
                            throw new NotImplementedException("Return statement in constructor");

                        // If we return ownership, there is an implicit move
                        if (returnType is UserObjectType objectType && objectType.IsOwned)
                            InferMoveExpressionType(returnExpression.ReturnValue.Expression);
                        else
                            InferExpressionType(ref returnExpression.ReturnValue.Expression);
                        InsertImplicitConversionIfNeeded(ref returnExpression.ReturnValue.Expression,
                            returnType);
                        var type = returnExpression.ReturnValue.Type =
                            returnExpression.ReturnValue.Expression.Type ?? throw new InvalidOperationException();
                        if (!IsAssignableFrom(returnType, type))
                            diagnostics.Add(TypeError.CannotConvert(file,
                                returnExpression.ReturnValue.Expression, type, returnType));
                    }
                    else if (returnType == DataType.Never)
                        diagnostics.Add(
                            TypeError.CantReturnFromNeverFunction(file, returnExpression.Span));
                    else if (returnType != DataType.Void)
                        diagnostics.Add(TypeError.ReturnExpressionMustHaveValue(file,
                            returnExpression.Span, returnType));

                    return returnExpression.Type = DataType.Never;
                }
                case IIntegerLiteralExpressionSyntax integerLiteral:
                    return integerLiteral.Type = new IntegerConstantType(integerLiteral.Value);
                case IStringLiteralExpressionSyntax stringLiteral:
                    return stringLiteral.Type = DataType.StringConstant;
                case IBoolLiteralExpressionSyntax boolLiteral:
                    return boolLiteral.Type = DataType.Bool;
                case IBinaryOperatorExpressionSyntax binaryOperatorExpression:
                {
                    InferExpressionType(ref binaryOperatorExpression.LeftOperand);
                    var leftType = binaryOperatorExpression.LeftOperand.Type;
                    var @operator = binaryOperatorExpression.Operator;
                    InferExpressionType(ref binaryOperatorExpression.RightOperand);
                    var rightType = binaryOperatorExpression.RightOperand.Type;

                    // If either is unknown, then we can't know whether there is a a problem.
                    // Note that the operator could be overloaded
                    if (leftType == DataType.Unknown || rightType == DataType.Unknown)
                        return binaryOperatorExpression.Type = DataType.Unknown;

                    bool compatible;
                    switch (@operator)
                    {
                        case BinaryOperator.Plus:
                        case BinaryOperator.Minus:
                        case BinaryOperator.Asterisk:
                        case BinaryOperator.Slash:
                            compatible = NumericOperatorTypesAreCompatible(ref binaryOperatorExpression.LeftOperand, ref binaryOperatorExpression.RightOperand, null);
                            binaryOperatorExpression.Type = compatible ? leftType : DataType.Unknown;
                            break;
                        case BinaryOperator.EqualsEquals:
                        case BinaryOperator.NotEqual:
                        case BinaryOperator.LessThan:
                        case BinaryOperator.LessThanOrEqual:
                        case BinaryOperator.GreaterThan:
                        case BinaryOperator.GreaterThanOrEqual:
                            compatible = (leftType == DataType.Bool && rightType == DataType.Bool)
                                         || NumericOperatorTypesAreCompatible(ref binaryOperatorExpression.LeftOperand, ref binaryOperatorExpression.RightOperand, null)
                                         /*|| OperatorOverloadDefined(@operator, binaryOperatorExpression.LeftOperand, ref binaryOperatorExpression.RightOperand)*/;
                            binaryOperatorExpression.Type = DataType.Bool;
                            break;
                        case BinaryOperator.And:
                        case BinaryOperator.Or:
                            compatible = leftType == DataType.Bool && rightType == DataType.Bool;
                            binaryOperatorExpression.Type = DataType.Bool;
                            break;
                        case BinaryOperator.DotDot:
                            throw new NotImplementedException("Type of `..` operator");
                        default:
                            throw ExhaustiveMatch.Failed(@operator);
                    }
                    if (!compatible)
                        diagnostics.Add(TypeError.OperatorCannotBeAppliedToOperandsOfType(file,
                            binaryOperatorExpression.Span, @operator,
                            binaryOperatorExpression.LeftOperand.Type,
                            binaryOperatorExpression.RightOperand.Type));

                    return binaryOperatorExpression.Type;
                }
                case INameExpressionSyntax identifierName:
                    return InferNameType(identifierName, false);
                case IUnaryOperatorExpressionSyntax unaryOperatorExpression:
                {
                    var operandType = InferExpressionType(ref unaryOperatorExpression.Operand);
                    var @operator = unaryOperatorExpression.Operator;

                    // If either is unknown, then we can't know whether there is a a problem
                    // (technically not true, for example, we could know that one arg should
                    // be a bool and isn't)
                    if (operandType == DataType.Unknown)
                        return unaryOperatorExpression.Type = DataType.Unknown;

                    bool typeError;
                    switch (@operator)
                    {
                        case UnaryOperator.Not:
                            typeError = operandType != DataType.Bool;
                            unaryOperatorExpression.Type = DataType.Bool;
                            break;
                        case UnaryOperator.Question:
                            typeError = false; // TODO check that the expression can have a pointer taken
                            unaryOperatorExpression.Type = new OptionalType(operandType);
                            break;
                        case UnaryOperator.Minus:
                        case UnaryOperator.Plus:
                            switch (operandType)
                            {
                                case IntegerConstantType integerType:
                                    typeError = false;
                                    unaryOperatorExpression.Type = integerType;
                                    break;
                                case SizedIntegerType sizedIntegerType:
                                    typeError = false;
                                    unaryOperatorExpression.Type = sizedIntegerType;
                                    break;
                                default:
                                    unaryOperatorExpression.Type = DataType.Unknown;
                                    typeError = true;
                                    break;
                            }
                            break;
                        default:
                            throw ExhaustiveMatch.Failed(@operator);
                    }
                    if (typeError)
                        diagnostics.Add(TypeError.OperatorCannotBeAppliedToOperandOfType(file,
                            unaryOperatorExpression.Span, @operator, operandType));

                    return unaryOperatorExpression.Type;
                }
                case INewObjectExpressionSyntax newObjectExpression:
                {
                    var argumentTypes = newObjectExpression.Arguments.Select(argument => InferExpressionType(ref argument.Expression)).ToFixedList();
                    // TODO handle named constructors here
                    var constructingType = typeAnalyzer.Evaluate(newObjectExpression.TypeSyntax);
                    if (!constructingType.IsKnown)
                    {
                        diagnostics.Add(NameBindingError.CouldNotBindConstructor(file, newObjectExpression.Span));
                        newObjectExpression.ConstructorSymbol = UnknownSymbol.Instance;
                        return newObjectExpression.Type  = DataType.Unknown;
                    }
                    var constructedType = (UserObjectType)constructingType;
                    var typeSymbol = GetSymbolForType(constructedType);
                    var constructors = typeSymbol.ChildSymbols[SpecialName.New].OfType<IFunctionSymbol>().ToFixedList();
                    constructors = ResolveOverload(constructors, null, argumentTypes);
                    switch (constructors.Count)
                    {
                        case 0:
                            diagnostics.Add(NameBindingError.CouldNotBindConstructor(file, newObjectExpression.Span));
                            newObjectExpression.ConstructorSymbol = UnknownSymbol.Instance;
                            newObjectExpression.ConstructorType = DataType.Unknown;
                            break;
                        case 1:
                            var constructorSymbol = constructors.Single();
                            newObjectExpression.ConstructorSymbol = constructorSymbol;
                            //var constructorType = constructorSymbol.Type;
                            //newObjectExpression.ConstructorType = constructorType;
                            //if (constructorType is FunctionType functionType)
                            //    foreach (var (arg, type) in newObjectExpression.Arguments.Zip(functionType.ParameterTypes))
                            //    {
                            //        InsertImplicitConversionIfNeeded(ref arg.Value, type);
                            //        CheckArgumentTypeCompatibility(type, arg);
                            //    }
                            break;
                        default:
                            diagnostics.Add(NameBindingError.AmbiguousConstructorCall(file, newObjectExpression.Span));
                            newObjectExpression.ConstructorSymbol = UnknownSymbol.Instance;
                            newObjectExpression.ConstructorType = DataType.Unknown;
                            break;
                    }

                    return newObjectExpression.Type = constructedType.AsOwnedUpgradable();
                }
                case IForeachExpressionSyntax foreachExpression:
                {
                    var declaredType = typeAnalyzer.Evaluate(foreachExpression.TypeSyntax);
                    CheckForeachInType(declaredType, ref foreachExpression.InExpression);
                    foreachExpression.VariableType =
                        declaredType ?? foreachExpression.InExpression.Type;
                    // TODO check the break types
                    InferBlockType(foreachExpression.Block);
                    // TODO assign correct type to the expression
                    return foreachExpression.Type = DataType.Void;
                }
                case IWhileExpressionSyntax whileExpression:
                {
                    CheckExpressionType(ref whileExpression.Condition, DataType.Bool);
                    InferBlockType(whileExpression.Block);
                    // TODO assign correct type to the expression
                    return whileExpression.Type = DataType.Void;
                }
                case ILoopExpressionSyntax loopExpression:
                    InferBlockType(loopExpression.Block);
                    // TODO assign correct type to the expression
                    return loopExpression.Type = DataType.Void;
                case IMethodInvocationExpressionSyntax methodInvocation:
                {
                    // This could:
                    // * Invoke a stand alone function
                    // * Invoke a static function
                    // * Invoke a method
                    // * Invoke a function pointer
                    var argumentTypes = methodInvocation.Arguments.Select(argument => InferExpressionType(ref argument.Expression)).ToFixedList();
                    InferExpressionType(ref methodInvocation.Target);
                    var targetType = methodInvocation.Target.Type;
                    // If it is unknown, we already reported an error
                    if (targetType == DataType.Unknown)
                        return methodInvocation.Type = DataType.Unknown;

                    // TODO improve function lookup and include the possibility that the symbol isn't a function
                    //diagnostics.Add(TypeError.MustBeCallable(file, methodInvocation.Target));
                    //return methodInvocation.Type = DataType.Unknown;
                    var typeSymbol = GetSymbolForType(targetType);
                    var methodSymbols = typeSymbol.ChildSymbols[methodInvocation.MethodNameSyntax.Name]
                        .OfType<IFunctionSymbol>().ToFixedList();
                    methodSymbols = ResolveOverload(methodSymbols, targetType, argumentTypes);
                    switch (methodSymbols.Count)
                    {
                        case 0:
                            diagnostics.Add(
                                NameBindingError.CouldNotBindMethod(file, methodInvocation.Span));
                            methodInvocation.MethodNameSyntax.ReferencedSymbol = UnknownSymbol.Instance;
                            methodInvocation.Type = DataType.Unknown;
                            break;
                        case 1:
                            var functionSymbol = methodSymbols.Single();
                            methodInvocation.MethodNameSyntax.ReferencedSymbol = functionSymbol;

                            var selfType = functionSymbol.Parameters.First().Type;
                            InsertImplicitConversionIfNeeded(ref methodInvocation.Target, selfType);
                            CheckArgumentTypeCompatibility(selfType, methodInvocation.Target, true);

                            // Skip the self parameter
                            foreach (var (arg, type) in methodInvocation.Arguments.Zip(functionSymbol
                                                                                       .Parameters.Skip(1)
                                                                                       .Select(p => p.Type)))
                            {
                                InsertImplicitConversionIfNeeded(ref arg.Expression, type);
                                CheckArgumentTypeCompatibility(type, arg.Expression);
                            }
                            methodInvocation.Type = functionSymbol.ReturnType;
                            break;
                        default:
                            diagnostics.Add(
                                NameBindingError.AmbiguousMethodCall(file, methodInvocation.Span));
                            methodInvocation.MethodNameSyntax.ReferencedSymbol = UnknownSymbol.Instance;
                            methodInvocation.Type = DataType.Unknown;
                            break;
                    }

                    return methodInvocation.Type;
                }
                case IAssociatedFunctionInvocationExpressionSyntax associatedFunctionInvocation:

                    throw new NotImplementedException();
                case IFunctionInvocationExpressionSyntax functionInvocation:
                    return InferFunctionInvocationType(functionInvocation);
                case IUnsafeExpressionSyntax unsafeExpression:
                    InferExpressionType(ref unsafeExpression.Expression);
                    return unsafeExpression.Type = unsafeExpression.Expression.Type;
                //case MutableTypeSyntax mutableExpression:
                //{
                //    var expressionType = InferExpressionType(mutableExpression.Referent);
                //    DataType type;

                //    switch (expressionType)
                //    {
                //        // If it is already mutable we can't redeclare it mutable (i.e. `mut mut x` is error)
                //        case UserObjectType objectType
                //            when objectType.Mutability.IsUpgradable:
                //            type = objectType.AsMutable();
                //            break;
                //        default:
                //            diagnostics.Add(TypeError.ExpressionCantBeMutable(file,
                //                mutableExpression.Referent));
                //            type = expressionType;
                //            break;
                //    }

                //    return mutableExpression.Type = type;
                //}
                case IIfExpressionSyntax ifExpression:
                    CheckExpressionType(ref ifExpression.Condition, DataType.Bool);
                    InferBlockType(ifExpression.ThenBlock);
                    switch (ifExpression.ElseClause)
                    {
                        default:
                            throw ExhaustiveMatch.Failed(ifExpression.ElseClause);
                        case null:
                            break;
                        case IIfExpressionSyntax _:
                        case IBlockExpressionSyntax _:
                            var elseExpression = (IExpressionSyntax)ifExpression.ElseClause;
                            InferExpressionType(ref elseExpression);
                            //ifExpression.ElseClause = elseExpression;
                            break;
                        case IResultStatementSyntax resultStatement:
                            InferExpressionType(ref resultStatement.Expression);
                            break;
                    }
                    // TODO assign a type to the expression
                    return ifExpression.Type = DataType.Void;
                //case ResultStatementSyntax resultExpression:
                //    InferExpressionType(ref resultExpression.Expression);
                //    return resultExpression.Type = DataType.Never;
                case IMemberAccessExpressionSyntax memberAccess:
                {
                    var left = InferExpressionType(ref memberAccess.Expression);
                    var symbol = GetSymbolForType(left);

                    switch (memberAccess.Member)
                    {
                        case INameExpressionSyntax identifier:
                            var memberSymbols = symbol.Lookup(identifier.Name).OfType<IBindingSymbol>().ToFixedList();
                            var type = AssignReferencedSymbolAndType(identifier, memberSymbols);
                            return memberAccess.Type = type;
                        default:
                            throw NonExhaustiveMatchException.For(memberAccess.Member);
                    }
                }
                case IBreakExpressionSyntax breakExpression:
                    InferExpressionType(ref breakExpression.Value);
                    return breakExpression.Type = DataType.Never;
                case INextExpressionSyntax nextExpression:
                    return nextExpression.Type = DataType.Never;
                case IAssignmentExpressionSyntax assignmentExpression:
                {
                    var left = InferExpressionType(ref assignmentExpression.LeftOperand);
                    InferExpressionType(ref assignmentExpression.RightOperand.Expression);
                    InsertImplicitConversionIfNeeded(ref assignmentExpression.RightOperand.Expression, left);
                    var right = assignmentExpression.RightOperand.Type
                        = assignmentExpression.RightOperand.Expression.Type ?? throw new InvalidOperationException();
                    if (!IsAssignableFrom(left, right))
                        diagnostics.Add(TypeError.CannotConvert(file,
                            assignmentExpression.RightOperand.Expression, right, left));
                    return assignmentExpression.Type = DataType.Void;
                }
                case ISelfExpressionSyntax selfExpression:
                    return selfExpression.Type = selfType ?? DataType.Unknown;
                //case IMoveTransferSyntax moveExpression:
                //{
                //    var type = InferMoveExpressionType(moveExpression.Expression);
                //    if (type is ReferenceType referenceType && !referenceType.IsOwned)
                //    {
                //        diagnostics.Add(TypeError.CannotMoveBorrowedValue(file, moveExpression));
                //        type = referenceType.WithLifetime(Lifetime.Owned);
                //    }

                //    if (type is UserObjectType objectType)
                //        type = objectType.AsOwnedUpgradable();
                //    return moveExpression.Type = type;
                //}
                case INoneLiteralExpressionSyntax noneLiteralExpression:
                    return noneLiteralExpression.Type = DataType.None;
                case IImplicitConversionExpression _:
                    throw new Exception("ImplicitConversionExpressions are inserted by BasicExpressionAnalyzer. They should not be present in the AST yet.");
                case ILifetimeExpressionSyntax _:
                    throw new Exception("Should be inferring type of type expression");
                case IBlockExpressionSyntax blockSyntax:
                    return InferBlockType(blockSyntax);
            }
        }

        private DataType InferFunctionInvocationType(IFunctionInvocationExpressionSyntax functionInvocationExpression)
        {
            var argumentTypes = functionInvocationExpression.Arguments.Select(argument => InferExpressionType(ref argument.Expression)).ToFixedList();
            var functionSymbols = functionInvocationExpression.FunctionNameSyntax.LookupInContainingScope()
                .OfType<IFunctionSymbol>().ToFixedList();
            functionSymbols = ResolveOverload(functionSymbols, null, argumentTypes);
            switch (functionSymbols.Count)
            {
                case 0:
                    diagnostics.Add(NameBindingError.CouldNotBindFunction(file, functionInvocationExpression.Span));
                    functionInvocationExpression.FunctionNameSyntax.ReferencedSymbol = UnknownSymbol.Instance;
                    functionInvocationExpression.Type = DataType.Unknown;
                    break;
                case 1:
                    var functionSymbol = functionSymbols.Single();
                    functionInvocationExpression.FunctionNameSyntax.ReferencedSymbol = functionSymbol;
                    foreach (var (arg, parameter) in
                        functionInvocationExpression.Arguments.Zip(functionSymbol.Parameters))
                    {
                        InsertImplicitConversionIfNeeded(ref arg.Expression, parameter.Type);
                        CheckArgumentTypeCompatibility(parameter.Type, arg.Expression);
                    }
                    functionInvocationExpression.Type = functionSymbol.ReturnType;
                    break;
                default:
                    diagnostics.Add(NameBindingError.AmbiguousFunctionCall(file, functionInvocationExpression.Span));
                    functionInvocationExpression.FunctionNameSyntax.ReferencedSymbol = UnknownSymbol.Instance;
                    functionInvocationExpression.Type = DataType.Unknown;
                    break;
            }
            return functionInvocationExpression.Type;
        }

        private DataType InferBlockType(IBlockOrResultSyntax blockOrResult)
        {
            switch (blockOrResult)
            {
                default:
                    throw ExhaustiveMatch.Failed(blockOrResult);
                case IBlockExpressionSyntax block:
                    foreach (var statement in block.Statements)
                        ResolveTypesInStatement(statement);

                    return block.Type = DataType.Void; // TODO assign the correct type to the block
                case IResultStatementSyntax result:
                    InferExpressionType(ref result.Expression);
                    return result.Expression.Type!;
            }
        }

        public DataType InferNameType(INameExpressionSyntax nameExpression, bool isMove)
        {
            var symbols = nameExpression.LookupInContainingScope();
            DataType type;
            switch (symbols.Count)
            {
                case 0:
                    diagnostics.Add(NameBindingError.CouldNotBindName(file, nameExpression.Span));
                    nameExpression.ReferencedSymbol = UnknownSymbol.Instance;
                    type = DataType.Unknown;
                    break;
                case 1:
                {
                    var symbol = symbols.Single();
                    switch (symbol)
                    {
                        case IBindingSymbol binding:
                        {
                            nameExpression.ReferencedSymbol = binding;
                            type = binding.Type;
                            if (type is UserObjectType objectType)
                            {
                                // A bare variable reference doesn't default to mutable
                                if (objectType.Mutability == Mutability.Mutable)
                                    type = objectType = objectType.AsExplicitlyUpgradable();
                                // A bare variable reference doesn't default to owned
                                if (!isMove && objectType.IsOwned)
                                    type = objectType.WithLifetime(Lifetime.None);
                            }
                        }
                        break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                }
                default:
                    diagnostics.Add(NameBindingError.AmbiguousName(file, nameExpression.Span));
                    nameExpression.ReferencedSymbol = UnknownSymbol.Instance;
                    type = DataType.Unknown;
                    break;
            }

            return nameExpression.Type = type;
        }

        private DataType InferMoveExpressionType(IExpressionSyntax expression)
        {
            if (expression == null)
                return DataType.Unknown;

            switch (expression)
            {
                case INameExpressionSyntax identifierName:
                    return InferNameType(identifierName, true);
                default:
                    throw new NotImplementedException("Tried to move out of expression type that isn't implemented");
            }
        }

        /// <summary>
        /// Eventually, a `foreach` `in` expression will just be a regular expression. However, at the
        /// moment, there isn't enough of the language to implement range expressions. So this
        /// check handles range expressions in the specific case of `foreach` only. It marks them
        /// as having the same type as the range endpoints.
        /// </summary>
        private DataType CheckForeachInType(DataType declaredType, ref IExpressionSyntax inExpression)
        {
            switch (inExpression)
            {
                case IBinaryOperatorExpressionSyntax binaryExpression when binaryExpression.Operator == BinaryOperator.DotDot:
                    InferExpressionType(ref binaryExpression.LeftOperand);
                    InferExpressionType(ref binaryExpression.RightOperand);
                    if (declaredType != null)
                    {
                        InsertImplicitConversionIfNeeded(ref binaryExpression.LeftOperand, declaredType);
                        InsertImplicitConversionIfNeeded(ref binaryExpression.RightOperand, declaredType);
                    }
                    return inExpression.Type = binaryExpression.LeftOperand.Type;
                default:
                    return InferExpressionType(ref inExpression);
            }
        }

        private void CheckArgumentTypeCompatibility(DataType type, IExpressionSyntax arg, bool selfArgument = false)
        {
            var fromType = arg.Type ?? throw new ArgumentException("argument must have a type");
            if (!IsAssignableFrom(type, fromType, selfArgument))
                diagnostics.Add(TypeError.CannotConvert(file, arg, fromType, type));
        }

        /// <summary>
        /// Tests whether a variable of the target type could be assigned from a variable of the source type
        /// </summary>
        private static bool IsAssignableFrom(DataType target, DataType source, bool allowUpgrade = false)
        {
            if (target.Equals(source))
                return true;
            if (target is UserObjectType targetReference && source is UserObjectType sourceReference)
            {
                if (targetReference.IsOwned && !sourceReference.IsOwned)
                    return false;

                if (!targetReference.Mutability.IsAssignableFrom(sourceReference.Mutability, allowUpgrade))
                    return false;

                // If they are equal except for lifetimes and mutability compatible, it is fine
                return targetReference.EqualExceptLifetimeAndMutability(sourceReference);
            }

            if (target is OptionalType targetOptional && source is OptionalType sourceOptional)
                return IsAssignableFrom(targetOptional.Referent, sourceOptional.Referent, allowUpgrade);

            return false;
        }

        private static ITypeSymbol GetSymbolForType(DataType type)
        {
            switch (type)
            {
                case UnknownType _:
                    return UnknownSymbol.Instance;
                case UserObjectType objectType:
                    return objectType.Symbol;
                case SizedIntegerType integerType:
                    return PrimitiveSymbols.Instance.OfType<ITypeSymbol>().Single(p => p.FullName == integerType.Name);
                case UnsizedIntegerType integerType:
                    return PrimitiveSymbols.Instance.OfType<ITypeSymbol>().Single(p => p.FullName == integerType.Name);
                default:
                    throw new NotImplementedException();
            }
        }

        private DataType AssignReferencedSymbolAndType(
            INameExpressionSyntax identifier,
            FixedList<IBindingSymbol> memberSymbols)
        {
            switch (memberSymbols.Count)
            {
                case 0:
                    diagnostics.Add(NameBindingError.CouldNotBindMember(file, identifier.Span));
                    identifier.ReferencedSymbol = UnknownSymbol.Instance;
                    return identifier.Type = DataType.Unknown;
                case 1:
                    var memberSymbol = memberSymbols.Single();
                    identifier.ReferencedSymbol = memberSymbol;
                    return identifier.Type = memberSymbol.Type;
                default:
                    diagnostics.Add(NameBindingError.AmbiguousName(file, identifier.Span));
                    identifier.ReferencedSymbol = UnknownSymbol.Instance;
                    return identifier.Type = DataType.Unknown;
            }
        }

        private bool NumericOperatorTypesAreCompatible(
            ref IExpressionSyntax leftOperand,
            ref IExpressionSyntax rightOperand,
            DataType resultType)
        {
            var leftType = leftOperand.Type;
            switch (leftType)
            {
                case IntegerConstantType _:
                    // TODO may need to promote based on size
                    throw new NotImplementedException();
                //return !IsIntegerType(rightType);
                case UnsizedIntegerType integerType:
                    // TODO this isn't right we might need to convert either of them
                    InsertImplicitConversionIfNeeded(ref rightOperand, integerType);
                    return rightOperand.Type is UnsizedIntegerType;
                case SizedIntegerType integerType:
                    // TODO this isn't right we might need to convert either of them
                    InsertImplicitConversionIfNeeded(ref rightOperand, integerType);
                    return rightOperand.Type is SizedIntegerType;
                case OptionalType _:
                    throw new NotImplementedException("Trying to do math on optional type");
                case NeverType _:
                case UnknownType _:
                    return true;
                case ReferenceType _:
                case BoolType _:
                case VoidType _: // This might need a special error message
                case StringConstantType _: // String concatenation will be handled outside this function
                                           // Other object types can't be used in numeric expressions
                    return false;
                default:
                    // In theory we could just return false here, but this way we are forced to note
                    // exactly which types this doesn't work on.
                    throw ExhaustiveMatch.Failed(leftType);
            }
        }

        //private bool OperatorOverloadDefined(BinaryOperator @operator, ExpressionSyntax leftOperand, ref ExpressionSyntax rightOperand)
        //{
        //    // all other operators are not yet implemented
        //    if (@operator != BinaryOperator.EqualsEquals)
        //        return false;

        //    if (!(leftOperand.Type is UserObjectType userObjectType))
        //        return false;
        //    var equalityOperators = userObjectType.Symbol.Lookup(SpecialName.OperatorEquals);
        //    if (equalityOperators.Count != 1)
        //        return false;
        //    var equalityOperator = equalityOperators.Single();
        //    if (!(equalityOperator.Type is FunctionType functionType) || functionType.Arity != 2)
        //        return false;
        //    InsertImplicitConversionIfNeeded(ref rightOperand, functionType.ParameterTypes[1]);
        //    return IsAssignableFrom(functionType.ParameterTypes[1], rightOperand.Type);

        //}

        // Re-expose type analyzer to BasicAnalyzer
        public DataType EvaluateType(ITypeSyntax typeSyntax)
        {
            return typeAnalyzer.Evaluate(typeSyntax);
        }

        //private void InferExpressionTypeInInvocation(ExpressionSyntax callee, FixedList<DataType> argumentTypes)
        //{
        //    switch (callee)
        //    {
        //        case NameSyntax identifierName:
        //        {
        //            var symbols = identifierName.LookupInContainingScope();
        //            symbols = ResolveOverload(symbols, null, argumentTypes);
        //            AssignReferencedSymbolAndType(identifierName, symbols);
        //        }
        //        break;
        //        case MemberAccessExpressionSyntax memberAccess:
        //        {
        //            var left = InferExpressionType(ref memberAccess.Expression);
        //            var containingSymbol = GetSymbolForType(left);
        //            var symbols = containingSymbol.Lookup(memberAccess.Member.Name);
        //            symbols = ResolveOverload(symbols, left, argumentTypes);
        //            memberAccess.Type = AssignReferencedSymbolAndType(memberAccess.Member, symbols);
        //        }
        //        break;
        //        default:
        //            throw new NotImplementedException();
        //    }
        //}

        private FixedList<IFunctionSymbol> ResolveOverload(
            FixedList<IFunctionSymbol> symbols,
            DataType selfType,
            FixedList<DataType> argumentTypes)
        {
            var expectedArgumentCount = argumentTypes.Count + (selfType != null ? 1 : 0);
            // Filter down to symbols that could possible match
            symbols = symbols.Where(f =>
            {
                if (f.Arity() != expectedArgumentCount)
                    return false;
                // TODO check compatibility of self type
                // TODO check compatibility over argument types

                return true;
            }).ToFixedList();
            // TODO Select most specific match
            return symbols;
        }
    }
}
