using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Borrowing;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Liveness
{
    public class LivenessAnalyzer
    {
        public static void Analyze(FixedList<MemberDeclarationSyntax> memberDeclarations)
        {
            var livenessAnalyzer = new LivenessAnalyzer();
            foreach (var function in memberDeclarations.OfType<FunctionDeclarationSyntax>())
                livenessAnalyzer.AnalyzeFunction(function);
        }

        private void AnalyzeFunction(FunctionDeclarationSyntax function)
        {
            // We can't check because of errors or no body
            if (function.Poisoned || function.ControlFlow == null)
                return;

            var edges = function.ControlFlow.Edges;

            // Compute aliveness at point after each statement
            var liveBefore = ComputeLiveness(function.ControlFlow, edges);
        }

        private static LiveVariables ComputeLiveness(ControlFlowGraph function, Edges edges)
        {
            var blocks = new Queue<BasicBlock>();
            blocks.EnqueueRange(function.ExitBlocks);
            var liveVariables = new LiveVariables(function);
            var numberOfVariables = function.VariableDeclarations.Count;

            while (blocks.TryDequeue(out var block))
            {
                var liveBeforeBlock = new BitArray(liveVariables.Before(block.Statements.First()));

                var liveAfterBlock = new BitArray(numberOfVariables);
                foreach (var successor in edges.From(block))
                    liveAfterBlock.Or(liveVariables.Before(successor.Statements.Last()));

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
                        case DeleteStatement deleteStatement:
                            liveSet[deleteStatement.VariableNumber] = true;
                            break;
                        case IfStatement _:
                        case GotoStatement _:
                            // We already or'ed together the live variables from successor blocks
                            break;
                        case ReturnStatement _:
                            // No effect on variables?
                            // TODO should we check the liveSet is empty?
                            break;
                        default:
                            throw NonExhaustiveMatchException.For(statement);
                    }

                    // For the next statement
                    liveAfterStatement = liveSet;
                }

                if (!liveBeforeBlock.ValuesEqual(liveVariables.Before(block.Statements.First())))
                    foreach (var basicBlock in edges.To(block)
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
                    variables[variableReference.VariableNumber] = false;
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
                    variables[place.CoreVariable()] = true;
                    break;
                case FunctionCall functionCall:
                    if (functionCall.Self != null) EnlivenVariables(variables, functionCall.Self);
                    foreach (var argument in functionCall.Arguments)
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
