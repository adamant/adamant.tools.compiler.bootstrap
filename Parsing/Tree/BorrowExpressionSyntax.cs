using System;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class BorrowExpressionSyntax : ExpressionSyntax, IBorrowExpressionSyntax
    {
        [SuppressMessage("Style", "IDE0044:Add readonly modifier",
            Justification = "Can't be readonly because a reference to it is exposed")]
        private IExpressionSyntax referent;
        public ref IExpressionSyntax Referent => ref referent;

        private IBindingSymbol? borrowedSymbol;

        [DisallowNull]
        public IBindingSymbol? BorrowedSymbol
        {
            get => borrowedSymbol;
            set
            {
                if (borrowedSymbol != null)
                    throw new InvalidOperationException("Can't set referenced symbol repeatedly");
                borrowedSymbol = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

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
