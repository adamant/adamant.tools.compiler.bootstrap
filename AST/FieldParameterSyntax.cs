using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public sealed class FieldParameterSyntax : ParameterSyntax
    {
        public SimpleName FieldName { get; }

        public ExpressionSyntax DefaultValue { get; }

        public FieldParameterSyntax(
            TextSpan span,
            Name fullName,
            SimpleName fieldName,
            ExpressionSyntax defaultValue)
            : base(span, false, fullName)
        {
            FieldName = fieldName;
            DefaultValue = defaultValue;
        }

        public override string ToString()
        {
            var defaultValue = DefaultValue != null ? " = " + DefaultValue : "";
            return $".{Name}{defaultValue}";
        }
    }
}
