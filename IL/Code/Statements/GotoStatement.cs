namespace Adamant.Tools.Compiler.Bootstrap.IL.Code.Statements
{
    public class GotoStatement : BranchingStatement
    {
        public readonly int BlockNumber;

        public GotoStatement(int blockNumber)
        {
            BlockNumber = blockNumber;
        }
    }
}
