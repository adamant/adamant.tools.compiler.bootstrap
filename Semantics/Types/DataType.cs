using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    public abstract class DataType
    {
        [NotNull]
        public abstract override string ToString();
    }
}
