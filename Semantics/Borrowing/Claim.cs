namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics.BorrowChecker
{
    public abstract class Claim
    {
        public int Variable { get; }
        public int Object { get; }

        protected Claim(int variable, int @object)
        {
            Variable = variable;
            Object = @object;
        }
    }
}
