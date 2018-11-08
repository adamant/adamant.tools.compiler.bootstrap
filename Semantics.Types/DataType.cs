using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    public abstract class DataType
    {
        [NotNull] public static UnknownType Unknown = UnknownType.Instance;
        [NotNull] public static BeingCheckedType BeingChecked = BeingCheckedType.Instance;

        [NotNull]
        public abstract override string ToString();
    }
}
