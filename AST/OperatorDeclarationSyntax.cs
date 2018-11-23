using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class OperatorDeclarationSyntax : FunctionDeclarationSyntax
    {
        [CanBeNull] public ExpressionSyntax ReturnTypeExpression { get; }

        public OperatorDeclarationSyntax(
            [NotNull] CodeFile file,
            [NotNull] FixedList<IModiferToken> modifiers,
            TextSpan nameSpan,
            [CanBeNull] FixedList<GenericParameterSyntax> genericParameters,
            [NotNull] FixedList<ParameterSyntax> parameters,
            [CanBeNull] ExpressionSyntax returnTypeExpression,
            [NotNull] FixedList<GenericConstraintSyntax> genericConstraints,
            [NotNull] FixedList<EffectSyntax> mayEffects,
            [NotNull] FixedList<EffectSyntax> noEffects,
            [NotNull] FixedList<ExpressionSyntax> requires,
            [NotNull] FixedList<ExpressionSyntax> ensures,
            [CanBeNull] BlockSyntax body)
            : base(file, modifiers, nameSpan, genericParameters, parameters, genericConstraints, mayEffects, noEffects, requires, ensures, body)
        {
            ReturnTypeExpression = returnTypeExpression;
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
