using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class OperatorFunctionDeclarationSyntax : FunctionDeclarationSyntax
    {
        [NotNull] public IOperatorKeywordToken OperatorKeyword { get; }
        public override IIdentifierTokenPlace Name => throw new System.NotImplementedException();
        [NotNull] public IOperatorTokenPlace Operator { get; }
        [NotNull] public ExpressionSyntax ReturnTypeExpression { get; }
        [NotNull] public FixedList<GenericConstraintSyntax> GenericConstraints { get; }

        public OperatorFunctionDeclarationSyntax(
            [NotNull] FixedList<IModiferToken> modifiers,
            [NotNull] IOperatorKeywordToken operatorKeyword,
            [NotNull] IOperatorTokenPlace @operator,
            [NotNull] FixedList<ParameterSyntax> parameters,
            [NotNull] ExpressionSyntax returnTypeExpression,
            [NotNull] FixedList<GenericConstraintSyntax> genericConstraints,
            [NotNull] FixedList<EffectSyntax> mayEffects,
            [NotNull] FixedList<EffectSyntax> noEffects,
            [NotNull] FixedList<ExpressionSyntax> requires,
            [NotNull] FixedList<ExpressionSyntax> ensures,
            [CanBeNull] BlockSyntax body)
            : base(TextSpan.Covering(operatorKeyword.Span, @operator.Span),
            modifiers, parameters, mayEffects, noEffects, requires, ensures, body)
        {
            OperatorKeyword = operatorKeyword;
            Operator = @operator;
            ReturnTypeExpression = returnTypeExpression;
            GenericConstraints = genericConstraints;
        }
    }
}
