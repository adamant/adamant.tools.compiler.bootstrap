using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Operands;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Places;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.TerminatorInstructions;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using ExhaustiveMatching;
using static Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions.CompareInstructionOperator;

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
        private readonly IConcreteCallableDeclarationSyntax callable;
        private readonly DataType? selfType;
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
        private Scope nextScope;
        private readonly Stack<Scope> scopes = new Stack<Scope>();
        private Scope CurrentScope => scopes.Peek();

        public ControlFlowGraphFabrication(IConcreteCallableDeclarationSyntax callable)
        {
            this.callable = callable;
            graph = new ControlFlowGraphBuilder(callable.File);
            // We start in the outer scope and need that on the stack
            var scope = Scope.Outer;
            scopes.Push(scope);
            nextScope = scope.Next();
            switch (callable)
            {
                default:
                    throw ExhaustiveMatch.Failed(callable);
                case IConcreteMethodDeclarationSyntax method:
                    returnType = method.ReturnType.Known();
                    break;
                case IConstructorDeclarationSyntax constructor:
                    selfType = constructor.SelfParameterType.Assigned();
                    returnType = DataType.Void; // the body should `return;`
                    break;
                case IAssociatedFunctionDeclaration associatedFunction:
                    returnType = associatedFunction.ReturnType.Known();
                    break;
                case IFunctionDeclarationSyntax function:
                    returnType = function.ReturnType.Known();
                    break;
            }
        }

        public ControlFlowGraph CreateGraph()
        {
            // Temp Variable for return
            if (selfType != null) graph.AddSelfParameter(selfType);

            foreach (var parameter in callable.Parameters.Where(p => !p.Unused))
                graph.AddParameter(parameter.IsMutableBinding, parameter.Type.Fulfilled(), CurrentScope, parameter.Name.UnqualifiedName);

            currentBlock = graph.NewBlock();
            foreach (var statement in callable.Body.Statements)
                Convert(statement);

            // Generate the implicit return statement
            if (currentBlock != null && !currentBlock.IsTerminated)
            {
                var span = callable.Span.AtEnd();
                //EndScope(span);
                currentBlock.End(new ReturnVoidInstruction(span, Scope.Outer));
            }

            return graph.Build();
        }


        private void Convert(IStatementSyntax statement)
        {
            switch (statement)
            {
                default:
                    throw ExhaustiveMatch.Failed(statement);
                case IVariableDeclarationStatementSyntax variableDeclaration:
                {
                    var variable = graph.AddVariable(variableDeclaration.IsMutableBinding,
                        variableDeclaration.Type.Assigned(),
                        CurrentScope, variableDeclaration.Name.UnqualifiedName);
                    if (variableDeclaration.Initializer != null)
                    {
                        ConvertIntoPlace(variableDeclaration.Initializer,
                           variable.Place(variableDeclaration.Initializer.Span));
                    }
                }
                break;
                case IExpressionStatementSyntax expressionStatement:
                {
                    var expression = expressionStatement.Expression;
                    if (!expression.Type.Assigned().IsKnown)
                        throw new ArgumentException("Expression must have a known type", nameof(statement));

                    Convert(expression);
                }
                break;
                case IResultStatementSyntax resultStatement:
                {
                    var expression = resultStatement.Expression;
                    if (!expression.Type.Assigned().IsKnown)
                        throw new ArgumentException("Expression must have a known type", nameof(statement));

                    Convert(expression);
                }
                break;
            }
        }

        /// <summary>
        /// Convert without expecting any result value
        /// </summary>
        private void Convert(IBlockOrResultSyntax blockOrResult)
        {
            switch (blockOrResult)
            {
                default:
                    throw ExhaustiveMatch.Failed(blockOrResult);
                case IBlockExpressionSyntax exp:
                    Convert((IExpressionSyntax)exp);
                    break;
                case IResultStatementSyntax statement:
                    Convert((IStatementSyntax)statement);
                    break;
            }
        }

        private void Convert(IElseClauseSyntax elseClause)
        {
            switch (elseClause)
            {
                default:
                    throw ExhaustiveMatch.Failed(elseClause);
                case IBlockOrResultSyntax blockOrResult:
                    Convert(blockOrResult);
                    break;
                case IIfExpressionSyntax exp:
                    Convert((IExpressionSyntax)exp);
                    break;
            }
        }

        /// <summary>
        /// Convert an expression without expecting any result value
        /// </summary>
        private void Convert(IExpressionSyntax expression)
        {
            switch (expression)
            {
                default:
                    throw ExhaustiveMatch.Failed(expression);
                case IBorrowExpressionSyntax _:
                case IShareExpressionSyntax _:
                case IMoveExpressionSyntax _:
                case INewObjectExpressionSyntax _:
                case IImplicitNoneConversionExpression _:
                case IImplicitConversionExpression _:
                case IFieldAccessExpressionSyntax _:
                case IForeachExpressionSyntax _:
                    throw new NotImplementedException($"Convert({expression.GetType().Name}) Not Implemented.");
                case ILoopExpressionSyntax exp:
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
                case IWhileExpressionSyntax whileExpression:
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
                //case IForeachExpressionSyntax exp:
                //{
                //    // For now, we support only range syntax `foreach x: T in z..y` ranges
                //    // aren't yet supported by the rest of the language, so they must be directly
                //    // translated here. The for each loop is basically desugared into:
                //    // var x: T = z;
                //    // let temp = y;
                //    // loop
                //    // {
                //    //     <loop body>
                //    //     x += 1;
                //    //     if x > temp => break;
                //    // }
                //    if (!(exp.InExpression is IBinaryOperatorExpressionSyntax inExpression)
                //        || (inExpression.Operator != BinaryOperator.DotDot
                //            && inExpression.Operator != BinaryOperator.LessThanDotDot
                //            && inExpression.Operator != BinaryOperator.DotDotLessThan
                //            && inExpression.Operator != BinaryOperator.LessThanDotDotLessThan))
                //        throw new NotImplementedException(
                //            "`foreach` in non-range expression not implemented");
                //    var startExpression = inExpression.LeftOperand;
                //    var endExpression = inExpression.RightOperand;

                //    var variableType = (IntegerType)exp.VariableType.Assigned();
                //    var loopVariable = graph.AddVariable(exp.IsMutableBinding,
                //        variableType, CurrentScope, exp.VariableName);
                //    var loopVariableLValue = loopVariable.LValueReference(exp.Span);
                //    var loopVariableReference = loopVariable.Reference(exp.Span);

                //    var includeStart = inExpression.Operator == BinaryOperator.DotDot
                //                       || inExpression.Operator == BinaryOperator.DotDotLessThan;
                //    var includeEnd = inExpression.Operator == BinaryOperator.DotDot
                //                       || inExpression.Operator == BinaryOperator.LessThanDotDot;

                //    // emit var x: T = z (+1);
                //    var startValue = ConvertToValue(startExpression);
                //    var one = new IntegerConstant(1, variableType, exp.Span);
                //    if (!includeStart)
                //    {
                //        var operand = ConvertToOperand(startValue, variableType);
                //        startValue = new BinaryOperation(operand, BinaryOperator.Plus, one,
                //            variableType);
                //    }
                //    AssignToPlace(loopVariableLValue, startValue, startExpression.Span);
                //    // let temp = y;
                //    var endValue = ConvertToOperand(endExpression);
                //    // Emit block
                //    var loopEntry = graph.NewEntryBlock(currentBlock,
                //        exp.Block.Span.AtStart(), CurrentScope);
                //    currentBlock = loopEntry;
                //    var conditionBlock = continueToBlock = graph.NewBlock();
                //    // TODO this generates the exit block too soon if the break condition is non-trivial
                //    var loopExit = ConvertLoopBody(exp.Block);
                //    // If it always breaks, there isn't a current block
                //    currentBlock?.End(new GotoInstruction(conditionBlock.Number,
                //        exp.Block.Span.AtEnd(), CurrentScope));
                //    currentBlock = conditionBlock;
                //    // emit x += 1;
                //    var valuePlusOne = new BinaryOperation(loopVariableReference,
                //        BinaryOperator.Plus, one, variableType);
                //    AssignToPlace(loopVariableLValue, valuePlusOne, startExpression.Span);
                //    // emit if x (>)|(>=) temp => break;
                //    var breakOperator = includeEnd ? BinaryOperator.GreaterThan : BinaryOperator.GreaterThanOrEqual;
                //    var breakCondition = new BinaryOperation(loopVariableReference,
                //                            breakOperator, endValue, variableType);
                //    var condition = ConvertToOperand(breakCondition);
                //    currentBlock.End(new IfInstruction(condition, loopExit.Number, loopEntry.Number,
                //            exp.Span, CurrentScope));
                //    currentBlock = loopExit;
                //    return;
                //}
                case IBreakExpressionSyntax exp:
                {
                    // TODO Do we need `ExitScope(exp.Span.AtEnd());` ?
                    // capture the current block for use in the lambda
                    var breakingBlock = currentBlock!;
                    addBreaks.Add(loopExit => breakingBlock.End(new GotoInstruction(loopExit.Number, exp.Span, CurrentScope)));
                    currentBlock = null;
                }
                break;
                case INextExpressionSyntax exp:
                {
                    // TODO Do we need `ExitScope(nextExpression.Span.AtEnd());` ?
                    currentBlock!.End(new GotoInstruction(continueToBlock?.Number ?? throw new InvalidOperationException(),
                        exp.Span, CurrentScope));
                    currentBlock = null;
                }
                break;
                case IIfExpressionSyntax exp:
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
                case IMethodInvocationExpressionSyntax exp:
                {
                    var methodName = exp.MethodNameSyntax.ReferencedSymbol!.FullName;
                    var target = ConvertToOperand(exp.ContextExpression);
                    var args = exp.Arguments.Select(a => ConvertToOperand(a.Expression)).ToFixedList();
                    currentBlock!.Add(new CallVirtualInstruction(target, methodName, args, exp.Span, CurrentScope));
                }
                break;
                case IAssignmentExpressionSyntax exp:
                {
                    var leftOperand = exp.LeftOperand;
                    var rightOperand = exp.RightOperand;
                    var assignInto = ConvertToPlaceWithoutSideEffects(leftOperand);
                    if (assignInto is null)
                    {
                        var tempVar = graph.Let(leftOperand.Type.Assigned().Known(), CurrentScope);
                        var tempSpan = rightOperand.Span.AtStart();
                        ConvertIntoPlace(rightOperand, tempVar.Place(tempSpan));
                        assignInto = ConvertToPlace(leftOperand);
                        Operand operand = tempVar.Move(tempSpan);
                        currentBlock!.Add(new AssignmentInstruction(assignInto, operand, exp.Span, CurrentScope));
                    }
                    else
                        ConvertIntoPlace(rightOperand, assignInto);
                }
                break;
                case IUnsafeExpressionSyntax exp:
                    Convert(exp.Expression);
                    break;
                case IBlockExpressionSyntax exp:
                    foreach (var statement in exp.Statements)
                        Convert(statement);
                    break;
                case IFunctionInvocationExpressionSyntax exp:
                {
                    var functionName = exp.FunctionNameSyntax.ReferencedSymbol!.FullName;
                    var args = exp.Arguments.Select(a => ConvertToOperand(a.Expression)).ToFixedList();
                    currentBlock!.Add(CallInstruction.ForFunction(functionName, args, exp.Span, CurrentScope));
                }
                break;
                case IReturnExpressionSyntax exp:
                {
                    if (exp.ReturnValue is null)
                        currentBlock!.End(new ReturnVoidInstruction(exp.Span, CurrentScope));
                    else
                    {
                        var returnValue = ConvertToOperand(exp.ReturnValue);
                        currentBlock!.End(new ReturnValueInstruction(returnValue, exp.Span, CurrentScope));
                    }

                    // There is no exit from a return block, hence null for exit block
                    currentBlock = null;
                }
                break;
                case INameExpressionSyntax _:
                case IBinaryOperatorExpressionSyntax _:
                case IUnaryOperatorExpressionSyntax _:
                case IBoolLiteralExpressionSyntax _:
                case IStringLiteralExpressionSyntax _:
                case ISelfExpressionSyntax _:
                case INoneLiteralExpressionSyntax _:
                case IIntegerLiteralExpressionSyntax _:
                    // These operation have no side effects, so if the result isn't needed, there is nothing to do
                    break;

            }
        }

        /// <summary>
        /// Convert the body of a loop. Ensures break statements are handled correctly.
        /// </summary>
        private BlockBuilder? ConvertLoopBody(IBlockExpressionSyntax body, bool exitRequired = true)
        {
            var oldAddBreaks = addBreaks;
            addBreaks = new List<Action<BlockBuilder>>();
            Convert((IExpressionSyntax)body);
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
        private void ConvertIntoPlace(IExpressionSyntax expression, Place resultPlace)
        {
            switch (expression)
            {
                default:
                    throw ExhaustiveMatch.Failed(expression);
                case ILoopExpressionSyntax _:
                case IWhileExpressionSyntax _:
                case IForeachExpressionSyntax _:
                case IMoveExpressionSyntax _:
                case IBorrowExpressionSyntax _:
                case IReturnExpressionSyntax _:
                case IBreakExpressionSyntax _:
                case INextExpressionSyntax _:
                case ISelfExpressionSyntax _:
                case IIfExpressionSyntax _:
                case IUnsafeExpressionSyntax _:
                case IImplicitImmutabilityConversionExpression _:
                case IImplicitOptionalConversionExpression _:
                case IBlockExpressionSyntax _:
                case INoneLiteralExpressionSyntax _:
                    throw new NotImplementedException($"ConvertIntoPlace({expression.GetType().Name}, Place) Not Implemented.");
                case IAssignmentExpressionSyntax exp:
                {
                    var leftOperand = exp.LeftOperand;
                    var rightOperand = exp.RightOperand;
                    var assignInto = ConvertToPlaceWithoutSideEffects(leftOperand);
                    if (assignInto is null)
                    {
                        var tempVar = graph.Let(leftOperand.Type.Assigned().Known(), CurrentScope);
                        var tempSpan = rightOperand.Span.AtStart();
                        ConvertIntoPlace(rightOperand, tempVar.Place(tempSpan));
                        assignInto = ConvertToPlace(leftOperand);
                        Operand operand = tempVar.Move(tempSpan);
                        currentBlock!.Add(new AssignmentInstruction(assignInto, operand, exp.Span, CurrentScope));
                    }
                    else
                        ConvertIntoPlace(rightOperand, assignInto);
                }
                break;
                case INameExpressionSyntax exp:
                {
                    // This occurs when the source code contains a simple assignment like `x = y`
                    var symbol = exp.ReferencedSymbol.Assigned();
                    var variable = graph.VariableFor(symbol.FullName.UnqualifiedName).Reference(exp.Span);
                    currentBlock!.Add(new AssignmentInstruction(resultPlace, variable, exp.Span, CurrentScope));
                }
                break;
                case IShareExpressionSyntax exp:
                    ConvertIntoPlace(exp.Referent, resultPlace);
                    break;
                case IBinaryOperatorExpressionSyntax exp:
                {
                    var resultType = exp.Type.Assigned().Known();
                    var operandType = exp.LeftOperand.Type.Assigned().Known();
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
                        case BinaryOperator.Or:
                        case BinaryOperator.And:
                            throw new NotImplementedException($"ConvertIntoPlace({expression.GetType().Name}, Place) Not Implemented for {exp.Operator}.");
                        case BinaryOperator.Plus:
                            currentBlock!.Add(new NumericInstruction(resultPlace, NumericInstructionOperator.Add,
                                (NumericType)resultType, leftOperand, rightOperand, CurrentScope));
                            break;
                        case BinaryOperator.Minus:
                            currentBlock!.Add(new NumericInstruction(resultPlace, NumericInstructionOperator.Subtract,
                                (NumericType)resultType, leftOperand, rightOperand, CurrentScope));
                            break;
                        case BinaryOperator.Asterisk:
                            currentBlock!.Add(new NumericInstruction(resultPlace, NumericInstructionOperator.Multiply,
                                (NumericType)resultType, leftOperand, rightOperand, CurrentScope));
                            break;
                        case BinaryOperator.Slash:
                            currentBlock!.Add(new NumericInstruction(resultPlace, NumericInstructionOperator.Divide,
                                (NumericType)resultType, leftOperand, rightOperand, CurrentScope));
                            break;
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
                case IUnaryOperatorExpressionSyntax exp:
                {
                    var type = exp.Type.Assigned().Known();
                    var operand = ConvertToOperand(exp.Operand);
                    switch (exp.Operator)
                    {
                        default:
                            throw ExhaustiveMatch.Failed(expression);
                        case UnaryOperator.Not:
                        case UnaryOperator.Plus:
                            throw new NotImplementedException($"ConvertToOperand({expression.GetType().Name}, Place) Not Implemented for {exp.Operator}.");
                        case UnaryOperator.Minus:
                            currentBlock!.Add(new NegateInstruction(resultPlace, (NumericType)type, operand, exp.Span, CurrentScope));
                            break;
                    }
                }
                break;
                case IFieldAccessExpressionSyntax exp:
                {
                    var context = ConvertToOperand(exp.ContextExpression);
                    currentBlock!.Add(new FieldAccessInstruction(resultPlace, context, exp.Field.Name, exp.Span, CurrentScope));
                }
                break;
                case IFunctionInvocationExpressionSyntax exp:
                {
                    var functionName = exp.FunctionNameSyntax.ReferencedSymbol!.FullName;
                    var args = exp.Arguments.Select(a => ConvertToOperand(a.Expression)).ToFixedList();
                    currentBlock!.Add(CallInstruction.ForFunction(resultPlace, functionName, args, exp.Span, CurrentScope));
                }
                break;
                case IMethodInvocationExpressionSyntax exp:
                {
                    var methodName = exp.MethodNameSyntax.ReferencedSymbol!.FullName;
                    var target = ConvertToOperand(exp.ContextExpression);
                    var args = exp.Arguments.Select(a => ConvertToOperand(a.Expression)).ToFixedList();
                    if (exp.ContextExpression.Type is ReferenceType)
                        currentBlock!.Add(new CallVirtualInstruction(resultPlace, target, methodName, args, exp.Span, CurrentScope));
                    else
                        currentBlock!.Add(CallInstruction.ForMethod(resultPlace, target, methodName, args, exp.Span, CurrentScope));
                }
                break;
                case INewObjectExpressionSyntax exp:
                {
                    var constructorName = exp.ConstructorSymbol!.FullName;
                    var args = exp.Arguments.Select(a => ConvertToOperand(a.Expression)).ToFixedList();
                    var constructedType = (UserObjectType)exp.TypeSyntax.NamedType.Assigned().Known();
                    currentBlock!.Add(new NewObjectInstruction(resultPlace, constructorName, constructedType, args, exp.Span, CurrentScope));
                }
                break;
                case IStringLiteralExpressionSyntax exp:
                    currentBlock!.Add(new LoadStringInstruction(resultPlace, exp.Value, exp.Span, CurrentScope));
                    break;
                case IBoolLiteralExpressionSyntax exp:
                    currentBlock!.Add(new LoadBoolInstruction(resultPlace, exp.Value, exp.Span, CurrentScope));
                    break;
                case IImplicitNumericConversionExpression exp:
                {
                    if (exp.Expression.Type.Assigned().Known() is IntegerConstantType constantType)
                        currentBlock!.Add(new LoadIntegerInstruction(resultPlace, constantType.Value,
                            (IntegerType)exp.Type.Assigned().Known(),
                            exp.Span, CurrentScope));
                    else
                        currentBlock!.Add(new ConvertInstruction(resultPlace, ConvertToOperand(exp.Expression),
                            (NumericType)exp.Expression.Type.Assigned().Known(), exp.ConvertToType,
                            exp.Span, CurrentScope));
                }
                break;
                case IIntegerLiteralExpressionSyntax exp:
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
        private Operand ConvertToOperand(IExpressionSyntax expression)
        {
            switch (expression)
            {
                default:
                    throw ExhaustiveMatch.Failed(expression);
                //throw new NotImplementedException($"ConvertToOperand({expression.GetType().Name}) Not Implemented.");
                case ISelfExpressionSyntax exp:
                    return graph.SelfVariable.Reference(exp.Span);
                case INameExpressionSyntax exp:
                {
                    var symbol = exp.ReferencedSymbol.Assigned();
                    return graph.VariableFor(symbol.FullName.UnqualifiedName).Reference(exp.Span);
                }
                case IAssignmentExpressionSyntax _:
                case IBinaryOperatorExpressionSyntax _:
                case IUnaryOperatorExpressionSyntax _:
                case IFieldAccessExpressionSyntax _:
                case IMethodInvocationExpressionSyntax _:
                case INewObjectExpressionSyntax _:
                case IImplicitNumericConversionExpression _:
                case IIntegerLiteralExpressionSyntax _:
                case IStringLiteralExpressionSyntax _:
                case IBoolLiteralExpressionSyntax _:
                case INoneLiteralExpressionSyntax _:
                case IImplicitImmutabilityConversionExpression _:
                case IImplicitOptionalConversionExpression _:
                case IUnsafeExpressionSyntax _:
                case IBlockExpressionSyntax _:
                case IImplicitNoneConversionExpression _:
                case IBreakExpressionSyntax _:
                case INextExpressionSyntax _:
                case IReturnExpressionSyntax _:
                case IMoveExpressionSyntax _:
                case IBorrowExpressionSyntax _:
                case IShareExpressionSyntax _:
                case IIfExpressionSyntax _:
                case IFunctionInvocationExpressionSyntax _:
                case IForeachExpressionSyntax _:
                case ILoopExpressionSyntax _:
                case IWhileExpressionSyntax _:
                {
                    var tempVar = graph.Let(expression.Type.Assigned().Known(), CurrentScope);
                    ConvertIntoPlace(expression, tempVar.Place(expression.Span));
                    return tempVar.Reference(expression.Span);
                }
            }
        }

        private Place ConvertToPlace(IExpressionSyntax expression)
        {
            throw new NotImplementedException($"ConvertToPlace({expression.GetType().Name}) Not Implemented.");
        }

        /// <summary>
        /// What is this function supposed to do?
        /// </summary>
        private Place? ConvertToPlaceWithoutSideEffects(IExpressionSyntax expression)
        {
            switch (expression)
            {
                default:
                    //throw ExhaustiveMatch.Failed(expression);
                    throw new NotImplementedException($"ConvertToPlaceWithoutSideEffects({expression.GetType().Name}) Not Implemented.");
                case IFieldAccessExpressionSyntax exp:
                {
                    var context = ConvertToOperand(exp.ContextExpression);
                    return new FieldPlace(context, exp.Field.Name, exp.Span);
                }
                case ISelfExpressionSyntax exp:
                    return new VariablePlace(Variable.Self, exp.Span);
                case INameExpressionSyntax exp:
                {
                    var symbol = exp.ReferencedSymbol.Assigned();
                    return graph.VariableFor(symbol.FullName.UnqualifiedName).Place(exp.Span);
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

        ///// <summary>
        ///// Assign a value into a place while making sure to correctly handle
        ///// value semantics.
        ///// </summary>
        //private void AssignToPlace(IPlace place, Value value, TextSpan span)
        //{
        //    if (value is VariableReference assignFrom
        //        && assignFrom.ValueSemantics == ValueSemantics.Own
        //        && place is VariableReference assignTo)
        //    {
        //        // There is a chance we are assigning into something that doesn't
        //        // accept ownership. In that case, we demote to a borrow or alias
        //        var variableSemantics = graph[assignTo.Variable].Type.ValueSemantics;
        //        switch (variableSemantics)
        //        {
        //            default:
        //                throw ExhaustiveMatch.Failed(variableSemantics);
        //            case ValueSemantics.Own:
        //                // They are both own, no problems
        //                break;
        //            case ValueSemantics.Alias:
        //                value = assignFrom.AsAlias();
        //                break;
        //            case ValueSemantics.Borrow:
        //                value = assignFrom.AsBorrow();
        //                break;
        //            case ValueSemantics.LValue:
        //            case ValueSemantics.Move:
        //            case ValueSemantics.Empty:
        //            case ValueSemantics.Copy:
        //                throw new NotImplementedException();
        //        }
        //    }

        //    currentBlock!.AddAssignment(place, value, span, CurrentScope);
        //}

        //private VariableReference AssignToTemp(DataType type, IValue value)
        //{
        //    var tempVariable = graph.Let(type.AssertKnown(), CurrentScope);
        //    currentBlock!.AddAssignment(tempVariable.LValueReference(value.Span), value, value.Span,
        //        CurrentScope);
        //    return tempVariable.Reference(value.Span);
        //}

        //private void ConvertToStatement(IStatementSyntax statement)
        //{
        //    switch (statement)
        //    {
        //        default:
        //            throw ExhaustiveMatch.Failed(statement);
        //        case IVariableDeclarationStatementSyntax variableDeclaration:
        //        {
        //            var variable = graph.AddVariable(variableDeclaration.IsMutableBinding,
        //                variableDeclaration.Type.Assigned(),
        //                CurrentScope, variableDeclaration.Name.UnqualifiedName);
        //            if (variableDeclaration.Initializer != null)
        //            {
        //                var value = ConvertToValue(variableDeclaration.Initializer);
        //                AssignToPlace(
        //                    variable.LValueReference(variableDeclaration.Initializer.Span), value,
        //                    variableDeclaration.Initializer.Span);
        //            }

        //            return;
        //        }
        //        case IExpressionStatementSyntax expressionStatement:
        //        {
        //            // Skip expressions with unknown type
        //            var expression = expressionStatement.Expression;
        //            if (!expression.Type.Assigned().IsKnown)
        //                return;

        //            if (expression.Type is EmptyType)
        //                ConvertExpressionToStatement(expression);
        //            else
        //                AssignToTemp(expression.Type.Assigned(), ConvertToValue(expression));

        //            return;
        //        }
        //        case IResultStatementSyntax _:
        //            throw new NotImplementedException();
        //    }
        //}



        //// TODO combine ConvertExpressionToStatement and ConvertToValue

        ///// <summary>
        ///// Converts an expression of type `void` or `never` to a statement
        ///// </summary>
        //private void ConvertExpressionToStatement(IExpressionSyntax expression)
        //{
        //    switch (expression)
        //    {
        //        default:
        //            throw NonExhaustiveMatchException.For(expression);
        //        case IUnaryOperatorExpressionSyntax _:
        //        case IBinaryOperatorExpressionSyntax _:
        //            throw new NotImplementedException();
        //        case IInvocationExpressionSyntax invocation:
        //            currentBlock.AddAction(ConvertInvocationToValue(invocation), invocation.Span,
        //                CurrentScope);
        //            return;
        //        case IReturnExpressionSyntax returnExpression:
        //        {
        //            if (returnExpression.ReturnValue != null)
        //            {
        //                var isOwn = returnType.ValueSemantics == ValueSemantics.Own;
        //                var value = isOwn
        //                    ? ConvertToOwn(returnExpression.ReturnValue, returnExpression.Span)
        //                    // TODO avoid getting a move from this just because it is a new object expression
        //                    : ConvertToValue(returnExpression.ReturnValue);
        //                AssignToPlace(
        //                    graph.ReturnVariable.LValueReference(returnExpression.ReturnValue.Span),
        //                    value, returnExpression.ReturnValue.Span);
        //            }

        //            ExitScope(returnExpression.Span.AtEnd());
        //            currentBlock.AddReturn(returnExpression.Span, CurrentScope);

        //            // There is no exit from a return block, hence null for exit block
        //            currentBlock = null;
        //            return;
        //        }

        //        case IBlockExpressionSyntax block:
        //        {
        //            // Starting a new nested scope
        //            EnterNewScope();

        //            foreach (var statementInBlock in block.Statements)
        //                ConvertToStatement(statementInBlock);

        //            // Ending that scope
        //            EndScope(block.Span.AtEnd());
        //            return;
        //        }
        //        case IUnsafeExpressionSyntax unsafeExpression:
        //            ConvertExpressionToStatement(unsafeExpression.Expression);
        //            return;
        //        case IAssignmentExpressionSyntax assignmentExpression:
        //        {
        //            var value = ConvertToValue(assignmentExpression.RightOperand);
        //            var place = ConvertToPlace(assignmentExpression.LeftOperand);

        //            if (assignmentExpression.Operator != AssignmentOperator.Simple)
        //            {
        //                var type = (SimpleType)assignmentExpression.RightOperand.Type.Assigned();
        //                var rightOperand = ConvertToOperand(value, type);
        //                BinaryOperator binaryOperator;
        //                switch (assignmentExpression.Operator)
        //                {
        //                    case AssignmentOperator.Simple:
        //                        throw new UnreachableCodeException("Case excluded by if statement");
        //                    case AssignmentOperator.Plus:
        //                        binaryOperator = BinaryOperator.Plus;
        //                        break;
        //                    case AssignmentOperator.Minus:
        //                        binaryOperator = BinaryOperator.Minus;
        //                        break;
        //                    case AssignmentOperator.Asterisk:
        //                        binaryOperator = BinaryOperator.Asterisk;
        //                        break;
        //                    case AssignmentOperator.Slash:
        //                        binaryOperator = BinaryOperator.Slash;
        //                        break;
        //                    default:
        //                        throw ExhaustiveMatch.Failed(assignmentExpression.Operator);
        //                }

        //                value = new BinaryOperation(
        //                    ConvertToOperand(place, assignmentExpression.LeftOperand.Type.Assigned()),
        //                    binaryOperator, rightOperand, type);
        //            }

        //            AssignToPlace(place, value, assignmentExpression.Span);
        //            return;
        //        }
        //        //case ResultStatementSyntax resultExpression:
        //        //    // Must be an expression of type `never`
        //        //    ConvertExpressionToStatement(resultExpression.Expression);
        //        //    ExitScope(resultExpression.Span.AtEnd());
        //        //    return;
        //    }
        //}

        //private Value ConvertToValue(IExpressionSyntax expression)
        //{
        //    switch (expression)
        //    {
        //        default:
        //            throw NonExhaustiveMatchException.For(expression);
        //        case INewObjectExpressionSyntax newObjectExpression:
        //        {
        //            var args = newObjectExpression.Arguments.Select(a => ConvertToOperand(a.Expression))
        //                .ToFixedList();
        //            var type = (UserObjectType)newObjectExpression.Type;
        //            // lifetime is implicitly owned since we are making a new one
        //            type = type.WithLifetime(Lifetime.None);
        //            return new ConstructorCall(type, args, newObjectExpression.Span);
        //        }
        //        case INameExpressionSyntax identifier:
        //        {
        //            var symbol = identifier.ReferencedSymbol;
        //            switch (symbol)
        //            {
        //                case IVariableDeclarationStatementSyntax _:
        //                case IParameterSyntax _:
        //                case IForeachExpressionSyntax _:
        //                    return graph.VariableFor(symbol.FullName.UnqualifiedName)
        //                        .Reference(identifier.Span);
        //                default:
        //                    return new DeclaredValue(symbol.FullName, identifier.Span);
        //            }
        //        }
        //        case IUnaryOperatorExpressionSyntax unaryExpression:
        //            return ConvertUnaryExpressionToValue(unaryExpression);
        //        case IBinaryOperatorExpressionSyntax binaryExpression:
        //            return ConvertBinaryExpressionToValue(binaryExpression);
        //        case IIntegerLiteralExpressionSyntax _:
        //            throw new InvalidOperationException(
        //                "Integer literals should have an implicit conversion around them");
        //        case IStringLiteralExpressionSyntax stringLiteral:
        //            return new StringConstant(stringLiteral.Value, stringLiteral.Span, stringLiteral.Type.Assigned().AssertKnown());
        //        case IBoolLiteralExpressionSyntax boolLiteral:
        //            return new BooleanConstant(boolLiteral.Value, boolLiteral.Span);
        //        case INoneLiteralExpressionSyntax _:
        //            throw new InvalidOperationException(
        //                "None literals should have an implicit conversion around them");
        //        case IImplicitNumericConversionExpression implicitNumericConversion:
        //            if (implicitNumericConversion.Expression.Type.Assigned().AssertKnown() is
        //                IntegerConstantType constantType)
        //                return new IntegerConstant(constantType.Value,
        //                    (IntegerType)implicitNumericConversion.Type.AssertKnown(),
        //                    implicitNumericConversion.Span);
        //            else
        //            {
        //                var valueOperand = ConvertToOperand(implicitNumericConversion.Expression);
        //                return new Conversion(
        //                    valueOperand,
        //                    (NumericType)implicitNumericConversion.Expression.Type.Assigned(),
        //                    (NumericType)implicitNumericConversion.Type.Assigned(),
        //                    implicitNumericConversion.Span);
        //            }
        //        case IImplicitOptionalConversionExpression implicitOptionalConversionExpression:
        //        {
        //            var value = ConvertToOperand(implicitOptionalConversionExpression.Expression);
        //            return new ConstructSome(implicitOptionalConversionExpression.ConvertToType,
        //                value, implicitOptionalConversionExpression.Span);
        //        }
        //        case IIfExpressionSyntax ifExpression:
        //            // TODO deal with the value of the if expression
        //            throw new NotImplementedException();
        //        case IUnsafeExpressionSyntax unsafeExpression:
        //            return ConvertToValue(unsafeExpression.Expression);
        //        case IInvocationExpressionSyntax invocation:
        //            return ConvertInvocationToValue(invocation);
        //        case IMemberAccessExpressionSyntax memberAccess:
        //        {
        //            var value = ConvertToPlace(memberAccess.Expression);
        //            var symbol = memberAccess.ReferencedSymbol;
        //            return new FieldAccess(value, symbol.FullName, ValueSemantics.LValue, memberAccess.Span);
        //        }
        //        case IMutableExpressionSyntax mutableExpression:
        //            // TODO shouldn't borrowing be explicit in the IR and don't we
        //            // need to be able to check mutability on borrows?
        //            return ConvertToValue(mutableExpression.Referent);
        //        case IMoveExpressionSyntax move:
        //            return ConvertToOwn(move.Referent, move.Span);
        //        case IImplicitImmutabilityConversionExpression implicitImmutabilityConversion:
        //        {
        //            var operand = ConvertToOperand(implicitImmutabilityConversion.Expression);
        //            switch (operand)
        //            {
        //                default:
        //                    throw NonExhaustiveMatchException.For(expression);
        //                //case BooleanConstant _:
        //                //case Utf8BytesConstant _:
        //                //case IntegerConstant _:
        //                //    return operand;
        //                case VariableReference varReference:
        //                    if (implicitImmutabilityConversion.Type.ValueSemantics
        //                        == ValueSemantics.Own)
        //                        return varReference.AsOwn(implicitImmutabilityConversion.Span);
        //                    else
        //                        return varReference.AsAlias();
        //            }
        //        }
        //        case ISelfExpressionSyntax selfExpression:
        //            return graph.VariableFor(SpecialName.Self).Reference(selfExpression.Span);
        //    }
        //}

        //private Value ConvertToOwn(IExpressionSyntax expression, TextSpan moveSpan)
        //{
        //    var operand = ConvertToOperand(expression);
        //    switch (operand)
        //    {
        //        case VariableReference variableReference:
        //            return variableReference.AsOwn(moveSpan);
        //        default:
        //            throw new NotImplementedException();
        //    }
        //}

        //private IOperand ConvertToOperand(IExpressionSyntax expression)
        //{
        //    var value = ConvertToValue(expression);
        //    return ConvertToOperand(value, expression.Type.Assigned());
        //}

        //private IOperand ConvertToOperand(IValue value, DataType type)
        //{
        //    if (value is IOperand operand)
        //        return operand;
        //    return AssignToTemp(type, value);
        //}

        //private Value ConvertBinaryExpressionToValue(IBinaryOperatorExpressionSyntax operatorExpression)
        //{
        //    switch (operatorExpression.Operator)
        //    {
        //        case BinaryOperator.Plus:
        //        case BinaryOperator.Minus:
        //        case BinaryOperator.Asterisk:
        //        case BinaryOperator.Slash:
        //        case BinaryOperator.EqualsEquals:
        //        case BinaryOperator.NotEqual:
        //        case BinaryOperator.LessThan:
        //        case BinaryOperator.LessThanOrEqual:
        //        case BinaryOperator.GreaterThan:
        //        case BinaryOperator.GreaterThanOrEqual:
        //        {
        //            // TODO handle calls to overloaded operators
        //            var leftOperand = ConvertToOperand(operatorExpression.LeftOperand);
        //            var rightOperand = ConvertToOperand(operatorExpression.RightOperand);
        //            switch (operatorExpression.LeftOperand.Type)
        //            {
        //                case SimpleType operandType:
        //                {
        //                    // What matters is the type we are operating on, for comparisons, that is different than the result type which is bool
        //                    return new BinaryOperation(leftOperand, operatorExpression.Operator,
        //                        rightOperand, operandType);
        //                }
        //                case UserObjectType _:
        //                {
        //                    if (operatorExpression.Operator != BinaryOperator.EqualsEquals)
        //                        throw new NotImplementedException();
        //                    //var equalityOperators = operandType.Symbol.Lookup(SpecialName.OperatorEquals);
        //                    //if (equalityOperators.Count == 1)
        //                    //{
        //                    //    var equalityOperator = equalityOperators.Single();
        //                    //    return new FunctionCall(equalityOperator.FullName,
        //                    //                //(FunctionType)equalityOperator.Type,
        //                    //                new[] { leftOperand, rightOperand },
        //                    //                expression.Span);
        //                    //}
        //                    throw new NotImplementedException();
        //                }
        //                default:
        //                    throw NonExhaustiveMatchException.For(operatorExpression.LeftOperand.Type);
        //            }
        //        }
        //        case BinaryOperator.And:
        //        case BinaryOperator.Or:
        //        {
        //            // TODO handle calls to overloaded operators
        //            // TODO handle short circuiting if needed
        //            var leftOperand = ConvertToOperand(operatorExpression.LeftOperand);
        //            var rightOperand = ConvertToOperand(operatorExpression.RightOperand);
        //            return new BinaryOperation(leftOperand, operatorExpression.Operator, rightOperand,
        //                (SimpleType)operatorExpression.Type);
        //        }
        //        case BinaryOperator.DotDot:
        //        case BinaryOperator.LessThanDotDot:
        //        case BinaryOperator.DotDotLessThan:
        //        case BinaryOperator.LessThanDotDotLessThan:
        //            throw new NotImplementedException("Conversion of range for binary operators");
        //        default:
        //            throw ExhaustiveMatch.Failed(operatorExpression.Operator);
        //    }
        //}

        //private Value ConvertUnaryExpressionToValue(IUnaryOperatorExpressionSyntax operatorExpression)
        //{
        //    switch (operatorExpression.Operator)
        //    {
        //        case UnaryOperator.Not:
        //        case UnaryOperator.Minus:
        //            var operand = ConvertToOperand(operatorExpression.Operand);
        //            return new UnaryOperation(operatorExpression.Operator, operand, operatorExpression.Span);
        //        case UnaryOperator.Plus:
        //            // This is a no-op
        //            return ConvertToValue(operatorExpression.Operand);
        //        default:
        //            throw ExhaustiveMatch.Failed(operatorExpression.Operator);
        //    }
        //}

        //private Value ConvertInvocationToValue(IInvocationExpressionSyntax invocationExpression)
        //{
        //    switch (invocationExpression)
        //    {
        //        default:
        //            throw ExhaustiveMatch.Failed(invocationExpression);
        //        case IMethodInvocationExpressionSyntax methodInvocation:
        //            return ConvertInvocationToValue(methodInvocation);
        //        case IFunctionInvocationExpressionSyntax functionInvocation:
        //            return ConvertInvocationToValue(functionInvocation);
        //    }
        //}

        //private Value ConvertInvocationToValue(IMethodInvocationExpressionSyntax invocationExpression)
        //{
        //    var self = ConvertToOperand(invocationExpression.Target);
        //    var arguments = invocationExpression.Arguments.Select(a => ConvertToOperand(a.Expression)).ToList();
        //    var symbol = invocationExpression.MethodNameSyntax.ReferencedSymbol;
        //    switch (invocationExpression.Target.Type)
        //    {
        //        case SimpleType _:
        //            // Full name because this isn't a member
        //            return new FunctionCall(symbol.FullName,
        //                self, arguments, invocationExpression.Span);
        //        default:
        //            return new VirtualFunctionCall(invocationExpression.Span,
        //                symbol.FullName.UnqualifiedName, self, arguments);
        //    }
        //}

        //private Value ConvertInvocationToValue(IFunctionInvocationExpressionSyntax invocationExpression)
        //{
        //    var functionSymbol = invocationExpression.FunctionNameSyntax.ReferencedSymbol;
        //    var arguments = invocationExpression.Arguments.Select(a => ConvertToOperand(a.Expression)).ToList();
        //    return new FunctionCall(functionSymbol.FullName, arguments, invocationExpression.Span);
        //}

        //private IPlace ConvertToPlace(IExpressionSyntax value)
        //{
        //    switch (value)
        //    {
        //        default:
        //            // TODO maybe we need some kind if ILValueExpression interface
        //            throw NonExhaustiveMatchException.For(value);
        //        case ISelfExpressionSyntax selfExpression:
        //            return graph.VariableFor(SpecialName.Self).LValueReference(selfExpression.Span);
        //        case INameExpressionSyntax identifier:
        //            // TODO what if this isn't just a variable?
        //            return graph.VariableFor(identifier.ReferencedSymbol.FullName.UnqualifiedName)
        //                        .LValueReference(identifier.Span);
        //        case IMemberAccessExpressionSyntax memberAccessExpression:
        //            var expressionValue = ConvertToPlace(memberAccessExpression.Expression);
        //            return new FieldAccess(expressionValue, memberAccessExpression.Member.Name, ValueSemantics.LValue, memberAccessExpression.Span);
        //    }
        //}
    }
}
