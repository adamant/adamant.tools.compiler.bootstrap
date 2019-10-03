using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    public sealed class DictionaryDebugView<K, V>
        where K : notnull
    {
        private readonly IReadOnlyDictionary<K, V> dictionary;

        public DictionaryDebugView(IReadOnlyDictionary<K, V> dictionary)
        {
            this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        [SuppressMessage("Performance", "CA1819:Properties should not return arrays")]
        public KeyValuePair<K, V>[] Items => dictionary.ToArray();
    }
}
