using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    [DebuggerDisplay("Count = {items.Count}")]
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    public class Diagnostics : IEnumerable<Diagnostic>
    {
        [NotNull] private readonly List<Diagnostic> items = new List<Diagnostic>();

        public void Add([NotNull] Diagnostic diagnostic)
        {
            Requires.NotNull(nameof(diagnostic), diagnostic);
            items.Add(diagnostic);
        }

        public void Add([NotNull, ItemNotNull] IEnumerable<Diagnostic> diagnostics)
        {
            Requires.NotNull(nameof(diagnostics), diagnostics);
            items.AddRange(diagnostics.ItemsNotNull());
        }

        [NotNull]
        public FixedList<Diagnostic> Build()
        {
            items.Sort();
            return items.ToFixedList();
        }

        public IEnumerator<Diagnostic> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)items).GetEnumerator();
        }
    }
}
