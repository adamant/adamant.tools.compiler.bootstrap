using System;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Scopes;
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
                    var initializer = Analyze(stmt.Initializer, graph, scope);
                    var variable = VariableDeclared(stmt, graph, scope);
                    if (variable is null || initializer is null) break;
                    variable.Assign(initializer);
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
            var referenceType = expression.Type.Known().UnderlyingReferenceType();
            var isReferenceType = !(referenceType is null);
            switch (expression)
            {
                default:
                    throw ExhaustiveMatch.Failed(expression);
                case IAssignmentExpressionSyntax exp:
                {
                    var leftPlace = AnalyzeAssignmentPlace(exp.LeftOperand, graph, scope);
                    var rightPlace = Analyze(exp.RightOperand, graph, scope);
                    if (!(rightPlace is null)) leftPlace?.Assign(rightPlace);
                    return null;
                }
                case ISelfExpressionSyntax exp:
                    throw new InvalidOperationException($"`self` reference not wrapped in move, borrow, or share");
                case IMoveExpressionSyntax exp:
                {
                    _ = referenceType ?? throw new InvalidOperationException("Can't move value type");

                    // The referent should be a name or `self` so we don't need to evaluate it
                    var variable = graph.VariableFor(exp.MovedSymbol.Assigned());
                    var temp = TempValue.For(exp);
                    temp?.MoveFrom(variable);
                    graph.Add(temp);
                    return temp;
                }
                case IBorrowExpressionSyntax exp:
                {
                    _ = referenceType ?? throw new InvalidOperationException("Can't borrow value type");

                    // If there is a variable, it is a simple borrow expression
                    var variable = graph.TryVariableFor(exp.BorrowedSymbol.Assigned());
                    if (!(variable is null))
                    {
                        var temp = TempValue.For(exp);
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
                    var variable = graph.TryVariableFor(exp.SharedSymbol.Assigned());
                    if (!(variable is null))
                    {
                        var temp = TempValue.For(exp);
                        temp?.ShareFrom(variable);
                        graph.Add(temp);
                        return temp;
                    }

                    // Can this happen?
                    throw new NotImplementedException();
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
                    if (!isReferenceType && exp.ContextExpression is ISelfExpressionSyntax) return null;

                    var context = Analyze(exp.ContextExpression, graph, scope);
                    if (!isReferenceType) return null;

                    throw new NotImplementedException("Analyze(expression) for IFieldAccessExpressionSyntax");

                    //// Treat this like a method call to a getter
                    //var context = Analyze(exp.ContextExpression, graph);
                    //var @object = graph.ObjectFor(exp);
                    //// TODO base the reference on the property type
                    //context?.Owns(@object, true);
                    //var temp = graph.NewTempValue();
                    //// TODO make temp reference the object
                    //return temp;
                }
                case IFunctionInvocationExpressionSyntax exp:
                {
                    var arguments = exp.Arguments.Select(a => Analyze(a.Expression, graph, scope)).ToFixedList();
                    var function = exp.FunctionNameSyntax.ReferencedSymbol.Assigned();
                    var parameters = function.Parameters;
                    foreach (var ((tempValue, argumentSyntax), parameter) in arguments
                                                                             .Zip(exp.Arguments).Zip(parameters))
                    {
                        if (tempValue is null) continue;
                        if (!(parameter.Type is ReferenceType parameterType))
                            throw new InvalidOperationException(
                                $"Expected parameter {parameter} to be a reference type");

                        UseArgument(tempValue, argumentSyntax.Span, graph);
                    }

                    return referenceType is null ? null : CaptureArguments(arguments, function);
                }
                case IMethodInvocationExpressionSyntax exp:
                {
                    var self = Analyze(exp.ContextExpression, graph, scope);
                    var arguments = exp.Arguments.Select(a => Analyze(a.Expression, graph, scope)).ToFixedList();
                    if (!isReferenceType) return null;
                    // TODO check self and arguments can be used
                    throw new NotImplementedException("Analyze(expression) for IMethodInvocationExpressionSyntax");

                    //var obj = graph.ObjectFor(exp);
                    //var temp = graph.NewTempValue();
                    //temp.Owns(obj, true); // TODO apply the correct operation
                    //return temp;
                }
                case IUnsafeExpressionSyntax exp:
                    return Analyze(exp.Expression, graph, scope);
                case IBlockExpressionSyntax exp:
                    return AnalyzeBlock(exp, graph, scope);
                case IReturnExpressionSyntax exp:
                    // TODO check the graph allows us to return this
                    return Analyze(exp.ReturnValue, graph, scope);
                case INewObjectExpressionSyntax exp:
                {
                    var arguments = exp.Arguments.Select(a => Analyze(a.Expression, graph, scope)).ToFixedList();
                    if (referenceType is null) return null;
                    // TODO check arguments can be used

                    var temp = TempValue.ForNewObject(exp);
                    // TODO tie the new object to the arguments
                    graph.Add(temp);
                    return temp;
                }
                case IStringLiteralExpressionSyntax exp:
                {
                    if (referenceType is null) return null;

                    // Create an untethered context object that doesn't need released
                    var temp = TempValue.ForNewContextObject(exp);
                    graph.Add(temp);
                    return temp;
                }
                case IImplicitNumericConversionExpression exp:
                    return Analyze(exp.Expression, graph, scope);
                case IIntegerLiteralExpressionSyntax _:
                case IBoolLiteralExpressionSyntax _:
                case INoneLiteralExpressionSyntax _:
                    return null;
                case IImplicitImmutabilityConversionExpression exp:
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
                case IImplicitNoneConversionExpression exp:
                    Analyze(exp.Expression, graph, scope);
                    // TODO Is there a chance this needs something done?
                    return null;
                case IImplicitOptionalConversionExpression exp:
                    return Analyze(exp.Expression, graph, scope);
            }
        }

        private void UseArgument(TempValue argument, TextSpan span, ReachabilityGraph graph)
        {
            foreach (var reference in argument.References)
            {
                // Must recompute because our use of earlier arguments can affect later ones
                graph.ComputeObjectStates();
                // Check if we are safe to use this reference
                switch (reference.DeclaredAccess)
                {
                    default:
                        throw ExhaustiveMatch.Failed(reference.DeclaredAccess);
                    case Access.Mutable:
                        // Must be no read-only access
                        if (reference.Referent.State == ObjectState.Mutable)
                            diagnostics.Add(BorrowError.CantBorrowWhileShared(file, span));

                        // And we must be a borrower from someone who has the mutable access
                        var originOfMutability = reference.Referent.OriginOfMutability;
                        if (originOfMutability is null || !originOfMutability.IsOriginFor(reference))
                            diagnostics.Add(BorrowError.CantBorrowFromThisReference(file, span));
                        break;
                    case Access.ReadOnly:
                        throw new NotImplementedException();
                    case Access.Identify:
                        // Always safe to take reference identity
                        break;
                }

                // Mark as used regardless to enable correct analysis of later expressions
                reference.Use();
            }
        }

        private static TempValue CaptureArguments(FixedList<TempValue?> arguments, IFunctionSymbol function)
        {
            throw new NotImplementedException();
        }

        private TempValue? Analyze(IElseClauseSyntax? clause, ReachabilityGraph graph, VariableScope scope)
        {
            switch (clause)
            {
                default:
                    throw ExhaustiveMatch.Failed(clause);
                case null:
                    return null;
                case IBlockExpressionSyntax exp:
                    return AnalyzeBlock(exp, graph, scope);
                case IIfExpressionSyntax exp:
                    return Analyze((IExpressionSyntax)exp, graph, scope);
                case IResultStatementSyntax exp:
                    return Analyze(exp.Expression, graph, scope);
            }
        }

        private TempValue? AnalyzeBlock(IBlockOrResultSyntax blockOrResult, ReachabilityGraph graph, VariableScope scope)
        {
            switch (blockOrResult)
            {
                default:
                    throw ExhaustiveMatch.Failed(blockOrResult);
                case IBlockExpressionSyntax exp:
                {
                    foreach (var statement in exp.Statements)
                        Analyze(statement, graph, scope);
                    // Assuming void return from blocks for now
                    return null;
                }
                case IResultStatementSyntax exp:
                    return Analyze(exp.Expression, graph, scope);
            }
        }

        private Variable? AnalyzeAssignmentPlace(IAssignableExpressionSyntax expression, ReachabilityGraph graph, VariableScope scope)
        {
            var isReferenceType = !(expression.Type.Known().UnderlyingReferenceType() is null);
            switch (expression)
            {
                default:
                    throw ExhaustiveMatch.Failed(expression);
                case IFieldAccessExpressionSyntax exp:
                {
                    var variable = graph.TryVariableFor(exp.ReferencedSymbol.Assigned());
                    if (!(variable is null)) return variable;

                    if (!isReferenceType && exp.ContextExpression is ISelfExpressionSyntax) return null;

                    var context = Analyze(exp.ContextExpression, graph, scope);
                    if (!isReferenceType) return null;

                    throw new NotImplementedException(
                        $"{nameof(AnalyzeAssignmentPlace)}(expression) not implemented for {expression.GetType().Name} other than `self`");
                }
                case INameExpressionSyntax exp:
                {
                    return isReferenceType ? graph.VariableFor(exp.ReferencedSymbol.Assigned()) : null;
                }
            }
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
            var (callerVariable, variable) = Variable.ForParameter(parameter);
            graph.Add(callerVariable);
            graph.Add(variable);
            if (!(variable is null))
                parameterScope.VariableDeclared(parameter);
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
                if (field.Type.Known().UnderlyingReferenceType() != null)
                {
                    var variable = Variable.ForField(field);
                    graph.Add(variable);
                }
        }

        private static Variable? VariableDeclared(
            IBindingSymbol bindingSymbol,
            ReachabilityGraph graph,
            VariableScope scope)
        {
            var referenceType = bindingSymbol.Type.Known().UnderlyingReferenceType();
            if (referenceType is null) return null;

            var variable = Variable.Declared(bindingSymbol);
            graph.Add(variable);
            scope.VariableDeclared(bindingSymbol);
            return variable;
        }
    }
}
