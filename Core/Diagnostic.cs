using System;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    public class Diagnostic : IComparable<Diagnostic>
    {
        public readonly CodeFile File;
        public readonly TextSpan Span;
        public readonly TextPosition StartPosition;
        public readonly TextPosition EndPosition;
        public readonly DiagnosticLevel Level;
        public readonly DiagnosticPhase Phase;
        public readonly int ErrorCode;
        public readonly string Message;

        public Diagnostic(
            CodeFile file,
            TextSpan span,
            DiagnosticLevel level,
            DiagnosticPhase phase,
            int errorCode,
            string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("message", nameof(message));

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

        public int CompareTo(Diagnostic other)
        {
            if (ReferenceEquals(this, other))
                return 0;
            if (ReferenceEquals(null, other))
                return 1;
            var startPositionComparison = StartPosition.CompareTo(other.StartPosition);
            if (startPositionComparison != 0)
                return startPositionComparison;
            var endPositionComparison = EndPosition.CompareTo(other.EndPosition);
            if (endPositionComparison != 0)
                return endPositionComparison;
            return ErrorCode.CompareTo(other.ErrorCode);
        }

        public override string ToString()
        {
            return $"{Level} {ErrorCode}: {Message} @{File.Reference}@{Span}";
        }
    }
}
