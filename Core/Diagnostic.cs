using System;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    public class Diagnostic : IComparable<Diagnostic>
    {
        [NotNull] public readonly CodeFile File;
        public readonly TextSpan Span;
        public readonly TextPosition StartPosition;
        public readonly TextPosition EndPosition;
        public readonly DiagnosticLevel Level;
        public readonly DiagnosticPhase Phase;
        public readonly int ErrorCode;
        [NotNull] public readonly string Message;

        public Diagnostic(
            [NotNull] CodeFile file,
            TextSpan span,
            DiagnosticLevel level,
            DiagnosticPhase phase,
            int errorCode,
            [NotNull] string message)
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

        public int CompareTo(Diagnostic other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var startPositionComparison = StartPosition.CompareTo(other.StartPosition);
            if (startPositionComparison != 0) return startPositionComparison;
            var endPositionComparison = EndPosition.CompareTo(other.EndPosition);
            if (endPositionComparison != 0) return endPositionComparison;
            return ErrorCode.CompareTo(other.ErrorCode);
        }
    }
}
