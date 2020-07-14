using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.FST;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic.ImplicitOperations
{
    internal class ImplicitMoveSyntax : ImplicitExpressionSyntax, IMoveExpressionSyntax
    {
        [SuppressMessage("Style", "IDE0044:Add readonly modifier",
            Justification = "Can't be readonly because a reference to it is exposed")]
        private IExpressionSyntax referent;

        public ref IExpressionSyntax Referent
        {
            [DebuggerStepThrough]
            get => ref referent;
        }

        private readonly IBindingSymbol movedSymbol;
        [DisallowNull]
        public IBindingSymbol? MovedSymbol
        {
            [DebuggerStepThrough]
            get => movedSymbol;
            set => throw new InvalidOperationException("Can't set referenced symbol repeatedly");
        }

        public ImplicitMoveSyntax(IExpressionSyntax referent, DataType type, IBindingSymbol movedSymbol)
            : base(type, referent.Span)
        {
            this.referent = referent;
            this.movedSymbol = movedSymbol ?? throw new ArgumentNullException(nameof(movedSymbol));
        }

        public override string ToString()
        {
            return $"⟦move⟧ {Referent}";
        }

        public string ToGroupedString(OperatorPrecedence surroundingPrecedence)
        {
            return $"({ToString()})";
        }
    }
}
