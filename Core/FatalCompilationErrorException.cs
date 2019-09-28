using System;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    public class FatalCompilationErrorException : Exception
    {
        public FixedList<Diagnostic> Diagnostics { get; }

        public FatalCompilationErrorException(FixedList<Diagnostic> diagnostics)
        {
            Diagnostics = diagnostics;
        }
    }
}
