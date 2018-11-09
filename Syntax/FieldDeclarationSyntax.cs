using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class FieldDeclarationSyntax : MemberDeclarationSyntax
    {
        [NotNull] [ItemNotNull] public SyntaxList<ModifierSyntax> Modifiers { get; }
        [NotNull] public IBindingToken Binding { get; }
        [CanBeNull] public FieldGetterSyntax Getter { get; }
        [NotNull] public IIdentifierTokenPlace Name { get; }
        [CanBeNull] public IColonTokenPlace Colon { get; }
        [CanBeNull] public ExpressionSyntax TypeExpression { get; }
        [CanBeNull] public new IEqualsToken Equals { get; }
        [CanBeNull] public ExpressionSyntax Initializer { get; }
        [NotNull] public ISemicolonTokenPlace Semicolon { get; }

        public FieldDeclarationSyntax(
            [NotNull][ItemNotNull] SyntaxList<ModifierSyntax> modifiers,
            [NotNull] IBindingToken binding,
            [CanBeNull] FieldGetterSyntax getter,
            [NotNull] IIdentifierTokenPlace name,
            [CanBeNull] IColonTokenPlace colon,
            [CanBeNull] ExpressionSyntax typeExpression,
            [CanBeNull] IEqualsToken @equals,
            [CanBeNull] ExpressionSyntax initializer,
            [NotNull] ISemicolonTokenPlace semicolon)
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
