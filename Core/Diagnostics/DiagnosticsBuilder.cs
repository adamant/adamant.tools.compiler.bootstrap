using System.Collections.Generic;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics
{
    public class DiagnosticsBuilder : IDiagnosticsCollector
    {
        [CanBeNull]
        private List<Diagnostic> items;

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
    }
}
