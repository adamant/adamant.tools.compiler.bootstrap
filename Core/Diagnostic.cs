using System;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    public class Diagnostic
    {
        public CodeFile File { get; }
        public TextSpan Span { get; }
        public TextPosition StartPosition { get; }
        public TextPosition EndPosition { get; }
        public DiagnosticLevel Level { get; }
        public DiagnosticPhase Phase { get; }
        public int ErrorCode { get; }
        public string Message { get; }

        public Diagnostic(
            CodeFile file,
            TextSpan span,
            DiagnosticLevel level,
            DiagnosticPhase phase,
            int errorCode,
            string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Can't be null or whitespace", nameof(message));

            Requires.ValidEnum(nameof(level), level);
            Requires.ValidEnum(nameof(phase), phase);

            File = file;
            Span = span;
            StartPosition = file.Code.PositionOfStart(span);
            EndPosition = file.Code.PositionOfEnd(span);
            Level = level;
            Phase = phase;
            ErrorCode = errorCode;
            Message = message;
        }

        public bool IsFatal => Level == DiagnosticLevel.FatalCompilationError;

        //public int CompareTo(Diagnostic other)
        //{
        //    if (ReferenceEquals(this, other))
        //        return 0;
        //    if (ReferenceEquals(null, other))
        //        return 1;
        //    var startPositionComparison = StartPosition.CompareTo(other.StartPosition);
        //    if (startPositionComparison != 0)
        //        return startPositionComparison;
        //    var endPositionComparison = EndPosition.CompareTo(other.EndPosition);
        //    if (endPositionComparison != 0)
        //        return endPositionComparison;
        //    return ErrorCode.CompareTo(other.ErrorCode);
        //}

        public override string ToString()
        {
            return $"{Level} {ErrorCode}: {Message} @{File.Reference}@{Span}";
        }
    }
}
