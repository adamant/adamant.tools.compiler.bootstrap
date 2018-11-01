using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Directives;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Namespaces
{
    public class FileNamespaceDeclarationSyntax : NamespaceDeclarationSyntax
    {
        [CanBeNull] public ISemicolonToken Semicolon { get; }

        public FileNamespaceDeclarationSyntax(
            [CanBeNull] NamespaceKeywordToken namespaceKeyword,
            [CanBeNull] NameSyntax name,
            [CanBeNull] ISemicolonToken semicolon,
            [NotNull] SyntaxList<UsingDirectiveSyntax> usingDirectives,
            [NotNull] SyntaxList<DeclarationSyntax> declarations)
            : base(namespaceKeyword, name, usingDirectives, declarations)
        {
            Semicolon = semicolon;
        }
    }
}
