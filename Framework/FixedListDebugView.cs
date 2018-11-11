using System;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    /// <summary>
    /// This class is used in place of the FixedList[T] class by the debugger for
    /// display purposes. It just exposes the collection items as an array at the root.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class FixedListDebugView<T>
        where T : class
    {
        [NotNull] private readonly FixedList<T> collection;

        public FixedListDebugView(FixedList<T> collection)
        {
            this.collection = collection ?? throw new ArgumentNullException(nameof(collection));
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => collection.ToArray();
    }
}
