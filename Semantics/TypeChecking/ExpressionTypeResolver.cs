using System;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Primitives;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.TypeChecking
{
    public class ExpressionTypeResolver
    {
        private readonly CodeFile file;
        private readonly Diagnostics diagnostics;
        private readonly DataType selfType;
        private readonly DataType returnType;

        public ExpressionTypeResolver(
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

        public void InferStatementType(StatementSyntax statement)
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
            VariableDeclarationStatementSyntax variableDeclaration)
        {
            variableDeclaration.Type.BeginFulfilling();
            if (variableDeclaration.Initializer != null)
                InferExpressionType(variableDeclaration.Initializer);

            if (variableDeclaration.TypeExpression != null)
            {
                var type = CheckAndEvaluateTypeExpression(variableDeclaration.TypeExpression);
                variableDeclaration.Type.Fulfill(type);
                variableDeclaration.Initializer = ImplicitConversion(variableDeclaration.Initializer, type);
                // TODO check that the initializer type is compatible with the variable type
            }
            else if (variableDeclaration.Initializer != null)
            {
                // We'll assume the expression type is it
                variableDeclaration.Type.Fulfill(variableDeclaration.Initializer.Type);
            }
            else
            {
                diagnostics.Add(TypeError.NotImplemented(file,
                    variableDeclaration.NameSpan,
                    "Inference of local variable types not implemented"));
                variableDeclaration.Type.Fulfill(DataType.Unknown);
            }
        }

        /// <summary>
        /// Create an implicit conversion if allowed and needed
        /// </summary>
        private ExpressionSyntax ImplicitConversion(
            ExpressionSyntax expression,
            DataType targetType)
        {
            if (expression == null) return null;

            switch (expression.Type)
            {
                case SizedIntegerType expressionType:
                {
                    switch (targetType)
                    {
                        case SizedIntegerType expectedType:
                            if (expectedType.Bits > expressionType.Bits
                                && (!expressionType.IsSigned || expectedType.IsSigned))
                                return new ImplicitNumericConversionExpression(expression, expectedType);
                            break;
                        case FloatingPointType expectedType:
                            if (expressionType.Bits < expectedType.Bits)
                                return new ImplicitNumericConversionExpression(expression, expectedType);
                            break;
                    }
                }
                break;
                case FloatingPointType expressionType:
                {
                    if (targetType is FloatingPointType expectedType
                        && expressionType.Bits < expectedType.Bits)
                        return new ImplicitNumericConversionExpression(expression, expectedType);
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
                                return new ImplicitNumericConversionExpression(expression, expectedType);
                            break;
                        case FloatingPointType expectedType:
                            throw new NotImplementedException();
                    }
                    break;
                case StringConstantType expressionType:
                {
                    if (targetType is ObjectType objectType)
                    {
                        var conversionOperators = objectType.Symbol.Lookup(SpecialName.OperatorStringLiteral);
                        if (conversionOperators.Count == 1) // TODO actually check we can call it
                        {
                            return new ImplicitLiteralConversionExpression(expression, objectType, conversionOperators.Single());
                        }
                        // TODO if there is more than one
                    }
                }
                break;
            }

            // No conversion
            return expression;
        }

        public DataType CheckExpressionType(
            ExpressionSyntax expression,
            DataType expectedType)
        {
            var actualType = InferExpressionType(expression);
            // TODO check for type compatibility not equality
            if (!expectedType.Equals(actualType))
                diagnostics.Add(TypeError.CannotConvert(file, expression, expectedType));
            return actualType;
        }

        public DataType InferExpressionType(ExpressionSyntax expression)
        {
            if (expression == null) return DataType.Unknown;

            switch (expression)
            {
                case ReturnExpressionSyntax returnExpression:
                    if (returnExpression.ReturnValue != null)
                    {
                        InferExpressionType(returnExpression.ReturnValue);
                        if (returnType != null) // TODO report an error
                        {
                            returnExpression.ReturnValue = ImplicitConversion(returnExpression.ReturnValue, returnType);
                            if (returnType != returnExpression.ReturnValue.Type)
                                diagnostics.Add(TypeError.CannotConvert(file,
                                    returnExpression.ReturnValue, returnType));
                        }
                    }
                    else
                    {
                        // TODO a void or never function shouldn't have this
                    }
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
                            break;
                        default:
                            diagnostics.Add(NameBindingError.AmbiguousName(file, identifierName.Span));
                            identifierName.ReferencedSymbol = UnknownSymbol.Instance;
                            type = DataType.Unknown;
                            break;
                    }

                    return identifierName.Type = type;
                }
                case UnaryExpressionSyntax unaryOperatorExpression:
                    return InferUnaryExpressionType(unaryOperatorExpression);
                case LifetimeTypeSyntax lifetimeType:
                    InferExpressionType(lifetimeType.ReferentTypeExpression);
                    if (lifetimeType.ReferentTypeExpression.Type != DataType.Type)
                        diagnostics.Add(TypeError.MustBeATypeExpression(file, lifetimeType.ReferentTypeExpression.Span));
                    return expression.Type = DataType.Type;
                case BlockSyntax blockExpression:
                    foreach (var statement in blockExpression.Statements)
                        InferStatementType(statement);

                    return expression.Type = DataType.Void;// TODO assign the correct type to the block
                case NewObjectExpressionSyntax newObjectExpression:
                    foreach (var argument in newObjectExpression.Arguments)
                        CheckArgument(argument);
                    // TODO verify argument types against called function
                    return expression.Type = CheckAndEvaluateTypeExpression(newObjectExpression.Constructor);
                case PlacementInitExpressionSyntax placementInitExpression:
                    foreach (var argument in placementInitExpression.Arguments)
                        CheckArgument(argument);

                    // TODO verify argument types against called function

                    return placementInitExpression.Type = CheckAndEvaluateTypeExpression(placementInitExpression.Initializer);
                case ForeachExpressionSyntax foreachExpression:
                    foreachExpression.Type =
                        CheckAndEvaluateTypeExpression(foreachExpression.TypeExpression);
                    InferExpressionType(foreachExpression.InExpression);

                    // TODO check the break types
                    InferExpressionType(foreachExpression.Block);
                    // TODO assign correct type to the expression
                    return expression.Type = DataType.Unknown;
                case WhileExpressionSyntax whileExpression:
                    CheckExpressionType(whileExpression.Condition, DataType.Bool);
                    InferExpressionType(whileExpression.Block);
                    // TODO assign correct type to the expression
                    return expression.Type = DataType.Unknown;
                case LoopExpressionSyntax loopExpression:
                    InferExpressionType(loopExpression.Block);
                    // TODO assign correct type to the expression
                    return expression.Type = DataType.Unknown;
                case InvocationSyntax invocation:
                {
                    // This could:
                    // * Invoke a stand alone function
                    // * Invoke a static function
                    // * Invoke a method
                    // * Invoke a function pointer
                    var argumentTypes = invocation.Arguments.Select(a => InferExpressionType(a.Value)).ToFixedList();
                    InferExpressionTypeInInvocation(invocation.Callee, argumentTypes);
                    var callee = invocation.Callee.Type;

                    if (callee is FunctionType functionType)
                    {
                        // TODO check argument types
                        return expression.Type = functionType.ReturnType;
                    }
                    // If it is unknown, we already reported an error
                    if (callee == DataType.Unknown)
                        return expression.Type = DataType.Unknown;

                    diagnostics.Add(TypeError.MustBeCallable(file, invocation.Callee));
                    return expression.Type = DataType.Unknown;
                }
                case GenericNameSyntax genericName:
                {
                    foreach (var argument in genericName.Arguments)
                        InferExpressionType(argument.Value);

                    genericName.NameType.BeginFulfilling();
                    var nameType = InferNameType(genericName.Name, genericName.Span);
                    //if (nameType is OverloadedType overloadedType)
                    //{
                    //    nameType = overloadedType.Types.OfType<GenericType>()
                    //        .Single(t => t.GenericArity == genericName.Arity).NotNull();
                    //}

                    // TODO check that argument types match function type
                    genericName.NameType.Fulfill(nameType);

                    switch (nameType)
                    {
                        // TODO implement
                        //case Metatype metatype:
                        //    genericInvocation.Type.Computed(
                        //        metatype.WithGenericArguments(
                        //            genericInvocation.Arguments.Select(a => a.Value.Type.AssertComputed())));
                        //    break;
                        case MetaFunctionType metaFunctionType:
                            return genericName.Type = metaFunctionType.ResultType;
                        case UnknownType _:
                            return genericName.Type = DataType.Unknown;
                        default:
                            throw NonExhaustiveMatchException.For(genericName.NameType);
                    }
                }
                case RefTypeSyntax refType:
                    CheckAndEvaluateTypeExpression(refType.ReferencedType);
                    return refType.Type = DataType.Type;
                case UnsafeExpressionSyntax unsafeExpression:
                    InferExpressionType(unsafeExpression.Expression);
                    return unsafeExpression.Type = unsafeExpression.Expression.Type;
                case MutableTypeSyntax mutableType:
                    return mutableType.Type = CheckAndEvaluateTypeExpression(mutableType.ReferencedTypeExpression);// TODO make that type mutable
                case IfExpressionSyntax ifExpression:
                    CheckExpressionType(ifExpression.Condition, DataType.Bool);
                    InferExpressionType(ifExpression.ThenBlock);
                    InferExpressionType(ifExpression.ElseClause);
                    // TODO assign a type to the expression
                    return ifExpression.Type = DataType.Unknown;
                case ResultExpressionSyntax resultExpression:
                    InferExpressionType(resultExpression.Expression);
                    return resultExpression.Type = DataType.Never;
                //                case UninitializedExpressionSyntax uninitializedExpression:
                //                    // TODO assign a type to the expression
                //                    return uninitializedExpression.Type.Computed(DataType.Unknown);
                case MemberAccessExpressionSyntax memberAccess:
                    return InferMemberAccessType(memberAccess);
                case BreakExpressionSyntax breakExpression:
                    InferExpressionType(breakExpression.Value);
                    return breakExpression.Type = DataType.Never;
                case AssignmentExpressionSyntax assignmentExpression:
                    var left = InferExpressionType(assignmentExpression.LeftOperand);
                    InferExpressionType(assignmentExpression.RightOperand);
                    assignmentExpression.RightOperand = ImplicitConversion(assignmentExpression.RightOperand, left);
                    // TODO Check compability of types
                    //throw new NotImplementedException("Check compability of types");
                    return DataType.Void;
                case SelfExpressionSyntax _:
                    return selfType ?? DataType.Unknown;
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
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

        private static ISymbol GetSymbolForType(DataType left)
        {
            switch (left)
            {
                case UnknownType _:
                    return UnknownSymbol.Instance;
                case ObjectType objectType:
                    return objectType.Symbol;
                case SizedIntegerType integerType:
                    // TODO this seems a very strange way to handle this. Shouldn't the symbol be on the type?
                    return PrimitiveSymbols.Instance.Single(p => p.FullName == integerType.Name);
                default:
                    throw NonExhaustiveMatchException.For(left);
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

        private DataType InferNameType(
            SimpleName name,
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
            ArgumentSyntax argument)
        {
            //            InferExpressionType(argument.Value);
            throw new NotImplementedException();
        }


        private DataType InferBinaryExpressionType(
            BinaryExpressionSyntax binaryExpression)
        {
            InferExpressionType(binaryExpression.LeftOperand);
            var leftOperand = binaryExpression.LeftOperand.Type;
            var leftOperandCore = leftOperand is LifetimeType l ? l.Referent : leftOperand;
            var @operator = binaryExpression.Operator;
            InferExpressionType(binaryExpression.RightOperand);
            var rightOperand = binaryExpression.RightOperand.Type;
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
                        return binaryExpression.Type = DataType.Bool;
                    default:
                        return binaryExpression.Type = DataType.Unknown;
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
                    binaryExpression.Type = !typeError ? leftOperand : DataType.Unknown;
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
                    binaryExpression.Type = DataType.Bool;
                    break;
                //case IEqualsToken _:
                //    typeError = leftOperandCore != rightOperandCore;
                //    if (!typeError)
                //        binaryOperatorExpression.Type.Computed(leftOperand);
                //    break;
                case BinaryOperator.And:
                case BinaryOperator.Or:
                    typeError = leftOperand != DataType.Bool || rightOperand != DataType.Bool;

                    binaryExpression.Type = DataType.Bool;
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
                    binaryExpression.LeftOperand.Type,
                    binaryExpression.RightOperand.Type));

            return binaryExpression.Type;
        }

        private bool CheckNumericOperator(
            ExpressionSyntax leftOperand,
            ExpressionSyntax rightOperand,
            DataType resultType)
        {
            var leftType = leftOperand.Type;
            var rightType = rightOperand.Type;
            switch (leftType)
            {
                case PointerType _:
                {
                    // TODO it may need to be size
                    //ImposeIntegerConstantType(UnsizedIntegerType.Offset, rightOperand);
                    throw new NotImplementedException();
                    //return rightType != DataType.Size &&
                    //       rightType != DataType.Offset;
                }
                case IntegerConstantType _:
                    // TODO may need to promote based on size
                    //ImposeIntegerConstantType(rightType, leftOperand);
                    throw new NotImplementedException();
                //return !IsIntegerType(rightType);
                case DataType type when IsIntegerType(type):
                    // TODO it may need to be size
                    //ImposeIntegerConstantType(leftType, rightOperand);
                    throw new NotImplementedException();
                //return !IsIntegerType(rightType);
                case ObjectType _:
                    // Other object types can't be used in numeric expressions
                    return false;
                default:
                    throw NonExhaustiveMatchException.For(leftType);
            }
        }

        private static bool IsIntegerType(DataType type)
        {
            return type is IntegerType;
        }

        private DataType InferUnaryExpressionType(UnaryExpressionSyntax unaryExpression)
        {
            InferExpressionType(unaryExpression.Operand);
            var operand = unaryExpression.Operand.Type;
            var @operator = unaryExpression.Operator;

            // If either is unknown, then we can't know whether there is a a problem
            // (technically not true, for example, we could know that one arg should
            // be a bool and isn't)
            if (operand == DataType.Unknown)
                return unaryExpression.Type = DataType.Unknown;

            bool typeError;
            switch (@operator)
            {
                case UnaryOperator.Not:
                    typeError = operand != DataType.Bool;
                    unaryExpression.Type = DataType.Bool;
                    break;
                case UnaryOperator.At:
                    typeError = false; // TODO check that the expression can have a pointer taken
                    if (operand is Metatype)
                        unaryExpression.Type = DataType.Type; // constructing a type
                    else
                        unaryExpression.Type = new PointerType(operand); // taking the address of something
                    break;
                case UnaryOperator.Question:
                    typeError = false; // TODO check that the expression can have a pointer taken
                    unaryExpression.Type = new PointerType(operand);
                    break;
                case UnaryOperator.Caret:
                    switch (operand)
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
                default:
                    throw NonExhaustiveMatchException.For(@operator);
            }
            if (typeError)
                diagnostics.Add(TypeError.OperatorCannotBeAppliedToOperandOfType(file,
                    unaryExpression.Span, @operator, operand));

            return unaryExpression.Type;
        }

        /// <summary>
        /// Evaluates a type expression to the type it identifies
        /// </summary>
        public DataType CheckAndEvaluateTypeExpression(ExpressionSyntax typeExpression)
        {
            if (typeExpression == null)
            {
                // TODO report error?
                return DataType.Unknown;
            }

            var type = InferExpressionType(typeExpression);
            if (!(type is Metatype)
                && type != DataType.Type)
            {
                diagnostics.Add(TypeError.MustBeATypeExpression(file, typeExpression.Span));
                return DataType.Unknown;
            }

            return TypeExpressionEvaluator.EvaluateExpression(typeExpression);
        }

        public void InferExpressionTypeInInvocation(ExpressionSyntax callee, FixedList<DataType> argumentTypes)
        {
            switch (callee)
            {
                case IdentifierNameSyntax identifierName:
                {
                    var symbols = identifierName.LookupInContainingScope();
                    symbols = ResolveOverload(symbols, argumentTypes);
                    AssignReferencedSymbolAndType(identifierName, symbols);
                }
                break;
                case MemberAccessExpressionSyntax memberAccess:
                {
                    var left = InferExpressionType(memberAccess.Expression);
                    var containingSymbol = GetSymbolForType(left);
                    var symbols = containingSymbol.Lookup(memberAccess.Member.Name);
                    symbols = ResolveOverload(symbols, argumentTypes);
                    memberAccess.Type = AssignReferencedSymbolAndType(memberAccess.Member, symbols);
                }
                break;
                default:
                    throw NonExhaustiveMatchException.For(callee);
            }
        }

        private FixedList<ISymbol> ResolveOverload(FixedList<ISymbol> symbols, FixedList<DataType> argumentTypes)
        {
            // Filter down to symbols that could possible match
            symbols = symbols.Where(s =>
            {
                if (s.Type is FunctionType functionType)
                {
                    if (functionType.Arity != argumentTypes.Count) return false;
                    // TODO check compability over argument types
                }

                return true;
            }).ToFixedList();
            // TODO Select most specific match
            return symbols;
        }
    }
}
