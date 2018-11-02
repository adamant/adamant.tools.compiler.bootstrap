using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    public interface IDiagnosticsCollector
    {
        void Publish([NotNull] Diagnostic diagnostic);
    }
}
