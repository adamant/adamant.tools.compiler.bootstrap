using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.FST;
using Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.DefiniteAssignment
{
    public class DefiniteAssignmentAnalyzer : IForwardDataFlowAnalyzer<VariableFlags>
    {
        #region Singleton
        public static readonly DefiniteAssignmentAnalyzer Instance = new DefiniteAssignmentAnalyzer();

        private DefiniteAssignmentAnalyzer() { }
        #endregion

        public IForwardDataFlowAnalysis<VariableFlags> BeginAnalysis(
            IConcreteCallableDeclarationSyntax callable,
            Diagnostics diagnostics)
        {
            return new DefiniteAssignmentAnalysis(callable, diagnostics);
        }
    }
}
