using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Liveness
{
    public class LivenessAnalyzer : IBackwardDataFlowAnalyzer<VariableFlags>
    {
        #region Singleton
        public static readonly LivenessAnalyzer Instance = new LivenessAnalyzer();

        private LivenessAnalyzer() { }
        #endregion

        public IBackwardDataFlowAnalysis<VariableFlags> BeginAnalysis(
            IConcreteCallableDeclarationSyntax callable,
            Diagnostics diagnostics)
        {
            return new LivenessAnalysis(callable);
        }
    }
}
