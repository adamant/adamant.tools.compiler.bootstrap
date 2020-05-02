using System;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Scopes;
using ExhaustiveMatching;
using static Adamant.Tools.Compiler.Bootstrap.Metadata.Types.ReferenceCapability;

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
        private readonly Diagnostics diagnostics;

        private ReachabilityAnalyzer(
            IConcreteCallableDeclarationSyntax callableDeclaration,
            Diagnostics diagnostics)
        {
            this.callableDeclaration = callableDeclaration;
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
            var graph = CreateParameterScope();
            foreach (var statement in callableDeclaration.Body.Statements)
                Analyze(statement, graph);

            // TODO handle implicit return at end
        }

        private static void Analyze(IStatementSyntax statement, ReachabilityGraph graph)
        {
            switch (statement)
            {
                default:
                    throw ExhaustiveMatch.Failed(statement);
                case IVariableDeclarationStatementSyntax stmt:
                {
                    var initializer = Analyze(stmt.Initializer, graph);
                    var referenceType = stmt.Type.Known().UnderlyingReferenceType();
                    if (referenceType is null) break;
                    var variable = graph.VariableDeclared(stmt);
                    if (initializer != null) variable.Assign(initializer);
                }
                break;
                case IExpressionStatementSyntax stmt:
                    Analyze(stmt.Expression, graph);
                    break;
                case IResultStatementSyntax _:
                    throw new NotImplementedException(
                        $"{nameof(Analyze)}({nameof(statement)}, graph) not implemented for {nameof(IResultStatementSyntax)}");
            }
        }

        /// <summary>
        /// Analyze an expression to apply its effects reachability graph.
        /// </summary>
        /// <returns>The place of the object resulting from evaluating this expression or null
        /// if the there is no result or the result is not an object reference.</returns>
        private static ObjectPlace? Analyze(IExpressionSyntax? expression, ReachabilityGraph graph)
        {
            if (expression is null) return null;
            var isReferenceType = !(expression.Type.Known().UnderlyingReferenceType() is null);
            switch (expression)
            {
                default:
                    // TODO implement this for all expression types
                    return null;
                //throw new NotImplementedException(
                //    $"{nameof(Analyze)}({nameof(expression)}, graph) not implemented for {expression.GetType().Name}");
                //throw ExhaustiveMatch.Failed(expression);
                case IAssignmentExpressionSyntax exp:
                {
                    // TODO analyze left operand
                    var leftPlace = AnalyzeAssignmentPlace(exp.LeftOperand, graph);
                    var rightPlace = Analyze(exp.RightOperand, graph);
                    if (rightPlace != null)
                        leftPlace?.Assign(rightPlace);
                    return null;
                }
                //case ISelfExpressionSyntax exp:
                //{
                //    var selfPlace = graph.VariableFor(SpecialName.Self);
                //    return selfPlace.References.Single().Referent;
                //}
                case IShareExpressionSyntax exp:
                    return Analyze(exp.Referent, graph);
                case INameExpressionSyntax name:
                {
                    if (!isReferenceType) return null;
                    var variablePlace = graph.VariableFor(name.ReferencedSymbol.Assigned());
                    return variablePlace.References.Single().Referent;
                }
                case IBinaryOperatorExpressionSyntax exp:
                {
                    Analyze(exp.LeftOperand, graph);
                    Analyze(exp.RightOperand, graph);
                    // All binary operators result in value types
                    return null;
                }
                case IFieldAccessExpressionSyntax exp:
                {
                    // Treat this like a method call to a getter
                    var context = Analyze(exp.ContextExpression, graph);
                    var @object = graph.ObjectFor(exp);
                    // TODO base the reference on the property type
                    context?.Owns(@object, true);
                    return @object;
                }
            }
        }

        private static AssignablePlace? AnalyzeAssignmentPlace(
            IAssignableExpressionSyntax expression,
            ReachabilityGraph graph)
        {
            var isReferenceType = !(expression.Type.Known().UnderlyingReferenceType() is null);
            switch (expression)
            {
                default:
                    throw new NotImplementedException(
                        $"{nameof(AnalyzeAssignmentPlace)}(expression) not implemented for {expression.GetType().Name}");
                case IFieldAccessExpressionSyntax exp:
                {
                    var contextPlace = Analyze(exp.ContextExpression, graph);
                    if (exp.ContextExpression is ISelfExpressionSyntax)
                    {
                        return isReferenceType ? graph.FieldFor(exp.ReferencedSymbol.Assigned()) : null;
                    }

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
        private ReachabilityGraph CreateParameterScope()
        {
            var callerScope = new CallerVariableScope();
            var parameterScope = new LexicalVariableScope(callerScope);
            var graph = new ReachabilityGraph(parameterScope);

            foreach (var parameter in callableDeclaration.Parameters)
            {
                // Non-reference types don't participate in reachability (yet)
                var referenceType = parameter.Type.Known().UnderlyingReferenceType();
                if (referenceType is null) continue;

                var parameterVariable = graph.VariableDeclared(parameter);
                var capability = referenceType.ReferenceCapability;
                switch (capability)
                {
                    default:
                        throw ExhaustiveMatch.Failed(capability);
                    case IsolatedMutable:
                    case Isolated:
                    {
                        // Isolated parameters are fully independent of the caller
                        var referencedObject = graph.ObjectFor(parameter);
                        parameterVariable.Owns(referencedObject, capability.IsMutable());
                    }
                    break;
                    case Owned:
                    case OwnedMutable:
                    {
                        var referencedObject = graph.ObjectFor(parameter);
                        parameterVariable.Owns(referencedObject, capability.IsMutable());
                        var callerObject = graph.CallerObjectFor(parameter);
                        referencedObject.Borrows(callerObject);
                    }
                    break;
                    case Held:
                    case HeldMutable:
                    {
                        var referencedObject = graph.ObjectFor(parameter);
                        parameterVariable.PotentiallyOwns(referencedObject, capability.IsMutable());
                        var callerObject = graph.CallerObjectFor(parameter);
                        referencedObject.Borrows(callerObject);
                    }
                    break;
                    case Borrowed:
                    {
                        var callerObject = graph.CallerObjectFor(parameter);
                        parameterVariable.Borrows(callerObject);
                    }
                    break;
                    case Shared:
                    {
                        var callerObject = graph.CallerObjectFor(parameter);
                        parameterVariable.Shares(callerObject);
                    }
                    break;
                    case Identity:
                    {
                        var callerObject = graph.CallerObjectFor(parameter);
                        parameterVariable.Identifies(callerObject);
                    }
                    break;
                }
            }

            return graph;
        }
    }
}
