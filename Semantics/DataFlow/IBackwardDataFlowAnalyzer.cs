using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow
{
    /// <summary>
    /// A factory for <see cref="IForwardDataFlowAnalysis{TState}"/>. This is used
    /// to start a new data flow analysis.
    /// </summary>
    public interface IBackwardDataFlowAnalyzer<TState>
    {
        IBackwardDataFlowAnalysis<TState> BeginAnalysis(
            IConcreteCallableDeclarationSyntax callable,
            Diagnostics diagnostics);
    }
}
