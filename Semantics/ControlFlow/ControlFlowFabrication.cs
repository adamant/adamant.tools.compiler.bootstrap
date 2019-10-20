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
using Adamant.Tools.Compiler.Bootstrap.Names;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.ControlFlow
{
    /// <summary>
    /// The fabrication of a single control flow graph from a single callable AST node
    /// </summary>
    public class ControlFlowFabrication
    {
        private readonly ControlFlowGraphBuilder graph;

        /// <summary>
        /// The block we are currently adding statements to. Thus after control flow statements this
        /// is the block the control flow exits to.
        /// </summary>
        private BlockBuilder? currentBlock;

        /// <summary>
        /// The block that a `break` statement should go to
        /// </summary>
        private BlockBuilder? breakToBlock;

        /// <summary>
        /// The block that a `next` statement should go to
        /// </summary>
        private BlockBuilder? continueToBlock;

        private Scope nextScope;
        private readonly Stack<Scope> scopes = new Stack<Scope>();
        private Scope CurrentScope => scopes.Peek();
        private DataType? returnType;

        public ControlFlowFabrication(CodeFile file)
        {
            graph = new ControlFlowGraphBuilder(file);
            // We start in the outer scope and need that on the stack
            var scope = Scope.Outer;
            scopes.Push(scope);
            nextScope = scope.Next();
        }

        public ControlFlowGraph CreateGraph(IConcreteMethodDeclarationSyntax method)
        {
            returnType = method.ReturnType.Known();

            // Temp Variable for return
            graph.AddReturnVariable(method.ReturnType.Known());

            // TODO don't emit temp variables for unused parameters
            foreach (var parameter in method.Parameters.Where(p => !p.Unused))
                graph.AddParameter(parameter.IsMutableBinding, parameter.Type.Fulfilled(),
                    CurrentScope, parameter.Name.UnqualifiedName);

            currentBlock = graph.NewBlock();
            breakToBlock = null;
            foreach (var statement in method.Body.Statements)
                ConvertToStatement(statement);

            // Generate the implicit return statement
            if (currentBlock != null && !currentBlock.IsTerminated)
            {
                var span = method.Span.AtEnd();
                EndScope(span);
                currentBlock.AddReturn(span,
                    Scope.Outer); // We officially ended the outer scope, but this is in it
            }

            return graph.Build();
        }

        public ControlFlowGraph CreateGraph(IConstructorDeclarationSyntax constructor)
        {
            returnType = constructor.DeclaringClass.DeclaresType.Fulfilled();

            // Temp Variable for return
            graph.AddSelfParameter(constructor.SelfParameterType);

            // TODO don't emit temp variables for unused parameters
            foreach (var parameter in constructor.Parameters.Where(p => !p.Unused))
                graph.AddParameter(parameter.IsMutableBinding, parameter.Type.Fulfilled(),
                    CurrentScope, parameter.Name.UnqualifiedName);

            currentBlock = graph.NewBlock();
            breakToBlock = null;
            foreach (var statement in constructor.Body.Statements)
                ConvertToStatement(statement);

            // Generate the implicit return statement
            if (currentBlock != null && !currentBlock.IsTerminated)
            {
                var span = constructor.Span.AtEnd();
                EndScope(span);
                currentBlock.AddReturn(span,
                    Scope.Outer); // We officially ended the outer scope, but this is in it
            }

            return graph.Build();
        }

        public ControlFlowGraph CreateGraph(IFunctionDeclarationSyntax method)
        {
            returnType = method.ReturnType.Known();

            // Temp Variable for return
            graph.AddReturnVariable(method.ReturnType.Known());

            // TODO don't emit temp variables for unused parameters
            foreach (var parameter in method.Parameters.Where(p => !p.Unused))
                graph.AddParameter(parameter.IsMutableBinding, parameter.Type.Fulfilled(),
                    CurrentScope, parameter.Name.UnqualifiedName);

            currentBlock = graph.NewBlock();
            breakToBlock = null;
            foreach (var statement in method.Body.Statements)
                ConvertToStatement(statement);

            // Generate the implicit return statement
            if (currentBlock != null && !currentBlock.IsTerminated)
            {
                var span = method.Span.AtEnd();
                EndScope(span);
                currentBlock.AddReturn(span,
                    Scope.Outer); // We officially ended the outer scope, but this is in it
            }

            return graph.Build();
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
        private void AssignToPlace(IPlace place, Value value, TextSpan span)
        {
            if (value is VariableReference assignFrom
                && assignFrom.ValueSemantics == ValueSemantics.Own
                && place is VariableReference assignTo)
            {
                // There is a chance we are assigning into something that doesn't
                // accept ownership. In that case, we demote to a borrow or alias
                var variableSemantics = graph[assignTo.Variable].Type.ValueSemantics;
                switch (variableSemantics)
                {
                    case ValueSemantics.Own:
                        // They are both own, no problems
                        break;
                    case ValueSemantics.Alias:
                        value = assignFrom.AsAlias();
                        break;
                    case ValueSemantics.Borrow:
                        value = assignFrom.AsBorrow();
                        break;
                    case ValueSemantics.LValue:
                    case ValueSemantics.Move:
                    case ValueSemantics.Empty:
                    case ValueSemantics.Copy:
                        throw new NotImplementedException();
                    default:
                        throw ExhaustiveMatch.Failed(variableSemantics);
                }
            }

            currentBlock.AddAssignment(place, value, span, CurrentScope);
        }

        private VariableReference AssignToTemp(DataType type, IValue value)
        {
            var tempVariable = graph.Let(type.AssertKnown(), CurrentScope);
            currentBlock.AddAssignment(tempVariable.LValueReference(value.Span), value, value.Span,
                CurrentScope);
            return tempVariable.Reference(value.Span);
        }

        private void ConvertToStatement(IStatementSyntax statement)
        {
            switch (statement)
            {
                case IVariableDeclarationStatementSyntax variableDeclaration:
                {
                    var variable = graph.AddVariable(variableDeclaration.IsMutableBinding,
                        variableDeclaration.Type, CurrentScope,
                        variableDeclaration.Name.UnqualifiedName);
                    if (variableDeclaration.Initializer != null)
                    {
                        var value = ConvertToValue(variableDeclaration.Initializer.Expression);
                        AssignToPlace(
                            variable.LValueReference(variableDeclaration.Initializer.Span), value,
                            variableDeclaration.Initializer.Span);
                    }

                    return;
                }
                case IExpressionStatementSyntax expressionStatement:
                {
                    // Skip expressions with unknown type
                    var expression = expressionStatement.Expression;
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

        private void ConvertToStatement(IBlockOrResultSyntax blockOrResult)
        {
            switch (blockOrResult)
            {
                default:
                    throw ExhaustiveMatch.Failed(blockOrResult);
                case IBlockExpressionSyntax block:
                    ConvertExpressionToStatement(block);
                    break;
                case IResultStatementSyntax resultStatement:
                    ConvertExpressionToStatement(resultStatement.Expression);
                    break;
            }
        }

        private void ConvertToStatement(IElseClauseSyntax elseClause)
        {
            switch (elseClause)
            {
                default:
                    throw ExhaustiveMatch.Failed(elseClause);
                case IIfExpressionSyntax ifExpression:
                    ConvertExpressionToStatement(ifExpression);
                    break;
                case IBlockOrResultSyntax blockOrResult:
                    ConvertToStatement(blockOrResult);
                    break;
            }
        }

        /// <summary>
        /// Converts an expression of type `void` or `never` to a statement
        /// </summary>
        private void ConvertExpressionToStatement(IExpressionSyntax expression)
        {
            switch (expression)
            {
                case IUnaryOperatorExpressionSyntax _:
                case IBinaryOperatorExpressionSyntax _:
                    throw new NotImplementedException();
                case IInvocationExpressionSyntax invocation:
                    currentBlock.AddAction(ConvertInvocationToValue(invocation), invocation.Span,
                        CurrentScope);
                    return;
                case IReturnExpressionSyntax returnExpression:
                {
                    if (returnExpression.ReturnValue != null)
                    {
                        var isOwn = returnType.ValueSemantics == ValueSemantics.Own;
                        var value = isOwn
                            ? ConvertToOwn(returnExpression.ReturnValue.Expression, returnExpression.Span)
                            // TODO avoid getting a move from this just because it is a new object expression
                            : ConvertToValue(returnExpression.ReturnValue.Expression);
                        AssignToPlace(
                            graph.ReturnVariable.LValueReference(returnExpression.ReturnValue.Span),
                            value, returnExpression.ReturnValue.Span);
                    }

                    ExitScope(returnExpression.Span.AtEnd());
                    currentBlock.AddReturn(returnExpression.Span, CurrentScope);

                    // There is no exit from a return block, hence null for exit block
                    currentBlock = null;
                    return;
                }
                case IForeachExpressionSyntax foreachExpression:
                {
                    // For now, we support only range syntax `foreach x: T in z..y` ranges
                    // aren't yet supported by the rest of the language, so they must be directly
                    // translated here. The for each loop is basically desugared into:
                    // var x: T = z;
                    // let temp = y;
                    // loop
                    // {
                    //     <loop body>
                    //     x += 1;
                    //     if x > temp => break;
                    // }
                    if (!(foreachExpression.InExpression is IBinaryOperatorExpressionSyntax inExpression)
                        || inExpression.Operator != BinaryOperator.DotDot)
                        throw new NotImplementedException(
                            "`foreach` in non-range expression not implemented");
                    var startExpression = inExpression.LeftOperand;
                    var endExpression = inExpression.RightOperand;

                    var variableType = (SimpleType)foreachExpression.VariableType;
                    var loopVariable = graph.AddVariable(foreachExpression.IsMutableBinding,
                        variableType, CurrentScope, foreachExpression.VariableName);
                    var loopVariableLValue = loopVariable.LValueReference(foreachExpression.Span);
                    var loopVariableReference = loopVariable.Reference(foreachExpression.Span);

                    var startValue = ConvertToValue(startExpression);
                    AssignToPlace(loopVariableLValue, startValue, startExpression.Span);
                    var endValue = ConvertToOperand(endExpression);
                    var loopEntry = graph.NewEntryBlock(currentBlock,
                        foreachExpression.Block.Span.AtStart(), CurrentScope);
                    currentBlock = loopEntry;
                    var conditionBlock = continueToBlock = graph.NewBlock();
                    var loopExit = breakToBlock = graph.NewBlock();
                    ConvertExpressionToStatement(foreachExpression.Block);
                    // If it always breaks, there isn't a current block
                    currentBlock?.AddGoto(conditionBlock, foreachExpression.Block.Span.AtEnd(),
                        CurrentScope);
                    currentBlock = conditionBlock;
                    var one = new IntegerConstant(1, variableType, foreachExpression.Span);
                    var addOneValue = new BinaryOperation(loopVariableReference,
                        BinaryOperator.Plus, one, variableType);
                    AssignToPlace(loopVariableLValue, addOneValue, startExpression.Span);
                    var breakCondition = new BinaryOperation(loopVariableReference,
                        BinaryOperator.GreaterThan, endValue, variableType);
                    currentBlock.AddIf(ConvertToOperand(breakCondition, DataType.Bool),
                        breakToBlock, loopEntry, foreachExpression.Span, CurrentScope);
                    currentBlock = loopExit;
                    return;
                }
                case IWhileExpressionSyntax whileExpression:
                {
                    // There is a block for the condition, it then goes either to
                    // the body or the after block.
                    var conditionBlock = graph.NewEntryBlock(currentBlock,
                        whileExpression.Condition.Span.AtStart(), CurrentScope);
                    currentBlock = conditionBlock;
                    var condition = ConvertToOperand(whileExpression.Condition);
                    var loopEntry = graph.NewBlock();
                    continueToBlock = conditionBlock;
                    breakToBlock = graph.NewBlock();
                    conditionBlock.AddIf(condition, loopEntry, breakToBlock,
                        whileExpression.Condition.Span, CurrentScope);
                    currentBlock = loopEntry;
                    ConvertExpressionToStatement(whileExpression.Block);
                    // If it always breaks, there isn't a current block
                    currentBlock?.AddGoto(conditionBlock, whileExpression.Block.Span.AtEnd(),
                        CurrentScope);
                    currentBlock = breakToBlock;
                    return;
                }
                case ILoopExpressionSyntax loopExpression:
                {
                    var loopEntry = graph.NewEntryBlock(currentBlock,
                        loopExpression.Block.Span.AtStart(), CurrentScope);
                    currentBlock = loopEntry;
                    continueToBlock = loopEntry;
                    breakToBlock = graph.NewBlock();
                    ConvertExpressionToStatement(loopExpression.Block);
                    // If it always breaks, there isn't a current block
                    currentBlock?.AddGoto(loopEntry, loopExpression.Block.Span.AtEnd(),
                        CurrentScope);
                    currentBlock = breakToBlock;
                    return;
                }
                case IBreakExpressionSyntax breakExpression:
                {
                    ExitScope(breakExpression.Span.AtEnd());
                    currentBlock.AddGoto(breakToBlock ?? throw new InvalidOperationException(),
                        breakExpression.Span, CurrentScope);
                    currentBlock = null;
                    return;
                }
                case INextExpressionSyntax nextExpression:
                {
                    ExitScope(nextExpression.Span.AtEnd());
                    currentBlock.AddGoto(continueToBlock ?? throw new InvalidOperationException(),
                        nextExpression.Span, CurrentScope);
                    currentBlock = null;
                    return;
                }
                case IIfExpressionSyntax ifExpression:
                {
                    var containingBlock = currentBlock;
                    var condition = ConvertToOperand(ifExpression.Condition);
                    var thenEntry = graph.NewBlock();
                    currentBlock = thenEntry;
                    ConvertToStatement(ifExpression.ThenBlock);
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
                        ConvertToStatement(ifExpression.ElseClause);
                        var elseExit = currentBlock;
                        if (thenExit != null || elseExit != null)
                        {
                            exit = graph.NewBlock();
                            thenExit?.AddGoto(exit, ifExpression.ThenBlock.Span.AtEnd(),
                                CurrentScope);
                            elseExit?.AddGoto(exit, ifExpression.ElseClause.Span.AtEnd(),
                                CurrentScope);
                        }
                    }

                    containingBlock.AddIf(condition, thenEntry, elseEntry,
                        ifExpression.Condition.Span, CurrentScope);
                    currentBlock = exit;
                    return;
                }
                case IBlockExpressionSyntax block:
                {
                    // Starting a new nested scope
                    EnterNewScope();

                    foreach (var statementInBlock in block.Statements)
                        ConvertToStatement(statementInBlock);

                    // Ending that scope
                    EndScope(block.Span.AtEnd());
                    return;
                }
                case IUnsafeExpressionSyntax unsafeExpression:
                    ConvertExpressionToStatement(unsafeExpression.Expression);
                    return;
                case IAssignmentExpressionSyntax assignmentExpression:
                {
                    var value = ConvertToValue(assignmentExpression.RightOperand.Expression);
                    var place = ConvertToPlace(assignmentExpression.LeftOperand);

                    if (assignmentExpression.Operator != AssignmentOperator.Simple)
                    {
                        var type = (SimpleType)assignmentExpression.RightOperand.Type;
                        var rightOperand = ConvertToOperand(value, type);
                        BinaryOperator binaryOperator;
                        switch (assignmentExpression.Operator)
                        {
                            case AssignmentOperator.Simple:
                                throw new UnreachableCodeException("Case excluded by if statement");
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
                                throw ExhaustiveMatch.Failed(assignmentExpression.Operator);
                        }

                        value = new BinaryOperation(
                            ConvertToOperand(place, assignmentExpression.LeftOperand.Type),
                            binaryOperator, rightOperand, type);
                    }

                    AssignToPlace(place, value, assignmentExpression.Span);
                    return;
                }
                //case ResultStatementSyntax resultExpression:
                //    // Must be an expression of type `never`
                //    ConvertExpressionToStatement(resultExpression.Expression);
                //    ExitScope(resultExpression.Span.AtEnd());
                //    return;
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
        }

        private Value ConvertToValue(IExpressionSyntax expression)
        {
            switch (expression)
            {
                default:
                    throw NonExhaustiveMatchException.For(expression);
                case INewObjectExpressionSyntax newObjectExpression:
                {
                    var args = newObjectExpression.Arguments.Select(a => ConvertToOperand(a.Expression))
                        .ToFixedList();
                    var type = (UserObjectType)newObjectExpression.Type;
                    // lifetime is implicitly owned since we are making a new one
                    type = type.WithLifetime(Lifetime.None);
                    return new ConstructorCall(type, args, newObjectExpression.Span);
                }
                case INameExpressionSyntax identifier:
                {
                    var symbol = identifier.ReferencedSymbol;
                    switch (symbol)
                    {
                        case IVariableDeclarationStatementSyntax _:
                        case IParameterSyntax _:
                        case IForeachExpressionSyntax _:
                            return graph.VariableFor(symbol.FullName.UnqualifiedName)
                                .Reference(identifier.Span);
                        default:
                            return new DeclaredValue(symbol.FullName, identifier.Span);
                    }
                }
                case IUnaryOperatorExpressionSyntax unaryExpression:
                    return ConvertUnaryExpressionToValue(unaryExpression);
                case IBinaryOperatorExpressionSyntax binaryExpression:
                    return ConvertBinaryExpressionToValue(binaryExpression);
                case IIntegerLiteralExpressionSyntax _:
                    throw new InvalidOperationException(
                        "Integer literals should have an implicit conversion around them");
                case IStringLiteralExpressionSyntax stringLiteral:
                    return new StringConstant(stringLiteral.Value, stringLiteral.Span, stringLiteral.Type.AssertKnown());
                case IBoolLiteralExpressionSyntax boolLiteral:
                    return new BooleanConstant(boolLiteral.Value, boolLiteral.Span);
                case INoneLiteralExpressionSyntax _:
                    throw new InvalidOperationException(
                        "None literals should have an implicit conversion around them");
                case IImplicitNumericConversionExpression implicitNumericConversion:
                    if (implicitNumericConversion.Expression.Type.AssertKnown() is
                        IntegerConstantType constantType)
                        return new IntegerConstant(constantType.Value,
                            implicitNumericConversion.Type.AssertKnown(),
                            implicitNumericConversion.Span);
                    else
                        throw new NotImplementedException();
                case IImplicitOptionalConversionExpression implicitOptionalConversionExpression:
                {
                    var value = ConvertToOperand(implicitOptionalConversionExpression.Expression);
                    return new ConstructSome(implicitOptionalConversionExpression.ConvertToType,
                        value, implicitOptionalConversionExpression.Span);
                }
                case IIfExpressionSyntax ifExpression:
                    // TODO deal with the value of the if expression
                    throw new NotImplementedException();
                case IUnsafeExpressionSyntax unsafeExpression:
                    return ConvertToValue(unsafeExpression.Expression);
                case IImplicitNoneConversionExpression implicitNoneConversion:
                    return new NoneConstant(implicitNoneConversion.ConvertToType,
                        implicitNoneConversion.Span);
                case IInvocationExpressionSyntax invocation:
                    return ConvertInvocationToValue(invocation);
                case IMemberAccessExpressionSyntax memberAccess:
                {
                    var value = ConvertToOperand(memberAccess.Expression);
                    var symbol = memberAccess.ReferencedSymbol;
                    //if (symbol is IAccessorSymbol accessor)
                    //    return new VirtualFunctionCall(memberAccess.Span, accessor.PropertyName.UnqualifiedName, value);

                    return new FieldAccess(value, memberAccess.ReferencedSymbol.FullName,
                        memberAccess.Span);
                }
                //case MutableTypeSyntax mutable:
                //    // TODO shouldn't borrowing be explicit in the IR and don't we
                //    // need to be able to check mutability on borrows?
                //    return ConvertToValue(mutable.Referent);
                case IMoveTransferSyntax move:
                    return ConvertToOwn(move.Expression, move.Span);
                case IImplicitImmutabilityConversionExpression implicitImmutabilityConversion:
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
                            if (implicitImmutabilityConversion.Type.ValueSemantics
                                == ValueSemantics.Own)
                                return varReference.AsOwn(implicitImmutabilityConversion.Span);
                            else
                                return varReference.AsAlias();
                        default:
                            throw NonExhaustiveMatchException.For(expression);
                    }
                }
                case ISelfExpressionSyntax selfExpression:
                    return graph.VariableFor(SpecialName.Self).Reference(selfExpression.Span);
            }
        }

        private Value ConvertToOwn(IExpressionSyntax expression, TextSpan moveSpan)
        {
            var operand = ConvertToOperand(expression);
            switch (operand)
            {
                case VariableReference variableReference:
                    return variableReference.AsOwn(moveSpan);
                default:
                    throw new NotImplementedException();
            }
        }

        private IOperand ConvertToOperand(IExpressionSyntax expression)
        {
            var value = ConvertToValue(expression);
            return ConvertToOperand(value, expression.Type);
        }

        private IOperand ConvertToOperand(IValue value, DataType type)
        {
            if (value is IOperand operand)
                return operand;
            return AssignToTemp(type, value);
            //var tempVariable = graph.Let(type.AssertKnown(), CurrentScope);
            //currentBlock.AddAssignment(tempVariable.LValueReference(value.Span), value, value.Span, CurrentScope);
            //return tempVariable.Reference(value.Span);
        }

        private Value ConvertBinaryExpressionToValue(IBinaryOperatorExpressionSyntax operatorExpression)
        {
            switch (operatorExpression.Operator)
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
                    var leftOperand = ConvertToOperand(operatorExpression.LeftOperand);
                    var rightOperand = ConvertToOperand(operatorExpression.RightOperand);
                    switch (operatorExpression.LeftOperand.Type)
                    {
                        case SimpleType operandType:
                        {
                            // What matters is the type we are operating on, for comparisons, that is different than the result type which is bool
                            return new BinaryOperation(leftOperand, operatorExpression.Operator,
                                rightOperand, operandType);
                        }
                        case UserObjectType _:
                        {
                            if (operatorExpression.Operator != BinaryOperator.EqualsEquals)
                                throw new NotImplementedException();
                            //var equalityOperators = operandType.Symbol.Lookup(SpecialName.OperatorEquals);
                            //if (equalityOperators.Count == 1)
                            //{
                            //    var equalityOperator = equalityOperators.Single();
                            //    return new FunctionCall(equalityOperator.FullName,
                            //                //(FunctionType)equalityOperator.Type,
                            //                new[] { leftOperand, rightOperand },
                            //                expression.Span);
                            //}
                            throw new NotImplementedException();
                        }
                        default:
                            throw NonExhaustiveMatchException.For(operatorExpression.LeftOperand.Type);
                    }
                }
                case BinaryOperator.And:
                case BinaryOperator.Or:
                {
                    // TODO handle calls to overloaded operators
                    // TODO handle short circuiting if needed
                    var leftOperand = ConvertToOperand(operatorExpression.LeftOperand);
                    var rightOperand = ConvertToOperand(operatorExpression.RightOperand);
                    return new BinaryOperation(leftOperand, operatorExpression.Operator, rightOperand,
                        (SimpleType)operatorExpression.Type);
                }
                case BinaryOperator.DotDot:
                    throw new NotImplementedException("Conversion of `..` for binary operators");
                default:
                    throw ExhaustiveMatch.Failed(operatorExpression.Operator);
            }
        }

        private Value ConvertUnaryExpressionToValue(IUnaryOperatorExpressionSyntax operatorExpression)
        {
            switch (operatorExpression.Operator)
            {
                case UnaryOperator.Not:
                case UnaryOperator.Minus:
                    var operand = ConvertToOperand(operatorExpression.Operand);
                    return new UnaryOperation(operatorExpression.Operator, operand, operatorExpression.Span);
                case UnaryOperator.Plus:
                    // This is a no-op
                    return ConvertToValue(operatorExpression.Operand);
                case UnaryOperator.Question:
                    //case UnaryOperator.At:
                    //case UnaryOperator.Caret:
                    throw new NotImplementedException(
                        "Unary expression conversion not implemented");
                default:
                    throw ExhaustiveMatch.Failed(operatorExpression.Operator);
            }
        }

        private Value ConvertInvocationToValue(IInvocationExpressionSyntax invocationExpression)
        {
            switch (invocationExpression)
            {
                default:
                    throw ExhaustiveMatch.Failed(invocationExpression);
                case IMethodInvocationExpressionSyntax methodInvocation:
                    return ConvertInvocationToValue(methodInvocation);
                case IAssociatedFunctionInvocationExpressionSyntax _:
                    throw new NotImplementedException();
                case IFunctionInvocationExpressionSyntax functionInvocation:
                    return ConvertInvocationToValue(functionInvocation);
            }
        }

        private Value ConvertInvocationToValue(IMethodInvocationExpressionSyntax invocationExpression)
        {
            var self = ConvertToOperand(invocationExpression.Target);
            var arguments = invocationExpression.Arguments.Select(a => ConvertToOperand(a.Expression)).ToList();
            var symbol = (IFunctionSymbol)invocationExpression.MethodNameSyntax.ReferencedSymbol;
            switch (invocationExpression.Target.Type)
            {
                case SimpleType _:
                    // Full name because this isn't a member
                    return new FunctionCall(symbol.FullName,
                        self, arguments, invocationExpression.Span);
                default:
                    return new VirtualFunctionCall(invocationExpression.Span,
                        symbol.FullName.UnqualifiedName, self, arguments);
            }
        }

        private Value ConvertInvocationToValue(IFunctionInvocationExpressionSyntax invocationExpression)
        {
            var functionSymbol = (IFunctionSymbol)invocationExpression.FunctionNameSyntax.ReferencedSymbol;
            var arguments = invocationExpression.Arguments.Select(a => ConvertToOperand(a.Expression)).ToList();
            return new FunctionCall(functionSymbol.FullName, arguments, invocationExpression.Span);
        }

        private IPlace ConvertToPlace(IExpressionSyntax value)
        {
            switch (value)
            {
                case INameExpressionSyntax identifier:
                    // TODO what if this isn't just a variable?
                    return graph.VariableFor(identifier.ReferencedSymbol.FullName.UnqualifiedName).LValueReference(value.Span);
                case IMemberAccessExpressionSyntax memberAccessExpression:
                    var expressionValue = ConvertToOperand(memberAccessExpression.Expression);
                    return new FieldAccess(expressionValue, memberAccessExpression.Member.Name, memberAccessExpression.Span);
                default:
                    throw NonExhaustiveMatchException.For(value);
            }
        }
    }
}
