using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Lifetimes;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.ControlFlow
{
    public class ControlFlowAnalyzer
    {
        public static void BuildGraphs(IEnumerable<MemberDeclarationSyntax> declarations)
        {
            foreach (var function in declarations.OfType<FunctionDeclarationSyntax>().Where(ShouldBuildGraph))
            {
                var builder = new ControlFlowAnalyzer();
                builder.BuildGraph(function);
            }
        }

        private static bool ShouldBuildGraph(FunctionDeclarationSyntax function)
        {
            return function.Body != null // It is not abstract
                   && function.GenericParameters == null; // It is not generic, generic functions need monomorphized
        }

        private readonly ControlFlowGraphBuilder graph = new ControlFlowGraphBuilder();

        /// <summary>
        /// The block we are currently adding statements to. Thus after control flow statements this
        /// is the block the control flow exits to.
        /// </summary>
        private BlockBuilder currentBlock;

        /// <summary>
        /// The block that a break statement should go to
        /// </summary>
        private BlockBuilder breakToBlock;

        private Scope nextScope;
        private readonly Stack<Scope> scopes = new Stack<Scope>();
        private Scope CurrentScope => scopes.Peek();
        private DataType returnType;

        private ControlFlowAnalyzer()
        {
            // We start in the outer scope and need that on the stack
            var scope = Scope.Outer;
            scopes.Push(scope);
            nextScope = scope.Next();
        }

        private void BuildGraph(FunctionDeclarationSyntax function)
        {
            returnType = function.ReturnType.Known();

            // Temp Variable for return
            if (function is ConstructorDeclarationSyntax constructor)
                graph.AddSelfParameter(constructor.SelfParameterType);
            else
                graph.AddReturnVariable(function.ReturnType.Known());

            // TODO don't emit temp variables for unused parameters
            foreach (var parameter in function.Parameters.Where(p => !p.Unused))
                graph.AddParameter(parameter.MutableBinding, parameter.Type.Known(), CurrentScope, parameter.Name.UnqualifiedName);

            currentBlock = graph.NewBlock();
            breakToBlock = null;
            foreach (var statement in function.Body.Statements)
                ConvertToStatement(statement);

            // Generate the implicit return statement
            if (currentBlock != null && !currentBlock.IsTerminated)
            {
                var span = function.Body.Span.AtEnd();
                EndScope(span);
                currentBlock.AddReturn(span, Scope.Outer); // We officially ended the outer scope, but this is in it
            }

            function.ControlFlow = graph.Build();
        }

        private void EnterNewScope()
        {
            scopes.Push(nextScope);
            nextScope = nextScope.Next();
        }

        /// <summary>
        /// An exist point for the current scope that doesn't end it. For example, a break statement
        /// </summary>
        private void ExitScope(TextSpan span)
        {
            currentBlock?.AddExitScope(span, CurrentScope);
        }

        private void EndScope(TextSpan span)
        {
            // In some cases we will have left the current block by a terminator,
            // so we don't need to emit an exit statement.
            currentBlock?.AddExitScope(span, CurrentScope);
            scopes.Pop();
        }

        /// <summary>
        /// Assign a value into a place while making sure to correctly handle
        /// value semantics.
        /// </summary>
        private void AssignToPlace(Place place, Value value, TextSpan span)
        {
            if (value is VariableReference assignFrom
                && assignFrom.ValueSemantics == ValueSemantics.Move
                && place is VariableReference assignTo)
            {
                // There is a chance we are assigning into something that doesn't
                // accept ownership. In that case, we demote to a borrow or alias
                var variableSemantics = graph[assignTo.Variable].Type.ValueSemantics;
                switch (variableSemantics)
                {
                    case ValueSemantics.Move:
                        // They are both move, no problems
                        break;
                    case ValueSemantics.Alias:
                        value = assignFrom.AsAlias();
                        break;
                    case ValueSemantics.Borrow:
                        value = assignFrom.AsBorrow();
                        break;
                    default:
                        throw NonExhaustiveMatchException.ForEnum(variableSemantics);
                }
            }

            currentBlock.AddAssignment(place, value, span, CurrentScope);
        }

        private VariableReference AssignToTemp(DataType type, Value value)
        {
            var tempVariable = graph.Let(type.AssertKnown(), CurrentScope);
            currentBlock.AddAssignment(
                tempVariable.LValueReference(value.Span),
                value, value.Span,
                CurrentScope);
            return tempVariable.Reference(value.Span);
        }

        private void ConvertToStatement(StatementSyntax statement)
        {
            switch (statement)
            {
                case VariableDeclarationStatementSyntax variableDeclaration:
                {
                    var variable = graph.AddVariable(variableDeclaration.MutableBinding,
                                        variableDeclaration.Type, CurrentScope,
                                        variableDeclaration.Name.UnqualifiedName);
                    if (variableDeclaration.Initializer != null)
                    {
                        var value = ConvertToValue(variableDeclaration.Initializer);
                        AssignToPlace(
                            variable.LValueReference(variableDeclaration.Initializer.Span),
                            value,
                            variableDeclaration.Initializer.Span);
                    }
                    return;
                }
                case ExpressionSyntax expression:
                {
                    // Skip expressions with unknown type
                    if (!expression.Type.IsKnown)
                        return;

                    if (expression.Type is VoidType || expression.Type is NeverType)
                        ConvertExpressionToStatement(expression);
                    else
                    {
                        //var tempVariable = graph.Let(expression.Type.AssertKnown(), CurrentScope);
                        var value = ConvertToValue(expression);
                        //currentBlock.AddAssignment(
                        //    tempVariable.LValueReference(expression.Span),
                        //    value, expression.Span,
                        //    CurrentScope);
                        AssignToTemp(expression.Type, value);
                    }

                    return;
                }
                default:
                    throw NonExhaustiveMatchException.For(statement);
            }
        }

        /// <summary>
        /// Converts an expression of type `void` or `never` to a statement
        /// </summary>
        private void ConvertExpressionToStatement(ExpressionSyntax expression)
        {
            switch (expression)
            {
                case UnaryExpressionSyntax _:
                case BinaryExpressionSyntax _:
                    throw new NotImplementedException();
                case InvocationSyntax invocation:
                    currentBlock.AddAction(ConvertInvocationToValue(invocation), invocation.Span, CurrentScope);
                    return;
                case ReturnExpressionSyntax returnExpression:
                    if (returnExpression.ReturnValue != null)
                    {
                        var isMove = returnType.ValueSemantics == ValueSemantics.Move;
                        var value = isMove
                            ? ConvertToMove(returnExpression.ReturnValue, returnExpression.Span)
                            // TODO avoid getting a move from this just because it is a new object expression
                            : ConvertToValue(returnExpression.ReturnValue);
                        AssignToPlace(
                            graph.ReturnVariable.LValueReference(returnExpression.ReturnValue.Span),
                            value,
                            returnExpression.ReturnValue.Span);
                    }

                    ExitScope(returnExpression.Span.AtEnd());
                    currentBlock.AddReturn(returnExpression.Span, CurrentScope);

                    // There is no exit from a return block, hence null for exit block
                    currentBlock = null;
                    return;
                case ForeachExpressionSyntax _:
                case WhileExpressionSyntax _:
                    throw new NotImplementedException();
                case LoopExpressionSyntax loopExpression:
                {
                    var loopEntry = graph.NewEntryBlock(currentBlock,
                                            loopExpression.Block.Span.AtStart(),
                                            CurrentScope);
                    currentBlock = loopEntry;
                    breakToBlock = graph.NewBlock();
                    ConvertExpressionToStatement(loopExpression.Block);
                    currentBlock?.AddGoto(loopEntry,
                        loopExpression.Block.Span.AtEnd(),
                        CurrentScope);
                    currentBlock = breakToBlock;
                    return;
                }
                case BreakExpressionSyntax breakExpression:
                    ExitScope(breakExpression.Span.AtEnd());
                    currentBlock.AddGoto(
                        breakToBlock ?? throw new InvalidOperationException(),
                        breakExpression.Span,
                        CurrentScope);
                    currentBlock = null;
                    return;
                case IfExpressionSyntax ifExpression:
                    var containingBlock = currentBlock;
                    var condition = ConvertToOperand(ifExpression.Condition);
                    var thenEntry = graph.NewBlock();
                    currentBlock = thenEntry;
                    ConvertExpressionToStatement(ifExpression.ThenBlock);
                    var thenExit = currentBlock;
                    BlockBuilder elseEntry;
                    BlockBuilder exit = null;
                    if (ifExpression.ElseClause == null)
                    {
                        elseEntry = exit = graph.NewBlock();
                        thenExit?.AddGoto(exit, ifExpression.ThenBlock.Span.AtEnd(), CurrentScope);
                    }
                    else
                    {
                        elseEntry = graph.NewBlock();
                        currentBlock = elseEntry;
                        ConvertExpressionToStatement(ifExpression.ElseClause);
                        var elseExit = currentBlock;
                        if (thenExit != null || elseExit != null)
                        {
                            exit = graph.NewBlock();
                            thenExit?.AddGoto(exit, ifExpression.ThenBlock.Span.AtEnd(), CurrentScope);
                            elseExit?.AddGoto(exit, ifExpression.ElseClause.Span.AtEnd(), CurrentScope);
                        }
                    }
                    containingBlock.AddIf(condition, thenEntry, elseEntry, ifExpression.Condition.Span, CurrentScope);
                    currentBlock = exit;
                    return;
                case BlockSyntax block:
                {
                    // Starting a new nested scope
                    EnterNewScope();

                    foreach (var statementInBlock in block.Statements)
                        ConvertToStatement(statementInBlock);

                    // Ending that scope
                    EndScope(block.Span.AtEnd());
                    return;
                }
                case UnsafeExpressionSyntax unsafeExpression:
                    ConvertToStatement(unsafeExpression.Expression);
                    return;
                case AssignmentExpressionSyntax assignmentExpression:
                {
                    var place = ConvertToPlace(assignmentExpression.LeftOperand);
                    var value = ConvertToValue(assignmentExpression.RightOperand);
                    if (assignmentExpression.Operator != AssignmentOperator.Direct)
                    {
                        var type = (SimpleType)assignmentExpression.RightOperand.Type;
                        var rightOperand = ConvertToOperand(value, type);
                        BinaryOperator binaryOperator;
                        switch (assignmentExpression.Operator)
                        {
                            case AssignmentOperator.Plus:
                                binaryOperator = BinaryOperator.Plus;
                                break;
                            case AssignmentOperator.Minus:
                                binaryOperator = BinaryOperator.Minus;
                                break;
                            case AssignmentOperator.Asterisk:
                                binaryOperator = BinaryOperator.Asterisk;
                                break;
                            case AssignmentOperator.Slash:
                                binaryOperator = BinaryOperator.Slash;
                                break;
                            default:
                                throw NonExhaustiveMatchException.ForEnum(assignmentExpression.Operator);
                        }
                        value = new BinaryOperation(place, binaryOperator, rightOperand, type);
                    }
                    AssignToPlace(place, value, assignmentExpression.Span);
                    return;
                }
                case ResultExpressionSyntax resultExpression:
                    // Must be an expression of type `never`
                    ConvertExpressionToStatement(resultExpression.Expression);
                    ExitScope(resultExpression.Span.AtEnd());
                    return;
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
        }

        private Value ConvertToValue(ExpressionSyntax expression)
        {
            switch (expression)
            {
                case NewObjectExpressionSyntax newObjectExpression:
                {
                    var args = newObjectExpression.Arguments
                                    .Select(a => ConvertToOperand(a.Value))
                                    .ToFixedList();
                    var type = (UserObjectType)newObjectExpression.Type;
                    // lifetime is implicitly owned since we are making a new one
                    type = (UserObjectType)type.WithLifetime(Lifetime.None);
                    return new ConstructorCall(type, args, newObjectExpression.Span);
                }
                case IdentifierNameSyntax identifier:
                {
                    var symbol = identifier.ReferencedSymbol;
                    switch (symbol)
                    {
                        case VariableDeclarationStatementSyntax _:
                        case ParameterSyntax _:
                            return graph.VariableFor(symbol.FullName.UnqualifiedName).Reference(identifier.Span);
                        default:
                            return new DeclaredValue(symbol.FullName, identifier.Span);
                    }
                }
                case UnaryExpressionSyntax unaryExpression:
                    return ConvertUnaryExpressionToValue(unaryExpression);
                case BinaryExpressionSyntax binaryExpression:
                    return ConvertBinaryExpressionToValue(binaryExpression);
                case IntegerLiteralExpressionSyntax _:
                    throw new InvalidOperationException("Integer literals should have an implicit conversion around them");
                case StringLiteralExpressionSyntax _:
                    throw new InvalidOperationException("String literals should have an implicit conversion around them");
                case BoolLiteralExpressionSyntax boolLiteral:
                    return new BooleanConstant(boolLiteral.Value, boolLiteral.Span);
                case ImplicitNumericConversionExpression implicitNumericConversion:
                    if (implicitNumericConversion.Expression.Type.AssertKnown() is IntegerConstantType constantType)
                        return new IntegerConstant(constantType.Value, implicitNumericConversion.Type.AssertKnown(), implicitNumericConversion.Span);
                    else
                        throw new NotImplementedException();
                case IfExpressionSyntax ifExpression:
                    // TODO deal with the value of the if expression
                    throw new NotImplementedException();
                case UnsafeExpressionSyntax unsafeExpression:
                    return ConvertToValue(unsafeExpression.Expression);
                case ImplicitLiteralConversionExpression implicitLiteralConversion:
                {
                    var conversionFunction = implicitLiteralConversion.ConversionFunction;
                    var literal = (StringLiteralExpressionSyntax)implicitLiteralConversion.Expression;
                    var constantLength = Utf8BytesConstant.Encoding.GetByteCount(literal.Value);
                    var sizeArgument = new IntegerConstant(constantLength, DataType.Size, literal.Span);
                    var bytesArgument = new Utf8BytesConstant(literal.Value, literal.Span);
                    return new FunctionCall(implicitLiteralConversion.Span,
                        conversionFunction.FullName,
                        (FunctionType)conversionFunction.Type,
                        sizeArgument,
                        bytesArgument);
                }
                case InvocationSyntax invocation:
                    return ConvertInvocationToValue(invocation);
                case MemberAccessExpressionSyntax memberAccess:
                {
                    var value = ConvertToOperand(memberAccess.Expression);
                    var symbol = memberAccess.ReferencedSymbol;
                    if (symbol is IAccessorSymbol accessor)
                        return new VirtualFunctionCall(memberAccess.Span, accessor.PropertyName.UnqualifiedName, value);

                    return new FieldAccessValue(value,
                        memberAccess.ReferencedSymbol.FullName, memberAccess.Span);
                }
                case MutableExpressionSyntax mutable:
                    // TODO shouldn't borrowing be explicit in the IR and don't we
                    // need to be able to check mutability on borrows?
                    return ConvertToValue(mutable.Expression);
                case MoveExpressionSyntax move:
                    return ConvertToMove(move.Expression, move.Span);
                case ImplicitImmutabilityConversionExpression implicitImmutabilityConversion:
                {
                    var operand = ConvertToOperand(implicitImmutabilityConversion.Expression);
                    switch (operand)
                    {
                        //case BooleanConstant _:
                        //case Utf8BytesConstant _:
                        //case IntegerConstant _:
                        //    return operand;
                        case Dereference _:
                            throw new NotImplementedException();
                        case VariableReference varReference:
                            if (implicitImmutabilityConversion.Type.ValueSemantics == ValueSemantics.Move)
                                return varReference.AsMove(implicitImmutabilityConversion.Span);
                            else
                                return varReference.AsAlias();
                        default:
                            throw NonExhaustiveMatchException.For(expression);
                    }
                }
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
        }

        private Value ConvertToMove(ExpressionSyntax expression, TextSpan moveSpan)
        {
            var operand = ConvertToOperand(expression);
            switch (operand)
            {
                case VariableReference variableReference:
                    return variableReference.AsMove(moveSpan);
                default:
                    throw new NotImplementedException();
            }
        }

        private Operand ConvertToOperand(ExpressionSyntax expression)
        {
            var value = ConvertToValue(expression);
            return ConvertToOperand(value, expression.Type);
        }

        private Operand ConvertToOperand(Value value, DataType type)
        {
            if (value is Operand operand) return operand;
            return AssignToTemp(type, value);
            //var tempVariable = graph.Let(type.AssertKnown(), CurrentScope);
            //currentBlock.AddAssignment(tempVariable.LValueReference(value.Span), value, value.Span, CurrentScope);
            //return tempVariable.Reference(value.Span);
        }

        private Value ConvertBinaryExpressionToValue(BinaryExpressionSyntax expression)
        {
            switch (expression.Operator)
            {
                case BinaryOperator.Plus:
                case BinaryOperator.Minus:
                case BinaryOperator.Asterisk:
                case BinaryOperator.Slash:
                case BinaryOperator.EqualsEquals:
                case BinaryOperator.NotEqual:
                case BinaryOperator.LessThan:
                case BinaryOperator.LessThanOrEqual:
                case BinaryOperator.GreaterThan:
                case BinaryOperator.GreaterThanOrEqual:
                {
                    // TODO handle calls to overloaded operators
                    var leftOperand = ConvertToOperand(expression.LeftOperand);
                    var rightOperand = ConvertToOperand(expression.RightOperand);
                    // What matters is the type we are operating on, for comparisons, that is different than the result type which is bool
                    var operandType = (SimpleType)expression.LeftOperand.Type;
                    return new BinaryOperation(leftOperand, expression.Operator, rightOperand, operandType);
                }
                case BinaryOperator.And:
                case BinaryOperator.Or:
                {
                    // TODO handle calls to overloaded operators
                    // TODO handle short circuiting if needed
                    var leftOperand = ConvertToOperand(expression.LeftOperand);
                    var rightOperand = ConvertToOperand(expression.RightOperand);
                    return new BinaryOperation(leftOperand, expression.Operator, rightOperand, (SimpleType)expression.Type);
                }
                default:
                    throw NonExhaustiveMatchException.ForEnum(expression.Operator);
            }
        }

        private Value ConvertUnaryExpressionToValue(UnaryExpressionSyntax expression)
        {
            switch (expression.Operator)
            {
                case UnaryOperator.Not:
                    var operand = ConvertToOperand(expression.Operand);
                    return new UnaryOperation(expression.Operator, operand, expression.Span);
                case UnaryOperator.Plus:
                    // This is a no-op
                    return ConvertToValue(expression.Operand);
                default:
                    throw NonExhaustiveMatchException.ForEnum(expression.Operator);
            }
        }

        private Value ConvertInvocationToValue(InvocationSyntax invocation)
        {
            switch (invocation.Callee)
            {
                case IdentifierNameSyntax identifier:
                {
                    var symbol = identifier.ReferencedSymbol;
                    var arguments = invocation.Arguments
                        .Select(a => ConvertToOperand(a.Value)).ToList();
                    return new FunctionCall(symbol.FullName, (FunctionType)symbol.Type, arguments, invocation.Span);
                }
                case MemberAccessExpressionSyntax memberAccess:
                {
                    var self = ConvertToOperand(memberAccess.Expression);
                    var arguments = invocation.Arguments
                        .Select(a => ConvertToOperand(a.Value)).ToList();
                    var symbol = memberAccess.ReferencedSymbol;
                    switch (symbol)
                    {
                        case IFunctionSymbol function:
                            switch (memberAccess.Expression.Type)
                            {
                                case SimpleType _:
                                    // case StructType _:
                                    // Full name because this isn't a member
                                    return new FunctionCall(function.FullName, (FunctionType)function.Type, self, arguments, invocation.Span);
                                default:
                                    return new VirtualFunctionCall(invocation.Span, function.FullName.UnqualifiedName, self, arguments);
                            }
                        default:
                            throw NonExhaustiveMatchException.For(symbol);
                    }
                }
                default:
                    throw NonExhaustiveMatchException.For(invocation.Callee);
            }
        }

        private Place ConvertToPlace(ExpressionSyntax value)
        {
            switch (value)
            {
                case IdentifierNameSyntax identifier:
                    // TODO what if this isn't just a variable?
                    return graph.VariableFor(identifier.ReferencedSymbol.FullName.UnqualifiedName).LValueReference(value.Span);
                default:
                    throw NonExhaustiveMatchException.For(value);
            }
        }
    }
}
