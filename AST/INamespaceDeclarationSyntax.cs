using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface INamespaceDeclarationSyntax : IDeclarationSyntax
    {
        bool InGlobalNamespace { get; }
        Name Name { get; }
        Name FullName { get; }
        FixedList<IUsingDirectiveSyntax> UsingDirectives { get; }
        FixedList<IDeclarationSyntax> Declarations { get; }
    }
}
