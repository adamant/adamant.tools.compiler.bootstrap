using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow;
using Adamant.Tools.Compiler.Bootstrap.Symbols.Trees;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Variables.BindingMutability
{
    public class BindingMutabilityAnalyzer : IForwardDataFlowAnalyzer<VariableFlags>
    {
        #region Singleton
        public static readonly BindingMutabilityAnalyzer Instance = new BindingMutabilityAnalyzer();

        private BindingMutabilityAnalyzer() { }
        #endregion

        public IForwardDataFlowAnalysis<VariableFlags> BeginAnalysis(
            IExecutableDeclaration declaration,
            ISymbolTree symbolTree,
            Diagnostics diagnostics)
        {
            return new BindingMutabilityAnalysis(declaration, symbolTree, diagnostics);
        }
    }
}
