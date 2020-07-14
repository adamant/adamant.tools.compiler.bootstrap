using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.FST;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.ILGen
{
    public class ILFactory
    {
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "OO")]
        public ControlFlowGraph CreateGraph(IConcreteCallableDeclarationSyntax callableDeclaration)
        {
            // TODO build control flow graphs for field initializers

            var fabrication = new ControlFlowGraphFabrication(callableDeclaration);
            return fabrication.CreateGraph();
        }
    }
}
