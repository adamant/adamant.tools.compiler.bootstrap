using System;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    public class ConsoleCompilerOutput : ICompilerOutput
    {
        #region Singleton
        [NotNull] public static readonly ConsoleCompilerOutput Instance = new ConsoleCompilerOutput();

        private ConsoleCompilerOutput() { }
        #endregion

        public void WriteLine([CanBeNull] string message)
        {
            Console.WriteLine(message);
        }
    }
}
