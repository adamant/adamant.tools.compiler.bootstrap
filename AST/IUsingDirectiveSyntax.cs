using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    public interface IUsingDirectiveSyntax : ISyntax
    {
        Name Name { get; }
    }
}
