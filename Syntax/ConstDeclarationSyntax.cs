using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class ConstDeclarationSyntax : MemberDeclarationSyntax
    {
        [NotNull] public FixedList<AttributeSyntax> Attributes { get; }
        [NotNull] [ItemNotNull] public FixedList<IModiferToken> Modifiers { get; }
        [NotNull] public SimpleName Name { get; }
        [CanBeNull] public ExpressionSyntax Type { get; }
        [CanBeNull] public ExpressionSyntax Initializer { get; }

        public ConstDeclarationSyntax(
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] [ItemNotNull] FixedList<IModiferToken> modifiers,
            [NotNull] string name,
            TextSpan nameSpan,
            [CanBeNull] ExpressionSyntax type,
            [CanBeNull] ExpressionSyntax initializer)
            : base(nameSpan)
        {
            Attributes = attributes;
            Modifiers = modifiers;
            Name = new SimpleName(name);
            Type = type;
            Initializer = initializer;
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
