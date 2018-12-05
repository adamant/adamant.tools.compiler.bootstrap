using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class ConstructorDeclarationSyntax : FunctionDeclarationSyntax
    {
        public ConstructorDeclarationSyntax(
            CodeFile file,
            FixedList<IModiferToken> modifiers,
            Name fullName,
            TextSpan nameSpan,
            FixedList<GenericParameterSyntax> genericParameters,
            FixedList<ParameterSyntax> parameters,
            FixedList<GenericConstraintSyntax> genericConstraints,
            FixedList<EffectSyntax> mayEffects,
            FixedList<EffectSyntax> noEffects,
            FixedList<ExpressionSyntax> requires,
            FixedList<ExpressionSyntax> ensures,
            BlockSyntax body)
            : base(file, modifiers, fullName, nameSpan, genericParameters, parameters, genericConstraints,
                mayEffects, noEffects, requires, ensures, body)
        {
        }

        public override string ToString()
        {
            if (GenericParameters != null)
                return $"{FullName}[{string.Join(", ", GenericParameters)}]({string.Join(", ", Parameters)})";

            return $"{FullName}({string.Join(", ", Parameters)})";
        }
    }
}
