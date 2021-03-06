using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Symbols.Trees;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow
{
    /// <summary>
    /// A factory for <see cref="IForwardDataFlowAnalysis{TState}"/>. This is used
    /// to start a new data flow analysis.
    /// </summary>
    public interface IForwardDataFlowAnalyzer<TState>
    {
        IForwardDataFlowAnalysis<TState> BeginAnalysis(
            IExecutableDeclaration declaration,
            ISymbolTree symbolTree,
            Diagnostics diagnostics);
    }
}
