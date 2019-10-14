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

        public void Check(ICallableDeclarationSyntax callableDeclaration)
        {
            switch (callableDeclaration)
            {
                case IConcreteMethodDeclarationSyntax function:
                    Check(function);
                    break;
                default:
                    // Nothing to check
                    break;
            }
        }

        private IDataFlowAnalysisChecker<TState> checker;
        private TState currentState;

        private void Check(IConcreteMethodDeclarationSyntax method)
        {
            checker = strategy.CheckerFor(method, diagnostics);
            currentState = checker.StartState();
            foreach (var statement in method.Body.Statements)
                VisitStatement(statement, default);
        }

        public override void VisitAssignmentExpression(IAssignmentExpressionSyntax assignmentExpression, Void args)
        {
            VisitLValueExpression(assignmentExpression.LeftOperand, args);
            VisitTransfer(assignmentExpression.RightOperand, args);
            currentState = checker.Assignment(assignmentExpression, currentState);
        }

        private void VisitLValueExpression(IExpressionSyntax expression, Void args)
        {
            switch (expression)
            {
                default:
                    throw NonExhaustiveMatchException.For(expression);
                case INameExpressionSyntax _:
                case null:
                    // Ignore
                    break;
                case IMemberAccessExpressionSyntax memberAccessExpression:
                    // The expression we are accessing the member off of is an rvalue
                    VisitExpression(memberAccessExpression.Expression, args);
                    break;
                    // TODO we will need to visit other expression types that also contain rvalues
            }
        }

        public override void VisitNameExpression(INameExpressionSyntax nameExpression, Void args)
        {
            currentState = checker.IdentifierName(nameExpression, currentState);
        }

        public override void VisitVariableDeclarationStatement(
            IVariableDeclarationStatementSyntax variableDeclaration,
            Void args)
        {
            base.VisitVariableDeclarationStatement(variableDeclaration, args);
            currentState = checker.VariableDeclaration(variableDeclaration, currentState);
        }

        public override void VisitFunctionInvocation(IFunctionInvocationExpressionSyntax functionInvocationExpression, Void args)
        {
            // overriden to avoid visiting the function name
            foreach (var argument in functionInvocationExpression.Arguments)
                VisitTransfer(argument, args);
        }
    }
}
