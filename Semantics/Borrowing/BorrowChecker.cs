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
        private readonly FixedDictionary<FunctionDeclarationSyntax, LiveVariables> liveness;
        private readonly Diagnostics diagnostics;
        private readonly HashSet<TextSpan> reportedDiagnosticSpans = new HashSet<TextSpan>();
        private readonly bool saveBorrowClaims;
        private int nextLifetimeNumber = 1;

        private Lifetime NewLifetime()
        {
            var lifetimeNumber = nextLifetimeNumber;
            nextLifetimeNumber += 1;
            return new Lifetime(lifetimeNumber);
        }

        private BorrowChecker(
            CodeFile file,
            FixedDictionary<FunctionDeclarationSyntax, LiveVariables> liveness,
            Diagnostics diagnostics,
            bool saveBorrowClaims)
        {
            this.file = file;
            this.liveness = liveness;
            this.diagnostics = diagnostics;
            this.saveBorrowClaims = saveBorrowClaims;
        }

        public static void Check(
            IEnumerable<MemberDeclarationSyntax> declarations,
            FixedDictionary<FunctionDeclarationSyntax, LiveVariables> liveness,
            Diagnostics diagnostics,
            bool saveBorrowClaims)
        {
            foreach (var declaration in declarations)
            {
                var borrowChecker = new BorrowChecker(declaration.File, liveness, diagnostics, saveBorrowClaims);
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
            var variables = function.ControlFlow.VariableDeclarations;
            var liveVariables = liveness[function];
            // Do borrow checking with claims
            var blocks = new Queue<BasicBlock>();
            blocks.Enqueue(function.ControlFlow.EntryBlock);
            // Compute parameter claims once in case for some reason we process the entry block repeatedly
            var parameterClaims = AcquireParameterClaims(function);
            var claims = new StatementClaims(parameterClaims);

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
                            //var title = claimsBeforeStatement.OwnedBy(deleteStatement.Place.CoreVariable());
                            //if (title != null)// TODO this should be an error
                            //{
                            //    CheckCanDelete(title.Lifetime, claimsBeforeStatement, deleteStatement.Span);
                            //    claimsAfterStatement.Remove(title);
                            //}
                            //break;
                            throw new NotSupportedException("There should be no delete statements in the IL at this phase");
                        }
                        case ExitScopeStatement exitScopeStatement:
                            // Any claims held by variables in this scope are released
                            var variablesInScope = function.ControlFlow.VariableDeclarations
                                .Where(d => d.Scope == exitScopeStatement.Scope)
                                .Select(d => d.Variable)
                                .ToList();
                            var lifetimesOwnedByVariablesInScope = variablesInScope
                                .Select(v => claimsAfterStatement.OwnedBy(v))
                                .Where(o => o != null).Select(o => o.Lifetime)
                                .ToList();
                            claimsAfterStatement.Release(variablesInScope);
                            var erroredLifetimes = lifetimesOwnedByVariablesInScope
                                .Where(l => claimsAfterStatement.IsShared(l));
                            var erroredVariables = erroredLifetimes.SelectMany(l =>
                                claimsAfterStatement.ClaimsTo(l).Select(c => c.Holder)
                                    .OfType<Variable>()).Distinct();
                            foreach (var variable in erroredVariables)
                            {
                                var declaration = function.ControlFlow.VariableDeclarations
                                                            .Single(d => d.Variable == variable);
                                ReportDiagnostic(BorrowError.SharedValueDoesNotLiveLongEnough(file, exitScopeStatement.Span, declaration.Name));
                            }
                            break;
                        default:
                            throw NonExhaustiveMatchException.For(statement);
                    }

                    var liveAfter = liveVariables.After(statement);
                    var droppedVariables = liveAfter.FalseIndexes()
                        .Select(i => new Variable(i))
                        .Where(v => !VariableOwnsSharedValue(v, claimsAfterStatement));
                    claimsAfterStatement.Release(droppedVariables);

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
                            blocks.Enqueue(function.ControlFlow[ifStatement.ThenBlock]);
                            blocks.Enqueue(function.ControlFlow[ifStatement.ElseBlock]);
                            break;
                        case GotoStatement gotoStatement:
                            blocks.Enqueue(function.ControlFlow[gotoStatement.GotoBlock]);
                            break;
                        case ReturnStatement _:
                            break;
                        default:
                            throw NonExhaustiveMatchException.For(block.Terminator);
                    }
                }
            }

            if (saveBorrowClaims) function.ControlFlow.BorrowClaims = claims;
        }

        private static bool VariableOwnsSharedValue(Variable variable, Claims outstandingClaims)
        {
            var ownerShip = outstandingClaims.OwnedBy(variable);
            // If it doesn't have ownership, then it doesn't own a shared value
            if (ownerShip == null) return false;
            return outstandingClaims.IsShared(ownerShip.Lifetime);
        }

        private Claims ClaimsBeforeBlock(
            FunctionDeclarationSyntax function,
            BasicBlock block,
            StatementClaims claims)
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
            foreach (var parameter in function.ControlFlow.Parameters
                .Where(v => v.Type is ReferenceType))
                claimsBeforeStatement.Add(new Borrows(parameter.Variable, NewLifetime()));

            return claimsBeforeStatement;
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
                    // no claims to acquire
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

                    AcquireReturnClaim(assignToPlace, callLifetime, outstandingClaims, claimsAfterStatement, variables);
                }
                break;
                case VirtualFunctionCall virtualFunctionCall:
                {
                    var callLifetime = NewLifetime();
                    var outstandingClaims = new Claims();
                    outstandingClaims.AddRange(claimsBeforeStatement);
                    if (virtualFunctionCall.Self != null)
                        AcquireClaim(callLifetime, virtualFunctionCall.Self, outstandingClaims);
                    foreach (var argument in virtualFunctionCall.Arguments)
                        AcquireClaim(callLifetime, argument, outstandingClaims);

                    AcquireReturnClaim(assignToPlace, callLifetime, outstandingClaims, claimsAfterStatement, variables);
                }
                break;
                default:
                    throw NonExhaustiveMatchException.For(value);
            }
        }

        private static void AcquireReturnClaim(
            Place assignToPlace,
            Lifetime callLifetime,
            Claims outstandingClaims,
            Claims claimsAfterStatement,
            FixedList<LocalVariableDeclaration> variables)
        {
            if (assignToPlace == null) return;

            var assignToVariable = VariableDeclaration(assignToPlace, variables);
            if (assignToVariable.Type is UserObjectType objectType
                && objectType.Mutability == Mutability.Mutable)
                AcquireBorrow(assignToVariable.Variable, callLifetime, claimsAfterStatement);
            else
                AcquireAlias(assignToVariable.Variable, callLifetime, claimsAfterStatement);

            // TODO For now, we just add all the call claims, this should be based on the function type instead
            claimsAfterStatement.AddRange(outstandingClaims);
        }

        private void AcquireOwnershipIfMoved(
            Place assignToPlace,
            FixedList<LocalVariableDeclaration> variables,
            Claims claimsAfterStatement)
        {
            if (assignToPlace == null) return;
            var assignToVariable = VariableDeclaration(assignToPlace, variables);
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

        private static LocalVariableDeclaration VariableDeclaration(Place assignToPlace, FixedList<LocalVariableDeclaration> variables)
        {
            return variables.Single(v => v.Variable == assignToPlace.CoreVariable());
        }

        private void AcquireClaim(
            IClaimHolder claimHolder,
            Operand operand,
            Claims outstandingClaims)
        {
            switch (operand)
            {
                case VariableReference varRef:
                    var lifetimeReferenced = outstandingClaims.LifetimeOf(varRef.Variable);
                    if (lifetimeReferenced == null)
                        break; // TODO this actually happens with things that should be copy etc. Handle correctly
                    var lifetime = lifetimeReferenced.Value;
                    switch (varRef.ValueSemantics)
                    {
                        case ValueSemantics.Borrow:
                        {
                            if (outstandingClaims.IsAliased(lifetime))
                                ReportDiagnostic(BorrowError.CantBorrowWhileAliased(file, operand.Span));
                            else if (!outstandingClaims.CurrentBorrower(lifetime).Equals(varRef.Variable))
                                ReportDiagnostic(BorrowError.CantBorrowWhileBorrowed(file, operand.Span));
                            else
                                outstandingClaims.Add(new Borrows(claimHolder, lifetime));
                        }
                        break;
                        case ValueSemantics.Alias:
                        {
                            if (!outstandingClaims.IsAliased(lifetime) &&
                                !outstandingClaims.CurrentBorrower(lifetime).Equals(varRef.Variable))
                                ReportDiagnostic(BorrowError.CantAliasWhileBorrowed(file, operand.Span));
                            else
                                outstandingClaims.Add(new Aliases(claimHolder, lifetime));
                        }
                        break;
                        case ValueSemantics.Own:
                        {
                            var currentOwner = outstandingClaims.OwnerOf(lifetime);
                            // Trying to move out of something that doesn't have ownership. We
                            // don't report an error, because that should already be reported by
                            // the type checker. We don't change claims, because ownership hasn't been
                            // changed.
                            if (currentOwner?.Holder != varRef.Variable)
                                break;
                            outstandingClaims.Remove(currentOwner);
                            switch (claimHolder)
                            {
                                case Variable variable:
                                    outstandingClaims.Add(new Owns(variable, lifetime));
                                    break;
                                case Lifetime _:
                                    // This occurs when we pass ownership as a function argument
                                    if (outstandingClaims.IsShared(lifetime))
                                        ReportDiagnostic(BorrowError.CantMoveIntoArgumentWhileShared(file, operand.Span));
                                    break;
                                default:
                                    throw NonExhaustiveMatchException.For(claimHolder);
                            }
                        }
                        break;
                        default:
                            throw NonExhaustiveMatchException.ForEnum(varRef.ValueSemantics);
                    }
                    break;
                case Place _:
                    throw new NotImplementedException();
                case Constant _:
                    // no claims to acquire
                    break;
                default:
                    throw NonExhaustiveMatchException.For(operand);
            }
        }

        private static void AcquireBorrow(
            IClaimHolder claimHolder,
            Lifetime lifetime,
            Claims outstandingClaims)
        {
            outstandingClaims.Add(new Borrows(claimHolder, lifetime));
        }

        private static void AcquireAlias(
            IClaimHolder claimHolder,
            Lifetime lifetime,
            Claims outstandingClaims)
        {
            outstandingClaims.Add(new Aliases(claimHolder, lifetime));
        }

        private void ReportDiagnostic(Diagnostic diagnostic)
        {
            // Presumably, if we have the exact same span, that means we reported this error before
            if (reportedDiagnosticSpans.Contains(diagnostic.Span)) return;

            reportedDiagnosticSpans.Add(diagnostic.Span);
            diagnostics.Add(diagnostic);
        }
    }
}
