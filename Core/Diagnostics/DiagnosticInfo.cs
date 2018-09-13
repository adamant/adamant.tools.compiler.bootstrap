using System;

namespace Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics
{
    // TODO need clearer name for this
    public class DiagnosticInfo
    {
        public readonly DiagnosticLevel Level;
        public readonly DiagnosticPhase Phase;
        public readonly int ErrorCode;
        public readonly string Message;

        internal DiagnosticInfo(DiagnosticLevel level, DiagnosticPhase phase, int errorCode, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("message", nameof(message));

            Requires.ValidEnum(nameof(level), level);
            Requires.ValidEnum(nameof(phase), phase);
            Level = level;
            Phase = phase;
            ErrorCode = errorCode;
            Message = message;
        }
    }
}
