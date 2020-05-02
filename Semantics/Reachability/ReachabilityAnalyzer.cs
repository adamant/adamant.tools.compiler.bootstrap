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
                case IResultStatementSyntax exp:
                    // TODO deal with passing the result to the block
                    Analyze(exp.Expression, graph);
                    break;
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
                    throw ExhaustiveMatch.Failed(expression);
                case IAssignmentExpressionSyntax exp:
                {
                    // TODO analyze left operand
                    var leftPlace = AnalyzeAssignmentPlace(exp.LeftOperand, graph);
                    var rightPlace = Analyze(exp.RightOperand, graph);
                    if (rightPlace != null)
                        leftPlace?.Assign(rightPlace);
                    return null;
                }
                case ISelfExpressionSyntax exp:
                {
                    var selfPlace = graph.VariableFor(exp.ReferencedSymbol.Assigned());
                    return selfPlace.References.Single().Referent;
                }
                case IMoveExpressionSyntax exp:
                    // TODO deal with move if needed
                    return Analyze(exp.Referent, graph);
                case IBorrowExpressionSyntax exp:
                    // TODO deal with borrowing if needed
                    return Analyze(exp.Referent, graph);
                case IShareExpressionSyntax exp:
                    // TODO deal with sharing if needed
                    return Analyze(exp.Referent, graph);
                case INameExpressionSyntax exp:
                {
                    if (!isReferenceType) return null;
                    var variablePlace = graph.VariableFor(exp.ReferencedSymbol.Assigned());
                    return variablePlace.References.Single().Referent;
                }
                case IBinaryOperatorExpressionSyntax exp:
                {
                    Analyze(exp.LeftOperand, graph);
                    Analyze(exp.RightOperand, graph);
                    // All binary operators result in value types
                    return null;
                }
                case IUnaryOperatorExpressionSyntax exp:
                {
                    Analyze(exp.Operand, graph);
                    // All unary operators result in value types
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
                case IFunctionInvocationExpressionSyntax exp:
                {
                    var arguments = exp.Arguments.Select(a => Analyze(a.Expression, graph)).ToFixedList();
                    // TODO check arguments can be used
                    return isReferenceType ? graph.ObjectFor(exp) : null;
                }
                case IMethodInvocationExpressionSyntax exp:
                {
                    var self = Analyze(exp.ContextExpression, graph);
                    var arguments = exp.Arguments.Select(a => Analyze(a.Expression, graph)).ToFixedList();
                    // TODO check self and arguments can be used
                    return isReferenceType ? graph.ObjectFor(exp) : null;
                }
                case IUnsafeExpressionSyntax exp:
                    return Analyze(exp.Expression, graph);
                case IBlockExpressionSyntax exp:
                    return AnalyzeBlock(exp, graph);
                case IReturnExpressionSyntax exp:
                    // TODO check the graph allows us to return this
                    return Analyze(exp.ReturnValue, graph);
                case INewObjectExpressionSyntax exp:
                {
                    var arguments = exp.Arguments.Select(a => Analyze(a.Expression, graph)).ToFixedList();
                    // TODO check arguments can be used
                    return graph.ObjectFor(exp);
                }
                case IStringLiteralExpressionSyntax exp:
                    return graph.ObjectFor(exp);
                case IImplicitNumericConversionExpression exp:
                    return Analyze(exp.Expression, graph);
                case IIntegerLiteralExpressionSyntax _:
                case IBoolLiteralExpressionSyntax _:
                case INoneLiteralExpressionSyntax _:
                    return null;
                case IImplicitImmutabilityConversionExpression exp:
                    // TODO does this need to be handled specially?
                    return Analyze(exp.Expression, graph);
                case IIfExpressionSyntax exp:
                {
                    Analyze(exp.Condition, graph);
                    AnalyzeBlock(exp.ThenBlock, graph);
                    Analyze(exp.ElseClause, graph);
                    // Assuming void for now
                    return null;
                }
                case ILoopExpressionSyntax exp:
                {
                    // TODO deal with flow analysis
                    AnalyzeBlock(exp.Block, graph);
                    // Assuming void for now
                    return null;
                }
                case IWhileExpressionSyntax exp:
                {
                    Analyze(exp.Condition, graph);
                    // TODO deal with flow analysis
                    AnalyzeBlock(exp.Block, graph);
                    // Assuming void for now
                    return null;
                }
                case IForeachExpressionSyntax exp:
                {
                    // TODO deal with flow analysis
                    Analyze(exp.InExpression, graph);
                    return AnalyzeBlock(exp.Block, graph);
                }
                case IBreakExpressionSyntax _:
                case INextExpressionSyntax _:
                    // TODO deal with control flow effects
                    return null;
                case IImplicitNoneConversionExpression exp:
                    Analyze(exp.Expression, graph);
                    // TODO Is there a chance this needs something done?
                    return null;
                case IImplicitOptionalConversionExpression exp:
                    return Analyze(exp.Expression, graph);
            }
        }

        private static ObjectPlace? Analyze(IElseClauseSyntax? clause, ReachabilityGraph graph)
        {
            switch (clause)
            {
                default:
                    throw ExhaustiveMatch.Failed(clause);
                case null:
                    return null;
                case IBlockExpressionSyntax exp:
                    return AnalyzeBlock(exp, graph);
                case IIfExpressionSyntax exp:
                    return Analyze((IExpressionSyntax)exp, graph);
                case IResultStatementSyntax exp:
                    return Analyze(exp.Expression, graph);
            }
        }

        private static ObjectPlace? AnalyzeBlock(IBlockOrResultSyntax blockOrResult, ReachabilityGraph graph)
        {
            switch (blockOrResult)
            {
                default:
                    throw ExhaustiveMatch.Failed(blockOrResult);
                case IBlockExpressionSyntax exp:
                {
                    foreach (var statement in exp.Statements) Analyze(statement, graph);
                    // Assuming void return from blocks for now
                    return null;
                }
                case IResultStatementSyntax exp:
                    return Analyze(exp.Expression, graph);
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
                    throw ExhaustiveMatch.Failed(expression);
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

            CreateSelfParameter(graph);
            foreach (var parameter in callableDeclaration.Parameters)
                CreateParameter(parameter, graph);

            return graph;
        }

        private void CreateSelfParameter(ReachabilityGraph graph)
        {
            ISelfParameterSyntax selfParameter;
            switch (callableDeclaration)
            {
                default:
                    throw ExhaustiveMatch.Failed(callableDeclaration);
                case IFunctionDeclarationSyntax _:
                case IAssociatedFunctionDeclarationSyntax _:
                    return;
                case IConcreteMethodDeclarationSyntax method:
                    selfParameter = method.SelfParameter;
                    break;
                case IConstructorDeclarationSyntax constructor:
                    selfParameter = constructor.ImplicitSelfParameter;
                    break;
            }

            CreateParameter(selfParameter, graph);
        }

        private static void CreateParameter(IParameterSyntax parameter, ReachabilityGraph graph)
        {
            // Non-reference types don't participate in reachability (yet)
            var referenceType = parameter.Type.Known().UnderlyingReferenceType();
            if (referenceType is null) return;

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
    }
}
