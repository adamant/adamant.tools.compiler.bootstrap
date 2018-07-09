using System;
using System.Collections.Generic;
using System.Text;

namespace Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics
{
    public enum DiagnosticLevel
    {
        Info = 1,
        Warning,
        RuntimeError, // An error which will occur at runtime but doesn't prevent compilation (i.e. pre-condition not met)
        CompilationError, // An error reported during compilation
        FatalCompilationError, // An error which prevents further compilation steps

    }
}
