using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.DefiniteAssignment
{
    public class DefiniteAssignmentStrategy : IDataFlowAnalysisStrategy<VariableFlags>
    {
        #region Singleton
        public static readonly DefiniteAssignmentStrategy Instance = new DefiniteAssignmentStrategy();

        private DefiniteAssignmentStrategy() { }
        #endregion

        public IDataFlowAnalysisChecker<VariableFlags> CheckerFor(
            IMethodDeclarationSyntax method,
            Diagnostics diagnostics)
        {
            return new DefiniteAssignmentChecker(method, diagnostics);
        }
    }
}
