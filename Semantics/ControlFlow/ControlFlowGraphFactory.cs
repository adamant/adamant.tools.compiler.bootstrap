using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.ControlFlow
{
    /// <summary>
    /// A factory for creating control flow graphs from callable AST nodes
    /// </summary>
    public class ControlFlowGraphFactory
    {
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Factory is passed as a dependency")]
        public ControlFlowGraph CreateGraph(IConcreteCallableDeclarationSyntax callableDeclaration)
        {
            // TODO build control flow graphs for field initializers

            var fabrication = new ControlFlowFabrication(callableDeclaration);
            return fabrication.CreateGraph();
        }
    }
}
