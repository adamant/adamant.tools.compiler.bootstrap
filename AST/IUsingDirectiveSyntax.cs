using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IUsingDirectiveSyntax : ISyntax
    {
        Name Name { get; }
    }
}
