using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public sealed class SelfParameter : Parameter
    {
        public new SelfParameterSymbol Symbol { get; }

        public SelfParameter(SelfParameterSymbol symbol)
            : base(symbol, symbol.IsMutableBinding, symbol.DataType)
        {
            Symbol = symbol;
        }
    }
}
