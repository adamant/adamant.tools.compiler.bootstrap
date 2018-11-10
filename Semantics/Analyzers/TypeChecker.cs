using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
using Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
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
                        CheckFunctionSignature(f);
                        break;
                    case TypeDeclarationAnalysis t:
                        CheckTypeDeclaration(t);
                        break;
                    default:
                        throw NonExhaustiveMatchException.For(analysis);
                }

            // Now that the signatures are done, we can check the bodies
            foreach (var function in analyses.OfType<FunctionDeclarationAnalysis>())
                CheckFunctionBody(function);
        }

        private void CheckFunctionSignature([NotNull] FunctionDeclarationAnalysis function)
        {
            // Check the signature first
            function.Type.BeginComputing();
            function.ReturnType.BeginComputing();
            if (function.IsGeneric)
                CheckGenericParameters(function.GenericParameters.NotNull(), function.Diagnostics);
            CheckParameters(function.Parameters, function.Diagnostics);

            var returnType = EvaluateTypeExpression(function.ReturnTypeExpression, function.Diagnostics);
            function.ReturnType.Computed(returnType);

            var functionType = returnType;
            // TODO better way to check for having regular arguments?
            if (!(function.Syntax.OpenParen is IMissingToken))
                functionType = new FunctionType(function.Parameters.Select(p => p.Type.AssertComputed()), functionType);

            if (function.IsGeneric && function.GenericParameters.NotNull().Any())
                functionType = new MetaFunctionType(function.GenericParameters.NotNull().Select(p => p.Type.AssertComputed()), functionType);

            function.Type.Computed(functionType);
        }

        private void CheckFunctionBody([NotNull] FunctionDeclarationAnalysis function)
        {
            foreach (var statement in function.Statements)
                CheckStatement(statement, function.Diagnostics);
        }

        private void CheckGenericParameters(
            [NotNull, ItemNotNull] IReadOnlyList<GenericParameterAnalysis> genericParameters,
            [NotNull] Diagnostics diagnostics)
        {
            foreach (var parameter in genericParameters)
            {
                parameter.Type.BeginComputing();
                parameter.Type.Computed(parameter.TypeExpression == null ?
                    ObjectType.Type
                    : EvaluateTypeExpression(parameter.TypeExpression, diagnostics));
            }
        }

        private void CheckParameters(
            [NotNull, ItemNotNull] IReadOnlyList<ParameterAnalysis> parameters,
            [NotNull] Diagnostics diagnostics)
        {
            foreach (var parameter in parameters)
            {
                parameter.Type.BeginComputing();
                if (parameter.TypeExpression != null)
                    parameter.Type.Computed(EvaluateTypeExpression(parameter.TypeExpression, diagnostics));
                else
                {
                    diagnostics.Add(TypeError.NotImplemented(parameter.Context.File,
                        parameter.Syntax.Span, "Self parameters not implemented"));
                    parameter.Type.Computed(DataType.Unknown);
                }
            }
        }

        private void CheckStatement(
            [NotNull] StatementAnalysis statement,
            [NotNull] Diagnostics diagnostics)
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
            [NotNull] Diagnostics diagnostics)
        {
            variableDeclaration.Type.BeginComputing();
            if (variableDeclaration.Initializer != null)
                CheckExpression(variableDeclaration.Initializer, diagnostics);

            if (variableDeclaration.TypeExpression != null)
            {
                variableDeclaration.Type.Computed(
                    EvaluateTypeExpression(variableDeclaration.TypeExpression, diagnostics));
                // TODO check that the initializer type is compatible with the variable type
            }
            else if (variableDeclaration.Initializer != null)
            {
                // We'll assume the expression type is it
                variableDeclaration.Type.Computed(variableDeclaration.Initializer.Type);
            }
            else
            {
                diagnostics.Add(TypeError.NotImplemented(variableDeclaration.Context.File,
                    variableDeclaration.Syntax.Name.Span,
                    "Inference of local variable types not implemented"));
                variableDeclaration.Type.Computed(DataType.Unknown);
            }
        }

        /// <summary>
        /// If the type has not been checked, this checks it and returns it.
        /// Also watches for type cycles
        /// </summary>
        private void CheckTypeDeclaration(
            [NotNull] TypeDeclarationAnalysis typeDeclaration)
        {
            switch (typeDeclaration.Type.State)
            {
                case AnalysisState.BeingComputed:
                    typeDeclaration.Diagnostics.Add(TypeError.CircularDefinition(typeDeclaration.Context.File, typeDeclaration.Syntax.SignatureSpan, typeDeclaration.Name));
                    return;
                case AnalysisState.Computed:
                    return;   // We have already checked it
                case AnalysisState.NotComputed:
                    // we need to compute it
                    break;
            }

            typeDeclaration.Type.BeginComputing();
            IEnumerable<DataType> genericParameterTypes = null;
            if (typeDeclaration.IsGeneric)
            {
                var genericParameters = typeDeclaration.GenericParameters.NotNull();
                CheckGenericParameters(genericParameters, typeDeclaration.Diagnostics);
                genericParameterTypes = genericParameters.Select(p => p.Type.AssertComputed());
            }
            switch (typeDeclaration.Syntax)
            {
                case ClassDeclarationSyntax classDeclaration:
                    var classType = new ObjectType(typeDeclaration.Name, true,
                        classDeclaration.Modifiers.Any(m => m is MutableModifierSyntax),
                        genericParameterTypes);
                    typeDeclaration.Type.Computed(new Metatype(classType));
                    break;
                case StructDeclarationSyntax structDeclaration:
                    var structType = new ObjectType(typeDeclaration.Name, false,
                        structDeclaration.Modifiers.Any(m => m is MutableModifierSyntax),
                        genericParameterTypes);
                    typeDeclaration.Type.Computed(new Metatype(structType));
                    break;
                case EnumStructDeclarationSyntax enumStructDeclaration:
                    var enumStructType = new ObjectType(typeDeclaration.Name, false,
                        enumStructDeclaration.Modifiers.Any(m => m is MutableModifierSyntax),
                        genericParameterTypes);
                    typeDeclaration.Type.Computed(new Metatype(enumStructType));
                    break;
                case EnumClassDeclarationSyntax enumStructDeclaration:
                    var enumClassType = new ObjectType(typeDeclaration.Name, true,
                        enumStructDeclaration.Modifiers.Any(m => m is MutableModifierSyntax),
                        genericParameterTypes);
                    typeDeclaration.Type.Computed(new Metatype(enumClassType));
                    break;
                case TypeDeclarationSyntax declarationSyntax:
                    var type = new ObjectType(typeDeclaration.Name, true,
                        declarationSyntax.Modifiers.Any(m => m is MutableModifierSyntax),
                        genericParameterTypes);
                    typeDeclaration.Type.Computed(new Metatype(type));
                    break;
                default:
                    throw NonExhaustiveMatchException.For(typeDeclaration.Syntax);
            }
        }

        // Checks the expression is well typed, and that the type of the expression is `bool`
        private void CheckExpressionTypeIsBool(
            [NotNull] ExpressionAnalysis expression,
            [NotNull] Diagnostics diagnostics)
        {
            CheckExpression(expression, diagnostics);
            if (expression.Type.AssertComputed() != ObjectType.Bool)
                diagnostics.Add(TypeError.MustBeABoolExpression(expression.Context.File, expression.Syntax.Span));
        }

        private void CheckExpression(
            [CanBeNull] ExpressionAnalysis expression,
            [NotNull] Diagnostics diagnostics)
        {
            if (expression == null) return;

            expression.Type.BeginComputing();
            switch (expression)
            {
                case PrimitiveTypeAnalysis primitive:
                    expression.Type.Computed(new Metatype(GetPrimitiveType(primitive)));
                    break;
                case ReturnExpressionAnalysis returnExpression:
                    if (returnExpression.ReturnExpression != null)
                        CheckExpression(returnExpression.ReturnExpression, diagnostics);
                    // TODO check that expression type matches function return type
                    expression.Type.Computed(ObjectType.Never);
                    break;
                case IntegerLiteralExpressionAnalysis _:
                    expression.Type.Computed(DataType.IntegerConstant);
                    break;
                case StringLiteralExpressionAnalysis _:
                    // TODO what about interpolated expressions?
                    expression.Type.Computed(ObjectType.String);
                    break;
                case BooleanLiteralExpressionAnalysis _:
                    expression.Type.Computed(ObjectType.Bool);
                    break;
                case BinaryOperatorExpressionAnalysis binaryOperatorExpression:
                    CheckBinaryOperator(binaryOperatorExpression, diagnostics);
                    break;
                case IdentifierNameAnalysis identifierName:
                    identifierName.Type.Computed(CheckName(expression.Context, identifierName.Syntax.Name, diagnostics));
                    break;
                case UnaryOperatorExpressionAnalysis unaryOperatorExpression:
                    CheckUnaryOperator(unaryOperatorExpression, diagnostics);
                    break;
                case LifetimeTypeAnalysis lifetimeType:
                    CheckExpression(lifetimeType.TypeName, diagnostics);
                    if (lifetimeType.TypeName.Type.AssertComputed() != ObjectType.Type)
                        diagnostics.Add(TypeError.MustBeATypeExpression(expression.Context.File, lifetimeType.TypeName.Syntax.Span));
                    expression.Type.Computed(ObjectType.Type);
                    break;
                case BlockAnalysis blockExpression:
                    foreach (var statement in blockExpression.Statements)
                        CheckStatement(statement, diagnostics);

                    expression.Type.Computed(ObjectType.Void);// TODO assign the correct type to the block
                    break;
                case NewObjectExpressionAnalysis newObjectExpression:
                    foreach (var argument in newObjectExpression.Arguments)
                        CheckArgument(argument, diagnostics);

                    expression.Type.Computed(EvaluateTypeExpression(newObjectExpression.ConstructorExpression, diagnostics));

                    // TODO verify argument types against called function
                    break;
                case PlacementInitExpressionAnalysis placementInitExpression:
                    foreach (var argument in placementInitExpression.Arguments)
                        CheckArgument(argument, diagnostics);

                    // TODO verify argument types against called function

                    placementInitExpression.Type.Computed(EvaluateTypeExpression(placementInitExpression.InitializerExpression, diagnostics));
                    break;
                case ForeachExpressionAnalysis foreachExpression:
                    foreachExpression.VariableType.Computed(
                        EvaluateTypeExpression(foreachExpression.TypeExpression, diagnostics));
                    CheckExpression(foreachExpression.InExpression, diagnostics);

                    // TODO check the break types
                    CheckExpression(foreachExpression.Block, diagnostics);
                    // TODO assign correct type to the expression
                    expression.Type.Computed(DataType.Unknown);
                    break;
                case WhileExpressionAnalysis whileExpression:
                    CheckExpressionTypeIsBool(whileExpression.Condition, diagnostics);
                    CheckExpression(whileExpression.Block, diagnostics);
                    // TODO assign correct type to the expression
                    expression.Type.Computed(DataType.Unknown);
                    break;
                case LoopExpressionAnalysis loopExpression:
                    CheckExpression(loopExpression.Block, diagnostics);
                    // TODO assign correct type to the expression
                    expression.Type.Computed(DataType.Unknown);
                    break;
                case InvocationAnalysis invocation:
                    CheckExpression(invocation.Callee, diagnostics);
                    // TODO the callee needs to be something callable
                    foreach (var argument in invocation.Arguments)
                        CheckExpression(argument.Value, diagnostics);
                    // TODO assign correct return type
                    expression.Type.Computed(DataType.Unknown);
                    break;
                case GenericInvocationAnalysis genericInvocation:
                {
                    foreach (var argument in genericInvocation.Arguments)
                        CheckExpression(argument.Value, diagnostics);

                    CheckExpression(genericInvocation.Callee, diagnostics);
                    var calleeType = genericInvocation.Callee.Type.AssertComputed();
                    if (calleeType is OverloadedType overloadedType)
                    {
                        genericInvocation.Callee.Type.Resolve(calleeType = overloadedType.Types
                            .OfType<GenericType>()
                            .Single(t => t.GenericArity == genericInvocation.GenericArity));
                    }

                    // TODO check that argument types match function type
                    switch (calleeType)
                    {
                        // TODO implemet
                        //case Metatype metatype:
                        //    genericInvocation.Type.Computed(
                        //        metatype.WithGenericArguments(
                        //            genericInvocation.Arguments.Select(a => a.Value.Type.AssertComputed())));
                        //    break;
                        case MetaFunctionType metaFunctionType:
                            genericInvocation.Type.Computed(metaFunctionType.ResultType);
                            break;
                        case UnknownType _:
                            genericInvocation.Type.Computed(DataType.Unknown);
                            break;
                        default:
                            throw NonExhaustiveMatchException.For(calleeType);
                    }
                }
                break;
                case GenericNameAnalysis genericName:
                {
                    foreach (var argument in genericName.Arguments)
                        CheckExpression(argument.Value, diagnostics);

                    var nameType = CheckName(genericName.Context, genericName.Syntax.Name,
                        diagnostics);
                    if (nameType is OverloadedType overloadedType)
                    {
                        nameType = overloadedType.Types.OfType<GenericType>()
                            .Single(t => t.GenericArity == genericName.GenericArity);
                    }

                    // TODO check that argument types match function type
                    genericName.NameType = nameType;

                    switch (genericName.NameType)
                    {
                        // TODO implement
                        //case Metatype metatype:
                        //    genericName.Type.Computed(
                        //        metatype.WithGenericArguments(
                        //            genericName.Arguments.Select(a => a.Value.Type.AssertComputed())));
                        //    break;
                        case UnknownType _:
                            genericName.Type.Computed(DataType.Unknown);
                            break;
                        default:
                            throw NonExhaustiveMatchException.For(genericName.NameType);
                    }
                }
                break;
                case RefTypeAnalysis refType:
                    EvaluateTypeExpression(refType.ReferencedType, diagnostics);
                    refType.Type.Computed(ObjectType.Type);
                    break;
                case UnsafeExpressionAnalysis unsafeExpression:
                    CheckExpression(unsafeExpression.Expression, diagnostics);
                    unsafeExpression.Type.Computed(unsafeExpression.Expression.Type);
                    break;
                case MutableTypeAnalysis mutableType:
                    mutableType.Type.Computed(EvaluateTypeExpression(mutableType.ReferencedType, diagnostics));// TODO make that type mutable
                    break;
                case IfExpressionAnalysis ifExpression:
                    CheckExpressionTypeIsBool(ifExpression.Condition, diagnostics);
                    CheckExpression(ifExpression.ThenBlock, diagnostics);
                    CheckExpression(ifExpression.ElseClause, diagnostics);
                    // TODO assign a type to the expression
                    ifExpression.Type.Computed(DataType.Unknown);
                    break;
                case ResultExpressionAnalysis resultExpression:
                    CheckExpression(resultExpression.Expression, diagnostics);
                    resultExpression.Type.Computed(ObjectType.Never);
                    break;
                case UninitializedExpressionAnalysis uninitializedExpression:
                    // TODO assign a type to the expression
                    uninitializedExpression.Type.Computed(DataType.Unknown);
                    break;
                case MemberAccessExpressionAnalysis memberAccess:
                    CheckExpression(memberAccess.Expression, diagnostics);
                    // TODO look up the member
                    // TODO assign a type to the expression
                    memberAccess.Type.Computed(DataType.Unknown);
                    break;
                case BreakExpressionAnalysis breakExpression:
                    CheckExpression(breakExpression.Expression, diagnostics);
                    breakExpression.Type.Computed(ObjectType.Never);
                    break;
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
        }

        [NotNull]
        private DataType CheckName(
            [NotNull] AnalysisContext context,
            [NotNull] IIdentifierTokenPlace name,
            [NotNull] Diagnostics diagnostics)
        {
            Requires.NotNull(nameof(context), context);
            Requires.NotNull(nameof(name), name);
            Requires.NotNull(nameof(diagnostics), diagnostics);

            // Missing name, just use unknown
            // Error should already be emitted
            if (name.Value == null)
                return DataType.Unknown; // unknown

            var declaration = context.Scope.Lookup(name.Value);
            switch (declaration)
            {
                case TypeDeclarationAnalysis typeDeclaration:
                    CheckTypeDeclaration(typeDeclaration);
                    return typeDeclaration.Type.AssertComputed();
                case ParameterAnalysis parameter:
                    return parameter.Type.AssertComputed();
                case VariableDeclarationStatementAnalysis variableDeclaration:
                    return variableDeclaration.Type.AssertComputed();
                case GenericParameterAnalysis genericParameter:
                    return genericParameter.Type.AssertComputed();
                case ForeachExpressionAnalysis foreachExpression:
                    return foreachExpression.VariableType.AssertComputed();
                case FunctionDeclarationAnalysis functionDeclaration:
                    return functionDeclaration.Type.AssertComputed();
                case null:
                    diagnostics.Add(NameBindingError.CouldNotBindName(context.File, name));
                    return DataType.Unknown; // unknown
                case TypeDeclaration typeDeclaration:
                    return typeDeclaration.Type.AssertResolved();
                case CompositeSymbol composite:
                    foreach (var typeDeclaration in composite.Symbols.OfType<TypeDeclarationAnalysis>())
                    {
                        CheckTypeDeclaration(typeDeclaration);
                        typeDeclaration.Type.AssertComputed();
                    }
                    return new OverloadedType(composite.Symbols.SelectMany(s => s.Types));
                default:
                    throw NonExhaustiveMatchException.For(declaration);
            }
        }

        private void CheckArgument(
            [NotNull] ArgumentAnalysis argument,
            [NotNull] Diagnostics diagnostics)
        {
            CheckExpression(argument.Value, diagnostics);
        }

        private void CheckBinaryOperator(
            [NotNull] BinaryOperatorExpressionAnalysis binaryOperatorExpression,
            [NotNull] Diagnostics diagnostics)
        {
            CheckExpression(binaryOperatorExpression.LeftOperand, diagnostics);
            var leftOperand = binaryOperatorExpression.LeftOperand.Type.AssertComputed();
            var leftOperandCore = leftOperand is LifetimeType l ? l.Referent : leftOperand;
            var @operator = binaryOperatorExpression.Syntax.Operator;
            CheckExpression(binaryOperatorExpression.RightOperand, diagnostics);
            var rightOperand = binaryOperatorExpression.RightOperand.Type.AssertComputed();
            var rightOperandCore = rightOperand is LifetimeType r ? r.Referent : rightOperand;

            // If either is unknown, then we can't know whether there is a a problem
            // (technically not true, for example, we could know that one arg should
            // be a bool and isn't)
            if (leftOperand == DataType.Unknown
                || rightOperand == DataType.Unknown)
            {
                switch (@operator)
                {
                    case IEqualsEqualsToken _:
                    case ILessThanToken _:
                    case ILessThanOrEqualToken _:
                    case IGreaterThanToken _:
                    case IGreaterThanOrEqualToken _:
                    case IAndKeywordToken _:
                    case IOrKeywordToken _:
                    case IXorKeywordToken _:
                        binaryOperatorExpression.Type.Computed(ObjectType.Bool);
                        break;
                    default:
                        binaryOperatorExpression.Type.Computed(DataType.Unknown);
                        break;
                }
                return;
            }

            bool typeError;
            switch (@operator)
            {
                case IPlusToken _:
                    typeError = CheckNumericOperator(
                        binaryOperatorExpression.LeftOperand,
                        binaryOperatorExpression.RightOperand,
                        null);
                    binaryOperatorExpression.Type.Computed(!typeError ? leftOperand : DataType.Unknown);
                    break;
                case IPlusEqualsToken _:
                    typeError = CheckNumericOperator(
                        binaryOperatorExpression.LeftOperand,
                        binaryOperatorExpression.RightOperand,
                        binaryOperatorExpression.LeftOperand.Type.AssertComputed());
                    //typeError = (leftOperand != rightOperand || leftOperand == ObjectType.Bool)
                    //    // TODO really pointer arithmetic should allow `size` and `offset`, but we don't have constants working correct yet
                    //    && !(leftOperand is PointerType && (rightOperand == ObjectType.Size || rightOperand == ObjectType.Int));
                    binaryOperatorExpression.Type.Computed(!typeError ? leftOperand : DataType.Unknown);
                    break;
                case IAsteriskEqualsToken _:
                    typeError = leftOperand != rightOperand || leftOperand == ObjectType.Bool;
                    binaryOperatorExpression.Type.Computed(!typeError ? leftOperand : DataType.Unknown);
                    break;
                case IEqualsEqualsToken _:
                case INotEqualToken _:
                case ILessThanToken _:
                case ILessThanOrEqualToken _:
                case IGreaterThanToken _:
                case IGreaterThanOrEqualToken _:
                    typeError = leftOperandCore != rightOperandCore;
                    binaryOperatorExpression.Type.Computed(ObjectType.Bool);
                    break;
                case IEqualsToken _:
                    typeError = leftOperandCore != rightOperandCore;
                    if (!typeError)
                        binaryOperatorExpression.Type.Computed(leftOperand);
                    break;
                case IAndKeywordToken _:
                case IOrKeywordToken _:
                case IXorKeywordToken _:
                    typeError = leftOperand != ObjectType.Bool || rightOperand != ObjectType.Bool;

                    binaryOperatorExpression.Type.Computed(ObjectType.Bool);
                    break;
                case IDotDotToken _:
                case IDotToken _:
                case ICaretDotToken _:
                    // TODO type check this
                    typeError = false;
                    break;
                case IDollarToken _:
                case IDollarLessThanToken _:
                case IDollarLessThanNotEqualToken _:
                case IDollarGreaterThanToken _:
                case IDollarGreaterThanNotEqualToken _:
                    typeError = leftOperand != ObjectType.Type;
                    break;
                case IAsKeywordToken _:
                    var asType = EvaluateCheckedTypeExpression(binaryOperatorExpression.RightOperand, diagnostics);
                    // TODO check that left operand can be converted to this
                    typeError = false;
                    binaryOperatorExpression.Type.Computed(asType);
                    break;
                default:
                    throw NonExhaustiveMatchException.For(@operator);
            }
            if (typeError)
                diagnostics.Add(TypeError.OperatorCannotBeAppliedToOperandsOfType(binaryOperatorExpression.Context.File,
                    binaryOperatorExpression.Syntax.Span, @operator,
                    binaryOperatorExpression.LeftOperand.Type,
                    binaryOperatorExpression.RightOperand.Type));
        }

        private bool CheckNumericOperator(
            [NotNull] ExpressionAnalysis leftOperand,
            [NotNull] ExpressionAnalysis rightOperand,
            [CanBeNull] DataType resultType)
        {
            var leftType = leftOperand.Type.AssertComputed();
            var rightType = rightOperand.Type.AssertComputed();
            switch (leftType)
            {
                case PointerType _:
                {
                    if (rightType is IntegerConstantType)
                        // TODO it may need to be size
                        rightType = rightOperand.Type.Resolve(ObjectType.Offset);

                    return rightType != ObjectType.Size &&
                           rightType != ObjectType.Offset;
                }
                case IntegerConstantType _:
                    if (IsIntegerType(rightType))
                        // TODO may need to promote based on size
                        leftOperand.Type.Resolve(rightType);

                    return !IsIntegerType(rightType);
                case DataType type when IsIntegerType(type):
                    if (rightType is IntegerConstantType)
                        // TODO it may need to be size
                        rightType = rightOperand.Type.Resolve(leftOperand.Type);

                    return !IsIntegerType(rightType);
                case ObjectType _:
                    // Other object types can't be used in numeric expressions
                    return false;
                default:
                    throw NonExhaustiveMatchException.For(leftType);
            }
        }

        private static bool IsIntegerType([NotNull] DataType type)
        {
            Requires.NotNull(nameof(type), type);
            return type is PrimitiveFixedIntegerType
                   || type == ObjectType.Size
                   || type == ObjectType.Offset;
        }

        private void CheckUnaryOperator(
            [NotNull] UnaryOperatorExpressionAnalysis unaryOperatorExpression,
            [NotNull] Diagnostics diagnostics)
        {
            CheckExpression(unaryOperatorExpression.Operand, diagnostics);
            var operand = unaryOperatorExpression.Operand.Type.AssertComputed();
            var @operator = unaryOperatorExpression.Syntax.Operator;

            // If either is unknown, then we can't know whether there is a a problem
            // (technically not true, for example, we could know that one arg should
            // be a bool and isn't)
            if (operand == DataType.Unknown)
            {
                unaryOperatorExpression.Type.Computed(DataType.Unknown);
                return;
            }

            bool typeError;
            switch (@operator)
            {
                case INotKeywordToken _:
                    typeError = operand != ObjectType.Bool;
                    unaryOperatorExpression.Type.Computed(ObjectType.Bool);
                    break;
                case IAtSignToken _:
                    typeError = false; // TODO check that the expression can have a pointer taken
                    if (operand is Metatype)
                        unaryOperatorExpression.Type.Computed(ObjectType.Type); // constructing a type
                    else
                        unaryOperatorExpression.Type.Computed(new PointerType(operand)); // taking the address of something
                    break;
                case IQuestionToken _:
                    typeError = false; // TODO check that the expression can have a pointer taken
                    unaryOperatorExpression.Type.Computed(new PointerType(operand));
                    break;
                case ICaretToken _:
                    switch (operand)
                    {
                        case PointerType pointerType:
                            unaryOperatorExpression.Type.Computed(pointerType.Referent);
                            typeError = false;
                            break;
                        default:
                            unaryOperatorExpression.Type.Computed(DataType.Unknown);
                            typeError = true;
                            break;
                    }
                    break;
                default:
                    throw NonExhaustiveMatchException.For(@operator);
            }
            if (typeError)
                diagnostics.Add(TypeError.OperatorCannotBeAppliedToOperandOfType(unaryOperatorExpression.Context.File,
                    unaryOperatorExpression.Syntax.Span, @operator, operand));
        }

        /// <summary>
        /// Evaluates a type expression to the type it identifies
        /// </summary>
        [NotNull]
        private DataType EvaluateTypeExpression(
            [CanBeNull] ExpressionAnalysis typeExpression,
            [NotNull] Diagnostics diagnostics)
        {
            if (typeExpression == null)
            {
                // TODO report error?
                return DataType.Unknown;
            }

            CheckExpression(typeExpression, diagnostics);
            var type = typeExpression.Type.AssertComputed();
            if (!(type is Metatype)
                && type != ObjectType.Type)
            {
                diagnostics.Add(TypeError.MustBeATypeExpression(typeExpression.Context.File,
                    typeExpression.Syntax.Span));
                return DataType.Unknown;
            }

            return EvaluateCheckedTypeExpression(typeExpression, diagnostics);
        }

        [NotNull]
        private DataType EvaluateCheckedTypeExpression(
            [NotNull] ExpressionAnalysis typeExpression,
            [NotNull] Diagnostics diagnostics)
        {
            switch (typeExpression)
            {
                case IdentifierNameAnalysis identifier:
                {
                    var identifierType = identifier.Type.AssertComputed();
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
                case PrimitiveTypeAnalysis primitive:
                    return GetPrimitiveType(primitive);
                case LifetimeTypeAnalysis lifetimeType:
                {
                    var type = EvaluateCheckedTypeExpression(lifetimeType.TypeName, diagnostics);
                    if (type == DataType.Unknown) return DataType.Unknown;
                    var lifetimeToken = lifetimeType.Syntax.Lifetime;
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
                case RefTypeAnalysis refType:
                {
                    var referent = EvaluateCheckedTypeExpression(refType.ReferencedType, diagnostics);
                    if (referent is ObjectType objectType)
                        return new RefType(refType.VariableBinding, objectType);
                    return DataType.Unknown;
                }
                case UnaryOperatorExpressionAnalysis unaryOperatorExpression:
                    switch (unaryOperatorExpression.Syntax.Operator)
                    {
                        case IAtSignToken _:
                            if (unaryOperatorExpression.Operand.Type.AssertComputed() is Metatype metatype)
                                return new PointerType(metatype.Instance);
                            // TODO evaluate to type
                            return DataType.Unknown;
                        default:
                            // TODO evaluate to type
                            return DataType.Unknown;
                    }
                case GenericInvocationAnalysis _:
                case GenericNameAnalysis _:
                {
                    var type = typeExpression.Type.AssertComputed();
                    if (type is Metatype metatype)
                        return metatype.Instance;

                    // TODO evaluate to type
                    return DataType.Unknown;
                }
                case BinaryOperatorExpressionAnalysis _:
                    // TODO evaluate to type
                    return DataType.Unknown;
                case MutableTypeAnalysis mutableType:
                    return EvaluateCheckedTypeExpression(mutableType.ReferencedType, diagnostics); // TODO make the type mutable
                default:
                    throw NonExhaustiveMatchException.For(typeExpression);
            }
        }

        [NotNull]
        private static DataType GetPrimitiveType([NotNull] PrimitiveTypeAnalysis primitive)
        {
            switch (primitive.Syntax.Keyword)
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
                case IStringKeywordToken _:
                    return ObjectType.String;
                case INeverKeywordToken _:
                    return ObjectType.Never;
                case ITypeKeywordToken _:
                    return ObjectType.Type;
                case IMetatypeKeywordToken _:
                    return ObjectType.Metatype;
                case IAnyKeywordToken _:
                    return ObjectType.Any;
                default:
                    throw NonExhaustiveMatchException.For(primitive.Syntax.Keyword);
            }
        }
    }
}
