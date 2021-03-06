using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core.Operators;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Operands;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Places;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.TerminatorInstructions;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;
using ExhaustiveMatching;
using static Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions.BooleanLogicOperator;
using static Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions.CompareInstructionOperator;
using static Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions.NumericInstructionOperator;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.ILGen
{
    /// <summary>
    /// The fabrication of a single control flow graph from a single callable AST node
    /// </summary>
    /// <remarks>
    /// The control flow graph can only be constructed from a completely valid AST.
    /// That is, there must be no fatal compiler errors.
    /// </remarks>
    public class ControlFlowGraphFabrication
    {
        private readonly IConcreteInvocableDeclaration invocable;
        private readonly SelfParameterSymbol? selfParameter;
        private readonly DataType returnType;
        private readonly ControlFlowGraphBuilder graph;

        /// <summary>
        /// The block we are currently adding statements to. Thus, after control flow statements this
        /// is the block the control flow exits (to/from?).
        /// </summary>
        private BlockBuilder? currentBlock;

        /// <summary>
        /// Actions are registered here to connect break statements to the loop exit block
        /// </summary>
        private List<Action<BlockBuilder>> addBreaks = new List<Action<BlockBuilder>>();

        /// <summary>
        /// The block that a `next` statement should go to
        /// </summary>
        private BlockBuilder? continueToBlock;

        /// <summary>
        /// The next available scope number
        /// </summary>
        //private Scope nextScope;
        private readonly Stack<Scope> scopes = new Stack<Scope>();
        private Scope CurrentScope => scopes.Peek();

        public ControlFlowGraphFabrication(IConcreteInvocableDeclaration invocable)
        {
            this.invocable = invocable;
            graph = new ControlFlowGraphBuilder(invocable.File);
            // We start in the outer scope and need that on the stack
            var scope = Scope.Outer;
            scopes.Push(scope);
            //nextScope = scope.Next();
            switch (invocable)
            {
                default:
                    throw ExhaustiveMatch.Failed(invocable);
                case IConcreteMethodDeclaration method:
                    selfParameter = method.SelfParameter.Symbol;
                    returnType = method.Symbol.ReturnDataType.Known();
                    break;
                case IConstructorDeclaration constructor:
                    selfParameter = constructor.ImplicitSelfParameter.Symbol;
                    returnType = DataType.Void; // the body should `return;`
                    break;
                case IAssociatedFunctionDeclaration associatedFunction:
                    returnType = associatedFunction.Symbol.ReturnDataType.Known();
                    break;
                case IFunctionDeclaration function:
                    returnType = function.Symbol.ReturnDataType.Known();
                    break;
            }

            // TODO really use return type
            _ = returnType;
        }

        public ControlFlowGraph CreateGraph()
        {
            // Temp Variable for return
            if (selfParameter != null) graph.AddSelfParameter(selfParameter);

            foreach (var parameter in invocable.Parameters.Where(p => !p.Unused).OfType<INamedParameter>())
                graph.AddParameter(parameter.Symbol.IsMutableBinding, parameter.Symbol.DataType, CurrentScope, parameter.Symbol);

            currentBlock = graph.NewBlock();
            foreach (var statement in invocable.Body.Statements)
                Convert(statement);

            // Generate the implicit return statement
            if (currentBlock != null && !currentBlock.IsTerminated)
            {
                var span = invocable.Span.AtEnd();
                //EndScope(span);
                currentBlock.End(new ReturnVoidInstruction(span, Scope.Outer));
            }

            return graph.Build();
        }


        private void Convert(IStatement statement)
        {
            switch (statement)
            {
                default:
                    throw ExhaustiveMatch.Failed(statement);
                case IVariableDeclarationStatement variableDeclaration:
                {
                    var variable = graph.AddVariable(variableDeclaration.Symbol.IsMutableBinding,
                        variableDeclaration.Symbol.DataType,
                        CurrentScope, variableDeclaration.Symbol);
                    if (variableDeclaration.Initializer != null)
                    {
                        ConvertIntoPlace(variableDeclaration.Initializer,
                           variable.Place(variableDeclaration.Initializer.Span));
                    }
                }
                break;
                case IExpressionStatement expressionStatement:
                {
                    var expression = expressionStatement.Expression;
                    if (!expression.DataType.Assigned().IsKnown)
                        throw new ArgumentException("Expression must have a known type", nameof(statement));

                    Convert(expression);
                }
                break;
                case IResultStatement resultStatement:
                {
                    var expression = resultStatement.Expression;
                    if (!expression.DataType.Assigned().IsKnown)
                        throw new ArgumentException("Expression must have a known type", nameof(statement));

                    Convert(expression);
                }
                break;
            }
        }

        /// <summary>
        /// Convert without expecting any result value
        /// </summary>
        private void Convert(IBlockOrResult blockOrResult)
        {
            switch (blockOrResult)
            {
                default:
                    throw ExhaustiveMatch.Failed(blockOrResult);
                case IBlockExpression exp:
                    Convert((IExpression)exp);
                    break;
                case IResultStatement statement:
                    Convert((IStatement)statement);
                    break;
            }
        }

        private void Convert(IElseClause elseClause)
        {
            switch (elseClause)
            {
                default:
                    throw ExhaustiveMatch.Failed(elseClause);
                case IBlockOrResult blockOrResult:
                    Convert(blockOrResult);
                    break;
                case IIfExpression exp:
                    Convert((IExpression)exp);
                    break;
            }
        }

        /// <summary>
        /// Convert an expression without expecting any result value
        /// </summary>
        private void Convert(IExpression expression)
        {
            switch (expression)
            {
                default:
                    throw ExhaustiveMatch.Failed(expression);
                case INewObjectExpression _:
                case IFieldAccessExpression _:
                    throw new NotImplementedException($"Convert({expression.GetType().Name}) Not Implemented.");
                case IBorrowExpression exp:
                    Convert(exp.Referent);
                    break;
                case IShareExpression exp:
                    Convert(exp.Referent);
                    break;
                case IMoveExpression exp:
                    Convert(exp.Referent);
                    break;
                case IImplicitNumericConversionExpression exp:
                    Convert(exp.Expression);
                    break;
                case IImplicitOptionalConversionExpression exp:
                    Convert(exp.Expression);
                    break;
                case IImplicitImmutabilityConversionExpression exp:
                    Convert(exp.Expression);
                    break;
                case ILoopExpression exp:
                {
                    var loopEntry = graph.NewEntryBlock(currentBlock!, exp.Block.Span.AtStart(), CurrentScope);
                    currentBlock = loopEntry;
                    continueToBlock = loopEntry;
                    var loopExit = ConvertLoopBody(exp.Block, exitRequired: false);
                    // If it always breaks, there isn't a current block
                    currentBlock?.End(new GotoInstruction(loopEntry.Number, exp.Block.Span.AtEnd(), CurrentScope));
                    currentBlock = loopExit;
                }
                break;
                case IWhileExpression whileExpression:
                {
                    // There is a block for the condition, it then goes either to
                    // the body or the after block.
                    var conditionBlock = graph.NewEntryBlock(currentBlock!,
                        whileExpression.Condition.Span.AtStart(), CurrentScope);
                    currentBlock = conditionBlock;
                    var condition = ConvertToOperand(whileExpression.Condition);
                    var loopEntry = graph.NewBlock();
                    continueToBlock = conditionBlock;
                    currentBlock = loopEntry;
                    var loopExit = ConvertLoopBody(whileExpression.Block);
                    // Generate if branch now that loop exit is known
                    conditionBlock.End(new IfInstruction(condition, loopEntry.Number, loopExit!.Number,
                            whileExpression.Condition.Span, CurrentScope));
                    // If it always breaks, there isn't a current block
                    currentBlock?.End(new GotoInstruction(conditionBlock.Number,
                        whileExpression.Block.Span.AtEnd(), CurrentScope));
                    currentBlock = loopExit;
                }
                break;
                case IForeachExpression exp:
                {
                    // For now, we support only range syntax `foreach x: T in z..y`. Range operators
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
                    if (!(exp.InExpression is IBinaryOperatorExpression inExpression)
                        || (inExpression.Operator != BinaryOperator.DotDot
                            && inExpression.Operator != BinaryOperator.LessThanDotDot
                            && inExpression.Operator != BinaryOperator.DotDotLessThan
                            && inExpression.Operator != BinaryOperator.LessThanDotDotLessThan))
                        throw new NotImplementedException(
                            "`foreach in` non-range expression not implemented");

                    var startExpression = inExpression.LeftOperand;
                    var endExpression = inExpression.RightOperand;

                    var variableType = (IntegerType)exp.Symbol.DataType;
                    var loopVariable = graph.AddVariable(exp.Symbol.IsMutableBinding,
                                            variableType, CurrentScope, exp.Symbol);
                    var loopVariablePlace = loopVariable.Place(exp.Span);
                    var loopVariableReference = loopVariable.Reference(exp.Span);

                    var includeStart = inExpression.Operator == BinaryOperator.DotDot
                                       || inExpression.Operator == BinaryOperator.DotDotLessThan;
                    var includeEnd = inExpression.Operator == BinaryOperator.DotDot
                                       || inExpression.Operator == BinaryOperator.LessThanDotDot;

                    // Load up the constant 1
                    var one = graph.Let(variableType, CurrentScope);
                    currentBlock!.Add(new LoadIntegerInstruction(one.Place(exp.Span), 1, variableType,
                                            exp.Span, CurrentScope));

                    // emit var x: T = z (+1);
                    ConvertIntoPlace(startExpression, loopVariablePlace);
                    if (!includeStart)
                    {
                        var addSpan = inExpression.LeftOperand.Span.AtEnd();
                        currentBlock!.Add(new NumericInstruction(loopVariablePlace, Add, variableType,
                                                loopVariableReference, one.Reference(addSpan), CurrentScope));
                    }

                    // let temp = y;
                    var endValue = ConvertToOperand(endExpression);

                    // Emit block
                    var loopEntry = graph.NewEntryBlock(currentBlock,
                                                exp.Block.Span.AtStart(), CurrentScope);
                    currentBlock = loopEntry;
                    var conditionBlock = continueToBlock = graph.NewBlock();
                    // TODO this generates the exit block too soon if the break condition is non-trivial
                    var loopExit = ConvertLoopBody(exp.Block)!;
                    // If it always breaks, there isn't a current block
                    currentBlock?.End(new GotoInstruction(conditionBlock.Number,
                                            exp.Block.Span.AtEnd(), CurrentScope));
                    currentBlock = conditionBlock;
                    // emit x += 1;
                    currentBlock.Add(new NumericInstruction(loopVariablePlace, Add, variableType, loopVariableReference, one.Reference(exp.Span), CurrentScope));

                    // emit if x (>)|(>=) temp => break;
                    var breakOperator = includeEnd ? GreaterThan : GreaterThanOrEqual;
                    var condition = graph.AddVariable(true, DataType.Bool, CurrentScope);
                    currentBlock.Add(new CompareInstruction(condition.Place(exp.Span),
                        breakOperator, variableType, loopVariableReference, endValue, CurrentScope));

                    currentBlock.End(new IfInstruction(condition.Reference(exp.Span), loopExit.Number, loopEntry.Number,
                            exp.Span, CurrentScope));
                    currentBlock = loopExit;
                }
                break;
                case IBreakExpression exp:
                {
                    // TODO Do we need `ExitScope(exp.Span.AtEnd());` ?
                    // capture the current block for use in the lambda
                    var breakingBlock = currentBlock!;
                    addBreaks.Add(loopExit => breakingBlock.End(new GotoInstruction(loopExit.Number, exp.Span, CurrentScope)));
                    currentBlock = null;
                }
                break;
                case INextExpression exp:
                {
                    // TODO Do we need `ExitScope(nextExpression.Span.AtEnd());` ?
                    currentBlock!.End(new GotoInstruction(continueToBlock?.Number ?? throw new InvalidOperationException(),
                        exp.Span, CurrentScope));
                    currentBlock = null;
                }
                break;
                case IIfExpression exp:
                {
                    var containingBlock = currentBlock!;
                    var condition = ConvertToOperand(exp.Condition);
                    var thenEntry = graph.NewBlock();
                    currentBlock = thenEntry;
                    Convert(exp.ThenBlock);
                    var thenExit = currentBlock;
                    BlockBuilder elseEntry;
                    BlockBuilder? exit = null;
                    if (exp.ElseClause is null)
                    {
                        elseEntry = exit = graph.NewBlock();
                        thenExit?.End(new GotoInstruction(exit.Number, exp.ThenBlock.Span.AtEnd(), CurrentScope));
                    }
                    else
                    {
                        elseEntry = graph.NewBlock();
                        currentBlock = elseEntry;
                        Convert(exp.ElseClause);
                        var elseExit = currentBlock;
                        if (thenExit != null || elseExit != null)
                        {
                            exit = graph.NewBlock();
                            thenExit?.End(new GotoInstruction(exit.Number, exp.ThenBlock.Span.AtEnd(), CurrentScope));
                            elseExit?.End(new GotoInstruction(exit.Number, exp.ElseClause.Span.AtEnd(), CurrentScope));
                        }
                    }

                    containingBlock.End(new IfInstruction(condition, thenEntry.Number, elseEntry.Number, exp.Condition.Span, CurrentScope));
                    currentBlock = exit;
                }
                break;
                case IMethodInvocationExpression exp:
                {
                    var method = exp.ReferencedSymbol;
                    var target = ConvertToOperand(exp.Context);
                    var args = exp.Arguments.Select(ConvertToOperand).ToFixedList();
                    currentBlock!.Add(new CallVirtualInstruction(target, method, args, exp.Span, CurrentScope));
                }
                break;
                case IAssignmentExpression exp:
                {
                    var leftOperand = exp.LeftOperand;
                    var assignInto = ConvertToPlace(leftOperand);
                    NumericInstructionOperator? op = exp.Operator switch
                    {
                        AssignmentOperator.Simple => null,
                        AssignmentOperator.Plus => Add,
                        AssignmentOperator.Minus => Subtract,
                        AssignmentOperator.Asterisk => Multiply,
                        AssignmentOperator.Slash => Divide,
                        _ => throw ExhaustiveMatch.Failed(exp.Operator),
                    };
                    if (op is null)
                        ConvertIntoPlace(exp.RightOperand, assignInto);
                    else
                    {
                        var rhs = ConvertToOperand(exp.RightOperand);
                        currentBlock!.Add(new NumericInstruction(assignInto, op.Value, (NumericType)leftOperand.DataType.Known(),
                            assignInto.ToOperand(leftOperand.Span), rhs, CurrentScope));
                    }
                }
                break;
                case IUnsafeExpression exp:
                    Convert(exp.Expression);
                    break;
                case IBlockExpression exp:
                    foreach (var statement in exp.Statements)
                        Convert(statement);
                    break;
                case IFunctionInvocationExpression exp:
                {
                    var functionName = exp.ReferencedSymbol;
                    var args = exp.Arguments.Select(ConvertToOperand).ToFixedList();
                    currentBlock!.Add(CallInstruction.ForFunction(functionName, args, exp.Span, CurrentScope));
                }
                break;
                case IReturnExpression exp:
                {
                    if (exp.Value is null)
                        currentBlock!.End(new ReturnVoidInstruction(exp.Span, CurrentScope));
                    else
                    {
                        var returnValue = ConvertToOperand(exp.Value);
                        currentBlock!.End(new ReturnValueInstruction(returnValue, exp.Span, CurrentScope));
                    }

                    // There is no exit from a return block, hence null for exit block
                    currentBlock = null;
                }
                break;
                case INameExpression _:
                case IBinaryOperatorExpression _:
                case IUnaryOperatorExpression _:
                case IBoolLiteralExpression _:
                case IStringLiteralExpression _:
                case ISelfExpression _:
                case INoneLiteralExpression _:
                case IIntegerLiteralExpression _:
                case IImplicitNoneConversionExpression _:
                    // These operation have no side effects, so if the result isn't needed, there is nothing to do
                    break;

            }
        }

        /// <summary>
        /// Convert the body of a loop. Ensures break statements are handled correctly.
        /// </summary>
        private BlockBuilder? ConvertLoopBody(IBlockExpression body, bool exitRequired = true)
        {
            var oldAddBreaks = addBreaks;
            addBreaks = new List<Action<BlockBuilder>>();
            Convert((IExpression)body);
            BlockBuilder? loopExit = null;
            // If this kind of loop requires an exit or if there is a break, create an exit block
            if (exitRequired || addBreaks.Any())
            {
                loopExit = graph.NewBlock();
                foreach (var addBreak in addBreaks) addBreak(loopExit);
                addBreaks = oldAddBreaks;
            }
            return loopExit;
        }

        /// <summary>
        /// Convert an expression that yields a value and assign that value into <paramref name="resultPlace"/>.
        /// </summary>
        private void ConvertIntoPlace(IExpression expression, Place resultPlace)
        {
            switch (expression)
            {
                default:
                    throw ExhaustiveMatch.Failed(expression);
                case ILoopExpression _:
                case IWhileExpression _:
                case IForeachExpression _:
                case IReturnExpression _:
                case IBreakExpression _:
                case INextExpression _:
                case IIfExpression _:
                case IUnsafeExpression _:
                case IBlockExpression _:
                case INoneLiteralExpression _:
                    throw new NotImplementedException($"ConvertIntoPlace({expression.GetType().Name}, Place) Not Implemented.");
                case ISelfExpression exp:
                {
                    // This occurs when the source code contains a simple assignment like `x = self`
                    var symbol = exp.ReferencedSymbol;
                    var variable = graph.VariableFor(symbol).Reference(exp.Span);
                    currentBlock!.Add(new AssignmentInstruction(resultPlace, variable, exp.Span, CurrentScope));
                }
                break;
                case IImplicitOptionalConversionExpression exp:
                {
                    var operand = ConvertToOperand(exp.Expression);
                    currentBlock!.Add(new SomeInstruction(resultPlace, exp.ConvertToType, operand, exp.Span, CurrentScope));
                }
                break;
                case IAssignmentExpression exp:
                    throw new NotImplementedException("Assignments don't have a result");
                case INameExpression exp:
                {
                    // This occurs when the source code contains a simple assignment like `x = y`
                    var symbol = exp.ReferencedSymbol;
                    var variable = graph.VariableFor(symbol).Reference(exp.Span);
                    currentBlock!.Add(new AssignmentInstruction(resultPlace, variable, exp.Span, CurrentScope));
                }
                break;
                case IBorrowExpression exp:
                    ConvertIntoPlace(exp.Referent, resultPlace);
                    break;
                case IShareExpression exp:
                    ConvertIntoPlace(exp.Referent, resultPlace);
                    break;
                case IMoveExpression exp:
                    ConvertIntoPlace(exp.Referent, resultPlace);
                    break;
                case IImplicitImmutabilityConversionExpression exp:
                    ConvertIntoPlace(exp.Expression, resultPlace);
                    break;
                case IBinaryOperatorExpression exp:
                {
                    var resultType = exp.DataType.Assigned().Known();
                    var operandType = exp.LeftOperand.DataType.Assigned().Known();
                    var leftOperand = ConvertToOperand(exp.LeftOperand);
                    var rightOperand = ConvertToOperand(exp.RightOperand);
                    switch (exp.Operator)
                    {
                        default:
                            throw ExhaustiveMatch.Failed(expression);
                        case BinaryOperator.DotDot:
                        case BinaryOperator.LessThanDotDot:
                        case BinaryOperator.DotDotLessThan:
                        case BinaryOperator.LessThanDotDotLessThan:
                            throw new NotImplementedException("Range operator control flow not implemented");
                        #region Logical Operators
                        case BinaryOperator.And:
                            // TODO handle calls to overloaded operators
                            // TODO handle short circuiting if needed
                            currentBlock!.Add(new BooleanLogicInstruction(resultPlace, And,
                                leftOperand, rightOperand, CurrentScope));
                            break;
                        case BinaryOperator.Or:
                            // TODO handle calls to overloaded operators
                            // TODO handle short circuiting if needed
                            currentBlock!.Add(new BooleanLogicInstruction(resultPlace, Or,
                                leftOperand, rightOperand, CurrentScope));
                            break;
                        #endregion
                        #region Binary Math Operators
                        case BinaryOperator.Plus:
                            currentBlock!.Add(new NumericInstruction(resultPlace, Add,
                                (NumericType)resultType, leftOperand, rightOperand, CurrentScope));
                            break;
                        case BinaryOperator.Minus:
                            currentBlock!.Add(new NumericInstruction(resultPlace, Subtract,
                                (NumericType)resultType, leftOperand, rightOperand, CurrentScope));
                            break;
                        case BinaryOperator.Asterisk:
                            currentBlock!.Add(new NumericInstruction(resultPlace, Multiply,
                                (NumericType)resultType, leftOperand, rightOperand, CurrentScope));
                            break;
                        case BinaryOperator.Slash:
                            currentBlock!.Add(new NumericInstruction(resultPlace, Divide,
                                (NumericType)resultType, leftOperand, rightOperand, CurrentScope));
                            break;
                        #endregion
                        #region Comparisons
                        case BinaryOperator.EqualsEquals:
                            currentBlock!.Add(new CompareInstruction(resultPlace, Equal,
                                (NumericType)operandType, leftOperand, rightOperand, CurrentScope));
                            break;
                        case BinaryOperator.NotEqual:
                            currentBlock!.Add(new CompareInstruction(resultPlace, NotEqual,
                                (NumericType)operandType, leftOperand, rightOperand, CurrentScope));
                            break;
                        case BinaryOperator.LessThan:
                            currentBlock!.Add(new CompareInstruction(resultPlace, LessThan,
                                (NumericType)operandType, leftOperand, rightOperand, CurrentScope));
                            break;
                        case BinaryOperator.LessThanOrEqual:
                            currentBlock!.Add(new CompareInstruction(resultPlace, LessThanOrEqual,
                                (NumericType)operandType, leftOperand, rightOperand, CurrentScope));
                            break;
                        case BinaryOperator.GreaterThan:
                            currentBlock!.Add(new CompareInstruction(resultPlace, GreaterThan,
                                (NumericType)operandType, leftOperand, rightOperand, CurrentScope));
                            break;
                        case BinaryOperator.GreaterThanOrEqual:
                            currentBlock!.Add(new CompareInstruction(resultPlace, GreaterThanOrEqual,
                                (NumericType)operandType, leftOperand, rightOperand, CurrentScope));
                            break;
                            #endregion Comparisons
                    }
                }
                break;
                case IUnaryOperatorExpression exp:
                {
                    var type = exp.DataType.Assigned().Known();
                    var operand = ConvertToOperand(exp.Operand);
                    switch (exp.Operator)
                    {
                        default:
                            throw ExhaustiveMatch.Failed(expression);
                        case UnaryOperator.Not:
                        case UnaryOperator.Plus: // TODO don't even allow unary plus in IL or AST, it is a noop
                            throw new NotImplementedException($"ConvertToOperand({expression.GetType().Name}, Place) Not Implemented for {exp.Operator}.");
                        case UnaryOperator.Minus:
                            currentBlock!.Add(new NegateInstruction(resultPlace, (NumericType)type, operand, exp.Span, CurrentScope));
                            break;
                    }
                }
                break;
                case IFieldAccessExpression exp:
                {
                    var context = ConvertToOperand(exp.Context);
                    var field = exp.ReferencedSymbol;
                    currentBlock!.Add(new FieldAccessInstruction(resultPlace, context, field, exp.Span, CurrentScope));
                }
                break;
                case IFunctionInvocationExpression exp:
                {
                    var functionName = exp.ReferencedSymbol;
                    var args = exp.Arguments.Select(ConvertToOperand).ToFixedList();
                    currentBlock!.Add(CallInstruction.ForFunction(resultPlace, functionName, args, exp.Span, CurrentScope));
                }
                break;
                case IMethodInvocationExpression exp:
                {
                    var methodName = exp.ReferencedSymbol;
                    var target = ConvertToOperand(exp.Context);
                    var args = exp.Arguments.Select(ConvertToOperand).ToFixedList();
                    if (exp.Context.DataType is ReferenceType)
                        currentBlock!.Add(new CallVirtualInstruction(resultPlace, target, methodName, args, exp.Span, CurrentScope));
                    else
                        currentBlock!.Add(CallInstruction.ForMethod(resultPlace, target, methodName, args, exp.Span, CurrentScope));
                }
                break;
                case INewObjectExpression exp:
                {
                    var constructor = exp.ReferencedSymbol;
                    var args = exp.Arguments.Select(ConvertToOperand).ToFixedList();
                    var constructedType = (ObjectType)exp.DataType.Known();
                    currentBlock!.Add(new NewObjectInstruction(resultPlace, constructor, constructedType, args, exp.Span, CurrentScope));
                }
                break;
                case IStringLiteralExpression exp:
                    currentBlock!.Add(new LoadStringInstruction(resultPlace, exp.Value, exp.Span, CurrentScope));
                    break;
                case IBoolLiteralExpression exp:
                    currentBlock!.Add(new LoadBoolInstruction(resultPlace, exp.Value, exp.Span, CurrentScope));
                    break;
                case IImplicitNumericConversionExpression exp:
                {
                    if (exp.Expression.DataType.Assigned().Known() is IntegerConstantType constantType)
                        currentBlock!.Add(new LoadIntegerInstruction(resultPlace, constantType.Value,
                            (IntegerType)exp.DataType.Assigned().Known(),
                            exp.Span, CurrentScope));
                    else
                        currentBlock!.Add(new ConvertInstruction(resultPlace, ConvertToOperand(exp.Expression),
                            (NumericType)exp.Expression.DataType.Assigned().Known(), exp.ConvertToType,
                            exp.Span, CurrentScope));
                }
                break;
                case IIntegerLiteralExpression exp:
                    throw new InvalidOperationException(
                        "Integer literals should have an implicit conversion around them");
                case IImplicitNoneConversionExpression exp:
                    currentBlock!.Add(new LoadNoneInstruction(resultPlace, exp.ConvertToType, exp.Span, CurrentScope));
                    break;
            }
        }

        /// <summary>
        /// Convert an expression that yields a value into an operand for another instruction
        /// </summary>
        private Operand ConvertToOperand(IExpression expression)
        {
            switch (expression)
            {
                default:
                    throw ExhaustiveMatch.Failed(expression);
                case ISelfExpression exp:
                    return graph.SelfVariable.Reference(exp.Span);
                case INameExpression exp:
                {
                    var symbol = exp.ReferencedSymbol;
                    return graph.VariableFor(symbol).Reference(exp.Span);
                }
                case IBorrowExpression exp:
                    return ConvertToOperand(exp.Referent);
                case IShareExpression exp:
                    return ConvertToOperand(exp.Referent);
                case IMoveExpression exp:
                    return ConvertToOperand(exp.Referent);
                case IAssignmentExpression _:
                case IBinaryOperatorExpression _:
                case IUnaryOperatorExpression _:
                case IFieldAccessExpression _:
                case IMethodInvocationExpression _:
                case INewObjectExpression _:
                case IImplicitNumericConversionExpression _:
                case IIntegerLiteralExpression _:
                case IStringLiteralExpression _:
                case IBoolLiteralExpression _:
                case INoneLiteralExpression _:
                case IImplicitImmutabilityConversionExpression _:
                case IImplicitOptionalConversionExpression _:
                case IUnsafeExpression _:
                case IBlockExpression _:
                case IImplicitNoneConversionExpression _:
                case IBreakExpression _:
                case INextExpression _:
                case IReturnExpression _:
                case IIfExpression _:
                case IFunctionInvocationExpression _:
                case IForeachExpression _:
                case ILoopExpression _:
                case IWhileExpression _:
                {
                    var tempVar = graph.Let(expression.DataType.Assigned().Known(), CurrentScope);
                    ConvertIntoPlace(expression, tempVar.Place(expression.Span));
                    return tempVar.Reference(expression.Span);
                }
            }
        }

        /// <summary>
        /// Convert an expression to a place. Used for LValues
        /// </summary>
        private Place ConvertToPlace(IExpression expression)
        {
            switch (expression)
            {
                default:
                    //throw ExhaustiveMatch.Failed(expression);
                    throw new NotImplementedException($"ConvertToPlaceWithoutSideEffects({expression.GetType().Name}) Not Implemented.");
                case IFieldAccessExpression exp:
                {
                    var context = ConvertToOperand(exp.Context);
                    var field = exp.ReferencedSymbol;
                    return new FieldPlace(context, field, exp.Span);
                }
                case ISelfExpression exp:
                    return new VariablePlace(Variable.Self, exp.Span);
                case INameExpression exp:
                {
                    var symbol = exp.ReferencedSymbol;
                    return graph.VariableFor(symbol).Place(exp.Span);
                }
            }
        }

        //private void EnterNewScope()
        //{
        //    scopes.Push(nextScope);
        //    nextScope = nextScope.Next();
        //}

        ///// <summary>
        ///// An exit point for the current scope that doesn't end it. For example, a break statement
        ///// </summary>
        //private void ExitScope(TextSpan span)
        //{
        //    currentBlock?.AddExitScope(span, CurrentScope);
        //}

        //private void EndScope(TextSpan span)
        //{
        //    // In some cases we will have left the current block by a terminator,
        //    // so we don't need to emit an exit statement.
        //    currentBlock?.AddExitScope(span, CurrentScope);
        //    scopes.Pop();
        //}
    }
}
