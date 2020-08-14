using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public sealed class NamedParameter : Parameter
    {
        public new VariableSymbol Symbol { get; }

        public NamedParameter(VariableSymbol symbol)
            : base(symbol, symbol.IsMutableBinding, symbol.DataType)
        {
            Symbol = symbol;
        }
    }
}
