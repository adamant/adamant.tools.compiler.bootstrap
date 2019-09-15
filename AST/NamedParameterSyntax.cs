using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class NamedParameterSyntax : ParameterSyntax
    {
        public bool IsParams { get; }

        public ExpressionSyntax TypeExpression { get; }
        public ExpressionSyntax DefaultValue { get; }

        public NamedParameterSyntax(
            TextSpan span,
            bool isParams,
            bool mutableBinding,
            Name fullName,
            ExpressionSyntax typeExpression,
            ExpressionSyntax defaultValue)
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
