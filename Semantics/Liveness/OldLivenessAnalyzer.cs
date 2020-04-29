namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Liveness
{
    ///// <summary>
    ///// Compute where variables are live and return a structure of that
    ///// </summary>
    //public static class LivenessAnalyzer
    //{
    //    public static FixedDictionary<ICallableDeclaration, LiveVariables> Check(
    //        FixedList<ICallableDeclaration> functions,
    //        bool saveLivenessAnalysis)
    //    {
    //        throw new NotImplementedException();
    //        //var analyses = new Dictionary<ICallableDeclaration, LiveVariables>();
    //        //foreach (var function in functions)
    //        //{
    //        //    var controlFlow = function.ControlFlowOld;
    //        //    if (controlFlow is null) continue;
    //        //    var liveness = ComputeLiveness(controlFlow);
    //        //    if (liveness != null)
    //        //    {
    //        //        analyses.Add(function, liveness);
    //        //        if (saveLivenessAnalysis)
    //        //            controlFlow.LiveVariables = liveness;
    //        //    }
    //        //}

    //        //return analyses.ToFixedDictionary();
    //    }

    //    /// <summary>
    //    /// Perform a backwards data flow analysis to determine where each variable is live or dead
    //    /// </summary>
    //    private static LiveVariables ComputeLiveness(ControlFlowGraphOld controlFlow)
    //    {
    //        var blocksQueue = new Queue<BasicBlock>();
    //        // Not just exit blocks because sometimes the exit blocks don't change vars and don't pull in other blocks
    //        blocksQueue.EnqueueRange(controlFlow.BasicBlocks);
    //        var liveVariables = new LiveVariables(controlFlow);
    //        var numberOfVariables = controlFlow.VariableDeclarations.Count;

    //        while (blocksQueue.TryDequeue(out var block))
    //        {
    //            var oldLiveBeforeBlock = new BitArray(liveVariables.Before(block.Statements[0]));

    //            var liveAfterBlock = new BitArray(numberOfVariables);
    //            foreach (var successor in controlFlow.Edges.From(block))
    //                liveAfterBlock.Or(liveVariables.Before(successor.Statements[0]));

    //            var liveAfterStatement = liveAfterBlock;

    //            foreach (var statement in block.Statements.Reverse())
    //            {
    //                var liveSet = liveVariables.Before(statement);
    //                liveSet.Or(liveAfterStatement);
    //                switch (statement)
    //                {
    //                    default:
    //                        throw ExhaustiveMatch.Failed(statement);
    //                    case AssignmentStatement assignment:
    //                        KillVariables(liveSet, assignment.Place);
    //                        EnlivenVariables(liveSet, assignment.Value);
    //                        break;
    //                    case ActionStatement action:
    //                        EnlivenVariables(liveSet, action.Value);
    //                        break;
    //                    case DeleteStatement delete:
    //                        liveSet[delete.Place.CoreVariable().Number] = true;
    //                        break;
    //                    case IfStatement _:
    //                    case GotoStatement _:
    //                        // We already or'ed together the live variables from successor blocks
    //                        break;
    //                    case ReturnStatement _:
    //                        // As a sanity check, no variables should be live after return
    //                        if (liveAfterStatement.Bits().AnyTrue())
    //                            throw new InvalidOperationException("Variables live after return statement");
    //                        // But the result variable might be live before it
    //                        if (controlFlow.ReturnVariable.TypeIsNotEmpty)
    //                            liveSet[Variable.Result.Number] = true;
    //                        break;
    //                    case ExitScopeStatement _:
    //                        // TODO use end scope statement to track liminal state (see  BorrowChecker)
    //                        break;
    //                }

    //                // For the next statement
    //                liveAfterStatement = liveSet;
    //            }

    //            if (!oldLiveBeforeBlock.ValuesEqual(liveVariables.Before(block.Statements[0])))
    //                foreach (var basicBlock in controlFlow.Edges.To(block)
    //                    .Where(fromBlock => !blocksQueue.Contains(fromBlock)).ToList())
    //                    blocksQueue.Enqueue(basicBlock);
    //        }
    //        return liveVariables;
    //    }

    //    private static void KillVariables(BitArray variables, IPlace place)
    //    {
    //        switch (place)
    //        {
    //            default:
    //                throw ExhaustiveMatch.Failed(place);
    //            case VariableReference variableReference:
    //                variables[variableReference.Variable.Number] = false;
    //                break;
    //            case FieldAccess fieldAccess:
    //                EnlivenVariables(variables, fieldAccess.Expression);
    //                break;
    //        }
    //    }
    //    private static void EnlivenVariables(BitArray variables, IValue value)
    //    {
    //        switch (value)
    //        {
    //            default:
    //                throw ExhaustiveMatch.Failed(value);
    //            case ConstructorCall constructorCall:
    //                foreach (var argument in constructorCall.Arguments)
    //                    EnlivenVariables(variables, argument);
    //                break;
    //            case UnaryOperation unaryOperation:
    //                EnlivenVariables(variables, unaryOperation.Operand);
    //                break;
    //            case BinaryOperation binaryOperation:
    //                EnlivenVariables(variables, binaryOperation.LeftOperand);
    //                EnlivenVariables(variables, binaryOperation.RightOperand);
    //                break;
    //            case IPlace place:
    //                variables[place.CoreVariable().Number] = true;
    //                break;
    //            case FunctionCall functionCall:
    //                if (functionCall.Self != null)
    //                    EnlivenVariables(variables, functionCall.Self);
    //                foreach (var argument in functionCall.Arguments)
    //                    EnlivenVariables(variables, argument);
    //                break;
    //            case VirtualFunctionCall virtualFunctionCall:
    //                EnlivenVariables(variables, virtualFunctionCall.Self);
    //                foreach (var argument in virtualFunctionCall.Arguments)
    //                    EnlivenVariables(variables, argument);
    //                break;
    //            case ConstructSome constructSome:
    //                EnlivenVariables(variables, constructSome.Value);
    //                break;
    //            case Constant _:
    //                // No variables
    //                break;
    //            case DeclaredValue _:
    //                throw new NotImplementedException();
    //            case Conversion conversion:
    //                EnlivenVariables(variables, conversion.Operand);
    //                break;
    //        }
    //    }
    //}
}
