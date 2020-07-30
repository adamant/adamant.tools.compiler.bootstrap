using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    [SuppressMessage("Performance", "CA1812:Class Never Instantiated")]
    internal class MoveExpressionSyntax : ExpressionSyntax, IMoveExpressionSyntax
    {
        [SuppressMessage("Style", "IDE0044:Add readonly modifier",
            Justification = "Can't be readonly because a reference to it is exposed")]
        private IExpressionSyntax referent;
        public ref IExpressionSyntax Referent
        {
            [DebuggerStepThrough]
            get => ref referent;
        }

        private IBindingSymbol? movedSymbol;
        [DisallowNull]
        public IBindingSymbol? MovedSymbol
        {
            [DebuggerStepThrough]
            get => movedSymbol;
            set
            {
                if (movedSymbol != null)
                    throw new InvalidOperationException("Can't set referenced symbol repeatedly");
                movedSymbol = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public MoveExpressionSyntax(TextSpan span, INameExpressionSyntax referent)
            : base(span) // TODO this could be a move or acquire?
        {
            this.referent = referent;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

        public override string ToString()
        {
            return $"move {Referent}";
        }
    }
}
