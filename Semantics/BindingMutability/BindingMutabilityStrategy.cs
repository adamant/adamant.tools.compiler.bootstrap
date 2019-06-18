using System;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.BindingMutability
{
    public class BindingMutabilityStrategy : IDataFlowAnalysisStrategy<VariablesAssigned>
    {
        public IDataFlowAnalysisChecker<VariablesAssigned> CheckerFor(FunctionDeclarationSyntax function, Diagnostics diagnostics)
        {
            throw new NotImplementedException();
        }
    }
}
