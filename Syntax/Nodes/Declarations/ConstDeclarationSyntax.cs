using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Modifiers;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens.Identifiers;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations
{
    public class ConstDeclarationSyntax : MemberDeclarationSyntax
    {
        [NotNull] [ItemNotNull] public SyntaxList<ModifierSyntax> Modifiers { get; }
        [NotNull] public ConstKeywordToken ConstKeyword { get; }
        [NotNull] public IIdentifierToken Name { get; }
        [CanBeNull] public IColonToken Colon { get; }
        [CanBeNull] public ExpressionSyntax TypeExpression { get; }
        [CanBeNull] public new EqualsToken Equals { get; }
        [CanBeNull] public ExpressionSyntax Initializer { get; }
        [NotNull] public ISemicolonToken Semicolon { get; }

        public ConstDeclarationSyntax(
            [NotNull][ItemNotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] ConstKeywordToken constKeyword,
            [NotNull] IIdentifierToken name,
            [CanBeNull] IColonToken colon,
            [CanBeNull] ExpressionSyntax typeExpression,
            [CanBeNull] EqualsToken @equals,
            [CanBeNull] ExpressionSyntax initializer,
            [NotNull] ISemicolonToken semicolon)
        {
            Requires.NotNull(nameof(modifiers), modifiers);
            Requires.NotNull(nameof(constKeyword), constKeyword);
            Requires.NotNull(nameof(name), name);
            Requires.NotNull(nameof(semicolon), semicolon);
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
