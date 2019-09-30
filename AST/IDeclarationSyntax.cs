using Adamant.Tools.Compiler.Bootstrap.Core;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(INamespaceDeclarationSyntax),
        typeof(IEntityDeclarationSyntax))]
    public interface IDeclarationSyntax : ISyntax
    {
        CodeFile File { get; }
        void MarkErrored();
    }
}
