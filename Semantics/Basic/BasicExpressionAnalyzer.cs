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

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic
{
    public class BasicExpressionAnalyzer
    {
        private readonly CodeFile file;
        private readonly Diagnostics diagnostics;
        private readonly DataType selfType;
        private readonly DataType returnType;

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
                    throw NonExhaustiveMatchException.For(statement);
            }
        }

        private void ResolveTypesInVariableDeclaration(
            VariableDeclarationStatementSyntax variableDeclaration)
        {
            InferExpressionType(variableDeclaration.Initializer);

            DataType type;
            if (variableDeclaration.TypeExpression != null)
                type = CheckAndEvaluateTypeExpression(variableDeclaration.TypeExpression);
            else if (variableDeclaration.Initializer != null)
            {
                type = variableDeclaration.Initializer.Type;
                // Use the initializer type unless it is constant
                switch (type)
                {
                    case IntegerConstantType integerConstant:
                        var value = integerConstant.Value;
                        var byteCount = value.GetByteCount();
                        type = byteCount <= 4 ? DataType.Int : DataType.Int64;
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
                    variableDeclaration.Type = targetType.AsOwned();
                else
                {
                    if (!IsAssignableFrom(type, initializerType))
                        diagnostics.Add(TypeError.CannotConvert(file, variableDeclaration.Initializer, initializerType, type));

                    variableDeclaration.Type = type;
                }
            }
            else
                variableDeclaration.Type = type;
        }

        /// <summary>
        /// Create an implicit conversion if allowed and needed
        /// </summary>
        private void InsertImplicitConversionIfNeeded(
            ref ExpressionSyntax expression,
            DataType targetType)
        {
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
                        case FloatingPointType expectedType:
                            if (expressionType.Bits < expectedType.Bits)
                                expression = new ImplicitNumericConversionExpression(expression, expectedType);
                            break;
                    }
                }
                break;
                case FloatingPointType expressionType:
                {
                    if (targetType is FloatingPointType expectedType
                        && expressionType.Bits < expectedType.Bits)
                        expression = new ImplicitNumericConversionExpression(expression, expectedType);
                }
                break;
                case IntegerConstantType expressionType:
                    switch (targetType)
                    {
                        case SizedIntegerType expectedType:
                            var bits = expressionType.Value.GetByteCount() * 8;
                            var requireSigned = expressionType.Value < 0;
                            if (expectedType.Bits >= bits
                               && (!requireSigned || expectedType.IsSigned))
                                expression = new ImplicitNumericConversionExpression(expression, expectedType);
                            break;
                        case FloatingPointType expectedType:
                            throw new NotImplementedException();
                    }
                    break;
                case StringConstantType _:
                {
                    if (targetType is UserObjectType objectType)
                    {
                        var conversionOperators = objectType.Symbol.Lookup(SpecialName.OperatorStringLiteral);
                        if (conversionOperators.Count == 1) // TODO actually check we can call it
                        {
                            expression = new ImplicitLiteralConversionExpression(expression, objectType, conversionOperators.Single());
                        }
                        // TODO if there is more than one
                    }
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
            if (expression == null) return DataType.Unknown;

            switch (expression)
            {
                case ReturnExpressionSyntax returnExpression:
                    if (returnExpression.ReturnValue != null)
                    {
                        if (returnType == null)
                            throw new NotImplementedException("Null return type. Report an error?");

                        // If we return ownership, there is an implicit move
                        if (returnType is UserObjectType objectType && objectType.IsOwned)
                            InferMoveExpressionType(returnExpression.ReturnValue);
                        else
                            InferExpressionType(returnExpression.ReturnValue);
                        InsertImplicitConversionIfNeeded(ref returnExpression.ReturnValue, returnType);
                        var type = returnExpression.ReturnValue.Type;
                        if (!IsAssignableFrom(returnType, type))
                            diagnostics.Add(TypeError.CannotConvert(file,
                                returnExpression.ReturnValue, type, returnType));
                    }
                    else if (returnType == DataType.Never)
                        diagnostics.Add(TypeError.CantReturnFromNeverFunction(file, returnExpression.Span));
                    else if (returnType != DataType.Void)
                        diagnostics.Add(TypeError.ReturnExpressionMustHaveValue(file, returnExpression.Span, returnType));

                    return expression.Type = DataType.Never;
                case IntegerLiteralExpressionSyntax integerLiteral:
                    return expression.Type = new IntegerConstantType(integerLiteral.Value);
                case StringLiteralExpressionSyntax _:
                    return expression.Type = DataType.StringConstant;
                case BoolLiteralExpressionSyntax _:
                    return expression.Type = DataType.Bool;
                case BinaryExpressionSyntax binaryOperatorExpression:
                    return InferBinaryExpressionType(binaryOperatorExpression);
                case IdentifierNameSyntax identifierName:
                    return InferIdentifierNameType(identifierName, false);
                case UnaryExpressionSyntax unaryOperatorExpression:
                    return InferUnaryExpressionType(unaryOperatorExpression);
                case ReferenceLifetimeSyntax lifetimeType:
                    InferExpressionType(lifetimeType.ReferentTypeExpression);
                    if (!IsType(lifetimeType.ReferentTypeExpression.Type))
                        diagnostics.Add(TypeError.MustBeATypeExpression(file, lifetimeType.ReferentTypeExpression.Span));
                    return expression.Type = DataType.Type;
                case BlockSyntax blockExpression:
                    foreach (var statement in blockExpression.Statements)
                        ResolveTypesInStatement(statement);

                    return expression.Type = DataType.Void;// TODO assign the correct type to the block
                case NewObjectExpressionSyntax newObjectExpression:
                    return InferConstructorCallType(newObjectExpression);
                case PlacementInitExpressionSyntax placementInitExpression:
                    foreach (var argument in placementInitExpression.Arguments)
                        InferArgumentType(argument);

                    // TODO verify argument types against called function

                    return placementInitExpression.Type = CheckAndEvaluateTypeExpression(placementInitExpression.Initializer);
                case ForeachExpressionSyntax foreachExpression:
                    foreachExpression.Type =
                        CheckAndEvaluateTypeExpression(foreachExpression.TypeExpression);
                    InferExpressionType(foreachExpression.InExpression);

                    // TODO check the break types
                    InferExpressionType(foreachExpression.Block);
                    // TODO assign correct type to the expression
                    return expression.Type = DataType.Void;
                case WhileExpressionSyntax whileExpression:
                    CheckExpressionType(whileExpression.Condition, DataType.Bool);
                    InferExpressionType(whileExpression.Block);
                    // TODO assign correct type to the expression
                    return expression.Type = DataType.Void;
                case LoopExpressionSyntax loopExpression:
                    InferExpressionType(loopExpression.Block);
                    // TODO assign correct type to the expression
                    return expression.Type = DataType.Void;
                case InvocationSyntax invocation:
                    return InferInvocationType(invocation);
                case GenericNameSyntax genericName:
                {
                    foreach (var argument in genericName.Arguments)
                        InferExpressionType(argument.Value);

                    // TODO lookup the symbol and construct a type
                    throw new NotImplementedException();
                    //genericName.NameType.BeginFulfilling();
                    //var nameType = InferNameType(genericName);

                    //// TODO check that argument types match function type
                    //genericName.NameType.Fulfill(nameType);

                    //switch (nameType)
                    //{
                    //    case MetaFunctionType metaFunctionType:
                    //        return genericName.Type = metaFunctionType.ResultType;
                    //    case UnknownType _:
                    //        return genericName.Type = DataType.Unknown;
                    //    default:
                    //        throw NonExhaustiveMatchException.For(genericName.NameType);
                    //}
                }
                case RefTypeSyntax refType:
                    CheckAndEvaluateTypeExpression(refType.ReferencedType);
                    return refType.Type = DataType.Type;
                case UnsafeExpressionSyntax unsafeExpression:
                    InferExpressionType(unsafeExpression.Expression);
                    return unsafeExpression.Type = unsafeExpression.Expression.Type;
                case MutableExpressionSyntax mutableExpression:
                {
                    var expressionType = InferExpressionType(mutableExpression.Expression);
                    DataType type;
                    switch (expressionType)
                    {
                        case Metatype metatype:
                        {
                            type = DataType.Type; // It names/describes a type
                            if (!(metatype.Instance is UserObjectType objectType && objectType.DeclaredMutable))
                                diagnostics.Add(TypeError.TypeDeclaredImmutable(file, mutableExpression.Expression));
                            break;
                        }
                        case TypeType _:
                            // TODO we need to check whether it is valid to use `mut` on this
                            type = DataType.Type; // It names/describes a type
                            break;
                        default:
                            switch (expressionType)
                            {
                                // If it is already mutable we can't redeclare it mutable (i.e. `mut mut x` is error)
                                case UserObjectType objectType
                                    when objectType.Mutability.IsUpgradable:
                                    type = objectType.AsMutable();
                                    break;
                                default:
                                    diagnostics.Add(TypeError.ExpressionCantBeMutable(file,
                                        mutableExpression.Expression));
                                    type = expressionType;
                                    break;
                            }
                            break;
                    }

                    return mutableExpression.Type = type;
                }
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
                    return InferMemberAccessType(memberAccess);
                case BreakExpressionSyntax breakExpression:
                    InferExpressionType(breakExpression.Value);
                    return breakExpression.Type = DataType.Never;
                case AssignmentExpressionSyntax assignmentExpression:
                    var left = InferExpressionType(assignmentExpression.LeftOperand);
                    InferExpressionType(assignmentExpression.RightOperand);
                    InsertImplicitConversionIfNeeded(ref assignmentExpression.RightOperand, left);
                    var right = assignmentExpression.RightOperand.Type;
                    if (!IsAssignableFrom(left, right))
                        diagnostics.Add(TypeError.CannotConvert(file, assignmentExpression.RightOperand, right, left));
                    return assignmentExpression.Type = DataType.Void;
                case SelfExpressionSyntax _:
                    return selfType ?? DataType.Unknown;
                case MoveExpressionSyntax moveExpression:
                {
                    var type = InferMoveExpressionType(moveExpression.Expression);
                    if (type is ReferenceType referenceType && !referenceType.IsOwned)
                    {
                        diagnostics.Add(TypeError.CannotMoveBorrowedValue(file, moveExpression));
                        type = referenceType.WithLifetime(Lifetime.Owned);
                    }
                    if (type is UserObjectType objectType) type = objectType.AsOwnedUpgradable();
                    return moveExpression.Type = type;
                }
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
        }

        private DataType InferIdentifierNameType(IdentifierNameSyntax identifierName, bool isMove)
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
                    identifierName.ReferencedSymbol = symbols.Single();
                    type = symbols.Single().Type;
                    if (type is UserObjectType objectType)
                    {
                        // A bare variable reference doesn't default to mutable
                        if (objectType.Mutability == Mutability.Mutable)
                            type = objectType = objectType.AsExplicitlyUpgradable();
                        // A bare variable reference doesn't default to owned
                        if (!isMove && objectType.IsOwned) type = objectType.WithLifetime(Lifetime.None);
                    }

                    break;
                default:
                    diagnostics.Add(NameBindingError.AmbiguousName(file, identifierName.Span));
                    identifierName.ReferencedSymbol = UnknownSymbol.Instance;
                    type = DataType.Unknown;
                    break;
            }

            return identifierName.Type = type;
        }

        private DataType InferConstructorCallType(NewObjectExpressionSyntax expression)
        {
            var argumentTypes = expression.Arguments.Select(InferArgumentType).ToFixedList();
            // TODO handle named constructors here
            var constructedType = (UserObjectType)CheckAndEvaluateTypeExpression(expression.Constructor);
            var typeSymbol = GetSymbolForType(constructedType);
            var constructors = typeSymbol.ChildSymbols[SpecialName.New];
            constructors = ResolveOverload(constructors, null, argumentTypes);
            switch (constructors.Count)
            {
                case 0:
                    diagnostics.Add(NameBindingError.CouldNotBindConstructor(file, expression.Span));
                    expression.ConstructorSymbol = UnknownSymbol.Instance;
                    expression.ConstructorType = DataType.Unknown;
                    break;
                case 1:
                    var constructorSymbol = constructors.Single();
                    expression.ConstructorSymbol = constructorSymbol;
                    var constructorType = constructorSymbol.Type;
                    expression.ConstructorType = constructorType;
                    if (constructorType is FunctionType functionType)
                        foreach (var (arg, type) in expression.Arguments.Zip(functionType.ParameterTypes))
                        {
                            InsertImplicitConversionIfNeeded(ref arg.Value, type);
                            CheckArgumentTypeCompatibility(type, arg);
                        }
                    break;
                default:
                    diagnostics.Add(NameBindingError.AmbiguousConstructor(file, expression.Span));
                    expression.ConstructorSymbol = UnknownSymbol.Instance;
                    expression.ConstructorType = DataType.Unknown;
                    break;
            }

            return expression.Type = constructedType.AsOwnedUpgradable();
        }

        private DataType InferMoveExpressionType(ExpressionSyntax expression)
        {
            if (expression == null) return DataType.Unknown;

            switch (expression)
            {
                case IdentifierNameSyntax identifierName:
                    return InferIdentifierNameType(identifierName, true);
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
        }

        private static bool IsType(DataType dataType)
        {
            switch (dataType)
            {
                case Metatype _:
                case DataType t when t == DataType.Type:
                    return true;
                default:
                    return false;
            }
        }

        private DataType InferInvocationType(InvocationSyntax invocation)
        {
            // This could:
            // * Invoke a stand alone function
            // * Invoke a static function
            // * Invoke a method
            // * Invoke a function pointer
            var argumentTypes = invocation.Arguments.Select(InferArgumentType).ToFixedList();
            InferExpressionTypeInInvocation(invocation.Callee, argumentTypes);
            var callee = invocation.Callee.Type;

            if (callee is FunctionType functionType)
            {
                foreach (var (arg, type) in invocation.Arguments.Zip(functionType.ParameterTypes))
                {
                    InsertImplicitConversionIfNeeded(ref arg.Value, type);
                    CheckArgumentTypeCompatibility(type, arg);
                }

                return invocation.Type = functionType.ReturnType;
            }

            // If it is unknown, we already reported an error
            if (callee == DataType.Unknown) return invocation.Type = DataType.Unknown;

            diagnostics.Add(TypeError.MustBeCallable(file, invocation.Callee));
            return invocation.Type = DataType.Unknown;
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
            if (target.Equals(source)) return true;
            if (target is UserObjectType targetReference && source is UserObjectType sourceReference)
            {
                if (targetReference.IsOwned && !sourceReference.IsOwned) return false;

                if (!targetReference.Mutability.IsAssignableFrom(sourceReference.Mutability))
                    return false;

                // If they are equal except for lifetimes and mutability compatible, it is fine
                return targetReference.EqualExceptLifetimeAndMutability(sourceReference);
            }

            return false;
        }

        private DataType InferMemberAccessType(MemberAccessExpressionSyntax memberAccess)
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

        private static ISymbol GetSymbolForType(DataType type)
        {
            switch (type)
            {
                case UnknownType _:
                    return UnknownSymbol.Instance;
                case UserObjectType objectType:
                    return objectType.Symbol;
                case SizedIntegerType integerType:
                    // TODO this seems a very strange way to handle this. Shouldn't the symbol be on the type?
                    return PrimitiveSymbols.Instance.Single(p => p.FullName == integerType.Name);
                default:
                    throw NonExhaustiveMatchException.For(type);
            }
        }

        private DataType AssignReferencedSymbolAndType(
            NameSyntax identifier,
            FixedList<ISymbol> memberSymbols)
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

        private DataType InferArgumentType(ArgumentSyntax argument)
        {
            return InferExpressionType(argument.Value);
        }

        private DataType InferBinaryExpressionType(
            BinaryExpressionSyntax binaryExpression)
        {
            InferExpressionType(binaryExpression.LeftOperand);
            var leftType = binaryExpression.LeftOperand.Type;
            var @operator = binaryExpression.Operator;
            InferExpressionType(binaryExpression.RightOperand);
            var rightType = binaryExpression.RightOperand.Type;

            // If either is unknown, then we can't know whether there is a a problem.
            // Note that the operator could be overloaded
            if (leftType == DataType.Unknown || rightType == DataType.Unknown)
                return binaryExpression.Type = DataType.Unknown;

            bool compatible;
            switch (@operator)
            {
                case BinaryOperator.Plus:
                case BinaryOperator.Minus:
                case BinaryOperator.Asterisk:
                case BinaryOperator.Slash:
                    compatible = NumericOperatorTypesAreCompatible(ref binaryExpression.LeftOperand, ref binaryExpression.RightOperand, null);
                    binaryExpression.Type = compatible ? leftType : DataType.Unknown;
                    break;
                case BinaryOperator.EqualsEquals:
                case BinaryOperator.NotEqual:
                case BinaryOperator.LessThan:
                case BinaryOperator.LessThanOrEqual:
                case BinaryOperator.GreaterThan:
                case BinaryOperator.GreaterThanOrEqual:
                    compatible = (leftType == DataType.Bool && rightType == DataType.Bool)
                        || NumericOperatorTypesAreCompatible(ref binaryExpression.LeftOperand, ref binaryExpression.RightOperand, null);
                    binaryExpression.Type = DataType.Bool;
                    break;
                case BinaryOperator.And:
                case BinaryOperator.Or:
                    compatible = leftType == DataType.Bool && rightType == DataType.Bool;
                    binaryExpression.Type = DataType.Bool;
                    break;
                default:
                    throw NonExhaustiveMatchException.ForEnum(@operator);
            }
            if (!compatible)
                diagnostics.Add(TypeError.OperatorCannotBeAppliedToOperandsOfType(file,
                    binaryExpression.Span, @operator,
                    binaryExpression.LeftOperand.Type,
                    binaryExpression.RightOperand.Type));

            return binaryExpression.Type;
        }

        private bool NumericOperatorTypesAreCompatible(
            ref ExpressionSyntax leftOperand,
            ref ExpressionSyntax rightOperand,
            DataType resultType)
        {
            var leftType = leftOperand.Type;
            switch (leftType)
            {
                case PointerType _:
                {
                    // TODO it may need to be size
                    throw new NotImplementedException();
                }
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
                case UserObjectType _:
                case BoolType _:
                case VoidType _: // This might need a special error message
                case StringConstantType _: // String concatenation will be handled outside this function
                                           // Other object types can't be used in numeric expressions
                    return false;
                default:
                    // In theory we could just return false here, but this way we are forced to note
                    // exactly which types this doesn't work on.
                    throw NonExhaustiveMatchException.For(leftType);
            }
        }

        private DataType InferUnaryExpressionType(UnaryExpressionSyntax unaryExpression)
        {
            var operandType = InferExpressionType(unaryExpression.Operand);
            var @operator = unaryExpression.Operator;

            // If either is unknown, then we can't know whether there is a a problem
            // (technically not true, for example, we could know that one arg should
            // be a bool and isn't)
            if (operandType == DataType.Unknown)
                return unaryExpression.Type = DataType.Unknown;

            bool typeError;
            switch (@operator)
            {
                case UnaryOperator.Not:
                    typeError = operandType != DataType.Bool;
                    unaryExpression.Type = DataType.Bool;
                    break;
                case UnaryOperator.At:
                    typeError = false; // TODO check that the expression can have a pointer taken
                    if (operandType is Metatype)
                        unaryExpression.Type = DataType.Type; // constructing a type
                    else
                        unaryExpression.Type = new PointerType(operandType); // taking the address of something
                    break;
                case UnaryOperator.Question:
                    typeError = false; // TODO check that the expression can have a pointer taken
                    unaryExpression.Type = new PointerType(operandType);
                    break;
                case UnaryOperator.Caret:
                    switch (operandType)
                    {
                        case PointerType pointerType:
                            unaryExpression.Type = pointerType.Referent;
                            typeError = false;
                            break;
                        default:
                            unaryExpression.Type = DataType.Unknown;
                            typeError = true;
                            break;
                    }
                    break;
                case UnaryOperator.Minus:
                    switch (operandType)
                    {
                        case IntegerConstantType integerType:
                            typeError = false;
                            unaryExpression.Type = integerType;
                            break;
                        case SizedIntegerType sizedIntegerType:
                            typeError = false;
                            unaryExpression.Type = sizedIntegerType;
                            break;
                        default:
                            unaryExpression.Type = DataType.Unknown;
                            typeError = true;
                            break;
                    }
                    break;
                default:
                    throw NonExhaustiveMatchException.ForEnum(@operator);
            }
            if (typeError)
                diagnostics.Add(TypeError.OperatorCannotBeAppliedToOperandOfType(file,
                    unaryExpression.Span, @operator, operandType));

            return unaryExpression.Type;
        }

        /// <summary>
        /// Evaluates a type expression to the type it identifies
        /// </summary>
        public DataType CheckAndEvaluateTypeExpression(ExpressionSyntax typeExpression)
        {
            if (typeExpression == null) return DataType.Unknown;

            var type = InferExpressionType(typeExpression);
            if (type is UnknownType) return DataType.Unknown;
            if (!IsType(type))
            {
                diagnostics.Add(TypeError.MustBeATypeExpression(file, typeExpression.Span));
                return DataType.Unknown;
            }

            return TypeExpressionEvaluator.CheckExpression(typeExpression);
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
                    throw NonExhaustiveMatchException.For(callee);
            }
        }

        private FixedList<ISymbol> ResolveOverload(FixedList<ISymbol> symbols, DataType selfType, FixedList<DataType> argumentTypes)
        {
            // Filter down to symbols that could possible match
            symbols = symbols.Where(s =>
            {
                if (s.Type is FunctionType functionType)
                {
                    if (functionType.Arity != argumentTypes.Count) return false;
                    // TODO check compatibility of self type
                    // TODO check compatibility over argument types
                }

                return true;
            }).ToFixedList();
            // TODO Select most specific match
            return symbols;
        }
    }
}
