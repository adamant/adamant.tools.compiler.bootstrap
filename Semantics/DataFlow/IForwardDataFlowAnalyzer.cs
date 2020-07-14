using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.FST;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow
{
    /// <summary>
    /// A factory for <see cref="IForwardDataFlowAnalysis{TState}"/>. This is used
    /// to start a new data flow analysis.
    /// </summary>
    public interface IForwardDataFlowAnalyzer<TState>
    {
        IForwardDataFlowAnalysis<TState> BeginAnalysis(
            IConcreteCallableDeclarationSyntax callable,
            Diagnostics diagnostics);
    }
}
