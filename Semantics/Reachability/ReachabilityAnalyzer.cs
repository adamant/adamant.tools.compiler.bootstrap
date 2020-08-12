using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph;
using Adamant.Tools.Compiler.Bootstrap.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability
{
    /// <summary>
    /// Analyze reachability of references to determine memory safety violations and
    /// insert deletes.
    /// </summary>
    /// <remarks>Currently, all value types are primitive types or the optional
    /// type. For simplicity, the reachability analyzer currently ignores all value
    /// types except optional references. Note that value type expressions must
    /// still be analyzed because they might contain subexpressions on reference
    /// types.</remarks>
    public class ReachabilityAnalyzer
    {
        private readonly IConcreteCallableDeclarationSyntax callableDeclaration;
        private readonly CodeFile file;
        private readonly Diagnostics diagnostics;

        private ReachabilityAnalyzer(IConcreteCallableDeclarationSyntax callableDeclaration, Diagnostics diagnostics)
        {
            this.callableDeclaration = callableDeclaration;
            file = callableDeclaration.File;
            this.diagnostics = diagnostics;
        }

        public static void Analyze(
            FixedList<IConcreteCallableDeclarationSyntax> callableDeclarations,
            Diagnostics diagnostics)
        {
            foreach (var callableDeclaration in callableDeclarations)
                new ReachabilityAnalyzer(callableDeclaration, diagnostics).Analyze();
        }

        private void Analyze()
        {
            var graph = new ReachabilityGraph();
            var scope = CreateParameterScope(graph);
            foreach (var statement in callableDeclaration.Body.Statements)
                Analyze(statement, graph, scope);

            // TODO handle implicit return at end
        }

        private void Analyze(IStatementSyntax statement, ReachabilityGraph graph, VariableScope scope)
        {
            switch (statement)
            {
                default:
                    throw ExhaustiveMatch.Failed(statement);
                case IVariableDeclarationStatementSyntax stmt:
                {
                    var initializer = AnalyzeAssignmentSource(stmt.Initializer, graph, scope);
                    var variable = VariableDeclared(stmt, graph, scope);
                    // TODO this variable's references effectively go away when it is no longer live
                    // TODO how does the idea of in use variables work with variables?
                    graph.Assign(variable, initializer);
                    if (!stmt.VariableIsLiveAfter)
                        // Variable is dead, effectively it can be removed
                        variable?.Dead();
                }
                break;
                case IExpressionStatementSyntax stmt:
                    Analyze(stmt.Expression, graph, scope);
                    break;
                case IResultStatementSyntax exp:
                    // TODO deal with passing the result to the block
                    Analyze(exp.Expression, graph, scope);
                    break;
            }
        }

        /// <summary>
        /// Analyze an expression to apply its effects reachability graph.
        /// </summary>
        /// <returns>The place of the object resulting from evaluating this expression or null
        /// if the there is no result or the result is not an object reference.</returns>
        private TempValue? Analyze(IExpressionSyntax? expression, ReachabilityGraph graph, VariableScope scope)
        {
            if (expression is null) return null;
            var referenceType = expression.DataType.Known().UnderlyingReferenceType();
            var isReferenceType = !(referenceType is null);
            switch (expression)
            {
                default:
                    throw ExhaustiveMatch.Failed(expression);
                case IAssignmentExpressionSyntax exp:
                {
                    var leftPlace = AnalyzeAssignmentPlace(exp.LeftOperand, graph, scope);
                    var rightPlace = AnalyzeAssignmentSource(exp.RightOperand, graph, scope);
                    graph.Assign(leftPlace, rightPlace);
                    return null;
                }
                case ISelfExpressionSyntax exp:
                    throw new InvalidOperationException($"`self` reference not wrapped in move, borrow, or share");
                case IMoveExpressionSyntax exp:
                {
                    _ = referenceType ?? throw new InvalidOperationException("Can't move value type");

                    // The referent should be a name or `self` so we don't need to evaluate it
                    var variable = graph.GetVariableFor(exp.MovedSymbol.Assigned());
                    var temp = TempValue.For(graph, exp);
                    temp?.MoveFrom(variable);
                    graph.Add(temp);
                    return temp;
                }
                case IBorrowExpressionSyntax exp:
                {
                    _ = referenceType ?? throw new InvalidOperationException("Can't borrow value type");

                    // If there is a variable, it is a simple borrow expression
                    var variable = graph.TryGetVariableFor(exp.BorrowedFromBinding.Assigned());
                    if (!(variable is null))
                    {
                        var temp = TempValue.For(graph, exp);
                        temp?.BorrowFrom(variable);
                        graph.Add(temp);
                        return temp;
                    }

                    // Can this happen?
                    throw new NotImplementedException();
                    //var temp = Analyze(exp.)
                    //var temp = graph.NewTempValue(referenceType);
                    //temp.BorrowFrom(variable);
                    //return temp;
                }
                case IShareExpressionSyntax exp:
                {
                    _ = referenceType ?? throw new InvalidOperationException("Can't share value type");

                    // The referent should be a name or `self` so we don't need to evaluate it
                    var variable = graph.TryGetVariableFor(exp.SharedMetadata.Assigned());
                    if (!(variable is null))
                    {
                        var temp = TempValue.For(graph, exp);
                        temp?.ShareFrom(variable);
                        graph.Add(temp);
                        return temp;
                    }
                    else
                    {
                        // It must be a field or something
                        var source = Analyze(exp.Referent, graph, scope)!;
                        var temp = TempValue.For(graph, exp);
                        temp?.ShareFrom(source);
                        graph.Add(temp);
                        return temp;
                    }
                }
                case INameExpressionSyntax exp:
                    if (!isReferenceType) return null;
                    throw new InvalidOperationException(
                        $"Reference type name `{exp}` not wrapped in move, borrow, or share");
                case IBinaryOperatorExpressionSyntax exp:
                {
                    Analyze(exp.LeftOperand, graph, scope);
                    Analyze(exp.RightOperand, graph, scope);
                    // All binary operators result in value types
                    return null;
                }
                case IUnaryOperatorExpressionSyntax exp:
                {
                    Analyze(exp.Operand, graph, scope);
                    // All unary operators result in value types
                    return null;
                }
                case IFieldAccessExpressionSyntax exp:
                {
                    // Value type fields act like local variables
                    if (!isReferenceType && exp.ContextExpression is ISelfExpressionSyntax) return null;

                    // Field access behaves like calling a get method
                    // Note: right now only objects (not values) have fields
                    var context = Analyze(exp.ContextExpression, graph, scope)!;
                    UseArgument(context, exp.ContextExpression.Span, graph);
                    if (isReferenceType)
                    {
                        var temp = graph.AddFieldAccess(exp)!;
                        CaptureArguments(context.YieldValue(), temp);
                        return temp;
                    }

                    // Temp dropped because it wasn't captured
                    graph.Drop(context);
                    return null;
                }
                case IFunctionInvocationExpressionSyntax exp:
                {
                    var arguments = exp.Arguments
                                       .Select(a => Analyze(a.Expression, graph, scope))
                                       .ToFixedList();
                    var function = exp.FunctionNameSyntax.ReferencedFunctionMetadata.Assigned();
                    var parameters = function.Parameters;
                    UseArguments(arguments, exp.Arguments, parameters, graph);
                    if (!(referenceType is null))
                        return CaptureArguments(exp, arguments, graph);

                    // All the arguments are dropped because they aren't captured
                    graph.Drop(arguments);
                    return null;
                }
                case IMethodInvocationExpressionSyntax exp:
                {
                    var selfArgument = Analyze(exp.ContextExpression, graph, scope);
                    var arguments = exp.Arguments.Select(a => Analyze(a.Expression, graph, scope)).ToFixedList();
                    var method = exp.MethodNameSyntax.ReferencedFunctionMetadata.Assigned();
                    var parameters = method.Parameters;
                    if (!(selfArgument is null))
                        UseArgument(selfArgument, exp.ContextExpression.Span, graph);
                    UseArguments(arguments, exp.Arguments, parameters, graph);
                    if (!(referenceType is null))
                        return CaptureArguments(exp, arguments.Prepend(selfArgument), graph);

                    // All the arguments are dropped because they aren't captured
                    graph.Drop(selfArgument);
                    graph.Drop(arguments);
                    return null;
                }
                case IUnsafeExpressionSyntax exp:
                    return Analyze(exp.Expression, graph, scope);
                case IBlockExpressionSyntax exp:
                    return AnalyzeBlock(exp, graph, scope);
                case IReturnExpressionSyntax exp:
                {
                    var temp = Analyze(exp.ReturnValue, graph, scope);
                    if (!(temp is null))
                    {
                        // TODO make a deep copy of the graph so the existing graph is intact
                        // create a new temp with the correct reference capabilities to the value
                        var returnValue = graph.AddReturnValue(exp, callableDeclaration.ReturnDataType)!;
                        returnValue.AssignFrom(temp, returnValue.ReferenceType.ReferenceCapability);
                        // Exit the function, releasing all temps and variables except the returned value
                        graph.ExitFunction(returnValue);
                        if (returnValue.PossibleReferents.Any(r => !r.IsAllocated))
                            diagnostics.Add(BorrowError.ValueDoesNotLiveLongEnough(file, exp.ReturnValue!.Span));
                    }
                    return null;
                }
                case INewObjectExpressionSyntax exp:
                {
                    var arguments = exp.Arguments.Select(a => Analyze(a.Expression, graph, scope)).ToFixedList();

                    var constructor = exp.ReferencedConstructor.Assigned();
                    var parameters = constructor.Parameters;
                    UseArguments(arguments, exp.Arguments, parameters, graph);
                    if (referenceType is null) return null;
                    var obj = graph.AddObject(exp)!;
                    CaptureArguments(arguments, obj);
                    return obj;
                }
                case IStringLiteralExpressionSyntax exp:
                {
                    if (referenceType is null) return null;

                    // Create an untethered context object that doesn't need released
                    var temp = TempValue.ForNewContextObject(graph, exp);
                    graph.Add(temp);
                    return temp;
                }
                case IImplicitNumericConversionExpressionSyntax exp:
                    return Analyze(exp.Expression, graph, scope);
                case IIntegerLiteralExpressionSyntax _:
                case IBoolLiteralExpressionSyntax _:
                case INoneLiteralExpressionSyntax _:
                    return null;
                case IImplicitImmutabilityConversionExpressionSyntax exp:
                    // TODO does this need to be handled specially?
                    return Analyze(exp.Expression, graph, scope);
                case IIfExpressionSyntax exp:
                {
                    Analyze(exp.Condition, graph, scope);
                    AnalyzeBlock(exp.ThenBlock, graph, scope);
                    Analyze(exp.ElseClause, graph, scope);
                    // Assuming void for now
                    return null;
                }
                case ILoopExpressionSyntax exp:
                {
                    // TODO deal with flow analysis
                    AnalyzeBlock(exp.Block, graph, scope);
                    // Assuming void for now
                    return null;
                }
                case IWhileExpressionSyntax exp:
                {
                    Analyze(exp.Condition, graph, scope);
                    // TODO deal with flow analysis
                    AnalyzeBlock(exp.Block, graph, scope);
                    // Assuming void for now
                    return null;
                }
                case IForeachExpressionSyntax exp:
                {
                    // TODO deal with flow analysis
                    Analyze(exp.InExpression, graph, scope);
                    return AnalyzeBlock(exp.Block, graph, scope);
                }
                case IBreakExpressionSyntax _:
                case INextExpressionSyntax _:
                    // TODO deal with control flow effects
                    return null;
                case IImplicitNoneConversionExpressionSyntax exp:
                    Analyze(exp.Expression, graph, scope);
                    // TODO Is there a chance this needs something done?
                    return null;
                case IImplicitOptionalConversionExpressionSyntax exp:
                    return Analyze(exp.Expression, graph, scope);
            }
        }

        private void UseArguments(
            FixedList<TempValue?> arguments,
            FixedList<IArgumentSyntax> argumentSyntaxes,
            IEnumerable<IBindingMetadata> parameters,
            ReachabilityGraph graph)
        {
            foreach (var ((argument, argumentSyntax), parameter) in arguments.Zip(argumentSyntaxes).Zip(parameters))
            {
                if (argument is null) continue;
                if (!(parameter.DataType is ReferenceType))
                    throw new InvalidOperationException($"Expected parameter {parameter} to be a reference type");

                UseArgument(argument, argumentSyntax.Span, graph);
            }
        }

        private void UseArgument(TempValue argument, TextSpan span, ReachabilityGraph graph)
        {
            foreach (var reference in argument.References)
            {
                // Check if we are safe to use this reference
                switch (reference.DeclaredAccess)
                {
                    default:
                        throw ExhaustiveMatch.Failed(reference.DeclaredAccess);
                    case Access.Mutable:
                    {
                        // Must be no read-only access
                        if (reference.Referent.GetCurrentAccess() == Access.ReadOnly)
                            diagnostics.Add(BorrowError.CantBorrowWhileShared(file, span));

                        // And we must be a borrower from someone who has the mutable access
                        var originOfMutability = reference.Referent.OriginOfMutability;
                        if (originOfMutability is null || !originOfMutability.IsOriginFor(reference))
                            diagnostics.Add(BorrowError.CantBorrowFromThisReference(file, span));
                        // And it can't be used for borrow already
                        else if (originOfMutability.IsUsedForBorrowExceptBy(reference))
                            diagnostics.Add(BorrowError.CantBorrowWhileBorrowed(file, span));
                    }
                    break;
                    case Access.ReadOnly:
                    {
                        if (reference.Referent.GetCurrentAccess() == Access.ReadOnly) break;

                        // Just because it is currently mutable doesn't mean we can't make it readonly
                        var originOfMutability = reference.Referent.OriginOfMutability;
                        if (originOfMutability?.IsUsedForBorrow() ?? false)
                            diagnostics.Add(BorrowError.CantShareWhileBorrowed(file, span));
                    }
                    break;
                    case Access.Identify:
                        // Always safe to take reference identity
                        break;
                }

                // Mark as used regardless to enable correct analysis of later expressions
                reference.Use(graph);
            }
        }

        private static TempValue CaptureArguments(
            IInvocationExpressionSyntax exp,
            IEnumerable<TempValue?> arguments,
            ReachabilityGraph graph)
        {
            var referenceType = exp.DataType.Known().UnderlyingReferenceType();
            if (referenceType is null)
                throw new ArgumentException($"{nameof(CaptureArguments)} only supports reference returning functions");

            var temp = graph.AddFunctionCall(exp)!;
            CaptureArguments(arguments, temp);
            return temp;
        }

        private static void CaptureArguments(IEnumerable<TempValue?> arguments, TempValue temp)
        {
            var referent = temp!.PossibleReferents.Single();
            foreach (var argument in arguments)
                if (!(argument is null))
                {
                    // TODO base this on reachability expressions instead of capturing every argument
                    referent.Capture(argument);
                }
        }

        private TempValue? Analyze(IElseClauseSyntax? clause, ReachabilityGraph graph, VariableScope scope)
        {
            return clause switch
            {
                null => null,
                IBlockExpressionSyntax exp => AnalyzeBlock(exp, graph, scope),
                IIfExpressionSyntax exp => Analyze((IExpressionSyntax)exp, graph, scope),
                IResultStatementSyntax exp => Analyze(exp.Expression, graph, scope),
                _ => throw ExhaustiveMatch.Failed(clause)
            };
        }

        private TempValue? AnalyzeBlock(IBlockOrResultSyntax blockOrResult, ReachabilityGraph graph, VariableScope scope)
        {
            switch (blockOrResult)
            {
                default:
                    throw ExhaustiveMatch.Failed(blockOrResult);
                case IBlockExpressionSyntax exp:
                {
                    var nestedScope = new VariableScope(scope);

                    foreach (var statement in exp.Statements)
                        Analyze(statement, graph, nestedScope);
                    foreach (var variable in nestedScope.Variables)
                        graph.EndVariableScope(variable);

                    // Assuming void return from blocks for now
                    return null;
                }
                case IResultStatementSyntax exp:
                    return Analyze(exp.Expression, graph, scope);
            }
        }

        private Variable? AnalyzeAssignmentPlace(IAssignableExpressionSyntax expression, ReachabilityGraph graph, VariableScope scope)
        {
            var isReferenceType = !(expression.DataType.Known().UnderlyingReferenceType() is null);
            switch (expression)
            {
                default:
                    throw ExhaustiveMatch.Failed(expression);
                case IFieldAccessExpressionSyntax exp:
                {
                    var variable = graph.TryGetVariableFor(exp.ReferencedBinding.Assigned());
                    if (!(variable is null)) return variable;

                    if (!isReferenceType && exp.ContextExpression is ISelfExpressionSyntax) return null;

                    var context = Analyze(exp.ContextExpression, graph, scope);
                    if (!isReferenceType)
                    {
                        graph.Drop(context);
                        return null;
                    }

                    throw new NotImplementedException(
                        $"{nameof(AnalyzeAssignmentPlace)}(expression) not implemented for {expression.GetType().Name} other than `self`");
                }
                case INameExpressionSyntax exp:
                {
                    return isReferenceType ? graph.GetVariableFor(exp.ReferencedBinding.Assigned()) : null;
                }
            }
        }

        /// <summary>
        /// Analyze whether the expression can be used as the right hand side of
        /// an assignment.
        /// </summary>
        private TempValue? AnalyzeAssignmentSource(
            IExpressionSyntax? expression,
            ReachabilityGraph graph,
            VariableScope scope)
        {
            if (expression is null) return null;

            var place = Analyze(expression, graph, scope);

            // An owned value is always safe to use
            if (place is null || place.References.All(r => r.CouldHaveOwnership))
                return place;

            // We aren't using the reference yet, but it still needs to be a reference
            // to something that hasn't been released.
            if (place.PossibleReferents.Any(p => !p.IsAllocated))
                diagnostics.Add(BorrowError.SharedValueDoesNotLiveLongEnough(file, expression.Span, null));

            return place;
        }

        /// <summary>
        /// Create both the caller and parameter scope with the correct relationships
        /// between the parameters and the callers.
        /// </summary>
        private VariableScope CreateParameterScope(ReachabilityGraph graph)
        {
            var parameterScope = new VariableScope();

            CreateSelfParameter(graph, parameterScope);
            foreach (var parameter in callableDeclaration.Parameters)
                CreateParameter(parameter, graph, parameterScope);

            return parameterScope;
        }

        private static void CreateParameter(
            IParameterSyntax parameter,
            ReachabilityGraph graph,
            VariableScope parameterScope)
        {
            var localVariable = graph.AddParameter(parameter);
            if (!(localVariable is null))
                parameterScope.VariableDeclared(localVariable.Symbol);
        }

        private void CreateSelfParameter(ReachabilityGraph graph, VariableScope parameterScope)
        {
            IClassDeclarationSyntax declaringClass;
            ISelfParameterSyntax selfParameter;
            switch (callableDeclaration)
            {
                default:
                    throw ExhaustiveMatch.Failed(callableDeclaration);
                case IFunctionDeclarationSyntax _:
                case IAssociatedFunctionDeclarationSyntax _:
                    return;
                case IConcreteMethodDeclarationSyntax method:
                    declaringClass = method.DeclaringClass;
                    selfParameter = method.SelfParameter;
                    break;
                case IConstructorDeclarationSyntax constructor:
                    declaringClass = constructor.DeclaringClass;
                    selfParameter = constructor.ImplicitSelfParameter;
                    break;
            }

            CreateFields(declaringClass, graph);
            CreateParameter(selfParameter, graph, parameterScope);
        }

        private static void CreateFields(IClassDeclarationSyntax declaringClass, ReachabilityGraph graph)
        {
            // TODO should fields have their own scope outside of parameter scope?
            foreach (var field in declaringClass.Members.OfType<IFieldDeclarationSyntax>())
                graph.AddField(field);
        }

        private static Variable? VariableDeclared(
            IBindingMetadata bindingSymbol,
            ReachabilityGraph graph,
            VariableScope scope)
        {
            var variable = graph.AddVariable(bindingSymbol);
            if (!(variable is null))
                scope.VariableDeclared(variable.Symbol);
            return variable;
        }
    }
}
