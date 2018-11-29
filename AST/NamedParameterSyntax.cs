using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class NamedParameterSyntax : ParameterSyntax
    {
        public bool IsParams { get; }

        [NotNull] public ExpressionSyntax TypeExpression { get; }
        [CanBeNull] public ExpressionSyntax DefaultValue { get; }

        public NamedParameterSyntax(
            TextSpan span,
            bool isParams,
            bool mutableBinding,
            [NotNull] Name fullName,
            [NotNull] ExpressionSyntax typeExpression,
            [CanBeNull] ExpressionSyntax defaultValue)
            : base(span, mutableBinding, fullName)
        {
            IsParams = isParams;
            TypeExpression = typeExpression;
            DefaultValue = defaultValue;
        }

        public override string ToString()
        {
            var defaultValue = DefaultValue != null ? " = " + DefaultValue : "";
            return $"{Name}: {TypeExpression}{defaultValue}";
        }
    }
}
