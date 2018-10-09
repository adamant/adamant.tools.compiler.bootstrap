using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IL;
using Adamant.Tools.Compiler.Bootstrap.IL.Code;
using Adamant.Tools.Compiler.Bootstrap.IL.Code.EndStatements;
using Adamant.Tools.Compiler.Bootstrap.IL.Code.LValues;
using Adamant.Tools.Compiler.Bootstrap.IL.Code.RValues;
using Adamant.Tools.Compiler.Bootstrap.IL.Code.Statements;
using Adamant.Tools.Compiler.Bootstrap.IL.Declarations;
using Adamant.Tools.Compiler.Bootstrap.IL.Refs;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.BorrowChecker
{
    public class BorrowChecker
    {
        public IEnumerable<Diagnostic> Check([NotNull] ILPackage package)
        {
            var diagnostics = new List<Diagnostic>();
            foreach (var declaration in package.Declarations)
                switch (declaration)
                {
                    case ILFunctionDeclaration function:
                        diagnostics.AddRange(Check(function));
                        break;

                    case ILTypeDeclaration typeDeclaration:
                        Check(typeDeclaration);
                        break;
                }

            return diagnostics;
        }

        private static void Check(ILTypeDeclaration typeDeclaration)
        {
            // Currently nothing to check
        }

        private static IEnumerable<Diagnostic> Check([NotNull] ILFunctionDeclaration function)
        {
            var diagnostics = new List<Diagnostic>();
            // TODO we need to check definite assignment as well

            var edges = new Edges(function);

            // Compute aliveness at point after each statement
            var liveBefore = ComputeLiveness(function, edges);

            // Now do borrow checking with claims
            var blocks = new Queue<BasicBlock>();
            blocks.Enqueue(function.EntryBlock);
            var claims = new Claims();
            var nextObject = function.VariableDeclarations.Count + 1;

            while (blocks.Any())
            {
                var block = blocks.Dequeue();

                var claimsBeforeStatement = new HashSet<Claim>();
                foreach (var predecessor in edges.To(block).Select(b => b.EndStatement))
                    claimsBeforeStatement.UnionWith(claims.After(predecessor));

                foreach (var statement in block.Statements)
                {
                    var claimsAfterStatement = claims.After(statement);
                    claimsAfterStatement.UnionWith(claimsBeforeStatement);

                    // Create/drop any claims modified by the statement
                    switch (statement)
                    {
                        case NewObjectStatement newObjectStatement:
                            {
                                var title = new Title(newObjectStatement.ResultInto.CoreVariable(),
                                    nextObject);
                                nextObject += 1;
                                claimsAfterStatement.Add(title);
                                break;
                            }
                        case AssignmentStatement assignmentStatement:
                            {
                                var claim = GetClaim(assignmentStatement.RValue, claimsBeforeStatement);
                                var loan = new Loan(assignmentStatement.LValue.CoreVariable(),
                                    assignmentStatement.RValue,
                                    claim.Object);
                                claimsAfterStatement.Add(loan);
                                break;
                            }
                        case DeleteStatement deleteStatement:
                            {
                                var title = GetTitle(deleteStatement.VariableNumber,
                                    claimsBeforeStatement);
                                CheckCanMove(title.Object, claimsBeforeStatement, diagnostics);
                                claimsAfterStatement.RemoveWhere(c => c.Variable == title.Variable);
                                break;
                            }
                        case AddStatement _: // Add only applies to copy types so no loans
                        case ReturnStatement _:
                            break;
                        default:
                            throw NonExhaustiveMatchException.For(statement);
                    }

                    // TODO drop claims due to liveness

                    // Get Ready for next statement
                    claimsBeforeStatement = claimsAfterStatement;
                }
            }

            return diagnostics;
        }

        private static void CheckCanMove(
            int @object,
            HashSet<Claim> claims,
            List<Diagnostic> diagnostics)
        {
            var canTake = claims.OfType<Loan>().SelectMany(l => l.Restrictions)
                .Any(r => r.Place == @object && !r.CanTake);
            if (!canTake)
            {
                //diagnostics.Add(SemanticError.BorrowedValueDoesNotLiveLongEnough());
                throw new NotImplementedException();
            }
        }

        private static Claim GetClaim(RValue rvalue, HashSet<Claim> claims)
        {
            var coreVariable = rvalue.CoreVariable();
            return claims.Single(t => t.Variable == coreVariable);
        }

        private static Title GetTitle(RValue rvalue, HashSet<Claim> claims)
        {
            var coreVariable = rvalue.CoreVariable();
            return GetTitle(coreVariable, claims);
        }

        private static Title GetTitle(int variable, HashSet<Claim> claims)
        {
            return claims.OfType<Title>().Single(t => t.Variable == variable);
        }

        private static LiveVariables ComputeLiveness(ILFunctionDeclaration function, Edges edges)
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
                foreach (var successor in edges.From(block).Select(b => b.Statements.First()))
                    liveAfterStatement.Or(liveVariables.Before(successor));

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

                    // For the next statement
                    liveAfterStatement = liveBeforeStatement;
                }

                if (!liveBeforeBlock.Equals(liveVariables.Before(block.Statements.First())))
                    foreach (var basicBlock in edges.To(block)
                        .Where(fromBlock => !blocks.Contains(fromBlock)).ToList())
                        blocks.Enqueue(basicBlock);
            }
            return liveVariables;
        }

        private static void KillVariables(BitArray variables, LValue lvalue)
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
        private static void LiveVariables(BitArray variables, RValue rValue)
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
