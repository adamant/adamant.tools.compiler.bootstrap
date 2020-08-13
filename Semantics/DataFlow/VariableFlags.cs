using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Symbols.Trees;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow
{
    public class VariableFlags
    {
        private readonly FixedDictionary<BindingSymbol, int> symbolMap;
        private readonly BitArray flags;

        public VariableFlags(IConcreteInvocableDeclarationSyntax invocable, SymbolTree symbolTree, bool defaultValue)
        {
            var invocableSymbol = invocable.Symbol.Result;
            symbolMap = symbolTree.Children(invocableSymbol).Cast<BindingSymbol>().Enumerate().ToFixedDictionary();
            flags = new BitArray(symbolMap.Count, defaultValue);
        }

        public VariableFlags(
            FixedDictionary<BindingSymbol, int> symbolMap,
            BitArray flags)
        {
            this.symbolMap = symbolMap;
            this.flags = flags;
        }

        /// <summary>
        /// Returns the state for the variable or null if the symbol isn't a
        /// variable.
        /// </summary>
        [SuppressMessage("Design", "CA1043:Use Integral Or String Argument For Indexers", Justification = "Symbols are like immutable strings")]
        public bool? this[BindingSymbol symbol] => symbolMap.TryGetValue(symbol, out var i) ? (bool?)flags[i] : null;

        public VariableFlags Set(BindingSymbol symbol, bool value)
        {
            // TODO if setting to the current value, don't need to clone
            var newFlags = Clone();
            newFlags.flags[symbolMap[symbol]] = value;
            return newFlags;
        }

        public VariableFlags Set(IEnumerable<BindingSymbol> symbols, bool value)
        {
            // TODO if setting to the current value, don't need to clone
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
