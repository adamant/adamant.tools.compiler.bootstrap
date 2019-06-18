using System;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.BindingMutability
{
    public class BindingMutabilityStrategy : IDataFlowAnalysisStrategy<VariableFlags>
    {
        #region Singleton
        public static BindingMutabilityStrategy Instance = new BindingMutabilityStrategy();

        private BindingMutabilityStrategy() { }
        #endregion

        public IDataFlowAnalysisChecker<VariableFlags> CheckerFor(FunctionDeclarationSyntax function, Diagnostics diagnostics)
        {
            return new BindingMutabilityChecker(function, diagnostics);
        }
    }
}
