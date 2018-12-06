using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class ConstDeclarationSyntax : MemberDeclarationSyntax
    {
        public FixedList<AttributeSyntax> Attributes { get; }
        public FixedList<IModiferToken> Modifiers { get; }
        public ExpressionSyntax TypeExpression { get; }
        public ExpressionSyntax Initializer { get; }

        public ConstDeclarationSyntax(
            CodeFile file,
            FixedList<AttributeSyntax> attributes,
            FixedList<IModiferToken> modifiers,
            Name fullName,
            TextSpan nameSpan,
            ExpressionSyntax typeExpression,
            ExpressionSyntax initializer)
            : base(file, fullName, nameSpan)
        {
            Attributes = attributes;
            Modifiers = modifiers;
            TypeExpression = typeExpression;
            Initializer = initializer;
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
