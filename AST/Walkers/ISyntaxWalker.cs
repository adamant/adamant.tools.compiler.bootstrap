using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Walkers
{
    public interface ISyntaxWalker : ISyntaxWalker<Void>
    {
        void Walk(ISyntax? syntax);
    }

    public interface ISyntaxWalker<in T>
    {
        void Walk(ISyntax? syntax, T arg);
    }
}
