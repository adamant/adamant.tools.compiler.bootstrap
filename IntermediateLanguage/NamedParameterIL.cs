using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public sealed class NamedParameterIL : ParameterIL
    {
        public new VariableSymbol Symbol { get; }

        public NamedParameterIL(VariableSymbol symbol)
            : base(symbol, symbol.IsMutableBinding, symbol.DataType)
        {
            Symbol = symbol;
        }
    }
}
