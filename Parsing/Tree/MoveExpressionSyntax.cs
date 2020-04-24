using System;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    [SuppressMessage("Performance", "CA1812:Class Never Instantiated")]
    internal class MoveExpressionSyntax : ExpressionSyntax, IMoveExpressionSyntax
    {
        [SuppressMessage("Style", "IDE0044:Add readonly modifier",
            Justification = "Can't be readonly because a reference to it is exposed")]
        private IExpressionSyntax referent;
        public ref IExpressionSyntax Referent => ref referent;

        private IBindingSymbol? movedSymbol;

        [DisallowNull]
        public IBindingSymbol? MovedSymbol
        {
            get => movedSymbol;
            set
            {
                if (movedSymbol != null)
                    throw new InvalidOperationException("Can't set referenced symbol repeatedly");
                movedSymbol = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public MoveExpressionSyntax(TextSpan span, INameExpressionSyntax referent)
            : base(span)
        {
            this.referent = referent;
        }

        public override string ToString()
        {
            return $"move {Referent}";
        }
    }
}
