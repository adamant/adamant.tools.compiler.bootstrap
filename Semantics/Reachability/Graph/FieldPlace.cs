using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    internal class FieldPlace : AssignablePlace
    {
        public FieldPlace(IBindingSymbol symbol)
            : base(symbol) { }
    }
}
