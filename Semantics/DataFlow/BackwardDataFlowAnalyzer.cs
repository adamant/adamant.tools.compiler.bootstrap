using System;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.AST.Walkers;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow
{
    internal class BackwardDataFlowAnalyzer<TState> : SyntaxWalker<bool>
        where TState : class
    {
        private readonly IBackwardDataFlowAnalyzer<TState> strategy;
        private readonly Diagnostics diagnostics;

        public BackwardDataFlowAnalyzer(IBackwardDataFlowAnalyzer<TState> strategy, Diagnostics diagnostics)
        {
            this.strategy = strategy;
            this.diagnostics = diagnostics;
        }

        private IBackwardDataFlowAnalysis<TState>? checker;
        private TState? currentState;

        protected override void WalkNonNull(ISyntax syntax, bool isLValue)
        {
            switch (syntax)
            {
                case IConcreteCallableDeclarationSyntax callableDeclaration:
                    checker = strategy.BeginAnalysis(callableDeclaration, diagnostics);
                    currentState = checker.StartState();
                    break;
                case IAssignmentExpressionSyntax assignmentExpression:
                    WalkNonNull(assignmentExpression.LeftOperand, true);
                    WalkNonNull(assignmentExpression.RightOperand, false);
                    currentState = checker!.Assignment(assignmentExpression, currentState!);
                    return;
                case INameExpressionSyntax nameExpression:
                    if (isLValue) return; // ignore
                    currentState = checker!.IdentifierName(nameExpression, currentState!);
                    return;
                case IVariableDeclarationStatementSyntax variableDeclaration:
                    WalkChildren(variableDeclaration, false);
                    currentState = checker!.VariableDeclaration(variableDeclaration, currentState!);
                    return;
                case IDeclarationSyntax _:
                    throw new InvalidOperationException($"Analyze data flow of declaration of type {syntax.GetType().Name}");
            }
            WalkChildren(syntax, false);
        }
    }
}