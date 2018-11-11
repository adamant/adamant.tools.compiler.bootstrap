using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class ConstDeclarationSyntax : MemberDeclarationSyntax
    {
        [NotNull] public FixedList<AttributeSyntax> Attributes { get; }
        [NotNull] [ItemNotNull] public FixedList<ModifierSyntax> Modifiers { get; }
        [NotNull] public IConstKeywordToken ConstKeyword { get; }
        [NotNull] public IIdentifierTokenPlace Name { get; }
        [CanBeNull] public IColonTokenPlace Colon { get; }
        [CanBeNull] public ExpressionSyntax TypeExpression { get; }
        [CanBeNull] public new IEqualsToken Equals { get; }
        [CanBeNull] public ExpressionSyntax Initializer { get; }
        [NotNull] public ISemicolonTokenPlace Semicolon { get; }

        public ConstDeclarationSyntax(
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull, ItemNotNull] FixedList<ModifierSyntax> modifiers,
            [NotNull] IConstKeywordToken constKeyword,
            [NotNull] IIdentifierTokenPlace name,
            [CanBeNull] IColonTokenPlace colon,
            [CanBeNull] ExpressionSyntax typeExpression,
            [CanBeNull] IEqualsToken @equals,
            [CanBeNull] ExpressionSyntax initializer,
            [NotNull] ISemicolonTokenPlace semicolon)
            : base(TextSpan.Covering(constKeyword.Span, name.Span))
        {
            Attributes = attributes;
            Modifiers = modifiers;
            ConstKeyword = constKeyword;
            Name = name;
            Colon = colon;
            TypeExpression = typeExpression;
            Equals = @equals;
            Initializer = initializer;
            Semicolon = semicolon;
        }
    }
}
