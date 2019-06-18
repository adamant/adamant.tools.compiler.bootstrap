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

        public void Check(MemberDeclarationSyntax memberDeclaration)
        {
            switch (memberDeclaration)
            {
                case FunctionDeclarationSyntax function:
                    Check(function);
                    break;
                default:
                    // Nothing to check
                    break;

            }
        }

        private IDataFlowAnalysisChecker<TState> checker;
        private TState currentState;

        private void Check(FunctionDeclarationSyntax function)
        {
            checker = strategy.CheckerFor(function, diagnostics);
            currentState = checker.StartState();
            VisitStatement(function.Body, default);
        }

        public override void VisitAssignmentExpression(AssignmentExpressionSyntax assignmentExpression, Void args)
        {
            base.VisitAssignmentExpression(assignmentExpression, args);
            currentState = checker.Assignment(assignmentExpression, currentState);
        }

        public override void VisitIdentifierName(IdentifierNameSyntax identifierName, Void args)
        {
            currentState = checker.IdentifierName(identifierName, currentState);
        }

        public override void VisitVariableDeclarationStatement(
            VariableDeclarationStatementSyntax variableDeclaration,
            Void args)
        {
            base.VisitVariableDeclarationStatement(variableDeclaration, args);
            currentState = checker.VariableDeclaration(variableDeclaration, currentState);
        }
    }
}
