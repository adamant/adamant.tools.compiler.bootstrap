using System;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using ExhaustiveMatching;
using static Adamant.Tools.Compiler.Bootstrap.Metadata.Types.ReferenceCapability;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability
{
    public class ReachabilityAnalyzer
    {
        private readonly IConcreteCallableDeclarationSyntax callableDeclaration;
        private readonly Diagnostics diagnostics;
        private readonly PlaceList places = new PlaceList();

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

            // In control flow order:
            //    Create new variable scopes
            //    Update reachability graph based on expressions/statements
            //    Join graphs for control flow
            //    At end of scope for variable drops
            throw new NotImplementedException();
        }

        private static void Analyze(IStatementSyntax statement, ReachabilityGraph graph)
        {
            _ = graph;
            switch (statement)
            {
                default:
                    throw ExhaustiveMatch.Failed(statement);
                case IVariableDeclarationStatementSyntax stmt:
                    throw new NotImplementedException();
                case IExpressionStatementSyntax stmt:
                    Analyze(stmt.Expression, graph);
                    break;
                case IResultStatementSyntax stmt:
                    throw new NotImplementedException();
            }
        }

        private static void Analyze(IExpressionSyntax expression, ReachabilityGraph graph)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create both the caller and parameter scope with the correct relationships
        /// between the parameters and the callers.
        /// </summary>
        private ReachabilityGraph CreateParameterScope()
        {
            var callerScope = new CallerPlaceScope(places);
            var parameterScope = new VariablePlaceScope(places, callerScope);
            var graph = new ReachabilityGraph(places, parameterScope);
            foreach (var parameter in callableDeclaration.Parameters)
            {
                var parameterVariable = graph.VariableDeclared(parameter.Name);
                // Non-reference types don't participate in reachability (yet)
                if (parameter.Type.Known() is ReferenceType referenceType)
                {
                    switch (referenceType.ReferenceCapability)
                    {
                        default:
                            throw ExhaustiveMatch.Failed(referenceType.ReferenceCapability);
                        case IsolatedMutable:
                        case Isolated:
                        {
                            // Isolated parameters are fully independent of the caller
                            var referencedObject = graph.ObjectFor(parameter);
                            parameterVariable.Owns(referencedObject);
                        }
                        break;
                        case Owned:
                        case OwnedMutable:
                        {
                            var referencedObject = graph.ObjectFor(parameter);
                            parameterVariable.Owns(referencedObject);
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
                            parameterVariable.Identifies(callerObject);
                        }
                        break;
                    }
                }
            }

            return graph;
        }
    }
}
