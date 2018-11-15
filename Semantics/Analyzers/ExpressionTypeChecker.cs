using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
using Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyzers
{
    public class ExpressionTypeChecker
    {
        [NotNull] private readonly CodeFile file;
        [NotNull] private readonly DataType returnType;
        [NotNull] private readonly Diagnostics diagnostics;

        public ExpressionTypeChecker(
            [NotNull] CodeFile file,
            [NotNull] DataType returnType,
            [NotNull] Diagnostics diagnostics)
        {
            this.file = file;
            this.returnType = returnType;
            this.diagnostics = diagnostics;
        }

        public void CheckStatement(
            [NotNull] StatementAnalysis statement)
        {
            switch (statement)
            {
                case VariableDeclarationStatementAnalysis variableDeclaration:
                    CheckVariableDeclaration(variableDeclaration);
                    break;
                case ExpressionStatementAnalysis expressionStatement:
                    CheckExpression(expressionStatement.Expression);
                    break;
                default:
                    throw NonExhaustiveMatchException.For(statement);
            }
        }

        private void CheckVariableDeclaration(
            [NotNull] VariableDeclarationStatementAnalysis variableDeclaration)
        {
            variableDeclaration.Type.BeginComputing();
            if (variableDeclaration.Initializer != null)
                CheckExpression(variableDeclaration.Initializer);

            if (variableDeclaration.TypeExpression != null)
            {
                var type = EvaluateTypeExpression(variableDeclaration.TypeExpression);
                variableDeclaration.Type.Computed(type);
                if (variableDeclaration.Initializer != null)
                    ImposeIntegerConstantType(type, variableDeclaration.Initializer);
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

        // Checks the expression is well typed, and that the type of the expression is `bool`
        private void CheckExpressionTypeIsBool([NotNull] ExpressionAnalysis expression)
        {
            CheckExpression(expression);
            if (expression.Type.AssertComputed() != ObjectType.Bool)
                diagnostics.Add(TypeError.MustBeABoolExpression(expression.Context.File, expression.Span));
        }

        public void CheckExpression([CanBeNull] ExpressionAnalysis expression)
        {
            if (expression == null) return;

            expression.Type.BeginComputing();
            switch (expression)
            {
                case PrimitiveTypeAnalysis primitive:
                    expression.Type.Computed(new Metatype(GetPrimitiveType(primitive)));
                    break;
                case ReturnExpressionAnalysis returnExpression:
                    if (returnExpression.ReturnValue != null)
                    {
                        CheckExpression(returnExpression.ReturnValue);
                        ImposeIntegerConstantType(returnType, returnExpression.ReturnValue);
                        if (returnType != returnExpression.ReturnValue.Type.AssertComputed())
                            diagnostics.Add(TypeError.CannotConvert(file, returnExpression.ReturnValue, returnType));
                    }
                    else
                    {
                        // TODO a void or never function shouldn't have this
                    }
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
                    CheckBinaryOperator(binaryOperatorExpression);
                    break;
                case IdentifierNameAnalysis identifierName:
                    identifierName.Type.Computed(CheckName(expression.Context, identifierName.Syntax.Name));
                    break;
                case UnaryOperatorExpressionAnalysis unaryOperatorExpression:
                    CheckUnaryOperator(unaryOperatorExpression);
                    break;
                case LifetimeTypeAnalysis lifetimeType:
                    CheckExpression(lifetimeType.TypeName);
                    if (lifetimeType.TypeName.Type.AssertComputed() != ObjectType.Type)
                        diagnostics.Add(TypeError.MustBeATypeExpression(expression.Context.File, lifetimeType.TypeName.Span));
                    expression.Type.Computed(ObjectType.Type);
                    break;
                case BlockAnalysis blockExpression:
                    foreach (var statement in blockExpression.Statements)
                        CheckStatement(statement);

                    expression.Type.Computed(ObjectType.Void);// TODO assign the correct type to the block
                    break;
                case NewObjectExpressionAnalysis newObjectExpression:
                    foreach (var argument in newObjectExpression.Arguments)
                        CheckArgument(argument);

                    expression.Type.Computed(EvaluateTypeExpression(newObjectExpression.ConstructorExpression));

                    // TODO verify argument types against called function
                    break;
                case PlacementInitExpressionAnalysis placementInitExpression:
                    foreach (var argument in placementInitExpression.Arguments)
                        CheckArgument(argument);

                    // TODO verify argument types against called function

                    placementInitExpression.Type.Computed(EvaluateTypeExpression(placementInitExpression.InitializerExpression));
                    break;
                case ForeachExpressionAnalysis foreachExpression:
                    foreachExpression.VariableType.Computed(
                        EvaluateTypeExpression(foreachExpression.TypeExpression));
                    CheckExpression(foreachExpression.InExpression);

                    // TODO check the break types
                    CheckExpression(foreachExpression.Block);
                    // TODO assign correct type to the expression
                    expression.Type.Computed(DataType.Unknown);
                    break;
                case WhileExpressionAnalysis whileExpression:
                    CheckExpressionTypeIsBool(whileExpression.Condition);
                    CheckExpression(whileExpression.Block);
                    // TODO assign correct type to the expression
                    expression.Type.Computed(DataType.Unknown);
                    break;
                case LoopExpressionAnalysis loopExpression:
                    CheckExpression(loopExpression.Block);
                    // TODO assign correct type to the expression
                    expression.Type.Computed(DataType.Unknown);
                    break;
                case InvocationAnalysis invocation:
                    CheckExpression(invocation.Callee);
                    // TODO the callee needs to be something callable
                    foreach (var argument in invocation.Arguments)
                        CheckExpression(argument.Value);
                    // TODO assign correct return type
                    expression.Type.Computed(DataType.Unknown);
                    break;
                case GenericInvocationAnalysis genericInvocation:
                {
                    foreach (var argument in genericInvocation.Arguments)
                        CheckExpression(argument.Value);

                    CheckExpression(genericInvocation.Callee);
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
                        CheckExpression(argument.Value);

                    var nameType = CheckName(genericName.Context, genericName.Syntax.Name);
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
                    EvaluateTypeExpression(refType.ReferencedType);
                    refType.Type.Computed(ObjectType.Type);
                    break;
                case UnsafeExpressionAnalysis unsafeExpression:
                    CheckExpression(unsafeExpression.Expression);
                    unsafeExpression.Type.Computed(unsafeExpression.Expression.Type);
                    break;
                case MutableTypeAnalysis mutableType:
                    mutableType.Type.Computed(EvaluateTypeExpression(mutableType.ReferencedType));// TODO make that type mutable
                    break;
                case IfExpressionAnalysis ifExpression:
                    CheckExpressionTypeIsBool(ifExpression.Condition);
                    CheckExpression(ifExpression.ThenBlock);
                    CheckExpression(ifExpression.ElseClause);
                    // TODO assign a type to the expression
                    ifExpression.Type.Computed(DataType.Unknown);
                    break;
                case ResultExpressionAnalysis resultExpression:
                    CheckExpression(resultExpression.Expression);
                    resultExpression.Type.Computed(ObjectType.Never);
                    break;
                case UninitializedExpressionAnalysis uninitializedExpression:
                    // TODO assign a type to the expression
                    uninitializedExpression.Type.Computed(DataType.Unknown);
                    break;
                case MemberAccessExpressionAnalysis memberAccess:
                    CheckExpression(memberAccess.Expression);
                    // TODO look up the member
                    // TODO assign a type to the expression
                    memberAccess.Type.Computed(DataType.Unknown);
                    break;
                case BreakExpressionAnalysis breakExpression:
                    CheckExpression(breakExpression.Expression);
                    breakExpression.Type.Computed(ObjectType.Never);
                    break;
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
        }

        [NotNull]
        private DataType CheckName(
            [NotNull] AnalysisContext context,
            [NotNull] IIdentifierTokenPlace name)
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
                    TypeChecker.CheckTypeDeclaration(typeDeclaration);
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
                        TypeChecker.CheckTypeDeclaration(typeDeclaration);
                        typeDeclaration.Type.AssertComputed();
                    }
                    return new OverloadedType(composite.Symbols.SelectMany(s => s.Types));
                default:
                    throw NonExhaustiveMatchException.For(declaration);
            }
        }

        private void CheckArgument(
            [NotNull] ArgumentAnalysis argument)
        {
            CheckExpression(argument.Value);
        }

        private void CheckBinaryOperator(
            [NotNull] BinaryOperatorExpressionAnalysis binaryOperatorExpression)
        {
            CheckExpression(binaryOperatorExpression.LeftOperand);
            var leftOperand = binaryOperatorExpression.LeftOperand.Type.AssertComputed();
            var leftOperandCore = leftOperand is LifetimeType l ? l.Referent : leftOperand;
            var @operator = binaryOperatorExpression.Syntax.Operator;
            CheckExpression(binaryOperatorExpression.RightOperand);
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
                    var asType = EvaluateCheckedTypeExpression(binaryOperatorExpression.RightOperand);
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

        private void ImposeIntegerConstantType([NotNull] DataType expectedType, [NotNull] ExpressionAnalysis expression)
        {
            // Don't impose a non-integer type
            if (!IsIntegerType(expectedType)) return;
            var currentType = expression.Type.AssertComputed();
            // If it isn't an integer constant, nothing to do
            if (currentType != DataType.IntegerConstant) return;

            switch (expression)
            {
                case LiteralExpressionAnalysis _:
                    expression.Type.Resolve(expectedType);
                    break;
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

        private void CheckUnaryOperator(
            [NotNull] UnaryOperatorExpressionAnalysis unaryOperatorExpression)
        {
            CheckExpression(unaryOperatorExpression.Operand);
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
        public DataType EvaluateTypeExpression([CanBeNull] ExpressionAnalysis typeExpression)
        {
            if (typeExpression == null)
            {
                // TODO report error?
                return DataType.Unknown;
            }

            CheckExpression(typeExpression);
            var type = typeExpression.Type.AssertComputed();
            if (!(type is Metatype)
                && type != ObjectType.Type)
            {
                diagnostics.Add(TypeError.MustBeATypeExpression(typeExpression.Context.File, typeExpression.Span));
                return DataType.Unknown;
            }

            return EvaluateCheckedTypeExpression(typeExpression);
        }

        [NotNull]
        private DataType EvaluateCheckedTypeExpression(
            [NotNull] ExpressionAnalysis typeExpression)
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
                    var type = EvaluateCheckedTypeExpression(lifetimeType.TypeName);
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
                    var referent = EvaluateCheckedTypeExpression(refType.ReferencedType);
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
                    return EvaluateCheckedTypeExpression(mutableType.ReferencedType); // TODO make the type mutable
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
