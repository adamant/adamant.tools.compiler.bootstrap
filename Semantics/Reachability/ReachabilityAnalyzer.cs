using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
        private readonly PlaceIdentifierList places = new PlaceIdentifierList();

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
            DeclareSelfFields(graph);
            foreach (var statement in callableDeclaration.Body.Statements)
                Analyze(statement, graph);

            // TODO handle implicit return at end
        }

        private void DeclareSelfFields(ReachabilityGraph graph)
        {
            IClassDeclarationSyntax declaringClass;
            switch (callableDeclaration)
            {
                default:
                    throw ExhaustiveMatch.Failed(callableDeclaration);
                case IAssociatedFunctionDeclaration _:
                case IFunctionDeclarationSyntax _:
                    return;
                case IConcreteMethodDeclarationSyntax method:
                    declaringClass = method.DeclaringClass;
                    break;
                case IConstructorDeclarationSyntax constructor:
               
                    declaringClass = constructor.DeclaringClass;
                    break;
            }

            foreach (var fieldDeclaration in declaringClass.Members.OfType<IFieldDeclarationSyntax>())
            {
                if (!IsReferenceType(fieldDeclaration.Type.Known(), out var referenceType)) continue;
                graph.FieldDeclared(fieldDeclaration.Name, referenceType);
            }
        }

        private static void Analyze(IStatementSyntax statement, ReachabilityGraph graph)
        {
            switch (statement)
            {
                default:
                    throw ExhaustiveMatch.Failed(statement);
                case IVariableDeclarationStatementSyntax stmt:
                    var initializer = Analyze(stmt.Initializer, graph);
                    if (IsReferenceType(stmt.Type.Known(), out var referenceType))
                    {
                        var variable = graph.VariableDeclared(stmt.Name, referenceType);
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
            var isReferenceType = IsReferenceType(expression.Type.Known(), out _);
            switch (expression)
            {
                default:
                    // TODO implement this for all expression types
                    throw new NotImplementedException(
                        $"{nameof(Analyze)}({nameof(expression)}, graph) not implemented for {expression.GetType().Name}");
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
                case ISelfExpressionSyntax _:
                {
                    var selfPlace = graph.VariableFor(SpecialName.Self);
                    return selfPlace.References.Single().Referent;
                }
                case IShareExpressionSyntax exp:
                    return Analyze(exp.Referent, graph);
                case INameExpressionSyntax name:
                {
                    if (!isReferenceType) return null;
                    var variablePlace = graph.VariableFor(name.Name);
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
            var isReferenceType = IsReferenceType(expression.Type.Known(), out _);
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
                        return isReferenceType ? graph.FieldFor(exp.Field.Name) : null;
                    }

                    throw new NotImplementedException(
                        $"{nameof(AnalyzeAssignmentPlace)}(expression) not implemented for {expression.GetType().Name} other than `self`");
                }
                case INameExpressionSyntax exp:
                {
                    return isReferenceType ? graph.VariableFor(exp.Name) : null;
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

            CreateConstructorSelfParameter(graph);
            foreach (var parameter in callableDeclaration.Parameters)
            {
                // Non-reference types don't participate in reachability (yet)
                if (!IsReferenceType(parameter.Type.Known(), out var referenceType)) continue;

                var parameterVariable = graph.VariableDeclared(parameter.Name, referenceType);
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
                        var callerObject = graph.CallerObjectFor(parameter, referenceType);
                        parameterVariable.Borrows(callerObject);
                    }
                    break;
                    case Shared:
                    {
                        var callerObject = graph.CallerObjectFor(parameter, referenceType);
                        parameterVariable.Shares(callerObject);
                    }
                    break;
                    case Identity:
                    {
                        var callerObject = graph.CallerObjectFor(parameter, referenceType);
                        parameterVariable.Identifies(callerObject);
                    }
                    break;
                }
            }

            return graph;
        }

        private void CreateConstructorSelfParameter(ReachabilityGraph graph)
        {
            if (!(callableDeclaration is IConstructorDeclarationSyntax constructor)) return;

            var selfType = (ReferenceType)constructor.SelfParameterType.Known();
            var selfVariable = graph.VariableDeclared(SpecialName.Self, selfType);
            var callerObject = graph.CallerObjectForSelf(constructor, selfType);
            selfVariable.Borrows(callerObject);
        }

        private static bool IsReferenceType(DataType type, [NotNullWhen(true)] out ReferenceType? referenceType)
        {
            switch (type)
            {
                case ReferenceType t:
                    referenceType = t;
                    return true;
                case OptionalType optionalType when optionalType.Referent is ReferenceType t:
                    referenceType = t;
                    return true;
                default:
                    referenceType = null;
                    return false;
            }
        }
    }
}
