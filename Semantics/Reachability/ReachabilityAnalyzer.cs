using System;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Identifiers;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Scopes;
using ExhaustiveMatching;
using static Adamant.Tools.Compiler.Bootstrap.Metadata.Types.ReferenceCapability;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability
{
    public class ReachabilityAnalyzer
    {
        private readonly IConcreteCallableDeclarationSyntax callableDeclaration;
        private readonly Diagnostics diagnostics;
        private readonly PlaceIdentifierList places = new PlaceIdentifierList();

        private ReachabilityAnalyzer(IConcreteCallableDeclarationSyntax callableDeclaration, Diagnostics diagnostics)
        {
            this.callableDeclaration = callableDeclaration;
            this.diagnostics = diagnostics;
        }

        public static void Analyze(FixedList<IConcreteCallableDeclarationSyntax> callableDeclarations, Diagnostics diagnostics)
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
            _ = graph;
            switch (statement)
            {
                default:
                    throw ExhaustiveMatch.Failed(statement);
                case IVariableDeclarationStatementSyntax stmt:
                {
                    var initializer = Analyze(stmt.Initializer, graph);
                    var variable = graph.VariableDeclared(stmt.Name);
                    if (initializer != null)
                        variable.Assign(initializer, stmt.Type.Known());
                }
                break;
                case IExpressionStatementSyntax stmt:
                    Analyze(stmt.Expression, graph);
                    break;
                case IResultStatementSyntax stmt:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Analyze an expression to apply its effects reachability graph.
        /// </summary>
        /// <returns>The place of the object resulting from evaluating this expression or null
        /// if the there is no result or the result is not an object reference.</returns>
        private static ObjectPlace? Analyze(IExpressionSyntax? expression, ReachabilityGraph graph)
        {
            switch (expression)
            {
                default:
                    // TODO implement this for all expression types
                    return null;
                //   throw new NotImplementedException($"{nameof(Analyze)}(expression) not implemented for {expression.GetType().Name}");
                //throw ExhaustiveMatch.Failed(expression);
                case null:
                    return null;
                case IAssignmentExpressionSyntax exp:
                {
                    // TODO analyze left operand
                    var leftPlace = AnalyzeAssignmentPlace(exp.LeftOperand, graph);
                    var rightPlace = Analyze(exp.RightOperand, graph)!;
                    leftPlace.Assign(rightPlace, exp.RightOperand.Type.Known());
                    return null;
                }
                case ISelfExpressionSyntax _:
                {
                    var selfPlace = graph.VariableFor(SpecialName.Self);
                    return selfPlace.Referent;
                }
                case IShareExpressionSyntax exp:
                    return Analyze(exp.Referent, graph);
                case INameExpressionSyntax name:
                {
                    var variablePlace = graph.VariableFor(name.Name);
                    return variablePlace.Referent;
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

        private static AssignablePlace AnalyzeAssignmentPlace(IAssignableExpressionSyntax expression, ReachabilityGraph graph)
        {
            _ = graph;
            switch (expression)
            {
                default:
                    throw new NotImplementedException($"{nameof(AnalyzeAssignmentPlace)}(expression) not implemented for {expression.GetType().Name}");
                case IFieldAccessExpressionSyntax exp:
                {
                    if (exp.ContextExpression is ISelfExpressionSyntax)
                    {
                        return graph.FieldFor(exp.Field.Name);
                    }

                    var contextPlace = Analyze(exp.ContextExpression, graph);
                    throw new NotImplementedException(
                        $"{nameof(AnalyzeAssignmentPlace)}(expression) not implemented for {expression.GetType().Name} other than `self`");
                }
                case INameExpressionSyntax exp:
                {
                    return graph.VariableFor(exp.Name);
                }
            }
        }

        /// <summary>
        /// Create both the caller and parameter scope with the correct relationships
        /// between the parameters and the callers.
        /// </summary>
        private ReachabilityGraph CreateParameterScope()
        {
            var callerScope = new CallerVariableScope(places);
            var parameterScope = new LexicalVariableScope(places, callerScope);
            var graph = new ReachabilityGraph(places, parameterScope);
            foreach (var parameter in callableDeclaration.Parameters)
            {
                var parameterVariable = graph.VariableDeclared(parameter.Name);
                // Non-reference types don't participate in reachability (yet)
                if (parameter.Type.Known() is ReferenceType referenceType)
                {
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
                            var callerObject = graph.CallerOwnedObjectFor(parameter);
                            referencedObject.Shares(callerObject);
                        }
                        break;
                        case Held:
                        case HeldMutable:
                        {
                            var referencedObject = graph.ObjectFor(parameter);
                            parameterVariable.PotentiallyOwns(referencedObject);
                            var callerObject = graph.CallerOwnedObjectFor(parameter);
                            referencedObject.Shares(callerObject);
                        }
                        break;
                        case Borrowed:
                        {
                            var callerObject = graph.CallerOwnedObjectFor(parameter);
                            parameterVariable.Borrows(callerObject);
                        }
                        break;
                        case Shared:
                        {
                            var callerObject = graph.CallerOwnedObjectFor(parameter);
                            parameterVariable.Shares(callerObject);
                        }
                        break;
                        case Identity:
                        {
                            var callerObject = graph.CallerOwnedObjectFor(parameter);
                            parameterVariable.OwningIdentifies(callerObject);
                        }
                        break;
                    }
                }
            }

            return graph;
        }
    }
}
