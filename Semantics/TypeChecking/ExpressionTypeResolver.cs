using System;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
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
            variableDeclaration.Type.BeginFulfilling();
            if (variableDeclaration.Initializer != null)
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

            variableDeclaration.Type.Fulfill(type);
            if (variableDeclaration.Initializer != null)
            {
                InsertImplicitConversionIfNeeded(ref variableDeclaration.Initializer, type);
                // TODO check that the initializer type is compatible with the variable type
            }
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
                case StringConstantType expressionType:
                {
                    if (targetType is ObjectType objectType)
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
                diagnostics.Add(TypeError.CannotConvert(file, expression, expectedType));
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
                        InferExpressionType(returnExpression.ReturnValue);
                        if (returnType != null) // TODO report an error
                        {
                            InsertImplicitConversionIfNeeded(ref returnExpression.ReturnValue, returnType);
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
                    if (!IsType(lifetimeType.ReferentTypeExpression.Type))
                        diagnostics.Add(TypeError.MustBeATypeExpression(file, lifetimeType.ReferentTypeExpression.Span));
                    return expression.Type = DataType.Type;
                case BlockSyntax blockExpression:
                    foreach (var statement in blockExpression.Statements)
                        ResolveTypesInStatement(statement);

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

                    genericName.NameType.BeginFulfilling();
                    var nameType = InferNameType(genericName);

                    // TODO check that argument types match function type
                    genericName.NameType.Fulfill(nameType);

                    switch (nameType)
                    {
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
                    // TODO Check compability of types
                    return assignmentExpression.Type = DataType.Void;
                case SelfExpressionSyntax _:
                    return selfType ?? DataType.Unknown;
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
        }

        private static bool IsType(DataType dataType)
        {
            return dataType == DataType.Type || dataType is Metatype;
        }

        private DataType InferInvocationType(InvocationSyntax invocation)
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
                foreach (var (arg, type) in invocation.Arguments.Zip(functionType.ParameterTypes))
                    InsertImplicitConversionIfNeeded(ref arg.Value, type);

                // TODO check argument types
                return invocation.Type = functionType.ReturnType;
            }

            // If it is unknown, we already reported an error
            if (callee == DataType.Unknown) return invocation.Type = DataType.Unknown;

            diagnostics.Add(TypeError.MustBeCallable(file, invocation.Callee));
            return invocation.Type = DataType.Unknown;
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
                case ObjectType objectType:
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

        private DataType InferNameType(GenericNameSyntax genericName)
        {
            throw new NotImplementedException();
        }

        private void CheckArgument(ArgumentSyntax argument)
        {
            throw new NotImplementedException();
        }

        private DataType InferBinaryExpressionType(
            BinaryExpressionSyntax binaryExpression)
        {
            InferExpressionType(binaryExpression.LeftOperand);
            var leftType = binaryExpression.LeftOperand.Type;
            var leftTypeCore = leftType is LifetimeType l ? l.Referent : leftType;
            var @operator = binaryExpression.Operator;
            InferExpressionType(binaryExpression.RightOperand);
            var rightType = binaryExpression.RightOperand.Type;
            var rightTypeCore = rightType is LifetimeType r ? r.Referent : rightType;

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
                    compatible = (leftTypeCore == DataType.Bool && rightTypeCore == DataType.Bool)
                        || NumericOperatorTypesAreCompatible(ref binaryExpression.LeftOperand, ref binaryExpression.RightOperand, null);
                    binaryExpression.Type = DataType.Bool;
                    break;
                case BinaryOperator.And:
                case BinaryOperator.Or:
                    compatible = leftType == DataType.Bool && rightType == DataType.Bool;
                    binaryExpression.Type = DataType.Bool;
                    break;
                default:
                    throw NonExhaustiveMatchException.For(@operator);
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
            var rightType = rightOperand.Type;
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
                case ObjectType _:
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
            if (!IsType(typeExpression.Type))
            {
                diagnostics.Add(TypeError.MustBeATypeExpression(file, typeExpression.Span));
                return DataType.Unknown;
            }

            return TypeExpressionEvaluator.EvaluateExpression(typeExpression);
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
