using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Xunit
{
    public partial class Assert
    {
        public static void Diagnostic(
            Diagnostic diagnostic,
            DiagnosticPhase phase,
            int errorCode,
            int start,
            int length)
        {
            NotNull(diagnostic);
            Equal(phase, diagnostic.Phase);
            Equal(errorCode, diagnostic.ErrorCode);
            True(start == diagnostic.Span.Start,
                $"Expected diagnostic start {start}, was {diagnostic.Span.Start}");
            True(length == diagnostic.Span.Length,
                $"Expected diagnostic length {length}, was {diagnostic.Span.Length}");
        }
    }
}
