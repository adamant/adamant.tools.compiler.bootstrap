using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Directives;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Namespaces
{
    public abstract class NamespaceDeclarationSyntax : DeclarationSyntax
    {
        [CanBeNull] public NamespaceKeywordToken NamespaceKeyword { get; }
        [CanBeNull] public NameSyntax Name { get; }
        [NotNull] public SyntaxList<UsingDirectiveSyntax> UsingDirectives { get; }
        [NotNull] public SyntaxList<DeclarationSyntax> Declarations { get; }

        protected NamespaceDeclarationSyntax(
            [CanBeNull] NamespaceKeywordToken namespaceKeyword,
            [CanBeNull] NameSyntax name,
            [NotNull] SyntaxList<UsingDirectiveSyntax> usingDirectives,
            [NotNull] SyntaxList<DeclarationSyntax> declarations)
        {
            Requires.NotNull(nameof(usingDirectives), usingDirectives);
            Requires.NotNull(nameof(declarations), declarations);
            NamespaceKeyword = namespaceKeyword;
            Name = name;
            UsingDirectives = usingDirectives;
            Declarations = declarations;
        }
    }
}
