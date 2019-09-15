using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class SelfTypeExpressionSyntax : TypeSyntax
    {
        public SelfTypeExpressionSyntax(TextSpan span)
            : base(span)
        {
        }

        public override string ToString()
        {
            return "Self";
        }
    }
}
