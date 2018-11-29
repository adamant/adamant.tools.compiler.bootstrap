using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class FieldDeclarationSyntax : DeclarationSyntax, IMemberDeclarationSyntax
    {
        [NotNull] public FixedList<AttributeSyntax> Attributes { get; }
        [NotNull] [ItemNotNull] public FixedList<IModiferToken> Modifiers { get; }
        public AccessModifier? GetterAccess { get; }
        [NotNull] public SimpleName Name { get; }
        [CanBeNull] public ExpressionSyntax TypeExpression { get; }
        [CanBeNull] public ExpressionSyntax Initializer { get; }
        [NotNull] public TypePromise Type { get; } = new TypePromise();

        public FieldDeclarationSyntax(
            [NotNull] CodeFile file,
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] [ItemNotNull] FixedList<IModiferToken> modifiers,
            AccessModifier? getterAccess,
            [NotNull] string name,
            TextSpan nameSpan,
            [CanBeNull] ExpressionSyntax typeExpression,
            [CanBeNull] ExpressionSyntax initializer)
            : base(file, nameSpan)
        {
            Attributes = attributes;
            Modifiers = modifiers;
            GetterAccess = getterAccess;
            Name = new SimpleName(name);
            TypeExpression = typeExpression;
            Initializer = initializer;
        }

        DeclarationSyntax IDeclarationSyntax.AsDeclarationSyntax => this;

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
