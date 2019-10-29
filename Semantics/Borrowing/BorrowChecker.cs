using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.Borrowing;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Borrowing
{
    /// <summary>
    /// Check for borrow errors
    /// </summary>
    ///
    // Plan for inserting deletes:
    // ---------------------------
    // When generating the CFG, output statements for when variables go out of scope. Then track another level
    // of liveness between alive and dead. This would be a new state, perhaps called "liminal" or "pending", which
    // indicated that the variable would not be used again, but may need to exist as the owner of something borrowed
    // until all outstanding borrows are resolved. Delete statements could then be inserted after borrow checking
    // at the point where values are no longer used. The variable leaving scope statements could then be ignored
    // or removed.
    // TODO inserting deletes based only on liveness led to issues.
    // There are cases when a variable is no longer directly used, but another variable has borrowed the value.
    // The owner then shows as dead, so we inserted the delete. But really, we needed to wait until the
    // borrow is gone to delete it. However, we can't just wait for all borrows to be dead to insert a delete
    // because things are guaranteed to be deleted when the owner goes out of scope. A borrow that extends
    // beyond the scope should be an error. We had been detecting this by noticing the borrow claim extended
    // past the delete statement. That worked, because the owning variable must always be dead after the scope
    // because there are no references to it outside the scope.
    public class BorrowChecker
    {
        private readonly CodeFile file;
        private readonly FixedDictionary<ICallableDeclaration, LiveVariables> liveness;
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
            FixedDictionary<ICallableDeclaration, LiveVariables> liveness,
            Diagnostics diagnostics,
            bool saveBorrowClaims)
        {
            this.file = file;
            this.liveness = liveness;
            this.diagnostics = diagnostics;
            this.saveBorrowClaims = saveBorrowClaims;
        }

        public static void Check(
            IEnumerable<ICallableDeclaration> callables,
            FixedDictionary<ICallableDeclaration, LiveVariables> liveness,
            Diagnostics diagnostics,
            bool saveBorrowClaims)
        {
            foreach (var callable in callables)
            {
                var borrowChecker = new BorrowChecker(callable.ControlFlow.File, liveness, diagnostics, saveBorrowClaims);
                borrowChecker.Check(callable);
            }
        }

        public void Check(ICallableDeclaration callable)
        {
            switch (callable)
            {
                default:
                    throw ExhaustiveMatch.Failed(callable);
                case FunctionDeclaration function:
                    Check(function, function.ControlFlow);
                    break;
                case ConstructorDeclaration constructor:
                    Check(constructor, constructor.ControlFlow);
                    break;
            }
        }

        private static void Check(IFieldDeclarationSyntax _field)
        {
            // Currently nothing to check
        }

        private void Check(ICallableDeclaration callable, ControlFlowGraph controlFlow)
        {
            var variables = controlFlow.VariableDeclarations;
            var liveVariables = liveness[callable];
            // Do borrow checking with claims
            var blocks = new Queue<BasicBlock>();
            blocks.Enqueue(controlFlow.EntryBlock);
            // Compute parameter claims once in case for some reason we process the entry block repeatedly
            var parameterClaims = AcquireParameterClaims(controlFlow);
            var claims = new StatementClaims(parameterClaims);
            var insertedDeletes = new InsertedDeletes();

            while (blocks.TryDequeue(out var block))
            {
                var claimsBeforeStatement = GetClaimsBeforeBlock(controlFlow, block, claims);

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
                            var variablesInScope = controlFlow.VariableDeclarations
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
                                var declaration = controlFlow.VariableDeclarations
                                                            .Single(d => d.Variable == variable);
                                ReportDiagnostic(BorrowError.SharedValueDoesNotLiveLongEnough(file, exitScopeStatement.Span, declaration.Name));
                            }
                            break;
                        default:
                            throw NonExhaustiveMatchException.For(statement);
                    }

                    var deletedVariables = ReleaseDeadClaims(claimsAfterStatement, liveVariables.After(statement));
                    InsertDeletes(statement, variables, deletedVariables, insertedDeletes);

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
                            blocks.Enqueue(controlFlow[ifStatement.ThenBlock]);
                            blocks.Enqueue(controlFlow[ifStatement.ElseBlock]);
                            break;
                        case GotoStatement gotoStatement:
                            blocks.Enqueue(controlFlow[gotoStatement.GotoBlock]);
                            break;
                        case ReturnStatement _:
                            break;
                        default:
                            throw NonExhaustiveMatchException.For(block.Terminator);
                    }
                }
            }

            controlFlow.InsertedDeletes = insertedDeletes;
            if (saveBorrowClaims)
                controlFlow.BorrowClaims = claims;
        }

        private Claims GetClaimsBeforeBlock(
            ControlFlowGraph controlFlow,
            BasicBlock block,
            StatementClaims claims)
        {
            var claimsBeforeStatement = new Claims();

            if (block == controlFlow.EntryBlock)
                claimsBeforeStatement.AddRange(claims.ParameterClaims);

            foreach (var predecessor in controlFlow.Edges.To(block).Select(b => b.Terminator))
                claimsBeforeStatement.AddRange(claims.After(predecessor));

            return claimsBeforeStatement;
        }

        private static List<Variable> ReleaseDeadClaims(
            Claims claimsAfterStatement,
            BitArray liveAfter)
        {
            var deadVariables = liveAfter.FalseIndexes().Select(i => new Variable(i));
            var deadVariablesOwnership = deadVariables.ToDictionary(v => v, claimsAfterStatement.OwnedBy);

            // First release dead variables that don't own their value
            var deadVariableShares = deadVariablesOwnership.Where(e => e.Value == null)
                .Select(e => e.Key);
            claimsAfterStatement.Release(deadVariableShares);

            // Now that any shares have been released, release ownership claims. This must be done
            // second because it depends on what shares are outstanding.
            var deadVariableOwns = deadVariablesOwnership
                .Where(e => e.Value != null && !claimsAfterStatement.IsShared(e.Value.Lifetime))
                .Select(e => e.Key).ToList();
            claimsAfterStatement.Release(deadVariableOwns);
            return deadVariableOwns;
        }

        private void InsertDeletes(
            ExpressionStatement statement,
            FixedList<VariableDeclaration> variables,
            List<Variable> deletedVariables,
            InsertedDeletes insertedDeletes)
        {
            foreach (var variable in deletedVariables)
            {
                var declaration = variables.Single(v => v.Variable == variable);
                var span = statement.Span.AtEnd();
                // We may be in a different scope than the variable declaration.
                // For example, the last use may be in a nested scope
                insertedDeletes.AddDeleteAfter(
                    statement,
                    declaration.Reference(span).AsOwn(span),
                    (UserObjectType)declaration.Type,
                    span,
                    statement.Scope);
            }
        }

        private static VariableDeclaration GetVariableDeclaration(IPlace assignToPlace, FixedList<VariableDeclaration> variables)
        {
            return variables.Single(v => v.Variable == assignToPlace.CoreVariable());
        }

        private void CheckStatement(
            IPlace assignToPlace,
            IValue value,
            Claims claimsBeforeStatement,
            Claims claimsAfterStatement,
            FixedList<VariableDeclaration> variables)
        {
            switch (value)
            {
                case ConstructorCall constructorCall:
                    foreach (var argument in constructorCall.Arguments)
                        AcquireClaim(assignToPlace?.CoreVariable(), argument, claimsAfterStatement, claimsAfterStatement);
                    // We have made a new object, assign it a new id
                    var objectId = NewLifetime();
                    // Variable acquires title on any new objects
                    AcquireOwnership(assignToPlace.CoreVariable(), objectId, claimsAfterStatement);
                    break;
                case UnaryOperation unaryOperation:
                    AcquireClaim(assignToPlace?.CoreVariable(), unaryOperation.Operand, claimsAfterStatement, claimsAfterStatement);
                    break;
                case BinaryOperation binaryOperation:
                    AcquireClaim(assignToPlace?.CoreVariable(), binaryOperation.LeftOperand, claimsAfterStatement, claimsAfterStatement);
                    AcquireClaim(assignToPlace?.CoreVariable(), binaryOperation.RightOperand, claimsAfterStatement, claimsAfterStatement);
                    break;
                case IntegerConstant _:
                    // no claims to acquire
                    break;
                case IOperand operand:
                    AcquireClaim(assignToPlace?.CoreVariable(), operand, claimsAfterStatement, claimsAfterStatement);
                    break;
                case FunctionCall functionCall:
                {
                    var callLifetime = NewLifetime();
                    var outstandingClaims = new Claims();
                    outstandingClaims.AddRange(claimsBeforeStatement);
                    if (functionCall.Self != null)
                        AcquireClaim(callLifetime, functionCall.Self, outstandingClaims, claimsAfterStatement);
                    foreach (var argument in functionCall.Arguments)
                        AcquireClaim(callLifetime, argument, outstandingClaims, claimsAfterStatement);

                    AcquireReturnClaim(assignToPlace, callLifetime, outstandingClaims, claimsAfterStatement, variables);
                }
                break;
                case VirtualFunctionCall virtualFunctionCall:
                {
                    var callLifetime = NewLifetime();
                    var outstandingClaims = new Claims();
                    outstandingClaims.AddRange(claimsBeforeStatement);
                    if (virtualFunctionCall.Self != null)
                        AcquireClaim(callLifetime, virtualFunctionCall.Self, outstandingClaims, claimsAfterStatement);
                    foreach (var argument in virtualFunctionCall.Arguments)
                        AcquireClaim(callLifetime, argument, outstandingClaims, claimsAfterStatement);

                    AcquireReturnClaim(assignToPlace, callLifetime, outstandingClaims, claimsAfterStatement, variables);
                }
                break;
                case FieldAccess fieldAccess:
                {
                    var instanceVariable = fieldAccess.CoreVariable();
                    var instanceLifetime = claimsBeforeStatement.ClaimBy(instanceVariable).Lifetime;
                    var fieldLifetime = NewLifetime();
                    var outstandingClaims = new Claims();
                    outstandingClaims.AddRange(claimsBeforeStatement);
                    // TODO this should be based on the field type
                    AcquireAlias(instanceLifetime, fieldLifetime, outstandingClaims);
                    // TODO this should be based on the access mode
                    AcquireAlias(assignToPlace?.CoreVariable(), fieldLifetime, outstandingClaims);
                    claimsAfterStatement.AddRange(outstandingClaims);
                }
                break;
                case ConstructSome constructSome:
                {
                    AcquireClaim(assignToPlace?.CoreVariable(), constructSome.Value, claimsAfterStatement, claimsAfterStatement);
                }
                break;
                default:
                    throw NonExhaustiveMatchException.For(value);
            }
        }

        private Claims AcquireParameterClaims(ControlFlowGraph controlFlow)
        {
            var claimsBeforeStatement = new Claims();
            foreach (var parameter in controlFlow.Parameters
                .Where(v => v.Type is ReferenceType))
                claimsBeforeStatement.Add(new Borrows(parameter.Variable, NewLifetime()));

            return claimsBeforeStatement;
        }

        private static void AcquireReturnClaim(
            IPlace? assignToPlace,
            Lifetime callLifetime,
            Claims outstandingClaims,
            Claims claimsAfterStatement,
            FixedList<VariableDeclaration> variables)
        {
            if (assignToPlace == null)
                return;

            var variableDeclaration = GetVariableDeclaration(assignToPlace, variables);
            var assignToVariable = variableDeclaration.Variable;
            if (variableDeclaration.Type is ReferenceType objectType)
            {
                if (objectType.IsOwned)
                    AcquireOwnership(assignToVariable, callLifetime, claimsAfterStatement);
                else if (objectType.Mutability == Mutability.Mutable)
                    AcquireBorrow(assignToVariable, callLifetime, claimsAfterStatement);
                else
                    AcquireAlias(assignToVariable, callLifetime, claimsAfterStatement);
            }
            else
                AcquireAlias(assignToVariable, callLifetime, claimsAfterStatement);

            // TODO For now, we just add all the call claims, this should be based on the function type instead
            claimsAfterStatement.AddRange(outstandingClaims);
        }

        private void AcquireClaim(
            IClaimHolder claimHolder,
            IOperand operand,
            Claims outstandingClaims,
            Claims claimsAfterStatement)
        {
            switch (operand)
            {
                default:
                    throw ExhaustiveMatch.Failed(operand);
                case VariableReference varRef:
                {
                    var lifetimeReferenced = outstandingClaims.LifetimeOf(varRef.Variable);
                    if (lifetimeReferenced == null)
                        break; // TODO this actually happens with things that should be copy etc. Handle correctly
                    var lifetime = lifetimeReferenced.Value;
                    switch (varRef.ValueSemantics)
                    {
                        default:
                            throw ExhaustiveMatch.Failed(varRef.ValueSemantics);
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
                            if (!outstandingClaims.IsAliased(lifetime)
                                && !outstandingClaims.CurrentBorrower(lifetime).Equals(varRef.Variable))
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
                            // Because claims are melded from multiple passes, the claims after statement
                            // would still contain this claim even though we removed it from outstanding
                            // claims. Thus we must remove it from claimsAfterStatement because this
                            // statement always takes ownership of the value.
                            claimsAfterStatement.Remove(currentOwner);
                            switch (claimHolder)
                            {
                                case Variable variable:
                                    AcquireOwnership(variable, lifetime, outstandingClaims);
                                    break;
                                case Lifetime _:
                                    // This occurs when we pass ownership as a function argument
                                    if (outstandingClaims.IsShared(lifetime))
                                        ReportDiagnostic(BorrowError.CantMoveIntoArgumentWhileShared(file, operand.Span));
                                    break;
                                default:
                                    throw ExhaustiveMatch.Failed(claimHolder);
                            }
                        }
                        break;
                        case ValueSemantics.LValue:
                        case ValueSemantics.Empty:
                        case ValueSemantics.Move:
                        case ValueSemantics.Copy:
                            throw new NotImplementedException();

                    }
                }
                break;
                case Constant _:
                    // no claims to acquire
                    break;
            }
        }

        private static void AcquireOwnership(
            Variable claimHolder,
            Lifetime lifetime,
            Claims outstandingClaims)
        {
            outstandingClaims.Add(new Owns(claimHolder, lifetime));
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
            if (reportedDiagnosticSpans.Contains(diagnostic.Span))
                return;

            reportedDiagnosticSpans.Add(diagnostic.Span);
            diagnostics.Add(diagnostic);
        }
    }
}
