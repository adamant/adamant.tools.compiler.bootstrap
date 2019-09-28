namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    public enum DiagnosticLevel
    {
        Info = 1,
        Warning,
        /// <summary>
        /// An error which will occur at runtime but doesn't prevent compilation (i.e. pre-condition not met)
        /// </summary>
        RuntimeError,
        /// <summary>
        /// A compilation error that doesn't prevent further compilation. For example, using `!=` for
        /// not equal.
        /// </summary>
        CompilationError,
        /// <summary>
        /// An error which prevents further compilation steps
        /// </summary>
        FatalCompilationError,
    }
}
