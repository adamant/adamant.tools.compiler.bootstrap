using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.Borrowing;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Liveness
{
    /// <summary>
    /// Compute where variables are live and return a structure of that
    /// </summary>
    public static class LivenessAnalyzer
    {
        public static FixedDictionary<ICallableDeclaration, LiveVariables> Check(
            FixedList<ICallableDeclaration> functions,
            bool saveLivenessAnalysis)
        {
            var analyses = new Dictionary<ICallableDeclaration, LiveVariables>();
            foreach (var function in functions)
            {
                var liveness = ComputeLiveness(function);
                if (liveness != null)
                {
                    analyses.Add(function, liveness);
                    if (saveLivenessAnalysis)
                        function.ControlFlow.LiveVariables = liveness;
                }
            }

            return analyses.ToFixedDictionary();
        }

        /// <summary>
        /// Perform a backwards data flow analysis to determine where each variable is live or dead
        /// </summary>
        private static LiveVariables ComputeLiveness(ICallableDeclaration function)
        {
            var controlFlow = function.ControlFlow;
            var blocks = new Queue<BasicBlock>();
            blocks.EnqueueRange(controlFlow.ExitBlocks);
            var liveVariables = new LiveVariables(controlFlow);
            var numberOfVariables = controlFlow.VariableDeclarations.Count;

            while (blocks.TryDequeue(out var block))
            {
                var liveBeforeBlock = new BitArray(liveVariables.Before(block.Statements.First()));

                var liveAfterBlock = new BitArray(numberOfVariables);
                foreach (var successor in controlFlow.Edges.From(block))
                    liveAfterBlock.Or(liveVariables.Before(successor.Statements.First()));

                if (block.Terminator is ReturnStatement && controlFlow.ReturnVariable.TypeIsNotEmpty)
                    liveAfterBlock[0] = true; // the return value is live after a block that returns

                var liveAfterStatement = liveAfterBlock;

                foreach (var statement in block.Statements.Reverse())
                {
                    var liveSet = liveVariables.Before(statement);
                    liveSet.Or(liveAfterStatement);
                    switch (statement)
                    {
                        case AssignmentStatement assignment:
                            KillVariables(liveSet, assignment.Place);
                            EnlivenVariables(liveSet, assignment.Value);
                            break;
                        case ActionStatement action:
                            EnlivenVariables(liveSet, action.Value);
                            break;
                        case DeleteStatement delete:
                            liveSet[delete.Place.CoreVariable().Number] = true;
                            break;
                        case IfStatement _:
                        case GotoStatement _:
                            // We already or'ed together the live variables from successor blocks
                            break;
                        case ReturnStatement _:
                            // No effect on variables?
                            // TODO should we check the liveSet is empty?
                            break;
                        case ExitScopeStatement _:
                            // TODO use end scope statement to track liminal state
                            break;
                        default:
                            throw NonExhaustiveMatchException.For(statement);
                    }

                    // For the next statement
                    liveAfterStatement = liveSet;
                }

                if (!liveBeforeBlock.ValuesEqual(liveVariables.Before(block.Statements.First())))
                    foreach (var basicBlock in controlFlow.Edges.To(block)
                        .Where(fromBlock => !blocks.Contains(fromBlock)).ToList())
                        blocks.Enqueue(basicBlock);
            }
            return liveVariables;
        }

        private static void KillVariables(BitArray variables, IPlace place)
        {
            switch (place)
            {
                default:
                    throw ExhaustiveMatch.Failed(place);
                case VariableReference variableReference:
                    variables[variableReference.Variable.Number] = false;
                    break;
                case FieldAccess fieldAccess:
                    EnlivenVariables(variables, fieldAccess.Expression);
                    break;
            }
        }
        private static void EnlivenVariables(BitArray variables, IValue value)
        {
            switch (value)
            {
                case ConstructorCall constructorCall:
                    foreach (var argument in constructorCall.Arguments)
                        EnlivenVariables(variables, argument);
                    break;
                case UnaryOperation unaryOperation:
                    EnlivenVariables(variables, unaryOperation.Operand);
                    break;
                case BinaryOperation binaryOperation:
                    EnlivenVariables(variables, binaryOperation.LeftOperand);
                    EnlivenVariables(variables, binaryOperation.RightOperand);
                    break;
                case IPlace place:
                    variables[place.CoreVariable().Number] = true;
                    break;
                case FunctionCall functionCall:
                    if (functionCall.Self != null)
                        EnlivenVariables(variables, functionCall.Self);
                    foreach (var argument in functionCall.Arguments)
                        EnlivenVariables(variables, argument);
                    break;
                case VirtualFunctionCall virtualFunctionCall:
                    EnlivenVariables(variables, virtualFunctionCall.Self);
                    foreach (var argument in virtualFunctionCall.Arguments)
                        EnlivenVariables(variables, argument);
                    break;
                case ConstructSome constructSome:
                    EnlivenVariables(variables, constructSome.Value);
                    break;
                case Constant _:
                    // No variables
                    break;
                default:
                    throw NonExhaustiveMatchException.For(value);
            }
        }
    }
}
