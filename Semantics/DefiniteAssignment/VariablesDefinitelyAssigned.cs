using System.Collections;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.DefiniteAssignment
{
    public class VariablesDefinitelyAssigned
    {
        public readonly FixedDictionary<ISymbol, int> SymbolMap;
        private readonly BitArray assigned;

        public VariablesDefinitelyAssigned(
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

        public VariablesDefinitelyAssigned Clone()
        {
            return new VariablesDefinitelyAssigned(SymbolMap, (BitArray)assigned.Clone());
        }

        public void Assigned(ISymbol symbol)
        {
            assigned[SymbolMap[symbol]] = true;
        }
    }
}
