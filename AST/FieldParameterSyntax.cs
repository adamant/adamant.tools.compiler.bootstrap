using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class FieldParameterSyntax : ParameterSyntax
    {
        public ExpressionSyntax DefaultValue { get; }

        public FieldParameterSyntax(
            TextSpan span,
            Name fullName,
            ExpressionSyntax defaultValue)
            : base(span, false, fullName)
        {
            DefaultValue = defaultValue;
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
