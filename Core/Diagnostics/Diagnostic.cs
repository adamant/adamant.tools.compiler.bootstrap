namespace Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics
{
    public class Diagnostic
    {
        public readonly DiagnosticInfo Info;
        // TODO location

        public Diagnostic(DiagnosticInfo info)
        {
            Info = info ?? throw new System.ArgumentNullException(nameof(info));
        }
    }
}
