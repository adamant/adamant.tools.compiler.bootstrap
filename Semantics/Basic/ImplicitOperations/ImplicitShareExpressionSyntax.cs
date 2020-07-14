using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.FST;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic.ImplicitOperations
{
    internal class ImplicitShareExpressionSyntax : ImplicitExpressionSyntax, IShareExpressionSyntax
    {
        [SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification =
            "Can't be readonly because a reference to it is exposed")]
        private IExpressionSyntax referent;
        public ref IExpressionSyntax Referent
        {
            [DebuggerStepThrough]
            get => ref referent;
        }

        [DisallowNull]
        public IBindingSymbol SharedSymbol { [DebuggerStepThrough] get; }

        public ImplicitShareExpressionSyntax(IExpressionSyntax referent, DataType type, IBindingSymbol sharedSymbol)
            : base(type, referent.Span, ExpressionSemantics.Share)
        {
            this.referent = referent;
            this.SharedSymbol = sharedSymbol ?? throw new ArgumentNullException(nameof(sharedSymbol));
        }

        public override string ToString()
        {
            return $"⟦share⟧ {Referent}";
        }

        public string ToGroupedString(OperatorPrecedence surroundingPrecedence)
        {
            return $"({ToString()})";
        }
    }
}
