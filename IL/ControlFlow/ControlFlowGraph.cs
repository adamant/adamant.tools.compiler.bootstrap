namespace Adamant.Tools.Compiler.Bootstrap.IL.ControlFlow
{
    public class ControlFlowGraph
    {
        public readonly Block EntryBlock;
        public readonly Block ExitBlock;

        public ControlFlowGraph(Block entryBlock, Block exitBlock)
        {
            EntryBlock = entryBlock;
            ExitBlock = exitBlock;
        }
    }
}
