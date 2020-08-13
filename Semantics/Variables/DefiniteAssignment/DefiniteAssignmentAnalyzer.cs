using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow;
using Adamant.Tools.Compiler.Bootstrap.Symbols.Trees;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Variables.DefiniteAssignment
{
    public class DefiniteAssignmentAnalyzer : IForwardDataFlowAnalyzer<VariableFlags>
    {
        #region Singleton
        public static readonly DefiniteAssignmentAnalyzer Instance = new DefiniteAssignmentAnalyzer();

        private DefiniteAssignmentAnalyzer() { }
        #endregion

        public IForwardDataFlowAnalysis<VariableFlags> BeginAnalysis(
            IConcreteInvocableDeclarationSyntax invocable,
            ISymbolTree symbolTree,
            Diagnostics diagnostics)
        {
            return new DefiniteAssignmentAnalysis(invocable, symbolTree, diagnostics);
        }
    }
}
