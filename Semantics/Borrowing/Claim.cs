namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Borrowing
{
    public abstract class Claim
    {
        public int Variable { get; }
        public int ObjectId { get; }

        protected Claim(int variable, int objectId)
        {
            Variable = variable;
            ObjectId = objectId;
        }
    }
}
