using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class FieldDeclarationSyntax : MemberDeclarationSyntax
    {
        [NotNull] [ItemNotNull] public FixedList<IModiferToken> Modifiers { get; }
        [NotNull] public IIdentifierTokenPlace Name { get; }
        [CanBeNull] public ExpressionSyntax TypeExpression { get; }
        [CanBeNull] public ExpressionSyntax Initializer { get; }

        public FieldDeclarationSyntax(
            [NotNull] [ItemNotNull] FixedList<IModiferToken> modifiers,
            [NotNull] IIdentifierTokenPlace name,
            [CanBeNull] ExpressionSyntax typeExpression,
            [CanBeNull] ExpressionSyntax initializer)
            : base(name.Span)
        {
            Modifiers = modifiers;
            Name = name;
            TypeExpression = typeExpression;
            Initializer = initializer;
        }
    }
}
