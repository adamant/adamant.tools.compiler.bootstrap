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

        private readonly IBindingMetadata movedSymbol;
        [DisallowNull]
        public IBindingMetadata? MovedSymbol
        {
            [DebuggerStepThrough]
            get => movedSymbol;
            set => throw new InvalidOperationException("Can't set referenced symbol repeatedly");
        }

        public Promise<BindingSymbol?> ReferencedSymbol { get; }

        public ImplicitMoveSyntax(IExpressionSyntax referent, DataType type, BindingSymbol? referencedSymbol, IBindingMetadata movedSymbol)
            : base(type, referent.Span)
        {
            this.referent = referent;
            ReferencedSymbol = Promise.ForValue(referencedSymbol);
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
