using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics
{
    public interface IDiagnosticsCollector
    {
        void Publish([NotNull] Diagnostic diagnostic);
    }
}
