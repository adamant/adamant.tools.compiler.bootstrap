using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Moves
{
    public class UseOfMovedValueAnalyzer : IForwardDataFlowAnalyzer<VariableFlags>
    {
        #region Singleton
        public static readonly UseOfMovedValueAnalyzer Instance = new UseOfMovedValueAnalyzer();

        private UseOfMovedValueAnalyzer() { }
        #endregion

        public IForwardDataFlowAnalysis<VariableFlags> BeginAnalysis(IConcreteCallableDeclarationSyntax callable, Diagnostics diagnostics)
        {
            return new UseOfMovedValueAnalysis(callable, diagnostics);
        }
    }
}
