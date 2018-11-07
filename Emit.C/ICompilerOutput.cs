using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    public interface ICompilerOutput
    {
        void WriteLine([CanBeNull] string message);
    }
}
