namespace Adamant.Tools.Compiler.Bootstrap.AST.Walkers
{
    public interface IExpressionWalker
    {
        /// <returns>Whether to continue walking this nodes children and exit</returns>
        bool Enter(IExpressionSyntax expression, ITreeWalker walker);
        void Exit(IExpressionSyntax expression, ITreeWalker walker);

        /// <returns>Whether to continue walking this nodes children and exit</returns>
        bool Enter(ITransferSyntax transfer, ITreeWalker walker);
        void Exit(ITransferSyntax transfer, ITreeWalker walker);
    }
}
