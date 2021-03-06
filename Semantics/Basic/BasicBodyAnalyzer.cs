using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Operators;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Basic.ImplicitOperations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Basic.InferredSyntax;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Symbols.Trees;
using Adamant.Tools.Compiler.Bootstrap.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic
{
    /// <summary>
    /// Do basic analysis of bodies.
    /// </summary>
    /// <remarks>
    /// Type checking doesn't concern anything with reachability. It only deals
    /// with types and reference capabilities.
    /// </remarks>
    public class BasicBodyAnalyzer
    {
        private readonly CodeFile file;
        private readonly Symbol containingSymbol;
        private readonly SymbolTreeBuilder symbolTreeBuilder;
        private readonly SymbolForest symbolTrees;
        private readonly ObjectTypeSymbol? stringSymbol;
        private readonly Diagnostics diagnostics;
        private readonly DataType? returnType;
        private readonly TypeResolver typeResolver;

        public BasicBodyAnalyzer(
            IEntityDeclarationSyntax containingDeclaration,
            SymbolTreeBuilder symbolTreeBuilder,
            SymbolForest symbolTrees,
            ObjectTypeSymbol? stringSymbol,
            Diagnostics diagnostics,
            DataType? returnType = null)
        {
            file = containingDeclaration.File;
            containingSymbol = containingDeclaration.Symbol.Result;
            this.symbolTreeBuilder = symbolTreeBuilder;
            this.stringSymbol = stringSymbol;
            this.diagnostics = diagnostics;
            this.symbolTrees = symbolTrees;
            this.returnType = returnType;
            typeResolver = new TypeResolver(file, diagnostics);
        }

        public void ResolveTypes(IBodySyntax body)
        {
            foreach (var statement in body.Statements)
                switch (statement)
                {
                    default:
                        throw ExhaustiveMatch.Failed(statement);
                    case IVariableDeclarationStatementSyntax variableDeclaration:
                        ResolveTypes(variableDeclaration);
                        break;
                    case IExpressionStatementSyntax expressionStatement:
                        InferType(ref expressionStatement.Expression);
                        break;
                }
        }

        private void ResolveTypes(IStatementSyntax statement)
        {
            switch (statement)
            {
                default:
                    throw ExhaustiveMatch.Failed(statement);
                case IVariableDeclarationStatementSyntax variableDeclaration:
                    ResolveTypes(variableDeclaration);
                    break;
                case IExpressionStatementSyntax expressionStatement:
                    InferType(ref expressionStatement.Expression);
                    break;
                case IResultStatementSyntax resultStatement:
                    InferType(ref resultStatement.Expression);
                    break;
            }
        }

        private void ResolveTypes(IVariableDeclarationStatementSyntax variableDeclaration)
        {
            DataType type;
            if (variableDeclaration.Type != null)
            {
                type = typeResolver.Evaluate(variableDeclaration.Type);
                CheckType(ref variableDeclaration.Initializer, type);
            }
            else if (variableDeclaration.Initializer != null)
                type = InferDeclarationType(ref variableDeclaration.Initializer, variableDeclaration.InferMutableType);
            else
            {
                diagnostics.Add(TypeError.NotImplemented(file, variableDeclaration.NameSpan,
                    "Inference of local variable types not implemented"));
                type = DataType.Unknown;
            }

            if (variableDeclaration.Initializer != null)
            {
                var initializerType = variableDeclaration.Initializer.DataType ?? throw new InvalidOperationException("Initializer type should be determined");

                if (!type.IsAssignableFrom(initializerType))
                    diagnostics.Add(TypeError.CannotConvert(file, variableDeclaration.Initializer, initializerType, type));
            }

            var symbol = new VariableSymbol((InvocableSymbol)containingSymbol, variableDeclaration.Name,
                variableDeclaration.DeclarationNumber.Result, variableDeclaration.IsMutableBinding, type);
            variableDeclaration.Symbol.Fulfill(symbol);
            symbolTreeBuilder.Add(symbol);
        }

        /// <summary>
        /// Infer the type of a variable declaration from an expression
        /// </summary>
        private DataType InferDeclarationType([NotNull] ref IExpressionSyntax expression, bool inferMutableType)
        {
            var type = InferType(ref expression);
            if (!type.IsKnown) return DataType.Unknown;
            type = type.ToNonConstantType();
            type = InsertImplicitConversionIfNeeded(ref expression, type);

            switch (expression)
            {
                case IMoveExpressionSyntax _:
                    // If we are explicitly moving then take the actual type
                    return type;
                case IBorrowExpressionSyntax _:
                {
                    // If we are explicitly borrowing or moving then take the actual type
                    if (!(type is ReferenceType referenceType))
                        throw new NotImplementedException("Compile error: can't borrow non reference type");

                    return referenceType.To(ReferenceCapability.Borrowed);
                }
                default:
                {
                    // We assume immutability on variables unless explicitly stated
                    if (!inferMutableType) return type.ToReadOnly();
                    if (type is ReferenceType referenceType)
                    {
                        if (!referenceType.IsMutable)
                            throw new NotImplementedException("Compile error: can't infer a mutable type");

                        return type;
                    }

                    throw new NotImplementedException("Compile error: can't infer mutability for non reference type");
                }
            }
        }

        public void CheckType([NotNull] ref IExpressionSyntax? expression, DataType expectedType)
        {
            if (expression is null) return;
            InferType(ref expression);
            var actualType = InsertImplicitConversionIfNeeded(ref expression, expectedType);
            if (!expectedType.IsAssignableFrom(actualType))
                diagnostics.Add(TypeError.CannotConvert(file, expression, actualType, expectedType));
        }

        /// <summary>
        /// Create an implicit conversion if allowed and needed
        /// </summary>
        private static DataType InsertImplicitConversionIfNeeded(
            ref IExpressionSyntax expression,
            DataType expectedType)
        {
            switch (expectedExpressionType: expectedType, Type: expression.DataType)
            {
                case (OptionalType targetType, OptionalType expressionType)
                        when expressionType.Referent is NeverType:
                    expression = new ImplicitNoneConversionExpression(expression, targetType);
                    break;
                case (OptionalType targetType, /* non-optional type */ _):
                    // If needed, convert the type to the referent type of the optional type
                    var type = InsertImplicitConversionIfNeeded(ref expression, targetType.Referent);
                    if (targetType.Referent.IsAssignableFrom(type))
                        expression = new ImplicitOptionalConversionExpression(expression, targetType);
                    break;
                case (FixedSizeIntegerType targetType, FixedSizeIntegerType expressionType):
                    if (targetType.Bits > expressionType.Bits && (!expressionType.IsSigned || targetType.IsSigned))
                        expression = new ImplicitNumericConversionExpression(expression, targetType);
                    break;
                case (FixedSizeIntegerType targetType, IntegerConstantType expressionType):
                {
                    var requireSigned = expressionType.Value < 0;
                    var bits = expressionType.Value.GetByteCount(!targetType.IsSigned) * 8;
                    if (targetType.Bits >= bits && (!requireSigned || targetType.IsSigned))
                        expression = new ImplicitNumericConversionExpression(expression, targetType);
                }
                break;
                case (PointerSizedIntegerType targetType, IntegerConstantType expressionType):
                {
                    var requireSigned = expressionType.Value < 0;
                    if (!requireSigned || targetType.IsSigned)
                        expression = new ImplicitNumericConversionExpression(expression, targetType);
                }
                break;
                case (ObjectType targetType, ObjectType expressionType)
                        when targetType.IsReadOnly && expressionType.IsMutable:
                    // TODO if source type is explicitly mutable, issue warning about using `mut` in immutable context
                    expression = new ImplicitImmutabilityConversionExpression(expression, expressionType.ToReadOnly());
                    break;
            }

            return expression.DataType!;
        }

        /// <summary>
        /// Infer the type of an expression and assign that type to the expression.
        /// </summary>
        /// <param name="expression">A reference to the repression. This is a reference so that the
        /// expression can be replaced because the parser can't always correctly determine
        /// what kind of expression it is without type information.</param>
        /// <param name="implicitShare">Whether implicit share expressions should be inserted around
        /// bare variable references.</param>
        private DataType InferType([NotNull] ref IExpressionSyntax? expression, bool implicitShare = true)
        {
            switch (expression)
            {
                default:
                    throw ExhaustiveMatch.Failed(expression);
                case null:
                    expression = null!; // Trick compiler into allowing it to stay null
                    return DataType.Unknown;
                case IShareExpressionSyntax _:
                    throw new InvalidOperationException("Share expressions should not be in the AST during basic analysis");
                case IMoveExpressionSyntax exp:
                    switch (exp.Referent)
                    {
                        case INameExpressionSyntax nameExpression:
                            nameExpression.Semantics = ExpressionSemantics.Acquire;
                            var type = InferType(ref exp.Referent, false);
                            switch (type)
                            {
                                case ReferenceType referenceType:
                                    if (!referenceType.IsMovable)
                                    {
                                        diagnostics.Add(TypeError.CannotMoveValue(file, exp));
                                        type = DataType.Unknown;
                                    }
                                    break;
                                default:
                                    throw new NotImplementedException("Non-moveable type can't be moved");
                            }

                            exp.ReferencedSymbol.Fulfill(nameExpression.ReferencedSymbol.Result);
                            exp.Semantics = ExpressionSemantics.Acquire;
                            return exp.DataType = type;
                        case IBorrowExpressionSyntax _:
                            throw new NotImplementedException("Raise error about `move mut` expression");
                        case IMoveExpressionSyntax _:
                            throw new NotImplementedException("Raise error about `move move` expression");
                        default:
                            throw new NotImplementedException("Tried to move out of expression type that isn't implemented");
                    }
                case IBorrowExpressionSyntax exp:
                    switch (exp.Referent)
                    {
                        case INameExpressionSyntax nameExpression:
                        {
                            nameExpression.Semantics = ExpressionSemantics.Borrow;
                            var type = InferType(ref exp.Referent, false);
                            switch (type)
                            {
                                case ReferenceType referenceType:
                                    if (!referenceType.IsMutable)
                                    {
                                        diagnostics.Add(TypeError.ExpressionCantBeMutable(file, exp.Referent));
                                        type = DataType.Unknown;
                                    }
                                    else
                                        type = referenceType.To(ReferenceCapability.Borrowed);

                                    break;
                                default:
                                    throw new NotImplementedException("Non-mutable type can't be borrowed mutably");
                            }

                            exp.ReferencedSymbol.Fulfill(nameExpression.ReferencedSymbol.Result);
                            return exp.DataType = type;
                        }
                        case IBorrowExpressionSyntax _:
                            throw new NotImplementedException("Raise error about `mut mut` expression");
                        case IMoveExpressionSyntax _:
                            throw new NotImplementedException("Raise error about `mut move` expression");
                        default:
                            throw new NotImplementedException("Tried mutate expression type that isn't implemented");
                    }
                case IReturnExpressionSyntax exp:
                {
                    if (exp.Value != null)
                    {
                        var expectedReturnType = returnType ?? throw new InvalidOperationException("Return statement in constructor");
                        InferType(ref exp.Value, false);
                        // If we return ownership, there can be an implicit move
                        // otherwise there could be an implicit share or borrow
                        InsertImplicitActionIfNeeded(ref exp.Value, expectedReturnType, implicitBorrowAllowed: false);
                        var actualType = InsertImplicitConversionIfNeeded(ref exp.Value, expectedReturnType);
                        if (!expectedReturnType.IsAssignableFrom(actualType))
                            diagnostics.Add(TypeError.CannotConvert(file, exp.Value, actualType, expectedReturnType));
                    }
                    else if (returnType == DataType.Never)
                        diagnostics.Add(TypeError.CantReturnFromNeverFunction(file, exp.Span));
                    else if (returnType != DataType.Void)
                        diagnostics.Add(TypeError.ReturnExpressionMustHaveValue(file, exp.Span, returnType ?? DataType.Unknown));

                    return exp.DataType = DataType.Never;
                }
                case IIntegerLiteralExpressionSyntax exp:
                    return exp.DataType = new IntegerConstantType(exp.Value);
                case IStringLiteralExpressionSyntax exp:
                    return exp.DataType = stringSymbol?.DeclaresDataType ?? (DataType)DataType.Unknown;
                case IBoolLiteralExpressionSyntax exp:
                    return exp.DataType = exp.Value ? DataType.True : DataType.False;
                case IBinaryOperatorExpressionSyntax binaryOperatorExpression:
                {
                    var leftType = InferType(ref binaryOperatorExpression.LeftOperand);
                    var @operator = binaryOperatorExpression.Operator;
                    var rightType = InferType(ref binaryOperatorExpression.RightOperand);

                    // If either is unknown, then we can't know whether there is a a problem.
                    // Note that the operator could be overloaded
                    if (leftType == DataType.Unknown || rightType == DataType.Unknown)
                        return binaryOperatorExpression.DataType = DataType.Unknown;

                    bool compatible;
                    switch (@operator)
                    {
                        case BinaryOperator.Plus:
                        case BinaryOperator.Minus:
                        case BinaryOperator.Asterisk:
                        case BinaryOperator.Slash:
                            compatible = NumericOperatorTypesAreCompatible(ref binaryOperatorExpression.LeftOperand, ref binaryOperatorExpression.RightOperand);
                            binaryOperatorExpression.DataType = compatible ? leftType : DataType.Unknown;
                            binaryOperatorExpression.Semantics = ExpressionSemantics.Copy;
                            break;
                        case BinaryOperator.EqualsEquals:
                        case BinaryOperator.NotEqual:
                        case BinaryOperator.LessThan:
                        case BinaryOperator.LessThanOrEqual:
                        case BinaryOperator.GreaterThan:
                        case BinaryOperator.GreaterThanOrEqual:
                            compatible = (leftType == DataType.Bool && rightType == DataType.Bool)
                                         || NumericOperatorTypesAreCompatible(ref binaryOperatorExpression.LeftOperand, ref binaryOperatorExpression.RightOperand)
                                         /*|| OperatorOverloadDefined(@operator, binaryOperatorExpression.LeftOperand, ref binaryOperatorExpression.RightOperand)*/;
                            binaryOperatorExpression.DataType = DataType.Bool;
                            binaryOperatorExpression.Semantics = ExpressionSemantics.Copy;
                            break;
                        case BinaryOperator.And:
                        case BinaryOperator.Or:
                            compatible = leftType == DataType.Bool && rightType == DataType.Bool;
                            binaryOperatorExpression.DataType = DataType.Bool;
                            binaryOperatorExpression.Semantics = ExpressionSemantics.Copy;
                            break;
                        case BinaryOperator.DotDot:
                        case BinaryOperator.LessThanDotDot:
                        case BinaryOperator.DotDotLessThan:
                        case BinaryOperator.LessThanDotDotLessThan:
                            throw new NotImplementedException("Type analysis of range operators");
                        default:
                            throw ExhaustiveMatch.Failed(@operator);
                    }
                    if (!compatible)
                        diagnostics.Add(TypeError.OperatorCannotBeAppliedToOperandsOfType(file,
                            binaryOperatorExpression.Span, @operator, leftType, rightType));

                    return binaryOperatorExpression.DataType;
                }
                case INameExpressionSyntax exp:
                {
                    var type = InferNameType(exp);
                    // In many contexts, variable names are implicitly shared
                    if (implicitShare)
                        type = InsertImplicitShareIfNeeded(ref expression, type);

                    // TODO do a more complete generation of expression semantics
                    if (exp.Semantics is null)
                        switch (type.Semantics)
                        {
                            case TypeSemantics.Copy:
                                exp.Semantics = ExpressionSemantics.Copy;
                                break;
                            case TypeSemantics.Move:
                                exp.Semantics = ExpressionSemantics.Move;
                                break;
                        }

                    return type;
                }
                case IUnaryOperatorExpressionSyntax exp:
                {
                    var @operator = exp.Operator;
                    switch (@operator)
                    {
                        default:
                            throw ExhaustiveMatch.Failed(@operator);
                        case UnaryOperator.Not:
                            CheckType(ref exp.Operand, DataType.Bool);
                            exp.DataType = DataType.Bool;
                            break;
                        case UnaryOperator.Minus:
                        case UnaryOperator.Plus:
                            var operandType = InferType(ref exp.Operand);
                            switch (operandType)
                            {
                                case IntegerConstantType integerType:
                                    exp.DataType = integerType;
                                    break;
                                case FixedSizeIntegerType sizedIntegerType:
                                    exp.DataType = sizedIntegerType;
                                    break;
                                case UnknownType _:
                                    exp.DataType = DataType.Unknown;
                                    break;
                                default:
                                    diagnostics.Add(TypeError.OperatorCannotBeAppliedToOperandOfType(file,
                                        exp.Span, @operator, operandType));
                                    exp.DataType = DataType.Unknown;
                                    break;
                            }
                            break;
                    }

                    return exp.DataType;
                }
                case INewObjectExpressionSyntax exp:
                {
                    var argumentTypes = exp.Arguments.Select(argument => InferType(ref argument.Expression)).ToFixedList();
                    // TODO handle named constructors here
                    var constructingType = typeResolver.Evaluate(exp.Type);
                    if (!constructingType.IsKnown)
                    {
                        diagnostics.Add(NameBindingError.CouldNotBindConstructor(file, exp.Span));
                        exp.ReferencedSymbol.Fulfill(null);
                        return exp.DataType  = DataType.Unknown;
                    }

                    // TODO handle null typesymbol
                    var typeSymbol = exp.Type.ReferencedSymbol.Result ?? throw new InvalidOperationException();
                    var classType = (ObjectType)constructingType;
                    ObjectType constructedType;
                    var constructorSymbols = symbolTreeBuilder.Children(typeSymbol).OfType<ConstructorSymbol>().ToFixedList();
                    constructorSymbols = ResolveOverload(constructorSymbols, argumentTypes);
                    switch (constructorSymbols.Count)
                    {
                        case 0:
                            diagnostics.Add(NameBindingError.CouldNotBindConstructor(file, exp.Span));
                            exp.ReferencedSymbol.Fulfill(null);
                            constructedType = classType.ToConstructorReturn();
                            break;
                        case 1:
                            var constructorSymbol = constructorSymbols.Single();
                            exp.ReferencedSymbol.Fulfill(constructorSymbol);
                            foreach (var (arg, parameterDataType) in exp.Arguments.Zip(constructorSymbol.ParameterDataTypes))
                            {
                                InsertImplicitConversionIfNeeded(ref arg.Expression, parameterDataType);
                                CheckArgumentTypeCompatibility(parameterDataType, arg.Expression);
                            }
                            constructedType = constructorSymbol.ReturnDataType;
                            break;
                        default:
                            diagnostics.Add(NameBindingError.AmbiguousConstructorCall(file, exp.Span));
                            exp.ReferencedSymbol.Fulfill(null);
                            constructedType = classType.ToConstructorReturn();
                            break;
                    }


                    return exp.DataType = constructedType;
                }
                case IForeachExpressionSyntax exp:
                {
                    var declaredType = typeResolver.Evaluate(exp.Type);
                    var expressionType = CheckForeachInType(declaredType, ref exp.InExpression);
                    var variableType = declaredType ?? expressionType;
                    var symbol = new VariableSymbol((InvocableSymbol)containingSymbol, exp.VariableName,
                        exp.DeclarationNumber.Result, exp.IsMutableBinding, variableType);
                    exp.Symbol.Fulfill(symbol);
                    symbolTreeBuilder.Add(symbol);

                    // TODO check the break types
                    InferBlockType(exp.Block);
                    // TODO assign correct type to the expression
                    exp.Semantics = ExpressionSemantics.Void;
                    return exp.DataType = DataType.Void;
                }
                case IWhileExpressionSyntax exp:
                {
                    CheckType(ref exp.Condition, DataType.Bool);
                    InferBlockType(exp.Block);
                    // TODO assign correct type to the expression
                    exp.Semantics = ExpressionSemantics.Void;
                    return exp.DataType = DataType.Void;
                }
                case ILoopExpressionSyntax exp:
                    InferBlockType(exp.Block);
                    // TODO assign correct type to the expression
                    exp.Semantics = ExpressionSemantics.Void;
                    return exp.DataType = DataType.Void;
                case IQualifiedInvocationExpressionSyntax exp:
                    return InferMethodInvocationType(exp, ref expression);
                case IUnqualifiedInvocationExpressionSyntax exp:
                    return InferFunctionInvocationType(exp);
                case IUnsafeExpressionSyntax exp:
                {
                    exp.DataType = InferType(ref exp.Expression);
                    exp.Semantics = exp.Expression.Semantics.Assigned();
                    return exp.DataType;
                }
                case IIfExpressionSyntax exp:
                    CheckType(ref exp.Condition, DataType.Bool);
                    InferBlockType(exp.ThenBlock);
                    switch (exp.ElseClause)
                    {
                        default:
                            throw ExhaustiveMatch.Failed(exp.ElseClause);
                        case null:
                            break;
                        case IIfExpressionSyntax _:
                        case IBlockExpressionSyntax _:
                            var elseExpression = (IExpressionSyntax)exp.ElseClause;
                            InferType(ref elseExpression);
                            //ifExpression.ElseClause = elseExpression;
                            break;
                        case IResultStatementSyntax resultStatement:
                            InferType(ref resultStatement.Expression);
                            break;
                    }
                    // TODO assign a type to the expression
                    exp.Semantics = ExpressionSemantics.Void;
                    return exp.DataType = DataType.Void;
                case IQualifiedNameExpressionSyntax exp:
                {
                    // Don't wrap the self expression in a share expression for field access
                    var isSelfField = exp.Context is ISelfExpressionSyntax;
                    var contextType = InferType(ref exp.Context, !isSelfField);
                    var member = exp.Field;
                    var contextSymbol = LookupSymbolForType(contextType);
                    if (contextSymbol is null)
                    {
                        member.ReferencedSymbol.Fulfill(null);
                        member.DataType = DataType.Unknown;
                        exp.Semantics = exp.Semantics ?? ExpressionSemantics.Copy;
                        return exp.DataType = DataType.Unknown;
                    }
                    // TODO Deal with no context symbol
                    var memberSymbols = symbolTreeBuilder.Children(contextSymbol!).OfType<FieldSymbol>()
                                                  .Where(s => s.Name == member.Name).ToFixedList();
                    var type = AssignReferencedSymbolAndType(member, memberSymbols);
                    // In many contexts, variable names are implicitly shared
                    if (implicitShare) type = InsertImplicitShareIfNeeded(ref expression, type);

                    if (exp.Semantics is null)
                        switch (type.Semantics)
                        {
                            case TypeSemantics.Copy:
                                exp.Semantics = ExpressionSemantics.Copy;
                                break;
                            case TypeSemantics.Move:
                                exp.Semantics = ExpressionSemantics.Move;
                                break;
                        }
                    return exp.DataType = type;
                }
                case IBreakExpressionSyntax exp:
                    InferType(ref exp.Value);
                    return exp.DataType = DataType.Never;
                case INextExpressionSyntax exp:
                    return exp.DataType = DataType.Never;
                case IAssignmentExpressionSyntax exp:
                {
                    var left = InferAssignmentTargetType(ref exp.LeftOperand);
                    InferType(ref exp.RightOperand);
                    InsertImplicitConversionIfNeeded(ref exp.RightOperand, left);
                    var right = exp.RightOperand.DataType ?? throw new InvalidOperationException();
                    if (!left.IsAssignableFrom(right))
                        diagnostics.Add(TypeError.CannotConvert(file,
                            exp.RightOperand, right, left));
                    exp.Semantics = ExpressionSemantics.Void;
                    return exp.DataType = DataType.Void;
                }
                case ISelfExpressionSyntax exp:
                {
                    var type = InferSelfType(exp);
                    if (implicitShare)
                        type = InsertImplicitShareIfNeeded(ref expression, type);

                    if (exp.Semantics is null)
                    {
                        if (type is ReferenceType referenceType)
                            exp.Semantics = referenceType.IsMutable
                                ? ExpressionSemantics.Borrow : ExpressionSemantics.Share;
                        else
                            throw new NotImplementedException("Could not assign semantics to `self` expression");
                    }
                    return type;
                }
                case INoneLiteralExpressionSyntax exp:
                    return exp.DataType = DataType.None;
                case IImplicitConversionExpressionSyntax _:
                    throw new Exception("ImplicitConversionExpressions are inserted by BasicExpressionAnalyzer. They should not be present in the AST yet.");
                case IBlockExpressionSyntax blockSyntax:
                    return InferBlockType(blockSyntax);
            }
        }

        private static DataType InsertImplicitShareIfNeeded([NotNull] ref IExpressionSyntax expression, DataType type)
        {
            // Value types aren't shared
            if (!(type is ReferenceType referenceType)) return type;

            BindingSymbol? referencedSymbol;
            switch (expression)
            {
                case INameExpressionSyntax exp:
                    exp.Semantics = ExpressionSemantics.Share;
                    referencedSymbol = exp.ReferencedSymbol.Result;
                    break;
                case ISelfExpressionSyntax exp:
                    referencedSymbol = exp.ReferencedSymbol.Result;
                    break;
                case IQualifiedNameExpressionSyntax exp:
                    exp.Field.Semantics = ExpressionSemantics.Share;
                    exp.Semantics = ExpressionSemantics.Share;
                    referencedSymbol = exp.ReferencedSymbol.Result;
                    break;
                default:
                    // implicit share isn't needed around other expressions
                    return type;
            }

            type = referenceType.To(ReferenceCapability.Shared);

            expression = new ImplicitShareExpressionSyntax(expression, type, referencedSymbol);

            return type;
        }

        private static void InsertImplicitBorrowIfNeeded([NotNull] ref IExpressionSyntax expression, DataType type)
        {
            // Value types aren't shared
            if (!(type is ReferenceType referenceType)) return;

            BindingSymbol? referencedSymbol;
            switch (expression)
            {
                case INameExpressionSyntax exp:
                    exp.Semantics = ExpressionSemantics.Borrow;
                    referencedSymbol = exp.ReferencedSymbol.Result;
                    break;
                case ISelfExpressionSyntax exp:
                    referencedSymbol = exp.ReferencedSymbol.Result;
                    break;
                default:
                    // implicit borrow isn't needed around other expressions
                    return;
            }

            type = referenceType.To(ReferenceCapability.Borrowed);

            expression = new ImplicitBorrowExpressionSyntax(expression, type, referencedSymbol);
        }

        private static void InsertImplicitMoveIfNeeded([NotNull] ref IExpressionSyntax expression, DataType type)
        {
            // Value types aren't moved
            if (!(type is ReferenceType referenceType)
                // Neither are non-moveable types
                || !referenceType.IsMovable)
                return;

            if (!(expression is INameExpressionSyntax name))
                // Implicit move not needed
                return;

            var referencedSymbol = name.ReferencedSymbol.Result;
            expression = new ImplicitMoveSyntax(expression, type, referencedSymbol);
            name.Semantics = ExpressionSemantics.Acquire;
            expression.Semantics = ExpressionSemantics.Acquire;
        }

        private static void InsertImplicitActionIfNeeded([NotNull] ref IExpressionSyntax expression, DataType toType, bool implicitBorrowAllowed)
        {
            var fromType = expression.DataType.Assigned();
            if (!(fromType is ReferenceType from) || !(toType is ReferenceType to)) return;

            if (@from.IsMovable && to.IsMovable)
                InsertImplicitMoveIfNeeded(ref expression, to);
            else if (@from.IsReadOnly || to.IsReadOnly)
                InsertImplicitShareIfNeeded(ref expression, to.ToReadOnly());
            else if (implicitBorrowAllowed)
                InsertImplicitBorrowIfNeeded(ref expression, to);
        }

        private DataType InferAssignmentTargetType([NotNull] ref IAssignableExpressionSyntax expression)
        {
            switch (expression)
            {
                default:
                    throw ExhaustiveMatch.Failed(expression);
                case IQualifiedNameExpressionSyntax exp:
                    // Don't wrap the self expression in a share expression for field access
                    var isSelfField = exp.Context is ISelfExpressionSyntax;
                    var contextType = InferType(ref exp.Context, !isSelfField);
                    var member = exp.Field;
                    var contextSymbol = LookupSymbolForType(contextType);
                    // TODO Deal with no context symbol
                    var memberSymbols = symbolTreeBuilder.Children(contextSymbol!).OfType<FieldSymbol>().Where(s => s.Name == member.Name).ToFixedList();
                    var type = AssignReferencedSymbolAndType(member, memberSymbols);
                    exp.Field.Semantics ??= ExpressionSemantics.CreateReference;
                    exp.Semantics = exp.Field.Semantics.Assigned();
                    return exp.DataType = type;
                case INameExpressionSyntax exp:
                    exp.Semantics = ExpressionSemantics.CreateReference;
                    return InferNameType(exp);
            }
        }

        private DataType InferMethodInvocationType(
            IQualifiedInvocationExpressionSyntax qualifiedInvocation,
            ref IExpressionSyntax expression)
        {
            // This could actually be any of the following since the parser can't distinguish them:
            // * Associated function invocation
            // * Namespaced function invocation
            // * Method invocation
            // First we need to distinguish those.
            var contextName = MethodContextAsName(qualifiedInvocation.Context);
            if (contextName != null)
            {
                var contextSymbols = qualifiedInvocation.ContainingLexicalScope.Lookup(contextName.Segments[0]).Select(p => p.Result);
                foreach (var name in contextName.Segments.Skip(1))
                    contextSymbols = contextSymbols.SelectMany(c => symbolTrees.Children(c).Where(s => s.Name == name));

                var functionSymbols = contextSymbols
                                      .SelectMany(c => symbolTrees
                                                       .Children(c).OfType<FunctionSymbol>()
                                                       .Where(s => s.Name == qualifiedInvocation.InvokedName))
                                      .ToFixedSet();
                if (functionSymbols.Any())
                {
                    // It is a namespaced or associated function invocation, modify the tree
                    var functionInvocation = new FunctionInvocationExpressionSyntax(
                        qualifiedInvocation.Span,
                        contextName,
                        qualifiedInvocation.InvokedName,
                        qualifiedInvocation.InvokedNameSpan,
                        qualifiedInvocation.Arguments,
                        functionSymbols);

                    expression = functionInvocation;
                    return InferFunctionInvocationType(functionInvocation);
                }
            }

            var argumentTypes = qualifiedInvocation.Arguments.Select(argument => InferType(ref argument.Expression)).ToFixedList();
            var contextType = InferType(ref qualifiedInvocation.Context, false);
            // If it is unknown, we already reported an error
            if (contextType == DataType.Unknown)
            {
                qualifiedInvocation.Semantics = ExpressionSemantics.Never;
                return qualifiedInvocation.DataType = DataType.Unknown;
            };

            var contextSymbol = LookupSymbolForType(contextType);
            var methodSymbols = symbolTrees.Children(contextSymbol!).OfType<MethodSymbol>()
                                                 .Where(s => s.Name == qualifiedInvocation.InvokedName).ToFixedList();
            methodSymbols = ResolveMethodOverload(contextType, methodSymbols, argumentTypes);

            switch (methodSymbols.Count)
            {
                case 0:
                    diagnostics.Add(NameBindingError.CouldNotBindMethod(file, qualifiedInvocation.Span));
                    qualifiedInvocation.ReferencedSymbol.Fulfill(null);
                    qualifiedInvocation.DataType = DataType.Unknown;
                    break;
                case 1:
                    var methodSymbol = methodSymbols.Single();
                    qualifiedInvocation.ReferencedSymbol.Fulfill(methodSymbol);

                    var selfParamType = methodSymbol.SelfDataType;
                    InsertImplicitActionIfNeeded(ref qualifiedInvocation.Context, selfParamType, implicitBorrowAllowed: true);

                    InsertImplicitConversionIfNeeded(ref qualifiedInvocation.Context, selfParamType);
                    CheckArgumentTypeCompatibility(selfParamType, qualifiedInvocation.Context);

                    foreach (var (arg, type) in qualifiedInvocation.Arguments
                                                                .Zip(methodSymbol.ParameterDataTypes))
                    {
                        InsertImplicitConversionIfNeeded(ref arg.Expression, type);
                        CheckArgumentTypeCompatibility(type, arg.Expression);
                    }

                    qualifiedInvocation.DataType = methodSymbol.ReturnDataType;
                    AssignInvocationSemantics(qualifiedInvocation, methodSymbol.ReturnDataType);
                    break;
                default:
                    diagnostics.Add(NameBindingError.AmbiguousMethodCall(file, qualifiedInvocation.Span));
                    qualifiedInvocation.ReferencedSymbol.Fulfill(null);
                    qualifiedInvocation.DataType = DataType.Unknown;
                    break;
            }

            return qualifiedInvocation.DataType;
        }

        /// <summary>
        /// Used on the target of a method invocation to see if it is could be the name of a namespace or class
        /// </summary>
        /// <returns>A name if the expression is a qualified name, otherwise null</returns>
        private static NamespaceName? MethodContextAsName(IExpressionSyntax expression)
        {
            return expression switch
            {
                IQualifiedNameExpressionSyntax memberAccess =>
                // if implicit self
                memberAccess.Context is null
                    ? null
                    : MethodContextAsName(memberAccess.Context)?.Qualify(memberAccess.Field.Name!),
                INameExpressionSyntax nameExpression => nameExpression.Name!,
                _ => null
            };
        }

        private DataType InferFunctionInvocationType(
            IUnqualifiedInvocationExpressionSyntax unqualifiedInvocationExpression)
        {
            var argumentTypes = unqualifiedInvocationExpression.Arguments.Select(argument => InferType(ref argument.Expression)).ToFixedList();
            var functionSymbols = unqualifiedInvocationExpression.LookupInContainingScope().Select(s => s.Result).ToFixedList();
            functionSymbols = ResolveOverload(functionSymbols, argumentTypes);
            switch (functionSymbols.Count)
            {
                case 0:
                    diagnostics.Add(NameBindingError.CouldNotBindFunction(file, unqualifiedInvocationExpression.Span));
                    unqualifiedInvocationExpression.ReferencedSymbol.Fulfill(null);
                    unqualifiedInvocationExpression.DataType = DataType.Unknown;
                    unqualifiedInvocationExpression.Semantics = ExpressionSemantics.Never;
                    break;
                case 1:
                    var functionSymbol = functionSymbols.Single();
                    unqualifiedInvocationExpression.ReferencedSymbol.Fulfill(functionSymbol);
                    foreach (var (arg, parameterDataType) in unqualifiedInvocationExpression.Arguments.Zip(functionSymbol.ParameterDataTypes))
                    {
                        InsertImplicitConversionIfNeeded(ref arg.Expression, parameterDataType);
                        CheckArgumentTypeCompatibility(parameterDataType, arg.Expression);
                    }

                    unqualifiedInvocationExpression.DataType = functionSymbol.ReturnDataType;
                    AssignInvocationSemantics(unqualifiedInvocationExpression, functionSymbol.ReturnDataType);
                    break;
                default:
                    diagnostics.Add(NameBindingError.AmbiguousFunctionCall(file, unqualifiedInvocationExpression.Span));
                    unqualifiedInvocationExpression.ReferencedSymbol.Fulfill(null);
                    unqualifiedInvocationExpression.DataType = DataType.Unknown;
                    unqualifiedInvocationExpression.Semantics = ExpressionSemantics.Never;
                    break;
            }
            return unqualifiedInvocationExpression.DataType;
        }

        private static void AssignInvocationSemantics(
            IInvocationExpressionSyntax invocationExpression,
            DataType type)
        {
            switch (type.Semantics)
            {
                default:
                    throw ExhaustiveMatch.Failed(type.Semantics);
                case TypeSemantics.Void:
                    invocationExpression.Semantics = ExpressionSemantics.Void;
                    break;
                case TypeSemantics.Move:
                    invocationExpression.Semantics = ExpressionSemantics.Move;
                    break;
                case TypeSemantics.Copy:
                    invocationExpression.Semantics = ExpressionSemantics.Copy;
                    break;
                case TypeSemantics.Never:
                    invocationExpression.Semantics = ExpressionSemantics.Never;
                    break;
                case TypeSemantics.Reference:
                    var referenceType = (ReferenceType)type;
                    if (referenceType.ReferenceCapability.CanBeAcquired())
                        invocationExpression.Semantics = ExpressionSemantics.Acquire;
                    else if (referenceType.IsMutable)
                        invocationExpression.Semantics = ExpressionSemantics.Borrow;
                    else
                        invocationExpression.Semantics = ExpressionSemantics.Share;
                    break;
            }
        }

        private DataType InferBlockType(IBlockOrResultSyntax blockOrResult)
        {
            switch (blockOrResult)
            {
                default:
                    throw ExhaustiveMatch.Failed(blockOrResult);
                case IBlockExpressionSyntax block:
                    foreach (var statement in block.Statements)
                        ResolveTypes(statement);

                    block.Semantics = ExpressionSemantics.Void;
                    return block.DataType = DataType.Void; // TODO assign the correct type to the block
                case IResultStatementSyntax result:
                    InferType(ref result.Expression);
                    return result.Expression.DataType!;
            }
        }

        public DataType InferNameType(INameExpressionSyntax nameExpression)
        {
            if (nameExpression.Name is null)
            {
                // Name unknown, no error
                nameExpression.ReferencedSymbol.Fulfill(null);
                return nameExpression.DataType = DataType.Unknown;
            }

            DataType? type;
            var symbols = nameExpression.LookupInContainingScope().ToFixedList();
            switch (symbols.Count)
            {
                case 0:
                    diagnostics.Add(NameBindingError.CouldNotBindName(file, nameExpression.Span));
                    nameExpression.ReferencedSymbol.Fulfill(null);
                    type = DataType.Unknown;
                    break;
                case 1:
                    var symbol = symbols.Single().Result;
                    // TODO fulfill the ReferencedSymbol once we have a correct symbol
                    nameExpression.ReferencedSymbol.Fulfill(symbol);
                    type = symbol.DataType;
                    break;
                default:
                    diagnostics.Add(NameBindingError.AmbiguousName(file, nameExpression.Span));
                    nameExpression.ReferencedSymbol.Fulfill(null);
                    type = DataType.Unknown;
                    break;
            }

            return nameExpression.DataType = type!;
        }

        private DataType InferSelfType(ISelfExpressionSyntax selfExpression)
        {
            DataType type;
            switch (containingSymbol)
            {
                default:
                    throw ExhaustiveMatch.Failed(containingSymbol);
                case MethodSymbol _:
                case ConstructorSymbol _:
                    var symbols = symbolTreeBuilder.Children(containingSymbol).OfType<SelfParameterSymbol>().ToList();
                    switch (symbols.Count)
                    {
                        case 0:
                            diagnostics.Add(NameBindingError.CouldNotBindName(file, selfExpression.Span));
                            type = DataType.Unknown;
                            selfExpression.ReferencedSymbol.Fulfill(null);
                            break;
                        case 1:
                            var symbol = symbols.Single();
                            type = symbol.DataType;
                            selfExpression.ReferencedSymbol.Fulfill(symbol);
                            break;
                        default:
                            diagnostics.Add(NameBindingError.AmbiguousName(file, selfExpression.Span));
                            type = DataType.Unknown;
                            selfExpression.ReferencedSymbol.Fulfill(null);
                            break;
                    }
                    break;
                case FunctionSymbol _:
                    diagnostics.Add(selfExpression.IsImplicit
                        ? SemanticError.ImplicitSelfOutsideMethod(file, selfExpression.Span)
                        : SemanticError.SelfOutsideMethod(file, selfExpression.Span));
                    type = DataType.Unknown;
                    selfExpression.ReferencedSymbol.Fulfill(null);
                    break;
                case NamespaceOrPackageSymbol _:
                case BindingSymbol _:
                case TypeSymbol _:
                    throw new InvalidOperationException("Invalid containing symbol for body");
            }

            return selfExpression.DataType = type;
        }

        /// <summary>
        /// Eventually, a `foreach` `in` expression will just be a regular expression. However, at the
        /// moment, there isn't enough of the language to implement range expressions. So this
        /// check handles range expressions in the specific case of `foreach` only. It marks them
        /// as having the same type as the range endpoints.
        /// </summary>
        private DataType CheckForeachInType(DataType? declaredType, ref IExpressionSyntax inExpression)
        {
            switch (inExpression)
            {
                case IBinaryOperatorExpressionSyntax binaryExpression
                    when binaryExpression.Operator == BinaryOperator.DotDot
                    || binaryExpression.Operator == BinaryOperator.LessThanDotDot
                    || binaryExpression.Operator == BinaryOperator.DotDotLessThan
                    || binaryExpression.Operator == BinaryOperator.LessThanDotDotLessThan:
                    var leftType = InferType(ref binaryExpression.LeftOperand);
                    InferType(ref binaryExpression.RightOperand);
                    if (declaredType != null)
                    {
                        leftType = InsertImplicitConversionIfNeeded(ref binaryExpression.LeftOperand, declaredType);
                        InsertImplicitConversionIfNeeded(ref binaryExpression.RightOperand, declaredType);
                    }

                    inExpression.Semantics = ExpressionSemantics.Copy; // Treat ranges as structs
                    return inExpression.DataType = leftType;
                default:
                    return InferType(ref inExpression);
            }
        }

        private void CheckArgumentTypeCompatibility(DataType type, IExpressionSyntax arg)
        {
            var fromType = arg.DataType ?? throw new ArgumentException("argument must have a type");
            if (!type.IsAssignableFrom(fromType))
                diagnostics.Add(TypeError.CannotConvert(file, arg, fromType, type));
        }

        private DataType AssignReferencedSymbolAndType(
            INameExpressionSyntax exp,
            FixedList<FieldSymbol> matchingSymbols)
        {
            switch (matchingSymbols.Count)
            {
                case 0:
                    diagnostics.Add(NameBindingError.CouldNotBindMember(file, exp.Span));
                    exp.Semantics = ExpressionSemantics.Never;
                    exp.ReferencedSymbol.Fulfill(null);
                    return exp.DataType = DataType.Unknown;
                case 1:
                    var memberSymbol = matchingSymbols.Single();
                    switch (memberSymbol.DataType.Semantics)
                    {
                        default:
                            throw ExhaustiveMatch.Failed(memberSymbol.DataType.Semantics);
                        case TypeSemantics.Copy:
                            exp.Semantics = ExpressionSemantics.Copy;
                            break;
                        case TypeSemantics.Never:
                            exp.Semantics = ExpressionSemantics.Never;
                            break;
                        case TypeSemantics.Reference:
                            // Needs to be assigned based on share/borrow expression
                            break;
                        case TypeSemantics.Move:
                            throw new InvalidOperationException("Can't move out of field");
                        case TypeSemantics.Void:
                            throw new InvalidOperationException("Can't assign semantics to void field");
                    }

                    exp.ReferencedSymbol.Fulfill(memberSymbol);
                    return exp.DataType = memberSymbol.DataType;
                default:
                    diagnostics.Add(NameBindingError.AmbiguousName(file, exp.Span));
                    exp.Semantics = ExpressionSemantics.Never;
                    exp.ReferencedSymbol.Fulfill(null);
                    return exp.DataType = DataType.Unknown;
            }
        }

        private static bool NumericOperatorTypesAreCompatible(
            ref IExpressionSyntax leftOperand,
            ref IExpressionSyntax rightOperand)
        {
            var leftType = leftOperand.DataType;
            switch (leftType)
            {
                default:
                    // In theory we could just make the default false, but this
                    // way we are forced to note exactly which types this doesn't work on.
                    throw ExhaustiveMatch.Failed(leftType);
                case IntegerConstantType _:
                    // TODO may need to promote based on size
                    throw new NotImplementedException();
                //return !IsIntegerType(rightType);
                case PointerSizedIntegerType integerType:
                    // TODO this isn't right we might need to convert either of them
                    InsertImplicitConversionIfNeeded(ref rightOperand, integerType);
                    return rightOperand.DataType is PointerSizedIntegerType;
                case FixedSizeIntegerType integerType:
                    // TODO this isn't right we might need to convert either of them
                    InsertImplicitConversionIfNeeded(ref rightOperand, integerType);
                    return rightOperand.DataType is FixedSizeIntegerType;
                case OptionalType _:
                    throw new NotImplementedException("Trying to do math on optional type");
                case NeverType _:
                case UnknownType _:
                    return true;
                case ReferenceType _:
                case BoolType _:
                case VoidType _: // This might need a special error message
                    return false;
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
            return typeResolver.Evaluate(typeSyntax);
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

        private static FixedList<TSymbol> ResolveOverload<TSymbol>(
            FixedList<TSymbol> symbols,
            FixedList<DataType> argumentTypes)
            where TSymbol : InvocableSymbol
        {
            // Filter down to symbols that could possible match
            symbols = symbols.Where(s =>
            {
                if (s.Arity != argumentTypes.Count) return false;
                // TODO check compatibility over argument types
                return true;
            }).ToFixedList();
            // TODO Select most specific match
            return symbols;
        }

        private static FixedList<MethodSymbol> ResolveMethodOverload(
            DataType selfType,
            FixedList<MethodSymbol> symbols,
            FixedList<DataType> argumentTypes)
        {
            // Filter down to symbols that could possible match
            symbols = symbols.Where(s =>
            {
                if (s.Arity != argumentTypes.Count) return false;
                // TODO check compatibility of self type
                _ = selfType;
                // TODO check compatibility over argument types

                return true;
            }).ToFixedList();

            // TODO Select most specific match
            return symbols;
        }

        private TypeSymbol? LookupSymbolForType(DataType dataType)
        {
            return dataType switch
            {
                UnknownType _ => null,
                ObjectType objectType => LookupSymbolForType(objectType),
                IntegerType integerType => symbolTrees.PrimitiveSymbolTree
                                                      .GlobalSymbols
                                                      .OfType<TypeSymbol>()
                                                      .Single(s => s.DeclaresDataType == integerType),
                _ => throw new NotImplementedException(
                    $"{nameof(LookupSymbolForType)} not implemented for {dataType.GetType().Name}")
            };
        }

        private TypeSymbol? LookupSymbolForType(ObjectType objectType)
        {

            var contextSymbols = symbolTrees.Packages.SafeCast<Symbol>();
            foreach (var name in objectType.ContainingNamespace.Segments)
            {
                contextSymbols = contextSymbols.SelectMany(c => symbolTrees.Children(c))
                                               .Where(s => s.Name == name);
            }

            return contextSymbols.SelectMany(c => symbolTrees.Children(c)).OfType<TypeSymbol>()
                                 .Single(s => s.Name == objectType.Name);
        }
    }
}
