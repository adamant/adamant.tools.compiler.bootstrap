using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class SelfExpressionSyntax : InstanceExpressionSyntax
    {
        public SelfExpressionSyntax(TextSpan span)
            : base(span)
        {
        }

        public override string ToString()
        {
            return "self";
        }
    }
}
