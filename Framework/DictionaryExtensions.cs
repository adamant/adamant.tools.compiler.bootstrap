using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    public static class DictionaryExtensions
    {
        public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary)
        {
            return new ReadOnlyDictionary<TKey, TValue>(dictionary);
        }
    }
}
