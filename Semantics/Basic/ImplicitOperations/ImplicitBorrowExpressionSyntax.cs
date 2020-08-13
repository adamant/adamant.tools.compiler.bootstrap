using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic.ImplicitOperations
{
    internal class ImplicitBorrowExpressionSyntax : ImplicitExpressionSyntax, IBorrowExpressionSyntax
    {
        [SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification =
            "Can't be readonly because a reference to it is exposed")]
        private IExpressionSyntax referent;

        public ref IExpressionSyntax Referent
        {
            [DebuggerStepThrough]
            get => ref referent;
        }
        public Promise<BindingSymbol?> ReferencedSymbol { get; }

        private readonly IBindingMetadata borrowedSymbol;
        [DisallowNull]
        public IBindingMetadata? BorrowedFromBinding
        {
            [DebuggerStepThrough]
            get => borrowedSymbol;
            set => throw new InvalidOperationException("Can't set referenced symbol repeatedly");
        }

        public ImplicitBorrowExpressionSyntax(IExpressionSyntax referent, DataType type, BindingSymbol? referencedSymbol, IBindingMetadata borrowedSymbol)
            : base(type, referent.Span, ExpressionSemantics.Borrow)
        {
            this.referent = referent;
            ReferencedSymbol = Promise.ForValue(referencedSymbol);
            this.borrowedSymbol = borrowedSymbol ?? throw new ArgumentNullException(nameof(borrowedSymbol));
        }

        public override string ToString()
        {
            return $"⟦borrow⟧ {Referent}";
        }

        public string ToGroupedString(OperatorPrecedence surroundingPrecedence)
        {
            return $"({this})";
        }
    }
}
