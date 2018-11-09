using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class FileNamespaceDeclarationSyntax : NamespaceDeclarationSyntax
    {
        [CanBeNull] public ISemicolonTokenPlace Semicolon { get; }

        public FileNamespaceDeclarationSyntax(
            [CanBeNull] INamespaceKeywordToken namespaceKeyword,
            [CanBeNull] NameSyntax name,
            [CanBeNull] ISemicolonTokenPlace semicolon,
            [NotNull] SyntaxList<UsingDirectiveSyntax> usingDirectives,
            [NotNull] SyntaxList<DeclarationSyntax> declarations)
            : base(namespaceKeyword, name, usingDirectives, declarations)
        {
            Semicolon = semicolon;
        }
    }
}
