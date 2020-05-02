using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    internal class VariablePlace : AssignablePlace
    {
        public VariablePlace(IBindingSymbol symbol)
            : base(symbol) { }
    }
}
