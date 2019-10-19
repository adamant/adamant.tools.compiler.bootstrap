using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.BindingMutability
{
    public class BindingMutabilityStrategy : IDataFlowAnalysisStrategy<VariableFlags>
    {
        #region Singleton
        public static readonly BindingMutabilityStrategy Instance = new BindingMutabilityStrategy();

        private BindingMutabilityStrategy() { }
        #endregion

        public IDataFlowAnalysisChecker<VariableFlags> CheckerFor(IConcreteCallableDeclarationSyntax callable, Diagnostics diagnostics)
        {
            return new BindingMutabilityChecker(callable, diagnostics);
        }
    }
}
