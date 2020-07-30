using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow
{
    public class VariableFlags
    {
        private readonly FixedDictionary<IMetadata, int> symbolMap;
        private readonly BitArray flags;

        public VariableFlags(IConcreteCallableDeclarationSyntax callable, bool defaultValue)
        {
            symbolMap = callable.ChildMetadata.Enumerate<IMetadata>().ToFixedDictionary();
            flags = new BitArray(symbolMap.Count, defaultValue);
        }

        public VariableFlags(
            FixedDictionary<IMetadata, int> symbolMap,
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
        public bool? this[IMetadata symbol] => symbolMap.TryGetValue(symbol, out var i) ? (bool?)flags[i] : null;

        public VariableFlags Set(IMetadata symbol, bool value)
        {
            // TODO if setting to the current value, don't need to clone
            var newFlags = Clone();
            newFlags.flags[symbolMap[symbol]] = value;
            return newFlags;
        }

        public VariableFlags Set(IEnumerable<IMetadata> symbols, bool value)
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
