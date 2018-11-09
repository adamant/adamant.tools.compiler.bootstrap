using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class BlockNamespaceDeclarationSyntax : NamespaceDeclarationSyntax
    {
        [NotNull] public SyntaxList<AttributeSyntax> Attributes { get; }
        [NotNull] public SyntaxList<ModifierSyntax> Modifiers { get; }
        [NotNull] public IOpenBraceTokenPlace OpenBrace { get; }
        [NotNull] public ICloseBraceTokenPlace CloseBrace { get; }

        public BlockNamespaceDeclarationSyntax(
            [NotNull] SyntaxList<AttributeSyntax> attributes,
            [NotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] INamespaceKeywordToken namespaceKeyword,
            [NotNull] NameSyntax name,
            [NotNull] IOpenBraceTokenPlace openBrace,
            [NotNull] SyntaxList<UsingDirectiveSyntax> usingDirectives,
            [NotNull] SyntaxList<DeclarationSyntax> declarations,
            [NotNull] ICloseBraceTokenPlace closeBrace)
            : base(namespaceKeyword, name, usingDirectives, declarations)
        {
            Requires.NotNull(nameof(attributes), attributes);
            Requires.NotNull(nameof(modifiers), modifiers);
            Requires.NotNull(nameof(namespaceKeyword), namespaceKeyword);
            Requires.NotNull(nameof(name), name);
            Requires.NotNull(nameof(openBrace), openBrace);
            Requires.NotNull(nameof(closeBrace), closeBrace);
            Attributes = attributes;
            Modifiers = modifiers;
            OpenBrace = openBrace;
            CloseBrace = closeBrace;
        }
    }
}
