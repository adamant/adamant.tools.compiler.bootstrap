using System.Collections;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow
{
    public class VariablesAssigned
    {
        public readonly FixedDictionary<ISymbol, int> SymbolMap;
        private readonly BitArray assigned;

        public VariablesAssigned(
            FixedDictionary<ISymbol, int> symbolMap,
            BitArray assigned)
        {
            SymbolMap = symbolMap;
            this.assigned = assigned;
        }

        public bool IsDefinitelyAssigned(int i)
        {
            return assigned[i];
        }

        public VariablesAssigned Clone()
        {
            return new VariablesAssigned(SymbolMap, (BitArray)assigned.Clone());
        }

        public void Assigned(ISymbol symbol)
        {
            assigned[SymbolMap[symbol]] = true;
        }
    }
}
