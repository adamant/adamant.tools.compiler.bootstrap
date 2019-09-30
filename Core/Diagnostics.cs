using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    [DebuggerDisplay("Count = {items.Count}")]
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    public class Diagnostics : IEnumerable<Diagnostic>
    {
        private readonly List<Diagnostic> items = new List<Diagnostic>();

        public Diagnostics() { }

        public Diagnostics(IEnumerable<Diagnostic> diagnostics)
        {
            items.AddRange(diagnostics);
        }

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

        public void ThrowIfFatalErrors()
        {
            if (items.Any(i => i.IsFatal))
                throw new FatalCompilationErrorException(items.ToFixedList());
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
