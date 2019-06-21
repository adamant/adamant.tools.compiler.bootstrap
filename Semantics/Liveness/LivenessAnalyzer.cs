using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.Borrowing;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Liveness
{
    /// <summary>
    /// Compute where variables are live and return a structure of that
    /// </summary>
    public class LivenessAnalyzer
    {
        public static FixedDictionary<FunctionDeclarationSyntax, LiveVariables> Analyze(
            FixedList<MemberDeclarationSyntax> memberDeclarations,
            bool saveLivenessAnalysis)
        {
            var analyses = new Dictionary<FunctionDeclarationSyntax, LiveVariables>();
            var livenessAnalyzer = new LivenessAnalyzer();
            foreach (var function in memberDeclarations.OfType<FunctionDeclarationSyntax>())
            {
                var liveness = livenessAnalyzer.AnalyzeFunction(function);
                if (liveness != null)
                {
                    analyses.Add(function, liveness);
                    if (saveLivenessAnalysis) function.ControlFlow.LiveVariables = liveness;
                }
            }

            return analyses.ToFixedDictionary();
        }

        private LiveVariables AnalyzeFunction(FunctionDeclarationSyntax function)
        {
            // We can't check because of errors or no body
            if (function.HasErrors || function.ControlFlow == null)
                return null;

            // Compute aliveness at point after each statement
            return ComputeLiveness(function.ControlFlow);
        }

        /// <summary>
        /// Perform a backwards data flow analysis to determine where each variable is live or dead
        /// </summary>
        private static LiveVariables ComputeLiveness(ControlFlowGraph function)
        {
            var blocks = new Queue<BasicBlock>();
            blocks.EnqueueRange(function.ExitBlocks);
            var liveVariables = new LiveVariables(function);
            var numberOfVariables = function.VariableDeclarations.Count;

            while (blocks.TryDequeue(out var block))
            {
                var liveBeforeBlock = new BitArray(liveVariables.Before(block.Statements.First()));

                var liveAfterBlock = new BitArray(numberOfVariables);
                foreach (var successor in function.Edges.From(block))
                    liveAfterBlock.Or(liveVariables.Before(successor.Statements.First()));

                if (block.Terminator is ReturnStatement && function.ReturnVariable.TypeIsNotEmpty)
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
                        case DeleteStatement _:
                            throw new InvalidOperationException("Delete statements are supposed to be added after liveness analysis");
                        //liveSet[deleteStatement.Place.CoreVariable()] = true;
                        //break;
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
                    foreach (var basicBlock in function.Edges.To(block)
                        .Where(fromBlock => !blocks.Contains(fromBlock)).ToList())
                        blocks.Enqueue(basicBlock);
            }
            return liveVariables;
        }

        private static void KillVariables(BitArray variables, Place lvalue)
        {
            switch (lvalue)
            {
                case Dereference dereference:
                    KillVariables(variables, dereference.DereferencedValue);
                    break;
                case VariableReference variableReference:
                    variables[variableReference.Variable.Number] = false;
                    break;
                default:
                    throw NonExhaustiveMatchException.For(lvalue);
            }
        }
        private static void EnlivenVariables(BitArray variables, Value value)
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
                case Place place:
                    variables[place.CoreVariable().Number] = true;
                    break;
                case FunctionCall functionCall:
                    if (functionCall.Self != null) EnlivenVariables(variables, functionCall.Self);
                    foreach (var argument in functionCall.Arguments)
                        EnlivenVariables(variables, argument);
                    break;
                case VirtualFunctionCall virtualFunctionCall:
                    EnlivenVariables(variables, virtualFunctionCall.Self);
                    foreach (var argument in virtualFunctionCall.Arguments)
                        EnlivenVariables(variables, argument);
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
