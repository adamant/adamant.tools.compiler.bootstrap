namespace Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics
{
    public class Diagnostic
    {
        public readonly CodeFile File;
        public readonly TextSpan Span;
        public readonly TextPosition Position;
        public readonly DiagnosticInfo Info;

        public Diagnostic(
            CodeFile file,
            TextSpan span,
            DiagnosticInfo info)
        {
            Info = info ?? throw new System.ArgumentNullException(nameof(info));
            File = file;
            Span = span;
            Position = file.Code.PositionOfStart(span);
        }
    }
}
