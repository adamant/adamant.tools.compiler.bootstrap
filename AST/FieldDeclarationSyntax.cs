using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class FieldDeclarationSyntax : MemberDeclarationSyntax
    {
        [NotNull] public FixedList<AttributeSyntax> Attributes { get; }
        [NotNull] [ItemNotNull] public FixedList<IModiferToken> Modifiers { get; }
        public AccessModifier? GetterAccess { get; }
        [CanBeNull] public ExpressionSyntax TypeExpression { get; }
        [CanBeNull] public ExpressionSyntax Initializer { get; }
        [NotNull] public TypePromise Type { get; } = new TypePromise();

        public FieldDeclarationSyntax(
            [NotNull] CodeFile file,
            [NotNull] FixedList<AttributeSyntax> attributes,
            [NotNull] [ItemNotNull] FixedList<IModiferToken> modifiers,
            AccessModifier? getterAccess,
            [NotNull] Name fullName,
            TextSpan nameSpan,
            [CanBeNull] ExpressionSyntax typeExpression,
            [CanBeNull] ExpressionSyntax initializer)
            : base(file, fullName, nameSpan)
        {
            Attributes = attributes;
            Modifiers = modifiers;
            GetterAccess = getterAccess;
            TypeExpression = typeExpression;
            Initializer = initializer;
        }

        protected override DataType GetDataType()
        {
            return Type.Fulfilled();
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
