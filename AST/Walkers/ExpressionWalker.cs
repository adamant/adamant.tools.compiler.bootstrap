namespace Adamant.Tools.Compiler.Bootstrap.AST.Walkers
{
    public abstract class ExpressionWalker : IExpressionWalker
    {
        public abstract bool Enter(IExpressionSyntax expression, ITreeWalker walker);

        public virtual void Exit(IExpressionSyntax expression, ITreeWalker walker) { }

        public abstract bool Enter(ITransferSyntax transfer, ITreeWalker walker);

        public virtual void Exit(ITransferSyntax transfer, ITreeWalker walker) { }
    }
}
