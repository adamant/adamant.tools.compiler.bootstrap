using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    public interface IEmitter<in T>
    {
        void Emit([NotNull] T value, [NotNull] Code code);
    }
}
