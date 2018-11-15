using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class NamedParameterSyntax : ParameterSyntax
    {
        public bool IsParams { get; }
        public bool MutableBinding { get; }
        [NotNull] public IIdentifierToken Name { get; }
        [NotNull] public ExpressionSyntax Type { get; }
        [CanBeNull] public ExpressionSyntax DefaultValue { get; }

        public NamedParameterSyntax(
            TextSpan span,
            bool isParams,
            bool mutableBinding,
            [NotNull] IIdentifierToken name,
            [NotNull] ExpressionSyntax type,
            [CanBeNull] ExpressionSyntax defaultValue)
            : base(span)
        {
            IsParams = isParams;
            MutableBinding = mutableBinding;
            Name = name;
            Type = type;
            DefaultValue = defaultValue;
        }
    }
}
