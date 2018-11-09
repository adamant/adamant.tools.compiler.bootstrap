using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class ConstDeclarationSyntax : MemberDeclarationSyntax
    {
        [NotNull] public SyntaxList<AttributeSyntax> Attributes { get; }
        [NotNull] [ItemNotNull] public SyntaxList<ModifierSyntax> Modifiers { get; }
        [NotNull] public IConstKeywordToken ConstKeyword { get; }
        [NotNull] public IIdentifierTokenPlace Name { get; }
        [CanBeNull] public IColonTokenPlace Colon { get; }
        [CanBeNull] public ExpressionSyntax TypeExpression { get; }
        [CanBeNull] public new IEqualsToken Equals { get; }
        [CanBeNull] public ExpressionSyntax Initializer { get; }
        [NotNull] public ISemicolonTokenPlace Semicolon { get; }

        public ConstDeclarationSyntax(
            [NotNull] SyntaxList<AttributeSyntax> attributes,
            [NotNull] [ItemNotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] IConstKeywordToken constKeyword,
            [NotNull] IIdentifierTokenPlace name,
            [CanBeNull] IColonTokenPlace colon,
            [CanBeNull] ExpressionSyntax typeExpression,
            [CanBeNull] IEqualsToken @equals,
            [CanBeNull] ExpressionSyntax initializer,
            [NotNull] ISemicolonTokenPlace semicolon)
            : base(TextSpan.Covering(constKeyword.Span, name.Span))
        {
            Requires.NotNull(nameof(attributes), attributes);
            Requires.NotNull(nameof(modifiers), modifiers);
            Requires.NotNull(nameof(constKeyword), constKeyword);
            Requires.NotNull(nameof(name), name);
            Requires.NotNull(nameof(semicolon), semicolon);
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
