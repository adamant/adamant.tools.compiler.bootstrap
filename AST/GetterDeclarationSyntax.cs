using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class GetterDeclarationSyntax : FunctionDeclarationSyntax, IAccessorSymbol
    {
        public FixedList<AttributeSyntax> Attributes { get; }
        public Name PropertyName { get; }
        public ExpressionSyntax LifetimeBounds { get; }
        public ExpressionSyntax ReturnTypeExpression { get; }

        public GetterDeclarationSyntax(
            CodeFile file,
            FixedList<AttributeSyntax> attributes,
            FixedList<IModiferToken> modifiers,
            Name propertyName,
            Name fullName,
            TextSpan nameSpan,
            FixedList<ParameterSyntax> parameters,
            ExpressionSyntax lifetimeBounds,
            ExpressionSyntax returnTypeExpression,
            FixedList<EffectSyntax> mayEffects,
            FixedList<EffectSyntax> noEffects,
            FixedList<ExpressionSyntax> requires,
            FixedList<ExpressionSyntax> ensures,
            BlockSyntax body)
            : base(file, modifiers, fullName, nameSpan, null, parameters,
                FixedList<GenericConstraintSyntax>.Empty, mayEffects, noEffects, requires, ensures, body)
        {
            Attributes = attributes;
            PropertyName = propertyName;
            LifetimeBounds = lifetimeBounds;
            ReturnTypeExpression = returnTypeExpression;
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
