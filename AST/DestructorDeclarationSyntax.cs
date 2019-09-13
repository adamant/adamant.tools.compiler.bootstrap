using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [VisitorNotSupported("Only implemented in parser")]
    public sealed class DestructorDeclarationSyntax : FunctionDeclarationSyntax
    {
        public DestructorDeclarationSyntax(
            CodeFile file,
            FixedList<IModiferToken> modifiers,
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
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
