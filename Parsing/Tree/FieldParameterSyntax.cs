using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class FieldParameterSyntax : ParameterSyntax, IFieldParameterSyntax
    {
        public SimpleName FieldName { get; }

        public IExpressionSyntax? DefaultValue { get; }

        public FieldParameterSyntax(
            TextSpan span,
            Name fullName,
            SimpleName fieldName,
            IExpressionSyntax? defaultValue)
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
