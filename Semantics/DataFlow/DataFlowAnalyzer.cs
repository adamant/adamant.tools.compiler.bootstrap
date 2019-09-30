using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.AST.Visitors;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow
{
    public class DataFlowAnalyzer<TState> : ExpressionVisitor<Void>
    {
        private readonly IDataFlowAnalysisStrategy<TState> strategy;
        private readonly Diagnostics diagnostics;

        public DataFlowAnalyzer(IDataFlowAnalysisStrategy<TState> strategy, Diagnostics diagnostics)
        {
            this.strategy = strategy;
            this.diagnostics = diagnostics;
        }

        public void Check(IEntityDeclarationSyntax entityDeclaration)
        {
            switch (entityDeclaration)
            {
                case IFunctionDeclarationSyntax function:
                    Check(function);
                    break;
                default:
                    // Nothing to check
                    break;

            }
        }

        private IDataFlowAnalysisChecker<TState> checker;
        private TState currentState;

        private void Check(IFunctionDeclarationSyntax function)
        {
            checker = strategy.CheckerFor(function, diagnostics);
            currentState = checker.StartState();
            foreach (var statement in function.Body)
                VisitStatement(statement, default);
        }

        public override void VisitAssignmentExpression(AssignmentExpressionSyntax assignmentExpression, Void args)
        {
            VisitLValueExpression(assignmentExpression.LeftOperand, args);
            VisitExpression(assignmentExpression.RightOperand, args);
            currentState = checker.Assignment(assignmentExpression, currentState);
        }

        private void VisitLValueExpression(ExpressionSyntax expression, Void args)
        {
            switch (expression)
            {
                case NameSyntax identifierName:
                case null:
                    // Ignore
                    break;
                case MemberAccessExpressionSyntax memberAccessExpression:
                    // The expression we are accessing the member off of is an rvalue
                    VisitExpression(memberAccessExpression.Expression, args);
                    break;
                // TODO we will need to visit other expression types that also contain rvalues
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
        }

        public override void VisitName(NameSyntax name, Void args)
        {
            currentState = checker.IdentifierName(name, currentState);
        }

        public override void VisitVariableDeclarationStatement(
            VariableDeclarationStatementSyntax variableDeclaration,
            Void args)
        {
            base.VisitVariableDeclarationStatement(variableDeclaration, args);
            currentState = checker.VariableDeclaration(variableDeclaration, currentState);
        }

        public override void VisitFunctionInvocation(FunctionInvocationSyntax functionInvocation, Void args)
        {
            // overriden to avoid visiting the function name
            foreach (var argument in functionInvocation.Arguments)
                VisitArgument(argument, args);
        }
    }
}
