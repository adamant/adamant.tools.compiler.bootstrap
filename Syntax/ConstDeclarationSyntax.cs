using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class ConstDeclarationSyntax : MemberDeclarationSyntax
    {
        [NotNull] public FixedList<AttributeSyntax> Attributes { get; }
        [NotNull] [ItemNotNull] public FixedList<IModiferToken> Modifiers { get; }
        [NotNull] public IIdentifierToken Name { get; }
        [CanBeNull] public ExpressionSyntax Type { get; }
        [CanBeNull] public ExpressionSyntax Initializer { get; }

        public ConstDeclarationSyntax(
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] [ItemNotNull] FixedList<IModiferToken> modifiers,
            [NotNull] IIdentifierToken name,
            [CanBeNull] ExpressionSyntax type,
            [CanBeNull] ExpressionSyntax initializer)
            : base(name.Span)
        {
            Attributes = attributes;
            Modifiers = modifiers;
            Name = name;
            Type = type;
            Initializer = initializer;
        }
    }
}
