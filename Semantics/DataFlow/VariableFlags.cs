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
        private readonly FixedDictionary<ISymbol, int> symbolMap;
        private readonly BitArray flags;

        public VariableFlags(IFunctionDeclarationSyntax function, bool defaultValue)
        {
            symbolMap = function.ChildSymbols.Values.SelectMany(l => l).Enumerate()
                .ToFixedDictionary(t => t.Item1, t => t.Item2);
            flags = new BitArray(symbolMap.Count, defaultValue);
        }

        public VariableFlags(
            FixedDictionary<ISymbol, int> symbolMap,
            BitArray flags)
        {
            this.symbolMap = symbolMap;
            this.flags = flags;
        }

        /// <summary>
        /// Returns the state for the variable or null if the symbol isn't a
        /// variable.
        /// </summary>
        public bool? this[ISymbol symbol] => symbolMap.TryGetValue(symbol, out var i) ? (bool?)flags[i] : null;

        public VariableFlags Set(ISymbol symbol, bool value)
        {
            var newFlags = Clone();
            newFlags.flags[symbolMap[symbol]] = value;
            return newFlags;
        }

        public VariableFlags Set(IEnumerable<ISymbol> symbols, bool value)
        {
            var newFlags = Clone();
            foreach (var symbol in symbols)
                newFlags.flags[symbolMap[symbol]] = value;

            return newFlags;
        }

        private VariableFlags Clone()
        {
            return new VariableFlags(symbolMap, (BitArray)flags.Clone());
        }
    }
}
