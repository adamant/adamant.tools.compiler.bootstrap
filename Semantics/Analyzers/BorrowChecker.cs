using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Borrowing;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
using Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyzers
{
    public class BorrowChecker
    {
        public void Check([NotNull][ItemNotNull] IEnumerable<MemberDeclarationAnalysis> declarations)
        {
            foreach (var declaration in declarations)
                Check(declaration);
        }

        public void Check([NotNull] MemberDeclarationAnalysis declaration)
        {
            switch (declaration)
            {
                case FunctionDeclarationAnalysis function:
                    Check(function);
                    break;

                case TypeDeclarationAnalysis typeDeclaration:
                    Check(typeDeclaration);
                    break;
            }
        }

        private static void Check(TypeDeclarationAnalysis typeDeclaration)
        {
            // Currently nothing to check
        }

        private static void Check([NotNull] FunctionDeclarationAnalysis function)
        {
            if (function.ControlFlow == null)
                return; // We can't do borrow checking because we couldn't even get a control flow graph

            // TODO we need to check definite assignment as well

            var diagnostics = function.Diagnostics;
            var edges = function.ControlFlow.Edges();

            // Compute aliveness at point after each statement
            var liveBefore = ComputeLiveness(function.ControlFlow, edges);

            // Now do borrow checking with claims
            var blocks = new Queue<BasicBlock>();
            blocks.Enqueue(function.ControlFlow.EntryBlock);
            var claims = new Claims();
            var nextObject = 1;

            while (blocks.Any())
            {
                var block = blocks.Dequeue().AssertNotNull();
                var claimsBeforeStatement = new HashSet<Claim>();

                if (block == function.ControlFlow.EntryBlock)
                {
                    foreach (var parameter in function.ControlFlow.VariableDeclarations.Where(v => v.IsParameter))
                    {
                        claimsBeforeStatement.Add(new Loan(parameter.Number, nextObject));
                        nextObject += 1;
                    }
                }

                //foreach (var predecessor in edges.To(block).Select(b => b.Terminator.AssertNotNull()))
                //    claimsBeforeStatement.UnionWith(claims.After(predecessor));

                foreach (var statement in block.Statements)
                {
                    var claimsAfterStatement = claims.After(statement);
                    claimsAfterStatement.UnionWith(claimsBeforeStatement);

                    // Create/drop any claims modified by the statement
                    switch (statement)
                    {
                        case NewObjectStatement newObjectStatement:
                        {
                            var title = new Title(newObjectStatement.ResultInto.CoreVariable(), nextObject);
                            nextObject += 1;
                            claimsAfterStatement.Add(title);
                            break;
                        }
                        //case AssignmentStatement assignmentStatement:
                        //{
                        //    var claim = GetClaim(assignmentStatement.Value, claimsBeforeStatement);
                        //    if (claim != null) // copy types don't have claims right now
                        //    {
                        //        var loan = new Loan(assignmentStatement.Place.CoreVariable(),
                        //            assignmentStatement.Value,
                        //            claim.Object);
                        //        claimsAfterStatement.Add(loan);
                        //    }
                        //    break;
                        //}
                        case DeleteStatement deleteStatement:
                        {
                            var title = GetTitle(deleteStatement.VariableNumber,
                                claimsBeforeStatement);
                            CheckCanMove(title.Object, claimsBeforeStatement, function, deleteStatement.Span, diagnostics);
                            claimsAfterStatement.RemoveWhere(c => c.Variable == title.Variable);
                            break;
                        }
                        default:
                            throw NonExhaustiveMatchException.For(statement);
                    }

                    // TODO drop claims due to liveness

                    // Get Ready for next statement
                    claimsBeforeStatement = claimsAfterStatement;
                }

                switch (block.Terminator)
                {
                    case ReturnTerminator _: // Add only applies to copy types so no loans
                        break;
                    default:
                        throw NonExhaustiveMatchException.For(block.Terminator);
                }
            }
        }

        private static void CheckCanMove(
            int @object,
            [NotNull] HashSet<Claim> claims,
            [NotNull] FunctionDeclarationAnalysis function,
            [NotNull] TextSpan span,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var canTake = claims.OfType<Loan>().SelectMany(l => l.Restrictions)
                .Any(r => r.Place == @object && !r.CanTake);

            if (!canTake)
                diagnostics.Publish(BorrowError.BorrowedValueDoesNotLiveLongEnough(function.Context.File, span));
        }

        //[CanBeNull]
        //private static Claim GetClaim([NotNull] IValue value, [NotNull][ItemNotNull] HashSet<Claim> claims)
        //{
        //    var coreVariable = value.CoreVariable();
        //    // Copy types don't have claims right now
        //    return claims.SingleOrDefault(t => t.Variable == coreVariable);
        //}

        //private static Title GetTitle([NotNull] RValue rvalue, [NotNull] HashSet<Claim> claims)
        //{
        //    var coreVariable = rvalue.CoreVariable();
        //    return GetTitle(coreVariable, claims);
        //}

        [NotNull]
        private static Title GetTitle([NotNull] int variable, [NotNull] HashSet<Claim> claims)
        {
            return claims.OfType<Title>().Single(t => t.Variable == variable).AssertNotNull();
        }

        private static LiveVariables ComputeLiveness([NotNull] ControlFlowGraph function, [NotNull] Edges edges)
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
                    var liveSet = liveVariables.Before(statement);
                    liveSet.Or(liveAfterStatement);
                    switch (statement)
                    {
                        case AssignmentStatement assignment:
                            KillVariables(liveSet, assignment.Place);
                            EnlivenVariables(liveSet, assignment.Value);
                            break;
                        //case AddStatement addStatement:
                        //    KillVariables(liveSet, addStatement.LValue);
                        //    EnlivenVariables(liveSet, addStatement.LeftOperand);
                        //    EnlivenVariables(liveSet, addStatement.RightOperand);
                        //    break;
                        case DeleteStatement deleteStatement:
                            liveSet[deleteStatement.VariableNumber] = true;
                            break;
                        //case NewObjectStatement newObjectStatement:
                        //    KillVariables(liveSet, newObjectStatement.ResultInto);
                        //    foreach (var argument in newObjectStatement.Arguments)
                        //        EnlivenVariables(liveSet, argument);
                        //    break;
                        //case IntegerLiteralStatement integerLiteralStatement:
                        //    EnlivenVariables(liveSet, integerLiteralStatement.Place);
                        //    break;
                        default:
                            throw NonExhaustiveMatchException.For(statement);
                    }

                    // For the next statement
                    liveAfterStatement = liveSet;
                }

                switch (block.Terminator)
                {
                    case ReturnTerminator _:
                        break;
                    default:
                        throw NonExhaustiveMatchException.For(block.Terminator);
                }

                if (!liveBeforeBlock.Equals(liveVariables.Before(block.Statements.First())))
                    foreach (var basicBlock in edges.To(block)
                        .Where(fromBlock => !blocks.Contains(fromBlock)).ToList())
                        blocks.Enqueue(basicBlock);
            }
            return liveVariables;
        }

        private static void KillVariables([NotNull] BitArray variables, [NotNull] Place lvalue)
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
        private static void EnlivenVariables([NotNull] BitArray variables, [NotNull] IValue value)
        {
            switch (value)
            {
                //case Dereference dereference:
                //    EnlivenVariables(variables, dereference.DereferencedValue);
                //    break;
                //case VariableReference variableReference:
                //    variables[variableReference.VariableNumber] = true;
                //    break;
                case Constant _:
                    // No variables
                    break;
                default:
                    throw NonExhaustiveMatchException.For(value);
            }
        }
    }
}
