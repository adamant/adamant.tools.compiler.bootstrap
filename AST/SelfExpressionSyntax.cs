using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class SelfExpressionSyntax : InstanceExpressionSyntax, ISelfExpressionSyntax
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
