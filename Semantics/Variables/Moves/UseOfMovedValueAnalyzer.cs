using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow;
using Adamant.Tools.Compiler.Bootstrap.Symbols.Trees;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Variables.Moves
{
    public class UseOfMovedValueAnalyzer : IForwardDataFlowAnalyzer<VariableFlags>
    {
        #region Singleton
        public static readonly UseOfMovedValueAnalyzer Instance = new UseOfMovedValueAnalyzer();

        private UseOfMovedValueAnalyzer() { }
        #endregion

        public IForwardDataFlowAnalysis<VariableFlags> BeginAnalysis(
            IConcreteInvocableDeclarationSyntax invocable,
            ISymbolTree symbolTree,
            Diagnostics diagnostics)
        {
            return new UseOfMovedValueAnalysis(invocable, symbolTree, diagnostics);
        }
    }
}
