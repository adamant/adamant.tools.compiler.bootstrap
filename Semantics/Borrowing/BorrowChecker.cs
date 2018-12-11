using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Borrowing
{
    public class BorrowChecker
    {
        private readonly CodeFile file;
        private readonly Diagnostics diagnostics;
        private int nextObjectId = 1;

        private int NewObjectId()
        {
            var objectId = nextObjectId;
            nextObjectId += 1;
            return objectId;
        }

        private BorrowChecker(CodeFile file, Diagnostics diagnostics)
        {
            this.file = file;
            this.diagnostics = diagnostics;
        }

        public static void Check(
            IEnumerable<MemberDeclarationSyntax> declarations,
            Diagnostics diagnostics)
        {
            foreach (var declaration in declarations)
            {
                var borrowChecker = new BorrowChecker(declaration.File, diagnostics);
                borrowChecker.Check(declaration);
            }
        }

        public void Check(MemberDeclarationSyntax declaration)
        {
            switch (declaration)
            {
                case TypeDeclarationSyntax typeDeclaration:
                    Check(typeDeclaration);
                    break;
                case FunctionDeclarationSyntax function:
                    Check(function);
                    break;
                default:
                    throw NonExhaustiveMatchException.For(declaration);
            }
        }

        private static void Check(TypeDeclarationSyntax _type)
        {
            // Currently nothing to check
        }

        private void Check(FunctionDeclarationSyntax function)
        {
            // We can't do borrow checking because of errors or no body
            if (function.Poisoned || function.ControlFlow == null)
                return;

            // TODO we need to check definite assignment as well

            var edges = function.ControlFlow.Edges;

            // Compute aliveness at point after each statement
            var liveBefore = ComputeLiveness(function.ControlFlow, edges);

            // Now do borrow checking with claims
            var blocks = new Queue<BasicBlock>();
            blocks.Enqueue(function.ControlFlow.EntryBlock);
            var claims = new Claims();

            while (blocks.Any())
            {
                var block = blocks.Dequeue();
                var claimsBeforeStatement = new HashSet<Claim>();

                if (block == function.ControlFlow.EntryBlock)
                    foreach (var parameter in function.ControlFlow.VariableDeclarations.Where(v =>
                        v.IsParameter))
                    {
                        claimsBeforeStatement.Add(new Loan(parameter.Number, nextObjectId));
                        nextObjectId += 1;
                    }

                foreach (var predecessor in edges.To(block).Select(b => b.Terminator))
                    claimsBeforeStatement.UnionWith(claims.After(predecessor));

                foreach (var statement in block.ExpressionStatements)
                {
                    var claimsAfterStatement = claims.After(statement);
                    claimsAfterStatement.UnionWith(claimsBeforeStatement);

                    // Create/drop any claims modified by the statement
                    switch (statement)
                    {
                        case AssignmentStatement assignmentStatement:
                            AcquireClaims(assignmentStatement.Place, assignmentStatement.Value, claimsBeforeStatement, claimsAfterStatement);
                            break;
                        case ActionStatement actionStatement:
                            AcquireClaims(null, actionStatement.Value, claimsBeforeStatement, claimsAfterStatement);
                            break;
                        case DeleteStatement deleteStatement:
                        {
                            var title = GetTitle(deleteStatement.VariableNumber, claimsBeforeStatement);
                            CheckCanMove(title.ObjectId, claimsBeforeStatement, deleteStatement.Span);
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
                    case IfStatement ifStatement:
                        blocks.Enqueue(function.ControlFlow.BasicBlocks[ifStatement.ThenBlockNumber]);
                        blocks.Enqueue(function.ControlFlow.BasicBlocks[ifStatement.ElseBlockNumber]);
                        break;
                    case GotoStatement gotoStatement:
                        blocks.Enqueue(function.ControlFlow.BasicBlocks[gotoStatement.GotoBlockNumber]);
                        break;
                    case ReturnStatement _: // Add only applies to copy types so no loans
                        break;
                    default:
                        throw NonExhaustiveMatchException.For(block.Terminator);
                }
            }
        }

        private void CheckCanMove(int objectId, HashSet<Claim> claims, TextSpan span)
        {
            var canTake = claims.OfType<Loan>().SelectMany(l => l.Restrictions)
                .Any(r => r.Place == objectId && !r.CanTake);

            if (!canTake)
                diagnostics.Add(BorrowError.BorrowedValueDoesNotLiveLongEnough(file, span));
        }

        private void AcquireClaims(
            Place assignToPlace,
            Value value,
            HashSet<Claim> claimsBeforeStatement,
            HashSet<Claim> claimsAfterStatement)
        {
            switch (value)
            {
                case ConstructorCall constructorCall:
                    foreach (var argument in constructorCall.Arguments)
                        AcquireClaim(assignToPlace, argument, claimsBeforeStatement, claimsAfterStatement);
                    // We have made a new object, assign it a new id
                    var objectId = NewObjectId();
                    // Variable acquires title on any new objects
                    var title = new Title(assignToPlace.CoreVariable(), objectId);
                    claimsAfterStatement.Add(title);
                    break;
                case UnaryOperation unaryOperation:
                    AcquireClaim(assignToPlace, unaryOperation.Operand, claimsBeforeStatement, claimsAfterStatement);
                    break;
                case BinaryOperation binaryOperation:
                    AcquireClaim(assignToPlace, binaryOperation.LeftOperand, claimsBeforeStatement, claimsAfterStatement);
                    AcquireClaim(assignToPlace, binaryOperation.RightOperand, claimsBeforeStatement, claimsAfterStatement);
                    break;
                case IntegerConstant _:
                    // no loans to acquire
                    break;
                case Operand operand:
                    AcquireClaim(assignToPlace, operand, claimsBeforeStatement, claimsAfterStatement);
                    break;
                case FunctionCall functionCall:
                    if (functionCall.Self != null)
                        AcquireClaim(assignToPlace, functionCall.Self, claimsBeforeStatement, claimsAfterStatement);
                    foreach (var argument in functionCall.Arguments)
                        AcquireClaim(assignToPlace, argument, claimsBeforeStatement, claimsAfterStatement);
                    break;
                default:
                    throw NonExhaustiveMatchException.For(value);
            }
        }

        private static void AcquireClaim(
            Place assignToPlace,
            Operand operand,
            HashSet<Claim> claimsBeforeStatement,
            HashSet<Claim> claimsAfterStatement)
        {
            switch (operand)
            {
                case CopyPlace copyPlace:
                    var coreVariable = copyPlace.Place.CoreVariable();
                    var claim = claimsBeforeStatement.SingleOrDefault(t => t.Variable == coreVariable);
                    // Copy types don't have claims right now
                    if (claim != null) // copy types don't have claims right now
                    {
                        var loan = new Loan(assignToPlace.CoreVariable(),
                            operand,
                            claim.ObjectId);
                        claimsAfterStatement.Add(loan);
                    }
                    break;
                case Constant _:
                    // no loans to acquire
                    break;
                default:
                    throw NonExhaustiveMatchException.For(operand);
            }
        }

        private static Title GetTitle(int variable, HashSet<Claim> claims)
        {
            return claims.OfType<Title>().Single(t => t.Variable == variable);
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

                if (!liveBeforeBlock.Equals(liveVariables.Before(block.Statements.First())))
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
                case CopyPlace copyPlace:
                    variables[copyPlace.Place.CoreVariable()] = true;
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
