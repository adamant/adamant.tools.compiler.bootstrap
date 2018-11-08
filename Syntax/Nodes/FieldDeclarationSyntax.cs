using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class FieldDeclarationSyntax : MemberDeclarationSyntax
    {
        [NotNull] [ItemNotNull] public SyntaxList<ModifierSyntax> Modifiers { get; }
        [NotNull] public IBindingKeywordToken Binding { get; }
        [CanBeNull] public FieldGetterSyntax Getter { get; }
        [NotNull] public IIdentifierToken Name { get; }
        [CanBeNull] public IColonToken Colon { get; }
        [CanBeNull] public ExpressionSyntax TypeExpression { get; }
        [CanBeNull] public new EqualsToken Equals { get; }
        [CanBeNull] public ExpressionSyntax Initializer { get; }
        [NotNull] public ISemicolonToken Semicolon { get; }

        public FieldDeclarationSyntax(
            [NotNull][ItemNotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] IBindingKeywordToken binding,
            [CanBeNull] FieldGetterSyntax getter,
            [NotNull] IIdentifierToken name,
            [CanBeNull] IColonToken colon,
            [CanBeNull] ExpressionSyntax typeExpression,
            [CanBeNull] EqualsToken @equals,
            [CanBeNull] ExpressionSyntax initializer,
            [NotNull] ISemicolonToken semicolon)
            : base(TextSpan.Covering(binding.Span, name.Span))
        {
            Requires.NotNull(nameof(modifiers), modifiers);
            Requires.NotNull(nameof(binding), binding);
            Requires.NotNull(nameof(name), name);
            Requires.NotNull(nameof(semicolon), semicolon);
            Modifiers = modifiers;
            Binding = binding;
            Getter = getter;
            Name = name;
            Colon = colon;
            TypeExpression = typeExpression;
            Equals = @equals;
            Initializer = initializer;
            Semicolon = semicolon;
        }
    }
}