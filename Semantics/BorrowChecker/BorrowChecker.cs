using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IL.Code;
using Adamant.Tools.Compiler.Bootstrap.IL.Code.EndStatements;
using Adamant.Tools.Compiler.Bootstrap.IL.Code.LValues;
using Adamant.Tools.Compiler.Bootstrap.IL.Code.RValues;
using Adamant.Tools.Compiler.Bootstrap.IL.Code.Statements;
using Adamant.Tools.Compiler.Bootstrap.IL.Refs;
using ILFunctionDeclaration = Adamant.Tools.Compiler.Bootstrap.IL.Declarations.FunctionDeclaration;
using ILPackage = Adamant.Tools.Compiler.Bootstrap.IL.Package;
using ILTypeDeclaration = Adamant.Tools.Compiler.Bootstrap.IL.Declarations.TypeDeclaration;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.BorrowChecker
{
    public class BorrowChecker
    {
        public void Check(ILPackage package)
        {
            foreach (var declaration in package.Declarations)
                switch (declaration)
                {
                    case ILFunctionDeclaration function:
                        Check(function);
                        break;

                    case ILTypeDeclaration typeDeclaration:
                        Check(typeDeclaration);
                        break;
                }
        }

        private void Check(ILTypeDeclaration typeDeclaration)
        {
            // Currently nothing to check
        }

        private void Check(ILFunctionDeclaration function)
        {
            var edges = new Edges(function);
            var transitions = new Transitions(function);
            // Compute aliveness at point after each statement
            var liveBefore = ComputeLiveness(function, edges, transitions);
        }

        private LiveVariables ComputeLiveness(ILFunctionDeclaration function, Edges edges, Transitions transitions)
        {
            var blocks = new Queue<BasicBlock>();
            blocks.Enqueue(function.ExitBlock);
            var liveVariables = new LiveVariables(function);
            var numberOfVariables = function.VariableDeclarations.Count;

            while (blocks.Any())
            {
                var block = blocks.Dequeue();
                var liveBeforeBlock = new BitArray(liveVariables.Before(block.Statements.First()));

                var liveAfterStatement = new BitArray(numberOfVariables);
                foreach (var predecessor in edges.From(block).Select(b => b.Statements.First()))
                    liveAfterStatement.Or(liveVariables.Before(predecessor));

                foreach (var statement in block.Statements.Reverse())
                {
                    var liveBeforeStatement = liveVariables.Before(statement);
                    liveBeforeStatement.Or(liveAfterStatement);
                    switch (statement)
                    {
                        case AssignmentStatement assignment:
                            KillVariables(liveBeforeStatement, assignment.LValue);
                            LiveVariables(liveBeforeStatement, assignment.RValue);
                            break;
                        case AddStatement addStatement:
                            KillVariables(liveBeforeStatement, addStatement.LValue);
                            LiveVariables(liveBeforeStatement, addStatement.LeftOperand);
                            LiveVariables(liveBeforeStatement, addStatement.RightOperand);
                            break;
                        case DeleteStatement deleteStatement:
                            liveBeforeStatement[deleteStatement.VariableNumber] = true;
                            break;
                        case NewObjectStatement newObjectStatement:
                            KillVariables(liveBeforeStatement, newObjectStatement.ResultInto);
                            foreach (var argument in newObjectStatement.Arguments)
                                LiveVariables(liveBeforeStatement, argument);
                            break;
                        case ReturnStatement _:
                            break;
                        default:
                            throw NonExhaustiveMatchException.For(statement);
                    }
                }

                if (!liveBeforeBlock.Equals(liveVariables.Before(block.Statements.First())))
                    foreach (var basicBlock in edges.To(block)
                        .Where(fromBlock => !blocks.Contains(fromBlock)).ToList())
                        blocks.Enqueue(basicBlock);
            }
            return liveVariables;
        }

        private void KillVariables(BitArray variables, LValue lvalue)
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
        private void LiveVariables(BitArray variables, RValue rValue)
        {
            switch (rValue)
            {
                case Dereference dereference:
                    LiveVariables(variables, dereference.DereferencedValue);
                    break;
                case VariableReference variableReference:
                    variables[variableReference.VariableNumber] = true;
                    break;
                default:
                    throw NonExhaustiveMatchException.For(rValue);
            }
        }
    }
}
