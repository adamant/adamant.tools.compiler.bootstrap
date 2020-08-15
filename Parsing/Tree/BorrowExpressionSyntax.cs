using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Promises;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class BorrowExpressionSyntax : ExpressionSyntax, IBorrowExpressionSyntax
    {
        [SuppressMessage("Style", "IDE0044:Add readonly modifier",
            Justification = "Can't be readonly because a reference to it is exposed")]
        private IExpressionSyntax referent;
        public ref IExpressionSyntax Referent => ref referent;
        public Promise<BindingSymbol?> ReferencedSymbol { get; } = new Promise<BindingSymbol?>();

        public BorrowExpressionSyntax(TextSpan span, IExpressionSyntax referent)
            : base(span, ExpressionSemantics.Borrow)
        {
            this.referent  = referent;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

        public override string ToString()
        {
            return $"mut {Referent}";
        }
    }
}
