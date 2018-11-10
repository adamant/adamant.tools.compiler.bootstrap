using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class BlockNamespaceDeclarationSyntax : NamespaceDeclarationSyntax
    {
        [NotNull] public FixedList<AttributeSyntax> Attributes { get; }
        [NotNull] public FixedList<IModiferToken> Modifiers { get; }

        public BlockNamespaceDeclarationSyntax(
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] FixedList<IModiferToken> modifiers,
            [NotNull] NameSyntax name,
            [NotNull] FixedList<UsingDirectiveSyntax> usingDirectives,
            [NotNull] FixedList<DeclarationSyntax> declarations)
            : base(name, usingDirectives, declarations)
        {
            Attributes = attributes;
            Modifiers = modifiers;
        }
    }
}
