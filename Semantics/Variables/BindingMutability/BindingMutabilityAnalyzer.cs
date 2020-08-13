using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
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
            IConcreteInvocableDeclarationSyntax invocable,
            SymbolTree symbolTree,
            Diagnostics diagnostics)
        {
            return new BindingMutabilityAnalysis(invocable, symbolTree, diagnostics);
        }
    }
}
