using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IUsingDirectiveSyntax : ISyntax
    {
        Name Name { get; }
    }
}
