using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Scopes;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Basic.ImplicitOperations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Basic.InferredSyntax;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
using ExhaustiveMatching;
using DataType = Adamant.Tools.Compiler.Bootstrap.Metadata.Types.DataType;
using UnaryOperator = Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.UnaryOperator;

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
        private readonly ITypeSymbol? stringSymbol;
        private readonly Diagnostics diagnostics;
        private readonly DataType? returnType;
        private readonly BasicTypeAnalyzer typeAnalyzer;

        public BasicBodyAnalyzer(
            CodeFile file,
            ITypeSymbol? stringSymbol,
            Diagnostics diagnostics,
            DataType? returnType = null)
        {
            this.file = file;
            this.stringSymbol = stringSymbol;
            this.diagnostics = diagnostics;
            this.returnType = returnType;
            typeAnalyzer = new BasicTypeAnalyzer(file, diagnostics);
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
            if (variableDeclaration.TypeSyntax != null)
            {
                type = typeAnalyzer.Evaluate(variableDeclaration.TypeSyntax);
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
                var initializerType = variableDeclaration.Initializer.Type ?? throw new InvalidOperationException("Initializer type should be determined");

                if (!type.IsAssignableFrom(initializerType))
                    diagnostics.Add(TypeError.CannotConvert(file, variableDeclaration.Initializer, initializerType, type));
            }

            variableDeclaration.Type = type;
        }

        /// <summary>
        /// Infer the type of a variable declaration from an expression
        /// </summary>
        private DataType InferDeclarationType([NotNull] ref IExpressionSyntax expression, bool inferMutableType)
        {
            var type = InferType(ref expression);
            if (!type.IsKnown) return DataType.Unknown;
            if (type is IntegerConstantType _)
                // TODO there should be a method that combines this with type inference
                return InsertImplicitConversionIfNeeded(ref expression, DataType.Int);

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

                    return referenceType.WithCapability(ReferenceCapability.Borrowed);
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
            switch (expectedExpressionType: expectedType, expression.Type)
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
                case (SizedIntegerType targetType, SizedIntegerType expressionType):
                    if (targetType.Bits > expressionType.Bits && (!expressionType.IsSigned || targetType.IsSigned))
                        expression = new ImplicitNumericConversionExpression(expression, targetType);
                    break;
                case (SizedIntegerType targetType, IntegerConstantType expressionType):
                {
                    var requireSigned = expressionType.Value < 0;
                    var bits = expressionType.Value.GetByteCount() * 8;
                    if (targetType.Bits >= bits && (!requireSigned || targetType.IsSigned))
                        expression = new ImplicitNumericConversionExpression(expression, targetType);
                }
                break;
                case (UnsizedIntegerType targetType, IntegerConstantType expressionType):
                {
                    var requireSigned = expressionType.Value < 0;
                    if (!requireSigned || targetType.IsSigned)
                        expression = new ImplicitNumericConversionExpression(expression, targetType);
                }
                break;
                case (UserObjectType targetType, UserObjectType expressionType)
                        when targetType.IsReadOnly && expressionType.IsMutable:
                    // TODO if source type is explicitly mutable, issue warning about using `mut` in immutable context
                    expression = new ImplicitImmutabilityConversionExpression(expression, expressionType.ToReadOnly());
                    break;
            }

            return expression.Type!;
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
                            nameExpression.ValueSemantics = ValueSemantics.Own;
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

                            exp.MovedSymbol = nameExpression.ReferencedSymbol!;
                            return exp.Type = type;
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
                            nameExpression.ValueSemantics = ValueSemantics.Borrow;
                            var type = InferType(ref exp.Referent, false);
                            switch (type)
                            {
                                case ReferenceType referenceType:
                                    if (!referenceType.IsMutable)
                                    {
                                        diagnostics.Add(
                                            TypeError.ExpressionCantBeMutable(file, exp.Referent));
                                        type = DataType.Unknown;
                                    }
                                    else
                                        type = referenceType.WithCapability(ReferenceCapability.Borrowed);

                                    break;
                                default:
                                    throw new NotImplementedException("Non-mutable type can't be borrowed mutably");
                            }

                            exp.BorrowedSymbol = nameExpression.ReferencedSymbol!;
                            return exp.Type = type;
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
                    if (exp.ReturnValue != null)
                    {
                        var expectedReturnType = returnType ?? throw new InvalidOperationException("Return statement in constructor");
                        // TODO this is inserting an implicit share when it isn't wanted
                        CheckType(ref exp.ReturnValue, expectedReturnType);

                        // If we return ownership, there can be an implicit move
                        InsertImplicitMoveIfNeeded(ref exp.ReturnValue, expectedReturnType);
                    }
                    else if (returnType == DataType.Never)
                        diagnostics.Add(TypeError.CantReturnFromNeverFunction(file, exp.Span));
                    else if (returnType != DataType.Void)
                        diagnostics.Add(TypeError.ReturnExpressionMustHaveValue(file, exp.Span, returnType ?? DataType.Unknown));

                    return exp.Type = DataType.Never;
                }
                case IIntegerLiteralExpressionSyntax exp:
                    return exp.Type = new IntegerConstantType(exp.Value);
                case IStringLiteralExpressionSyntax exp:
                    return exp.Type = stringSymbol?.DeclaresType ?? DataType.Unknown;
                case IBoolLiteralExpressionSyntax exp:
                    return exp.Type = DataType.Bool;
                case IBinaryOperatorExpressionSyntax binaryOperatorExpression:
                {
                    var leftType = InferType(ref binaryOperatorExpression.LeftOperand);
                    var @operator = binaryOperatorExpression.Operator;
                    var rightType = InferType(ref binaryOperatorExpression.RightOperand);

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
                            compatible = NumericOperatorTypesAreCompatible(ref binaryOperatorExpression.LeftOperand, ref binaryOperatorExpression.RightOperand);
                            binaryOperatorExpression.Type = compatible ? leftType : DataType.Unknown;
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
                            binaryOperatorExpression.Type = DataType.Bool;
                            break;
                        case BinaryOperator.And:
                        case BinaryOperator.Or:
                            compatible = leftType == DataType.Bool && rightType == DataType.Bool;
                            binaryOperatorExpression.Type = DataType.Bool;
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

                    return binaryOperatorExpression.Type;
                }
                case INameExpressionSyntax exp:
                {
                    var type = InferNameType(exp);
                    // In many contexts, variable names are implicitly shared
                    if (implicitShare)
                        type = InsertImplicitShareIfNeeded(ref expression, type);

                    if (exp.ValueSemantics is null
                        && (type.ValueSemantics == ValueSemantics.Copy
                            || type.ValueSemantics == ValueSemantics.Move))
                        exp.ValueSemantics = type.ValueSemantics;

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
                            exp.Type = DataType.Bool;
                            break;
                        case UnaryOperator.Minus:
                        case UnaryOperator.Plus:
                            var operandType = InferType(ref exp.Operand);
                            switch (operandType)
                            {
                                case IntegerConstantType integerType:
                                    exp.Type = integerType;
                                    break;
                                case SizedIntegerType sizedIntegerType:
                                    exp.Type = sizedIntegerType;
                                    break;
                                case UnknownType _:
                                    exp.Type = DataType.Unknown;
                                    break;
                                default:
                                    diagnostics.Add(TypeError.OperatorCannotBeAppliedToOperandOfType(file,
                                        exp.Span, @operator, operandType));
                                    exp.Type = DataType.Unknown;
                                    break;
                            }
                            break;
                    }

                    return exp.Type;
                }
                case INewObjectExpressionSyntax exp:
                {
                    var argumentTypes = exp.Arguments.Select(argument => InferType(ref argument.Expression)).ToFixedList();
                    // TODO handle named constructors here
                    var constructingType = typeAnalyzer.Evaluate(exp.TypeSyntax);
                    if (!constructingType.IsKnown)
                    {
                        diagnostics.Add(NameBindingError.CouldNotBindConstructor(file, exp.Span));
                        exp.ConstructorSymbol = UnknownSymbol.Instance;
                        return exp.Type  = DataType.Unknown;
                    }
                    var constructedType = (UserObjectType)constructingType;
                    var typeSymbol = exp.TypeSyntax.ContainingScope.Assigned().GetSymbolForType(constructedType);
                    var constructors = typeSymbol.ChildSymbols[SpecialName.New].OfType<IFunctionSymbol>().ToFixedList();
                    constructors = ResolveOverload(constructors, argumentTypes);
                    switch (constructors.Count)
                    {
                        case 0:
                            diagnostics.Add(NameBindingError.CouldNotBindConstructor(file, exp.Span));
                            exp.ConstructorSymbol = UnknownSymbol.Instance;
                            break;
                        case 1:
                            var constructorSymbol = constructors.Single();
                            exp.ConstructorSymbol = constructorSymbol;
                            foreach (var (arg, parameter) in exp.Arguments.Zip(constructorSymbol.Parameters))
                            {
                                InsertImplicitConversionIfNeeded(ref arg.Expression, parameter.Type);
                                CheckArgumentTypeCompatibility(parameter.Type, arg.Expression);
                            }
                            break;
                        default:
                            diagnostics.Add(NameBindingError.AmbiguousConstructorCall(file, exp.Span));
                            exp.ConstructorSymbol = UnknownSymbol.Instance;
                            break;
                    }

                    constructedType = constructedType.WithCapability(ReferenceCapability.Isolated);
                    if (constructedType.DeclaredMutable)
                        constructedType = constructedType.ToMutable();
                    return exp.Type = constructedType;
                }
                case IForeachExpressionSyntax exp:
                {
                    var declaredType = typeAnalyzer.Evaluate(exp.TypeSyntax);
                    var expressionType = CheckForeachInType(declaredType, ref exp.InExpression);
                    exp.VariableType =  declaredType ?? expressionType;
                    // TODO check the break types
                    InferBlockType(exp.Block);
                    // TODO assign correct type to the expression
                    return exp.Type = DataType.Void;
                }
                case IWhileExpressionSyntax exp:
                {
                    CheckType(ref exp.Condition, DataType.Bool);
                    InferBlockType(exp.Block);
                    // TODO assign correct type to the expression
                    return exp.Type = DataType.Void;
                }
                case ILoopExpressionSyntax exp:
                    InferBlockType(exp.Block);
                    // TODO assign correct type to the expression
                    return exp.Type = DataType.Void;
                case IMethodInvocationExpressionSyntax exp:
                    return InferMethodInvocationType(exp, ref expression);
                case IFunctionInvocationExpressionSyntax exp:
                    return InferFunctionInvocationType(exp);
                case IUnsafeExpressionSyntax exp:
                    return exp.Type = InferType(ref exp.Expression);
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
                    return exp.Type = DataType.Void;
                case IFieldAccessExpressionSyntax exp:
                {
                    // Don't wrap the self expression in a share expression for field access
                    var isSelfField = exp.ContextExpression is ISelfExpressionSyntax;
                    var contextType = InferType(ref exp.ContextExpression, !isSelfField);
                    var contextSymbol = exp.Field.ContainingScope.Assigned().GetSymbolForType(contextType);
                    var member = exp.Field;
                    var memberSymbols = contextSymbol.Lookup(member.Name).OfType<IBindingSymbol>().ToFixedList();
                    var type = AssignReferencedSymbolAndType(member, memberSymbols);
                    return exp.Type = type;
                }
                case IBreakExpressionSyntax exp:
                    InferType(ref exp.Value);
                    return exp.Type = DataType.Never;
                case INextExpressionSyntax exp:
                    return exp.Type = DataType.Never;
                case IAssignmentExpressionSyntax assignmentExpression:
                {
                    var left = InferAssignmentTargetType(ref assignmentExpression.LeftOperand);
                    InferType(ref assignmentExpression.RightOperand);
                    InsertImplicitConversionIfNeeded(ref assignmentExpression.RightOperand, left);
                    var right = assignmentExpression.RightOperand.Type ?? throw new InvalidOperationException();
                    if (!left.IsAssignableFrom(right))
                        diagnostics.Add(TypeError.CannotConvert(file,
                            assignmentExpression.RightOperand, right, left));
                    return assignmentExpression.Type = DataType.Void;
                }
                case ISelfExpressionSyntax exp:
                {
                    var type = InferSelfType(exp);
                    if (implicitShare)
                        type = InsertImplicitShareIfNeeded(ref expression, type);

                    return type;
                }
                case INoneLiteralExpressionSyntax exp:
                    return exp.Type = DataType.None;
                case IImplicitConversionExpression _:
                    throw new Exception("ImplicitConversionExpressions are inserted by BasicExpressionAnalyzer. They should not be present in the AST yet.");
                case IBlockExpressionSyntax blockSyntax:
                    return InferBlockType(blockSyntax);
            }
        }

        private static DataType InsertImplicitShareIfNeeded([NotNull] ref IExpressionSyntax expression, DataType type)
        {
            // Value types aren't shared
            if (!(type is ReferenceType referenceType)) return type;

            IBindingSymbol referencedSymbol;
            switch (expression)
            {
                case INameExpressionSyntax exp:
                    exp.ValueSemantics = ValueSemantics.Share;
                    referencedSymbol = exp.ReferencedSymbol.Assigned();
                    break;
                case ISelfExpressionSyntax exp:
                    referencedSymbol = exp.ReferencedSymbol.Assigned();
                    break;
                default:
                    // implicit share isn't needed around other expressions
                    return type;
            }

            type = referenceType.WithCapability(ReferenceCapability.Shared);

            expression = new ImplicitShareExpressionSyntax(expression, type, referencedSymbol);

            return type;
        }

        private static DataType InsertImplicitBorrowIfNeeded([NotNull] ref IExpressionSyntax expression, DataType type)
        {
            // Value types aren't shared
            if (!(type is ReferenceType referenceType)) return type;

            IBindingSymbol referencedSymbol;
            switch (expression)
            {
                case INameExpressionSyntax exp:
                    exp.ValueSemantics = ValueSemantics.Borrow;
                    referencedSymbol = exp.ReferencedSymbol.Assigned();
                    break;
                case ISelfExpressionSyntax exp:
                    referencedSymbol = exp.ReferencedSymbol.Assigned();
                    break;
                default:
                    // implicit borrow isn't needed around other expressions
                    return type;
            }

            type = referenceType.WithCapability(ReferenceCapability.Borrowed);

            expression = new ImplicitBorrowExpressionSyntax(expression, type, referencedSymbol);

            return type;
        }

        private static DataType InsertImplicitMoveIfNeeded(
            [NotNull] ref IExpressionSyntax expression,
            DataType type)
        {
            // Value types aren't moved
            if (!(type is ReferenceType referenceType)
                // Neither are non-moveable types
                || !referenceType.IsMovable) return type;

            if (!(expression is INameExpressionSyntax name))
                // Implicit move not needed
                return type;

            var referencedSymbol = name.ReferencedSymbol.Assigned();
            expression = new ImplicitMoveSyntax(expression, type, referencedSymbol);

            return type;
        }

        private DataType InferAssignmentTargetType([NotNull] ref IAssignableExpressionSyntax expression)
        {
            switch (expression)
            {
                default:
                    throw ExhaustiveMatch.Failed(expression);
                case IFieldAccessExpressionSyntax exp:
                    // Don't wrap the self expression in a share expression for field access
                    var isSelfField = exp.ContextExpression is ISelfExpressionSyntax;
                    var contextType = InferType(ref exp.ContextExpression, !isSelfField);
                    var contextSymbol = exp.Field.ContainingScope.Assigned().GetSymbolForType(contextType);
                    var member = exp.Field;
                    var memberSymbols = contextSymbol.Lookup(member.Name).OfType<IBindingSymbol>().ToFixedList();
                    var type = AssignReferencedSymbolAndType(member, memberSymbols);
                    return exp.Type = type;
                case INameExpressionSyntax exp:
                    exp.ValueSemantics = ValueSemantics.LValue;
                    return InferNameType(exp);
            }
        }

        private DataType InferMethodInvocationType(
            IMethodInvocationExpressionSyntax methodInvocation,
            ref IExpressionSyntax expression)
        {
            // This could actually be any of the following since the parser can't distinguish them:
            // * Associated function invocation
            // * Namespaced function invocation
            // * Method invocation
            // First we need to distinguish those.
            var targetName = MethodContextAsName(methodInvocation.ContextExpression);
            if (targetName != null)
            {
                var scope = methodInvocation.MethodNameSyntax.ContainingScope.Assigned();

                var functionName = targetName.Qualify(methodInvocation.MethodNameSyntax.Name);
                // This will find both namespaced function calls and associated function calls
                if (scope.Lookup(functionName).OfType<IFunctionSymbol>().Any())
                {
                    // It is a namespaced or associated function invocation, modify the tree
                    var nameSpan = TextSpan.Covering(methodInvocation.ContextExpression.Span, methodInvocation.MethodNameSyntax.Span);
                    var nameSyntax = new CallableNameSyntax(nameSpan, functionName, scope);
                    var functionInvocation = new FunctionInvocationExpressionSyntax(
                        methodInvocation.Span,
                        nameSyntax,
                        functionName,
                        methodInvocation.Arguments);

                    expression = functionInvocation;
                    return InferFunctionInvocationType(functionInvocation);
                }
            }

            var argumentTypes = methodInvocation.Arguments.Select(argument => InferType(ref argument.Expression)).ToFixedList();
            var contextType = InferType(ref methodInvocation.ContextExpression, false);
            // If it is unknown, we already reported an error
            if (contextType == DataType.Unknown) return methodInvocation.Type = DataType.Unknown;

            var contextTypeSymbol = methodInvocation.MethodNameSyntax.ContainingScope.Assigned().GetSymbolForType(contextType);
            contextTypeSymbol.ChildSymbols.TryGetValue((SimpleName)methodInvocation.MethodNameSyntax.Name, out var childSymbols);
            var methodSymbols = (childSymbols ?? FixedList<ISymbol>.Empty).OfType<IMethodSymbol>().ToFixedList();
            methodSymbols = ResolveOverload(contextType, methodSymbols, argumentTypes);
            switch (methodSymbols.Count)
            {
                case 0:
                    diagnostics.Add(NameBindingError.CouldNotBindMethod(file, methodInvocation.Span));
                    methodInvocation.MethodNameSyntax.ReferencedSymbol = UnknownSymbol.Instance;
                    methodInvocation.Type = DataType.Unknown;
                    break;
                case 1:
                    var methodSymbol = methodSymbols.Single();
                    methodInvocation.MethodNameSyntax.ReferencedSymbol = methodSymbol;

                    var selfParamType = methodSymbol.SelfParameterSymbol.Type;
                    if (selfParamType is ReferenceType paramType
                        && contextType is ReferenceType argType)
                        switch (paramType.IsReadOnly, argType.IsReadOnly)
                        {
                            // TODO what about move?
                            case (true, _):
                                InsertImplicitShareIfNeeded(ref methodInvocation.ContextExpression, argType.ToReadOnly());
                                break;
                            case (false, false):
                                InsertImplicitBorrowIfNeeded(ref methodInvocation.ContextExpression, argType);
                                break;
                        }

                    InsertImplicitConversionIfNeeded(ref methodInvocation.ContextExpression, selfParamType);
                    CheckArgumentTypeCompatibility(selfParamType, methodInvocation.ContextExpression);

                    foreach (var (arg, type) in methodInvocation.Arguments.Zip(methodSymbol
                                                                               .Parameters.Select(p => p.Type)))
                    {
                        InsertImplicitConversionIfNeeded(ref arg.Expression, type);
                        CheckArgumentTypeCompatibility(type, arg.Expression);
                    }

                    methodInvocation.Type = methodSymbol.ReturnType;
                    break;
                default:
                    diagnostics.Add(NameBindingError.AmbiguousMethodCall(file, methodInvocation.Span));
                    methodInvocation.MethodNameSyntax.ReferencedSymbol = UnknownSymbol.Instance;
                    methodInvocation.Type = DataType.Unknown;
                    break;
            }

            return methodInvocation.Type;
        }

        /// <summary>
        /// Used on the target of a method invocation to see if it is really the name of a namespace or class
        /// </summary>
        /// <returns>A name if the expression is a qualified name, otherwise null</returns>
        private static Name? MethodContextAsName(IExpressionSyntax expression)
        {
            return expression switch
            {
                IFieldAccessExpressionSyntax memberAccess =>
                // if implicit self
                memberAccess.ContextExpression is null
                    ? null
                    : MethodContextAsName(memberAccess.ContextExpression)?.Qualify(memberAccess.Field.Name),
                INameExpressionSyntax nameExpression => nameExpression.Name,
                _ => null
            };
        }

        private DataType InferFunctionInvocationType(IFunctionInvocationExpressionSyntax functionInvocationExpression)
        {
            var argumentTypes = functionInvocationExpression.Arguments.Select(argument => InferType(ref argument.Expression)).ToFixedList();
            var scope = functionInvocationExpression.FunctionNameSyntax.ContainingScope.Assigned();
            var functionSymbols = scope.Lookup(functionInvocationExpression.FullName)
                .OfType<IFunctionSymbol>().ToFixedList();
            functionSymbols = ResolveOverload(functionSymbols, argumentTypes);
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
                        ResolveTypes(statement);

                    return block.Type = DataType.Void; // TODO assign the correct type to the block
                case IResultStatementSyntax result:
                    InferType(ref result.Expression);
                    return result.Expression.Type!;
            }
        }

        public DataType InferNameType(INameExpressionSyntax nameExpression)
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
                        }
                        break;
                        default:
                            diagnostics.Add(NameBindingError.CouldNotBindName(file, nameExpression.Span));
                            nameExpression.ReferencedSymbol = UnknownSymbol.Instance;
                            type = DataType.Unknown;
                            break;
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

        private DataType InferSelfType(ISelfExpressionSyntax selfExpression)
        {
            var symbols = selfExpression.LookupInContainingScope();
            DataType type;
            switch (symbols.Count)
            {
                case 0:
                    diagnostics.Add(selfExpression.IsImplicit
                        ? SemanticError.ImplicitSelfOutsideMethod(file, selfExpression.Span)
                        : SemanticError.SelfOutsideMethod(file, selfExpression.Span));
                    selfExpression.ReferencedSymbol = UnknownSymbol.Instance;
                    type = DataType.Unknown;
                    break;
                case 1:
                {
                    var symbol = symbols.Single();
                    switch (symbol)
                    {
                        case IBindingSymbol binding:
                        {
                            selfExpression.ReferencedSymbol = binding;
                            type = binding.Type;
                        }
                        break;
                        default:
                            diagnostics.Add(NameBindingError.CouldNotBindName(file, selfExpression.Span));
                            selfExpression.ReferencedSymbol = UnknownSymbol.Instance;
                            type = DataType.Unknown;
                            break;
                    }

                    break;
                }
                default:
                    diagnostics.Add(NameBindingError.AmbiguousName(file, selfExpression.Span));
                    selfExpression.ReferencedSymbol = UnknownSymbol.Instance;
                    type = DataType.Unknown;
                    break;
            }

            return selfExpression.Type = type;
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
                    return inExpression.Type = leftType;
                default:
                    return InferType(ref inExpression);
            }
        }

        private void CheckArgumentTypeCompatibility(DataType type, IExpressionSyntax arg)
        {
            var fromType = arg.Type ?? throw new ArgumentException("argument must have a type");
            if (!type.IsAssignableFrom(fromType))
                diagnostics.Add(TypeError.CannotConvert(file, arg, fromType, type));
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

        private static bool NumericOperatorTypesAreCompatible(
            ref IExpressionSyntax leftOperand,
            ref IExpressionSyntax rightOperand)
        {
            var leftType = leftOperand.Type;
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

        private static FixedList<IFunctionSymbol> ResolveOverload(
            FixedList<IFunctionSymbol> symbols,
            FixedList<DataType> argumentTypes)
        {
            // Filter down to symbols that could possible match
            symbols = symbols.Where(f =>
            {
                if (f.Arity() != argumentTypes.Count) return false;
                // TODO check compatibility over argument types
                return true;
            }).ToFixedList();
            // TODO Select most specific match
            return symbols;
        }

        private static FixedList<IMethodSymbol> ResolveOverload(
            DataType selfType,
            FixedList<IMethodSymbol> symbols,
            FixedList<DataType> argumentTypes)
        {
            // Filter down to symbols that could possible match
            symbols = symbols.Where(f =>
            {
                if (f.Arity() != argumentTypes.Count)
                    return false;
                // TODO check compatibility of self type
                _ = selfType;
                // TODO check compatibility over argument types

                return true;
            }).ToFixedList();
            // TODO Select most specific match
            return symbols;
        }
    }
}
