using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow;
using Adamant.Tools.Compiler.Bootstrap.Symbols.Trees;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Liveness
{
    public class LivenessAnalyzer : IBackwardDataFlowAnalyzer<VariableFlags>
    {
        #region Singleton
        public static readonly LivenessAnalyzer Instance = new LivenessAnalyzer();

        private LivenessAnalyzer() { }
        #endregion

        public IBackwardDataFlowAnalysis<VariableFlags> BeginAnalysis(
            IConcreteInvocableDeclarationSyntax invocable,
            SymbolTree symbolTree,
            Diagnostics _)
        {
            return new LivenessAnalysis(invocable, symbolTree);
        }
    }
}
