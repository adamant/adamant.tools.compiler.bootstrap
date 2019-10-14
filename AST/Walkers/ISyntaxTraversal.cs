namespace Adamant.Tools.Compiler.Bootstrap.AST.Walkers
{
    public interface ISyntaxTraversal
    {
        void Walk(ISyntax? syntax);
    }
}
