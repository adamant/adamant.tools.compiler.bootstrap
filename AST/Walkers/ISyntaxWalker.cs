namespace Adamant.Tools.Compiler.Bootstrap.AST.Walkers
{
    public interface ISyntaxWalker
    {
        void Walk(ISyntax? syntax);
    }
}
