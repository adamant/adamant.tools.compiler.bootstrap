using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    public interface IConverter<in T>
    {
        string Convert([NotNull] T value);
    }
}
