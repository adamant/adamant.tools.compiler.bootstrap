using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class NamedParameterSyntax : ParameterSyntax
    {
        public ExpressionSyntax TypeExpression { get; }
        public ExpressionSyntax DefaultValue { get; }

        public NamedParameterSyntax(
            TextSpan span,
            //bool isParams,
            bool isMutableBinding,
            Name fullName,
            ExpressionSyntax typeExpression,
            ExpressionSyntax defaultValue)
            : base(span, isMutableBinding, fullName)
        {
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
