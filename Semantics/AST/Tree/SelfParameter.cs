using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class SelfParameter : Parameter, ISelfParameter
    {
        public SelfParameterSymbol Symbol { get; }
        BindingSymbol IBinding.Symbol => Symbol;
        public SelfParameter(TextSpan span, SelfParameterSymbol symbol, bool unused)
            : base(span, unused)
        {
            Symbol = symbol;
        }

        public override string ToString()
        {
            var value = "self";
            if (Symbol.IsMutableBinding) value = "mut " + value;
            return value;
        }
    }
}
