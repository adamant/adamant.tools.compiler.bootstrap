using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class MutableExpressionSyntax : ExpressionSyntax, IMutableExpressionSyntax
    {
        private IExpressionSyntax referent;
        public ref IExpressionSyntax Referent => ref referent;

        public MutableExpressionSyntax(TextSpan span, IExpressionSyntax referent)
            : base(span)
        {
            this.referent  = referent;
        }

        public override string ToString()
        {
            return $"mut {Referent}";
        }
    }
}
