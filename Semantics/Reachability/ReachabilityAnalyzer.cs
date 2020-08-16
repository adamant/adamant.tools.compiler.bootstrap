using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph;
using Adamant.Tools.Compiler.Bootstrap.Symbols.Trees;
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
        private readonly IExecutableDeclaration declaration;
        private readonly ISymbolTree symbolTree;
        private readonly CodeFile file;
        private readonly Diagnostics diagnostics;

        private ReachabilityAnalyzer(
            IExecutableDeclaration declaration,
            ISymbolTree symbolTree,
            Diagnostics diagnostics)
        {
            this.declaration = declaration;
            this.symbolTree = symbolTree;
            file = declaration.File;
            this.diagnostics = diagnostics;
        }

        public static void Analyze(
            FixedSet<IExecutableDeclaration> declarations,
            ISymbolTree symbolTree,
            Diagnostics diagnostics)
        {
            foreach (var declaration in declarations)
                new ReachabilityAnalyzer(declaration, symbolTree, diagnostics).Analyze();
        }

        private void Analyze()
        {
            var graph = new ReachabilityGraph();
            var scope = CreateParameterScope(graph);
            switch (declaration)
            {
                default:
                    throw ExhaustiveMatch.Failed(declaration);
                case IConcreteInvocableDeclaration syn:
                    foreach (var statement in syn.Body.Statements)
                        Analyze(statement, graph, scope);
                    break;
                case IFieldDeclaration _:
                    // TODO support field initializers
                    //Analyze(syn.Initializer, graph, scope);
                    break;
            }
            // TODO handle implicit return at end
        }

        private void Analyze(IStatement statement, ReachabilityGraph graph, VariableScope scope)
        {
            switch (statement)
            {
                default:
                    throw ExhaustiveMatch.Failed(statement);
                case IVariableDeclarationStatement stmt:
                {
                    var initializer = AnalyzeAssignmentSource(stmt.Initializer, graph, scope);
                    var variable = VariableDeclared(stmt, graph, scope);
                    // TODO this variable's references effectively go away when it is no longer live
                    // TODO how does the idea of in use variables work with variables?
                    graph.Assign(variable, initializer);
                    if (!stmt.VariableIsLiveAfter.Result)
                        // Variable is dead, effectively it can be removed
                        variable?.Dead();
                }
                break;
                case IExpressionStatement stmt:
                    Analyze(stmt.Expression, graph, scope);
                    break;
                case IResultStatement exp:
                    // TODO deal with passing the result to the block
                    Analyze(exp.Expression, graph, scope);
                    break;
            }
        }

        /// <summary>
        /// Analyze an expression to apply its effects to the reachability graph.
        /// </summary>
        /// <returns>The place of the object resulting from evaluating this expression or null
        /// if the there is no result or the result is not an object reference.</returns>
        private TempValue? Analyze(IExpression? expression, ReachabilityGraph graph, VariableScope scope)
        {
            if (expression is null) return null;
            var referenceType = expression.DataType.Known().UnderlyingReferenceType();
            var isReferenceType = !(referenceType is null);
            switch (expression)
            {
                default:
                    throw ExhaustiveMatch.Failed(expression);
                case IAssignmentExpression exp:
                {
                    var leftPlace = AnalyzeAssignmentPlace(exp.LeftOperand, graph, scope);
                    var rightPlace = AnalyzeAssignmentSource(exp.RightOperand, graph, scope);
                    graph.Assign(leftPlace, rightPlace);
                    return null;
                }
                case ISelfExpression exp:
                    throw new InvalidOperationException($"`self` reference not wrapped in move, borrow, or share");
                case IMoveExpression exp:
                {
                    _ = referenceType ?? throw new InvalidOperationException("Can't move value type");

                    // The referent should be a name or `self` so we don't need to evaluate it
                    var variable = graph.GetVariableFor(exp.ReferencedSymbol);
                    var temp = TempValue.For(graph, exp);
                    temp?.MoveFrom(variable);
                    graph.Add(temp);
                    return temp;
                }
                case IBorrowExpression exp:
                {
                    _ = referenceType ?? throw new InvalidOperationException("Can't borrow value type");

                    // If there is a variable, it is a simple borrow expression
                    var variable = graph.TryGetVariableFor(exp.ReferencedSymbol);
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
                case IShareExpression exp:
                {
                    _ = referenceType ?? throw new InvalidOperationException("Can't share value type");

                    // The referent should be a name or `self` so we don't need to evaluate it
                    var variable = graph.TryGetVariableFor(exp.ReferencedSymbol);
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
                case INameExpression exp:
                    if (!isReferenceType) return null;
                    throw new InvalidOperationException(
                        $"Reference type name `{exp}` not wrapped in move, borrow, or share");
                case IBinaryOperatorExpression exp:
                {
                    Analyze(exp.LeftOperand, graph, scope);
                    Analyze(exp.RightOperand, graph, scope);
                    // All binary operators result in value types
                    return null;
                }
                case IUnaryOperatorExpression exp:
                {
                    Analyze(exp.Operand, graph, scope);
                    // All unary operators result in value types
                    return null;
                }
                case IFieldAccessExpression exp:
                {
                    // Value type fields act like local variables
                    if (!isReferenceType && exp.Context is ISelfExpression) return null;

                    // Field access behaves like calling a get method
                    // Note: right now only objects (not values) have fields
                    var context = Analyze(exp.Context, graph, scope)!;
                    UseArgument(context, exp.Context.Span, graph);
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
                case IFunctionInvocationExpression exp:
                {
                    var arguments = exp.Arguments
                                       .Select(e => Analyze(e, graph, scope))
                                       .ToFixedList();
                    var function = exp.ReferencedSymbol;
                    var parameterDataTypes = function.ParameterDataTypes;
                    UseArguments(arguments, exp.Arguments, parameterDataTypes, graph);
                    if (!(referenceType is null))
                        return CaptureArguments(exp, arguments, graph);

                    // All the arguments are dropped because they aren't captured
                    graph.Drop(arguments);
                    return null;
                }
                case IMethodInvocationExpression exp:
                {
                    var selfArgument = Analyze(exp.Context, graph, scope);
                    var arguments = exp.Arguments.Select(e => Analyze(e, graph, scope)).ToFixedList();
                    var method = exp.ReferencedSymbol;
                    var parameterDataTypes = method.ParameterDataTypes;
                    if (!(selfArgument is null))
                        UseArgument(selfArgument, exp.Context.Span, graph);
                    UseArguments(arguments, exp.Arguments, parameterDataTypes, graph);
                    if (!(referenceType is null))
                        return CaptureArguments(exp, arguments.Prepend(selfArgument), graph);

                    // All the arguments are dropped because they aren't captured
                    graph.Drop(selfArgument);
                    graph.Drop(arguments);
                    return null;
                }
                case IUnsafeExpression exp:
                    return Analyze(exp.Expression, graph, scope);
                case IBlockExpression exp:
                    return AnalyzeBlock(exp, graph, scope);
                case IReturnExpression exp:
                {
                    var temp = Analyze(exp.Value, graph, scope);
                    if (!(temp is null))
                    {
                        if (!(declaration is IConcreteInvocableDeclaration invocable))
                            throw new InvalidOperationException("Return statement outside of invocable");

                        // TODO make a deep copy of the graph so the existing graph is intact
                        // create a new temp with the correct reference capabilities to the value
                        var returnValue = graph.AddReturnValue(exp, invocable.Symbol.ReturnDataType
                                                                             .Known())!;
                        returnValue.AssignFrom(temp, returnValue.ReferenceType.ReferenceCapability);
                        // Exit the function, releasing all temps and variables except the returned value
                        graph.ExitFunction(returnValue);
                        if (returnValue.PossibleReferents.Any(r => !r.IsAllocated))
                            diagnostics.Add(BorrowError.ValueDoesNotLiveLongEnough(file, exp.Value!.Span));
                    }
                    return null;
                }
                case INewObjectExpression exp:
                {
                    var arguments = exp.Arguments.Select(e => Analyze(e, graph, scope)).ToFixedList();

                    var constructor = exp.ReferencedSymbol;
                    var parameterDataTypes = constructor.ParameterDataTypes;
                    UseArguments(arguments, exp.Arguments, parameterDataTypes, graph);
                    if (referenceType is null) return null;
                    var obj = graph.AddObject(exp)!;
                    CaptureArguments(arguments, obj);
                    return obj;
                }
                case IStringLiteralExpression exp:
                {
                    if (referenceType is null) return null;

                    // Create an untethered context object that doesn't need released
                    var temp = TempValue.ForNewContextObject(graph, exp);
                    graph.Add(temp);
                    return temp;
                }
                case IImplicitNumericConversionExpression exp:
                    return Analyze(exp.Expression, graph, scope);
                case IIntegerLiteralExpression _:
                case IBoolLiteralExpression _:
                case INoneLiteralExpression _:
                    return null;
                case IImplicitImmutabilityConversionExpression exp:
                    // TODO does this need to be handled specially?
                    return Analyze(exp.Expression, graph, scope);
                case IIfExpression exp:
                {
                    Analyze(exp.Condition, graph, scope);
                    AnalyzeBlock(exp.ThenBlock, graph, scope);
                    Analyze(exp.ElseClause, graph, scope);
                    // Assuming void for now
                    return null;
                }
                case ILoopExpression exp:
                {
                    // TODO deal with flow analysis
                    AnalyzeBlock(exp.Block, graph, scope);
                    // Assuming void for now
                    return null;
                }
                case IWhileExpression exp:
                {
                    Analyze(exp.Condition, graph, scope);
                    // TODO deal with flow analysis
                    AnalyzeBlock(exp.Block, graph, scope);
                    // Assuming void for now
                    return null;
                }
                case IForeachExpression exp:
                {
                    // TODO deal with flow analysis
                    Analyze(exp.InExpression, graph, scope);
                    return AnalyzeBlock(exp.Block, graph, scope);
                }
                case IBreakExpression _:
                case INextExpression _:
                    // TODO deal with control flow effects
                    return null;
                case IImplicitNoneConversionExpression exp:
                    Analyze(exp.Expression, graph, scope);
                    // TODO Is there a chance this needs something done?
                    return null;
                case IImplicitOptionalConversionExpression exp:
                    return Analyze(exp.Expression, graph, scope);
            }
        }

        private void UseArguments(
            FixedList<TempValue?> arguments,
            FixedList<IExpression> argumentSyntaxes,
            IEnumerable<DataType> parameterDataTypes,
            ReachabilityGraph graph)
        {
            foreach (var ((argument, argumentSyntax), parameterDataType) in arguments.Zip(argumentSyntaxes).Zip(parameterDataTypes))
            {
                if (argument is null) continue;
                if (!(parameterDataType is ReferenceType))
                    // TODO this used to give the parameter name, would be nice to do so again
                    throw new InvalidOperationException($"Expected parameter of type {parameterDataType} to be a reference type");

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
            IInvocationExpression exp,
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

        private TempValue? Analyze(IElseClause? clause, ReachabilityGraph graph, VariableScope scope)
        {
            return clause switch
            {
                null => null,
                IBlockExpression exp => AnalyzeBlock(exp, graph, scope),
                IIfExpression exp => Analyze((IExpression)exp, graph, scope),
                IResultStatement exp => Analyze(exp.Expression, graph, scope),
                _ => throw ExhaustiveMatch.Failed(clause)
            };
        }

        private TempValue? AnalyzeBlock(IBlockOrResult blockOrResult, ReachabilityGraph graph, VariableScope scope)
        {
            switch (blockOrResult)
            {
                default:
                    throw ExhaustiveMatch.Failed(blockOrResult);
                case IBlockExpression exp:
                {
                    var nestedScope = new VariableScope(scope);

                    foreach (var statement in exp.Statements)
                        Analyze(statement, graph, nestedScope);
                    foreach (var variable in nestedScope.Variables)
                        graph.EndVariableScope(variable);

                    // Assuming void return from blocks for now
                    return null;
                }
                case IResultStatement exp:
                    return Analyze(exp.Expression, graph, scope);
            }
        }

        private Variable? AnalyzeAssignmentPlace(IAssignableExpression expression, ReachabilityGraph graph, VariableScope scope)
        {
            var isReferenceType = !(expression.DataType.Known().UnderlyingReferenceType() is null);
            switch (expression)
            {
                default:
                    throw ExhaustiveMatch.Failed(expression);
                case IFieldAccessExpression exp:
                {
                    var variable = graph.TryGetVariableFor(exp.ReferencedSymbol);
                    if (!(variable is null)) return variable;

                    if (!isReferenceType && exp.Context is ISelfExpression) return null;

                    var context = Analyze(exp.Context, graph, scope);
                    if (!isReferenceType)
                    {
                        graph.Drop(context);
                        return null;
                    }

                    throw new NotImplementedException(
                        $"{nameof(AnalyzeAssignmentPlace)}(expression) not implemented for {expression.GetType().Name} other than `self`");
                }
                case INameExpression exp:
                {
                    return isReferenceType ? graph.GetVariableFor(exp.ReferencedSymbol) : null;
                }
            }
        }

        /// <summary>
        /// Analyze whether the expression can be used as the right hand side of
        /// an assignment.
        /// </summary>
        private TempValue? AnalyzeAssignmentSource(
            IExpression? expression,
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
            if (declaration is IConcreteInvocableDeclaration invocable)
                foreach (var parameter in invocable.Parameters.OfType<INamedParameter>())
                    CreateParameter(parameter, graph, parameterScope);

            return parameterScope;
        }

        private static void CreateParameter(
            IBindingParameter parameter,
            ReachabilityGraph graph,
            VariableScope parameterScope)
        {
            var localVariable = graph.AddParameter(parameter);
            if (!(localVariable is null))
                parameterScope.VariableDeclared(localVariable.Symbol);
        }

        private void CreateSelfParameter(ReachabilityGraph graph, VariableScope parameterScope)
        {
            IClassDeclaration declaringClass;
            ISelfParameter? selfParameter;
            switch (declaration)
            {
                default:
                    throw ExhaustiveMatch.Failed(declaration);
                case IFunctionDeclaration _:
                case IAssociatedFunctionDeclaration _:
                    return;
                case IConcreteMethodDeclaration syn:
                    declaringClass = syn.DeclaringClass;
                    selfParameter = syn.SelfParameter;
                    break;
                case IConstructorDeclaration syn:
                    declaringClass = syn.DeclaringClass;
                    selfParameter = syn.ImplicitSelfParameter;
                    break;
                case IFieldDeclaration syn:
                    declaringClass = syn.DeclaringClass;
                    selfParameter = null;
                    break;
            }

            CreateFields(declaringClass, graph);
            if (!(selfParameter is null))
                CreateParameter(selfParameter, graph, parameterScope);
        }

        private static void CreateFields(IClassDeclaration declaringClass, ReachabilityGraph graph)
        {
            // TODO should fields have their own scope outside of parameter scope?
            foreach (var field in declaringClass.Members.OfType<IFieldDeclaration>())
                graph.AddField(field);
        }

        private static Variable? VariableDeclared(
            IVariableDeclarationStatement variableSyntax,
            ReachabilityGraph graph,
            VariableScope scope)
        {
            var variable = graph.AddVariable(variableSyntax.Symbol);
            if (!(variable is null))
                scope.VariableDeclared(variable.Symbol);
            return variable;
        }
    }
}
