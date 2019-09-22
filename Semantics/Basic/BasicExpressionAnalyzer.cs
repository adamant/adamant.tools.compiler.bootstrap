using System;
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
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic
{
    public class BasicExpressionAnalyzer
    {
        private readonly CodeFile file;
        private readonly Diagnostics diagnostics;
        private readonly DataType selfType;
        private readonly DataType returnType;
        private readonly BasicTypeAnalyzer typeAnalyzer;

        public BasicExpressionAnalyzer(
            CodeFile file,
            Diagnostics diagnostics,
            DataType selfType = null,
            DataType returnType = null)
        {
            this.file = file;
            this.diagnostics = diagnostics;
            this.returnType = returnType;
            this.selfType = selfType;
            typeAnalyzer = new BasicTypeAnalyzer(file, diagnostics, this);
        }

        public void ResolveTypesInStatement(StatementSyntax statement)
        {
            switch (statement)
            {
                case VariableDeclarationStatementSyntax variableDeclaration:
                    ResolveTypesInVariableDeclaration(variableDeclaration);
                    break;
                case ExpressionSyntax expression:
                    InferExpressionType(expression);
                    break;
                default:
                    throw ExhaustiveMatch.Failed(statement);
            }
        }

        private void ResolveTypesInVariableDeclaration(
            VariableDeclarationStatementSyntax variableDeclaration)
        {
            InferExpressionType(variableDeclaration.Initializer);

            DataType type;
            if (variableDeclaration.TypeSyntax != null)
                type = typeAnalyzer.Evaluate(variableDeclaration.TypeSyntax);
            else if (variableDeclaration.Initializer != null)
            {
                type = variableDeclaration.Initializer.Type;
                // Use the initializer type unless it is constant
                switch (type)
                {
                    case IntegerConstantType integerConstant:
                        var value = integerConstant.Value;
                        var byteCount = value.GetByteCount();
                        //type = byteCount <= 4 ? DataType.Int : DataType.Int64;
                        type = DataType.Int;
                        throw new NotImplementedException("account for small integer constants");
                        break;
                    case StringConstantType stringConstant:
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
                InsertImplicitConversionIfNeeded(ref variableDeclaration.Initializer, type);
                var initializerType = variableDeclaration.Initializer.Type;
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
            ref ExpressionSyntax expression,
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
                {
                    switch (targetType)
                    {
                        case SizedIntegerType expectedType:
                            if (expectedType.Bits > expressionType.Bits
                                && (!expressionType.IsSigned || expectedType.IsSigned))
                                expression = new ImplicitNumericConversionExpression(expression, expectedType);
                            break;
                    }
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
                            .Cast<IFunctionSymbol>().Single(c => c.Parameters.Count() == 1);
                        expression = new ImplicitLiteralConversionExpression(expression, targetType,
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
            ExpressionSyntax expression,
            DataType expectedType)
        {
            var actualType = InferExpressionType(expression);
            // TODO check for type compatibility not equality
            if (!expectedType.Equals(actualType))
                diagnostics.Add(TypeError.CannotConvert(file, expression, actualType, expectedType));
            return actualType;
        }

        private DataType InferExpressionType(ExpressionSyntax expression)
        {
            switch (expression)
            {
                case ReturnExpressionSyntax returnExpression:
                {
                    if (returnExpression.ReturnValue != null)
                    {
                        if (returnType == null)
                            throw new NotImplementedException("Null return type. Report an error?");

                        // If we return ownership, there is an implicit move
                        if (returnType is UserObjectType objectType && objectType.IsOwned)
                            InferMoveExpressionType(returnExpression.ReturnValue);
                        else
                            InferExpressionType(returnExpression.ReturnValue);
                        InsertImplicitConversionIfNeeded(ref returnExpression.ReturnValue,
                            returnType);
                        var type = returnExpression.ReturnValue.Type;
                        if (!IsAssignableFrom(returnType, type))
                            diagnostics.Add(TypeError.CannotConvert(file,
                                returnExpression.ReturnValue, type, returnType));
                    }
                    else if (returnType == DataType.Never)
                        diagnostics.Add(
                            TypeError.CantReturnFromNeverFunction(file, returnExpression.Span));
                    else if (returnType != DataType.Void)
                        diagnostics.Add(TypeError.ReturnExpressionMustHaveValue(file,
                            returnExpression.Span, returnType));

                    return returnExpression.Type = DataType.Never;
                }
                case IntegerLiteralExpressionSyntax integerLiteral:
                    return integerLiteral.Type = new IntegerConstantType(integerLiteral.Value);
                case StringLiteralExpressionSyntax stringLiteral:
                    return stringLiteral.Type = DataType.StringConstant;
                case BoolLiteralExpressionSyntax boolLiteral:
                    return boolLiteral.Type = DataType.Bool;
                case BinaryExpressionSyntax binaryOperatorExpression:
                {
                    InferExpressionType(binaryOperatorExpression.LeftOperand);
                    var leftType = binaryOperatorExpression.LeftOperand.Type;
                    var @operator = binaryOperatorExpression.Operator;
                    InferExpressionType(binaryOperatorExpression.RightOperand);
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
                case IdentifierNameSyntax identifierName:
                    return InferIdentifierNameType(identifierName, false);
                case UnaryExpressionSyntax unaryOperatorExpression:
                {
                    var operandType = InferExpressionType(unaryOperatorExpression.Operand);
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
                case BlockSyntax blockExpression:
                    foreach (var statement in blockExpression.Statements)
                        ResolveTypesInStatement(statement);

                    return blockExpression.Type = DataType.Void; // TODO assign the correct type to the block
                case NewObjectExpressionSyntax newObjectExpression:
                {
                    var argumentTypes = newObjectExpression.Arguments.Select(InferArgumentType).ToFixedList();
                    // TODO handle named constructors here
                    var constructingType = typeAnalyzer.Evaluate(newObjectExpression.Constructor);
                    if (!constructingType.IsKnown)
                    {
                        diagnostics.Add(NameBindingError.CouldNotBindConstructor(file, newObjectExpression.Span));
                        newObjectExpression.ConstructorSymbol = UnknownSymbol.Instance;
                        return newObjectExpression.Type = newObjectExpression.ConstructorType = DataType.Unknown;
                    }
                    var constructedType = (UserObjectType)constructingType;
                    var typeSymbol = GetSymbolForType(constructedType);
                    var constructors = typeSymbol.ChildSymbols[SpecialName.New];
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
                            diagnostics.Add(NameBindingError.AmbiguousConstructor(file, newObjectExpression.Span));
                            newObjectExpression.ConstructorSymbol = UnknownSymbol.Instance;
                            newObjectExpression.ConstructorType = DataType.Unknown;
                            break;
                    }

                    return newObjectExpression.Type = constructedType.AsOwnedUpgradable();
                }
                case ForeachExpressionSyntax foreachExpression:
                {
                    var declaredType = typeAnalyzer.Evaluate(foreachExpression.TypeSyntax);
                    CheckForeachInType(declaredType, foreachExpression.InExpression);
                    foreachExpression.VariableType =
                        declaredType ?? foreachExpression.InExpression.Type;
                    // TODO check the break types
                    InferExpressionType(foreachExpression.Block);
                    // TODO assign correct type to the expression
                    return foreachExpression.Type = DataType.Void;
                }
                case WhileExpressionSyntax whileExpression:
                {
                    CheckExpressionType(whileExpression.Condition, DataType.Bool);
                    InferExpressionType(whileExpression.Block);
                    // TODO assign correct type to the expression
                    return whileExpression.Type = DataType.Void;
                }
                case LoopExpressionSyntax loopExpression:
                    InferExpressionType(loopExpression.Block);
                    // TODO assign correct type to the expression
                    return loopExpression.Type = DataType.Void;
                case InvocationSyntax invocation:
                {
                    // This could:
                    // * Invoke a stand alone function
                    // * Invoke a static function
                    // * Invoke a method
                    // * Invoke a function pointer
                    var argumentTypes =
                        invocation.Arguments.Select(InferArgumentType).ToFixedList();
                    InferExpressionTypeInInvocation(invocation.Callee, argumentTypes);
                    var callee = invocation.Callee.Type;

                    //if (callee is FunctionType functionType)
                    //{
                    //    foreach (var (arg, type) in invocation.Arguments.Zip(functionType
                    //        .ParameterTypes))
                    //    {
                    //        InsertImplicitConversionIfNeeded(ref arg.Value, type);
                    //        CheckArgumentTypeCompatibility(type, arg);
                    //    }

                    //    return invocation.Type = functionType.ReturnType;
                    //}

                    // If it is unknown, we already reported an error
                    if (callee == DataType.Unknown)
                        return invocation.Type = DataType.Unknown;

                    diagnostics.Add(TypeError.MustBeCallable(file, invocation.Callee));
                    return invocation.Type = DataType.Unknown;
                }
                case UnsafeExpressionSyntax unsafeExpression:
                    InferExpressionType(unsafeExpression.Expression);
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
                case IfExpressionSyntax ifExpression:
                    CheckExpressionType(ifExpression.Condition, DataType.Bool);
                    InferExpressionType(ifExpression.ThenBlock);
                    InferExpressionType(ifExpression.ElseClause);
                    // TODO assign a type to the expression
                    return ifExpression.Type = DataType.Void;
                case ResultExpressionSyntax resultExpression:
                    InferExpressionType(resultExpression.Expression);
                    return resultExpression.Type = DataType.Never;
                case MemberAccessExpressionSyntax memberAccess:
                {
                    var left = InferExpressionType(memberAccess.Expression);
                    var symbol = GetSymbolForType(left);

                    switch (memberAccess.Member)
                    {
                        case IdentifierNameSyntax identifier:
                            var memberSymbols = symbol.Lookup(identifier.Name);
                            var type = AssignReferencedSymbolAndType(identifier, memberSymbols);
                            return memberAccess.Type = type;
                        default:
                            throw NonExhaustiveMatchException.For(memberAccess.Member);
                    }
                }
                case BreakExpressionSyntax breakExpression:
                    InferExpressionType(breakExpression.Value);
                    return breakExpression.Type = DataType.Never;
                case NextExpressionSyntax nextExpression:
                    return nextExpression.Type = DataType.Never;
                case AssignmentExpressionSyntax assignmentExpression:
                {
                    var left = InferExpressionType(assignmentExpression.LeftOperand);
                    InferExpressionType(assignmentExpression.RightOperand);
                    InsertImplicitConversionIfNeeded(ref assignmentExpression.RightOperand, left);
                    var right = assignmentExpression.RightOperand.Type;
                    if (!IsAssignableFrom(left, right))
                        diagnostics.Add(TypeError.CannotConvert(file,
                            assignmentExpression.RightOperand, right, left));
                    return assignmentExpression.Type = DataType.Void;
                }
                case SelfExpressionSyntax selfExpression:
                    return selfExpression.Type = selfType ?? DataType.Unknown;
                case MoveExpressionSyntax moveExpression:
                {
                    var type = InferMoveExpressionType(moveExpression.Expression);
                    if (type is ReferenceType referenceType && !referenceType.IsOwned)
                    {
                        diagnostics.Add(TypeError.CannotMoveBorrowedValue(file, moveExpression));
                        type = referenceType.WithLifetime(Lifetime.Owned);
                    }

                    if (type is UserObjectType objectType)
                        type = objectType.AsOwnedUpgradable();
                    return moveExpression.Type = type;
                }
                case NoneLiteralExpressionSyntax noneLiteralExpression:
                    return noneLiteralExpression.Type = DataType.None;
                case ImplicitConversionExpression _:
                    throw new Exception("ImplicitConversionExpressions are inserted by BasicExpressionAnalyzer. They should not be present in the AST yet.");
                case LifetimeExpressionSyntax _:
                    throw new Exception("Should be inferring type of type expression");
                case null:
                    return DataType.Unknown;
                default:
                    throw ExhaustiveMatch.Failed(expression);
            }
        }

        public DataType InferIdentifierNameType(IdentifierNameSyntax identifierName, bool isMove)
        {
            var symbols = identifierName.LookupInContainingScope();
            DataType type;
            switch (symbols.Count)
            {
                case 0:
                    diagnostics.Add(NameBindingError.CouldNotBindName(file, identifierName.Span));
                    identifierName.ReferencedSymbol = UnknownSymbol.Instance;
                    type = DataType.Unknown;
                    break;
                case 1:
                {
                    var symbol = symbols.Single();
                    identifierName.ReferencedSymbol = symbol;
                    switch (symbol)
                    {
                        case IBindingSymbol binding:
                        {
                            type = binding.Type;
                            if (type is UserObjectType objectType)
                            {
                                // A bare variable reference doesn't default to mutable
                                if (objectType.Mutability == Mutability.Mutable)
                                    type = objectType = objectType.AsExplicitlyUpgradable();
                                // A bare variable reference doesn't default to owned
                                if (!isMove && objectType.IsOwned)
                                    type = objectType.WithLifetime(Lifetime.None);
                                //}
                                throw new NotImplementedException();
                            }
                        }
                        break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                }
                default:
                    diagnostics.Add(NameBindingError.AmbiguousName(file, identifierName.Span));
                    identifierName.ReferencedSymbol = UnknownSymbol.Instance;
                    type = DataType.Unknown;
                    break;
            }

            return identifierName.Type = type;
        }

        private DataType InferMoveExpressionType(ExpressionSyntax expression)
        {
            if (expression == null)
                return DataType.Unknown;

            switch (expression)
            {
                case IdentifierNameSyntax identifierName:
                    return InferIdentifierNameType(identifierName, true);
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
        private DataType CheckForeachInType(DataType declaredType, ExpressionSyntax inExpression)
        {
            switch (inExpression)
            {
                case BinaryExpressionSyntax binaryExpression when binaryExpression.Operator == BinaryOperator.DotDot:
                    InferExpressionType(binaryExpression.LeftOperand);
                    InferExpressionType(binaryExpression.RightOperand);
                    if (declaredType != null)
                    {
                        InsertImplicitConversionIfNeeded(ref binaryExpression.LeftOperand, declaredType);
                        InsertImplicitConversionIfNeeded(ref binaryExpression.RightOperand, declaredType);
                    }
                    return inExpression.Type = binaryExpression.LeftOperand.Type;
                default:
                    return InferExpressionType(inExpression);
            }
        }

        private void CheckArgumentTypeCompatibility(DataType type, ArgumentSyntax arg)
        {
            var fromType = arg.Value.Type;
            if (!IsAssignableFrom(type, fromType))
                diagnostics.Add(TypeError.CannotConvert(file, arg.Value, fromType, type));
        }

        /// <summary>
        /// Tests whether a variable of the target type could be assigned from a variable of the source type
        /// </summary>
        private static bool IsAssignableFrom(DataType target, DataType source)
        {
            if (target.Equals(source))
                return true;
            if (target is UserObjectType targetReference && source is UserObjectType sourceReference)
            {
                if (targetReference.IsOwned && !sourceReference.IsOwned)
                    return false;

                if (!targetReference.Mutability.IsAssignableFrom(sourceReference.Mutability))
                    return false;

                // If they are equal except for lifetimes and mutability compatible, it is fine
                return targetReference.EqualExceptLifetimeAndMutability(sourceReference);
            }

            if (target is OptionalType targetOptional && source is OptionalType sourceOptional)
                return IsAssignableFrom(targetOptional.Referent, sourceOptional.Referent);

            return false;
        }

        private static ISymbol GetSymbolForType(DataType type)
        {
            switch (type)
            {
                case UnknownType _:
                    return UnknownSymbol.Instance;
                case UserObjectType objectType:
                    return objectType.Symbol;
                case SizedIntegerType integerType:
                    return PrimitiveSymbols.Instance.Single(p => p.FullName == integerType.Name);
                case UnsizedIntegerType integerType:
                    return PrimitiveSymbols.Instance.Single(p => p.FullName == integerType.Name);
                default:
                    throw new NotImplementedException();
            }
        }

        private DataType AssignReferencedSymbolAndType(
            IdentifierNameSyntax identifier,
            FixedList<ISymbol> memberSymbols)
        {
            switch (memberSymbols.Count)
            {
                case 0:
                    diagnostics.Add(NameBindingError.CouldNotBindMember(file, identifier.Span));
                    identifier.ReferencedSymbol = UnknownSymbol.Instance;
                    return identifier.Type = DataType.Unknown;
                case 1:
                    //var memberSymbol = memberSymbols.Single();
                    //identifier.ReferencedSymbol = memberSymbol;
                    //return identifier.Type = memberSymbol.Type;
                    throw new NotImplementedException();
                default:
                    diagnostics.Add(NameBindingError.AmbiguousName(file, identifier.Span));
                    identifier.ReferencedSymbol = UnknownSymbol.Instance;
                    return identifier.Type = DataType.Unknown;
            }
        }

        private DataType InferArgumentType(ArgumentSyntax argument)
        {
            return InferExpressionType(argument.Value);
        }

        private bool NumericOperatorTypesAreCompatible(
            ref ExpressionSyntax leftOperand,
            ref ExpressionSyntax rightOperand,
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
        public DataType EvaluateType(TypeSyntax typeSyntax)
        {
            return typeAnalyzer.Evaluate(typeSyntax);
        }

        private void InferExpressionTypeInInvocation(ExpressionSyntax callee, FixedList<DataType> argumentTypes)
        {
            switch (callee)
            {
                case IdentifierNameSyntax identifierName:
                {
                    var symbols = identifierName.LookupInContainingScope();
                    symbols = ResolveOverload(symbols, null, argumentTypes);
                    AssignReferencedSymbolAndType(identifierName, symbols);
                }
                break;
                case MemberAccessExpressionSyntax memberAccess:
                {
                    var left = InferExpressionType(memberAccess.Expression);
                    var containingSymbol = GetSymbolForType(left);
                    var symbols = containingSymbol.Lookup(memberAccess.Member.Name);
                    symbols = ResolveOverload(symbols, left, argumentTypes);
                    memberAccess.Type = AssignReferencedSymbolAndType(memberAccess.Member, symbols);
                }
                break;
                default:
                    throw new NotImplementedException();
            }
        }

        private FixedList<ISymbol> ResolveOverload(FixedList<ISymbol> symbols, DataType selfType, FixedList<DataType> argumentTypes)
        {
            // Filter down to symbols that could possible match
            symbols = symbols.Where(s =>
            {
                //if (s.Type is FunctionType functionType)
                //{
                //    if (functionType.Arity != argumentTypes.Count)
                //        return false;
                //    // TODO check compatibility of self type
                //    // TODO check compatibility over argument types
                //}

                return true;
            }).ToFixedList();
            // TODO Select most specific match
            return symbols;
        }
    }
}
