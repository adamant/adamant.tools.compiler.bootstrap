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
            // TODO this doesn't handle loops correctly
            switch (syntax)
            {
                case IConcreteCallableDeclarationSyntax exp:
                    checker = strategy.BeginAnalysis(exp, diagnostics);
                    currentState = checker.StartState();
                    break;
                case IAssignmentExpressionSyntax exp:
                    WalkNonNull(exp.RightOperand, false);
                    WalkNonNull(exp.LeftOperand, true);
                    currentState = checker!.Assignment(exp, currentState!);
                    return;
                case INameExpressionSyntax exp:
                    if (isLValue) return; // ignore
                    currentState = checker!.IdentifierName(exp, currentState!);
                    return;
                case IVariableDeclarationStatementSyntax exp:
                    currentState = checker!.VariableDeclaration(exp, currentState!);
                    WalkChildrenInReverse(exp, false);
                    return;
                case IForeachExpressionSyntax exp:
                    WalkNonNull(exp.Block, isLValue);
                    currentState = checker!.VariableDeclaration(exp, currentState!);
                    WalkNonNull(exp.InExpression, isLValue);
                    return;
                case IFieldAccessExpressionSyntax exp:
                    WalkNonNull(exp.ContextExpression, isLValue);
                    // Don't walk the field name, it shouldn't be treated as a variable
                    return;
                case ITypeSyntax _:
                    return;
                case IDeclarationSyntax _:
                    throw new InvalidOperationException($"Analyze data flow of declaration of type {syntax.GetType().Name}");
            }
            WalkChildrenInReverse(syntax, isLValue);
        }
    }
}
