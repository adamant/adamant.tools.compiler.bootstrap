using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class FieldDeclarationSyntax : MemberDeclarationSyntax
    {
        public FixedList<AttributeSyntax> Attributes { get; }
        public FixedList<IModiferToken> Modifiers { get; }
        public AccessModifier? GetterAccess { get; }
        public ExpressionSyntax TypeExpression { get; }
        public ExpressionSyntax Initializer { get; }

        public FieldDeclarationSyntax(
            CodeFile file,
            FixedList<AttributeSyntax> attributes,
            FixedList<IModiferToken> modifiers,
            AccessModifier? getterAccess,
            Name fullName,
            TextSpan nameSpan,
            ExpressionSyntax typeExpression,
            ExpressionSyntax initializer)
            : base(file, fullName, nameSpan)
        {
            Attributes = attributes;
            Modifiers = modifiers;
            GetterAccess = getterAccess;
            TypeExpression = typeExpression;
            Initializer = initializer;
        }

        public override string ToString()
        {
            var result = $"{Name}: {TypeExpression}";
            if (Initializer != null) result += Initializer.ToString();
            result += ";";
            return result;
        }
    }
}
