using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    [DebuggerDisplay("Count = {items.Count}")]
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    public class Diagnostics : IEnumerable<Diagnostic>
    {
        private readonly List<Diagnostic> items = new List<Diagnostic>();

        public int Count => items.Count;

        public void Add(Diagnostic diagnostic)
        {
            items.Add(diagnostic);
        }

        public void Add(IEnumerable<Diagnostic> diagnostics)
        {
            items.AddRange(diagnostics);
        }

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
