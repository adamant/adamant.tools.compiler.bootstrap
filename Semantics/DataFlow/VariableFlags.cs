using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow
{
    public class VariableFlags
    {
        public readonly FixedDictionary<ISymbol, int> SymbolMap;
        private readonly BitArray flags;

        public VariableFlags(FunctionDeclarationSyntax function, bool defaultValue)
        {
            SymbolMap = function.ChildSymbols.Values.SelectMany(l => l).Enumerate()
                .ToFixedDictionary(t => t.Item1, t => t.Item2);
            flags = new BitArray(SymbolMap.Count, defaultValue);
        }

        public VariableFlags(
            FixedDictionary<ISymbol, int> symbolMap,
            BitArray flags)
        {
            SymbolMap = symbolMap;
            this.flags = flags;
        }

        public bool this[int i] => flags[i];

        public VariableFlags Set(ISymbol symbol, bool value)
        {
            var newFlags = Clone();
            newFlags.flags[SymbolMap[symbol]] = value;
            return newFlags;
        }

        public VariableFlags Set(IEnumerable<ISymbol> symbols, bool value)
        {
            var newFlags = Clone();
            foreach (var symbol in symbols)
                newFlags.flags[SymbolMap[symbol]] = value;

            return newFlags;
        }

        private VariableFlags Clone()
        {
            return new VariableFlags(SymbolMap, (BitArray)flags.Clone());
        }
    }
}
