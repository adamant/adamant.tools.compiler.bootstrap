using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Promises;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
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
        public Promise<BindingSymbol?> ReferencedSymbol { get; }

        public ImplicitShareExpressionSyntax(IExpressionSyntax referent, DataType type, BindingSymbol? referencedSymbol)
            : base(type, referent.Span, ExpressionSemantics.Share)
        {
            this.referent = referent;
            ReferencedSymbol = Promise.ForValue(referencedSymbol);
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
