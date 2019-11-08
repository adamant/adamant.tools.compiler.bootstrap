using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.ILGen
{
    public class ILFactory
    {
        public ControlFlowGraph CreateGraph(IConcreteCallableDeclarationSyntax callableDeclaration)
        {
            // TODO build control flow graphs for field initializers

            var fabrication = new ControlFlowGraphFabrication(callableDeclaration);
            return fabrication.CreateGraph();
        }
    }
}
