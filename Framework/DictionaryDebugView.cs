using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    public sealed class DictionaryDebugView<K, V>
    {
        [NotNull] private readonly IReadOnlyDictionary<K, V> dictionary;

        public DictionaryDebugView(IReadOnlyDictionary<K, V> dictionary)
        {
            this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public KeyValuePair<K, V>[] Items => dictionary.ToArray();
    }
}
