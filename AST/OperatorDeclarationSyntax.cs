using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class OperatorDeclarationSyntax : FunctionDeclarationSyntax
    {
        public ExpressionSyntax LifetimeBounds { get; }
        public ExpressionSyntax ReturnTypeExpression { get; }

        public OperatorDeclarationSyntax(
            CodeFile file,
            FixedList<IModiferToken> modifiers,
            Name fullName,
            TextSpan nameSpan,
            FixedList<GenericParameterSyntax> genericParameters,
            FixedList<ParameterSyntax> parameters,
            ExpressionSyntax lifetimeBounds,
            ExpressionSyntax returnTypeExpression,
            FixedList<GenericConstraintSyntax> genericConstraints,
            FixedList<EffectSyntax> mayEffects,
            FixedList<EffectSyntax> noEffects,
            FixedList<ExpressionSyntax> requires,
            FixedList<ExpressionSyntax> ensures,
            BlockSyntax body)
            : base(file, modifiers, fullName, nameSpan, genericParameters, parameters, genericConstraints, mayEffects, noEffects, requires, ensures, body)
        {
            LifetimeBounds = lifetimeBounds;
            ReturnTypeExpression = returnTypeExpression;
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
