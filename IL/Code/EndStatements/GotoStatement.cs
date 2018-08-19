namespace Adamant.Tools.Compiler.Bootstrap.IL.Code.EndStatements
{
    public class GotoStatement : EndStatement
    {
        public readonly int BlockNumber;

        public GotoStatement(int blockNumber)
        {
            BlockNumber = blockNumber;
        }
    }
}
