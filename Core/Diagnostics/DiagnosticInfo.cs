using System;

namespace Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics
{
    public class DiagnosticInfo
    {
        public readonly DiagnosticLevel Level;
        public readonly DiagnosticPhase Phase;
        public readonly string Message;

        public DiagnosticInfo(DiagnosticLevel level, DiagnosticPhase phase, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("message", nameof(message));

            Requires.ValidEnum(nameof(level), level);
            Requires.ValidEnum(nameof(phase), phase);
            Level = level;
            Phase = phase;
            Message = message;
        }
    }
}
