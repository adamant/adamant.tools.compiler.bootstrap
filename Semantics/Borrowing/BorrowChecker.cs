using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.Borrowing;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Borrowing
{
    /// <summary>
    /// Check for borrow errors
    /// </summary>
    public class BorrowChecker
    {
        private readonly CodeFile file;
        private readonly Diagnostics diagnostics;
        private readonly bool saveBorrowClaims;
        private int nextLifetimeNumber = 1;

        private Lifetime NewLifetime()
        {
            var lifetimeNumber = nextLifetimeNumber;
            nextLifetimeNumber += 1;
            return new Lifetime(lifetimeNumber);
        }

        private BorrowChecker(CodeFile file, Diagnostics diagnostics, bool saveBorrowClaims)
        {
            this.file = file;
            this.diagnostics = diagnostics;
            this.saveBorrowClaims = saveBorrowClaims;
        }

        public static void Check(
            IEnumerable<MemberDeclarationSyntax> declarations,
            Diagnostics diagnostics,
            bool saveBorrowClaims)
        {
            foreach (var declaration in declarations)
            {
                var borrowChecker = new BorrowChecker(declaration.File, diagnostics, saveBorrowClaims);
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

            var variables = function.ControlFlow.VariableDeclarations;
            // Do borrow checking with claims
            var blocks = new Queue<BasicBlock>();
            blocks.Enqueue(function.ControlFlow.EntryBlock);
            // Compute parameter claims once in case for some reason we process the entry block repeatedly
            var parameterClaims = AcquireParameterClaims(function);
            var claims = new ControlFlowGraphClaims(parameterClaims);

            while (blocks.TryDequeue(out var block))
            {
                var claimsBeforeStatement = ClaimsBeforeBlock(function, block, claims);

                foreach (var statement in block.ExpressionStatements)
                {
                    var claimsAfterStatement = claims.After(statement);
                    claimsAfterStatement.AddRange(claimsBeforeStatement);

                    // Create/drop any claims modified by the statement
                    switch (statement)
                    {
                        case AssignmentStatement assignmentStatement:
                            CheckStatement(assignmentStatement.Place, assignmentStatement.Value, claimsBeforeStatement, claimsAfterStatement, variables);
                            break;
                        case ActionStatement actionStatement:
                            CheckStatement(null, actionStatement.Value, claimsBeforeStatement, claimsAfterStatement, variables);
                            break;
                        case DeleteStatement deleteStatement:
                        {
                            // The variable we are deleting through is supposed to have the title
                            var title = claimsBeforeStatement.OwnedBy(deleteStatement.Place.CoreVariable());
                            CheckCanDelete(title.Lifetime, claimsBeforeStatement, deleteStatement.Span);
                            claimsAfterStatement.Remove(title);
                            break;
                        }
                        default:
                            throw NonExhaustiveMatchException.For(statement);
                    }

                    // TODO drop claims due to liveness

                    // Get Ready for next statement
                    claimsBeforeStatement = claimsAfterStatement;
                }

                var claimsAfterBlock = claims.After(block.Terminator);
                if (!claimsBeforeStatement.SequenceEqual(claimsAfterBlock))
                {
                    claimsAfterBlock.AddRange(claimsBeforeStatement);
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

            if (saveBorrowClaims) function.ControlFlow.BorrowClaims = claims;
        }

        private Claims ClaimsBeforeBlock(
            FunctionDeclarationSyntax function,
            BasicBlock block,
            ControlFlowGraphClaims claims)
        {
            var claimsBeforeStatement = new Claims();

            if (block == function.ControlFlow.EntryBlock)
                claimsBeforeStatement.AddRange(claims.ParameterClaims);

            foreach (var predecessor in function.ControlFlow.Edges.To(block).Select(b => b.Terminator))
                claimsBeforeStatement.AddRange(claims.After(predecessor));

            return claimsBeforeStatement;
        }

        private Claims AcquireParameterClaims(FunctionDeclarationSyntax function)
        {
            var claimsBeforeStatement = new Claims();
            foreach (var parameter in function.ControlFlow.VariableDeclarations
                .Where(v => v.IsParameter && v.Type is ReferenceType))
            {
                claimsBeforeStatement.Add(new Borrows(parameter.Variable, NewLifetime()));
            }

            return claimsBeforeStatement;
        }

        private void CheckCanDelete(Lifetime lifetime, Claims claims, TextSpan span)
        {
            var isBorrowedOrShared = claims.IsBorrowedOrShared(lifetime);

            if (isBorrowedOrShared)
                // TODO this should be a different error message
                diagnostics.Add(BorrowError.BorrowedValueDoesNotLiveLongEnough(file, span));
        }

        private void CheckStatement(
            Place assignToPlace,
            Value value,
            Claims claimsBeforeStatement,
            Claims claimsAfterStatement,
            FixedList<LocalVariableDeclaration> variables)
        {
            switch (value)
            {
                case ConstructorCall constructorCall:
                    foreach (var argument in constructorCall.Arguments)
                        AcquireClaim(assignToPlace?.CoreVariable(), argument, claimsAfterStatement);
                    // We have made a new object, assign it a new id
                    var objectId = NewLifetime();
                    // Variable acquires title on any new objects
                    var title = new Owns(assignToPlace.CoreVariable(), objectId);
                    claimsAfterStatement.Add(title);
                    break;
                case UnaryOperation unaryOperation:
                    AcquireClaim(assignToPlace?.CoreVariable(), unaryOperation.Operand, claimsAfterStatement);
                    break;
                case BinaryOperation binaryOperation:
                    AcquireClaim(assignToPlace?.CoreVariable(), binaryOperation.LeftOperand, claimsAfterStatement);
                    AcquireClaim(assignToPlace?.CoreVariable(), binaryOperation.RightOperand, claimsAfterStatement);
                    break;
                case IntegerConstant _:
                    // no loans to acquire
                    break;
                case Operand operand:
                    AcquireClaim(assignToPlace?.CoreVariable(), operand, claimsAfterStatement);
                    break;
                case FunctionCall functionCall:
                {
                    var callLifetime = NewLifetime();
                    var outstandingClaims = new Claims();
                    outstandingClaims.AddRange(claimsBeforeStatement);
                    if (functionCall.Self != null)
                        AcquireClaim(callLifetime, functionCall.Self, outstandingClaims);
                    foreach (var argument in functionCall.Arguments)
                        AcquireClaim(callLifetime, argument, outstandingClaims);
                    if (assignToPlace != null)
                    {
                        AcquireClaim(assignToPlace.CoreVariable(), callLifetime, claimsBeforeStatement, claimsAfterStatement);
                        // For now, we just add all the call claims, this should be based on the function type instead
                        claimsAfterStatement.AddRange(outstandingClaims);
                    }
                }
                break;
                case VirtualFunctionCall virtualFunctionCall:
                {
                    if (virtualFunctionCall.Self != null)
                        AcquireClaim(assignToPlace?.CoreVariable(), virtualFunctionCall.Self, claimsAfterStatement);
                    foreach (var argument in virtualFunctionCall.Arguments)
                        AcquireClaim(assignToPlace?.CoreVariable(), argument,
                            claimsAfterStatement);
                    AcquireOwnershipIfMoved(assignToPlace, variables, claimsAfterStatement);
                }
                break;
                default:
                    throw NonExhaustiveMatchException.For(value);
            }
        }

        private void AcquireOwnershipIfMoved(
            Place assignToPlace,
            FixedList<LocalVariableDeclaration> variables,
            Claims claimsAfterStatement)
        {
            if (assignToPlace == null) return;
            var assignToVariable = variables.Single(v => v.Variable == assignToPlace.CoreVariable());
            if (assignToVariable.Type is ReferenceType referenceType
                && referenceType.IsOwned)
            {
                // We have taken ownership of a new object, assign it a new id
                var objectId = NewLifetime();
                // Variable acquires title on any new objects
                var title = new Owns(assignToPlace.CoreVariable(), objectId);
                claimsAfterStatement.Add(title);
            }
        }

        private static void AcquireClaim(
            IClaimHolder claimHolder,
            Operand operand,
            Claims outstandingClaims)
        {
            switch (operand)
            {
                case VariableReference varRef:
                    switch (varRef.Kind)
                    {
                        case VariableReferenceKind.Borrow:
                            // TODO check if we can borrow from this variable
                            var borrowingLifetime = outstandingClaims.LifetimeOf(varRef.Variable);
                            if (borrowingLifetime != null)
                                outstandingClaims.Add(new Borrows(claimHolder,
                                    borrowingLifetime.Value));
                            break;
                        case VariableReferenceKind.Share:
                            // TODO check if we can share from this variable
                            var sharingLifetime = outstandingClaims.LifetimeOf(varRef.Variable);
                            if (sharingLifetime != null)
                                outstandingClaims.Add(new Borrows(claimHolder,
                                    sharingLifetime.Value));
                            break;
                        default:
                            throw NonExhaustiveMatchException.ForEnum(varRef.Kind);
                    }
                    break;
                case Place place:
                    //var coreVariable = place.CoreVariable();
                    //var claim = claimsBeforeStatement.SingleOrDefault(t => t.Holder.Equals(coreVariable));
                    //// Copy types don't have claims right now
                    //if (claim != null) // copy types don't have claims right now
                    //{
                    //    var loan = new Borrows(claimHolder, claim.Lifetime);
                    //    statementClaims.Add(loan);
                    //}
                    throw new NotImplementedException();
                    break;
                case Constant _:
                    // no loans to acquire
                    break;
                default:
                    throw NonExhaustiveMatchException.For(operand);
            }
        }

        private static void AcquireClaim(
            IClaimHolder claimHolder,
            Lifetime lifetime,
            Claims claimsBeforeStatement,
            Claims statementClaims)
        {
            statementClaims.Add(new Borrows(claimHolder, lifetime));
        }
    }
}
