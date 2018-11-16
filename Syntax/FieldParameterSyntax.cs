using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class FieldParameterSyntax : ParameterSyntax
    {
        [NotNull] public IIdentifierToken Value { get; }
        [CanBeNull] public ExpressionSyntax DefaultValue { get; }

        public FieldParameterSyntax(
            TextSpan span,
            [NotNull] IIdentifierToken value,
            [CanBeNull] ExpressionSyntax defaultValue)
            : base(span)
        {
            Value = value;
            DefaultValue = defaultValue;
        }
    }
}
