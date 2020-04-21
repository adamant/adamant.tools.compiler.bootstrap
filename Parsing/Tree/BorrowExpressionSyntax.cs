using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class BorrowExpressionSyntax : ExpressionSyntax, IBorrowExpressionSyntax
    {
        [SuppressMessage("Style", "IDE0044:Add readonly modifier",
            Justification = "Can't be readonly because a reference to it is exposed")]
        private IExpressionSyntax referent;
        public ref IExpressionSyntax Referent => ref referent;

        public BorrowExpressionSyntax(TextSpan span, IExpressionSyntax referent)
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
