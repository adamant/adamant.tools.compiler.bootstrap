using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public sealed class SelfParameterIL : ParameterIL
    {
        public new SelfParameterSymbol Symbol { get; }

        public SelfParameterIL(SelfParameterSymbol symbol)
            : base(symbol, symbol.IsMutableBinding, symbol.DataType)
        {
            Symbol = symbol;
        }
    }
}
