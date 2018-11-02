using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    public class DiagnosticsBuilder : IDiagnosticsCollector, IEnumerable<Diagnostic>
    {
        [CanBeNull] private List<Diagnostic> items;

        public void Publish([NotNull] Diagnostic diagnostic)
        {
            // Make sure there is a list to add to, `Build()` steals it
            if (items == null)
                items = new List<Diagnostic>();

            items.Add(diagnostic);
        }

        public void Publish([NotNull][ItemNotNull] IEnumerable<Diagnostic> diagnostics)
        {
            // Make sure there is a list to add to, `Build()` steals it
            if (items == null)
                items = new List<Diagnostic>();

            items.AddRange(diagnostics);
        }

        [NotNull]
        public Diagnostics Build()
        {
            // Steal the current list
            var currentDiagnostics = items ?? new List<Diagnostic>();
            items = null;

            // Uses an internal constructor for efficiency
            return new Diagnostics(currentDiagnostics);
        }

        public IEnumerator<Diagnostic> GetEnumerator()
        {
            return (items ?? Enumerable.Empty<Diagnostic>()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
