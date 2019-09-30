using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow
{
    public interface IDataFlowAnalysisStrategy<TState>
    {
        IDataFlowAnalysisChecker<TState> CheckerFor(
            IFunctionDeclarationSyntax function,
            Diagnostics diagnostics);
    }
}
