using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.FST;
using Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.BindingMutability
{
    public class BindingMutabilityAnalyzer : IForwardDataFlowAnalyzer<VariableFlags>
    {
        #region Singleton
        public static readonly BindingMutabilityAnalyzer Instance = new BindingMutabilityAnalyzer();

        private BindingMutabilityAnalyzer() { }
        #endregion

        public IForwardDataFlowAnalysis<VariableFlags> BeginAnalysis(IConcreteCallableDeclarationSyntax callable, Diagnostics diagnostics)
        {
            return new BindingMutabilityAnalysis(callable, diagnostics);
        }
    }
}
