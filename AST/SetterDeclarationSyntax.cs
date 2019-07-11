using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [VisitorNotSupported("Only implemented in parser")]
    public class SetterDeclarationSyntax : FunctionDeclarationSyntax, IAccessorSymbol
    {
        public FixedList<AttributeSyntax> Attributes { get; }
        public Name PropertyName { get; }

        public SetterDeclarationSyntax(
            CodeFile file,
            FixedList<AttributeSyntax> attributes,
            FixedList<IModiferToken> modifiers,
            Name propertyName,
            Name fullName,
            TextSpan nameSpan,
            FixedList<ParameterSyntax> parameters,
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
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
