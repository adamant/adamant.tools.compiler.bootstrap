using System;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    /// <summary>
    /// The data type of a value in an Adamant program. This includes potentially
    /// unresolved types like `UnknownType` or `IntegerConstantType`.
    /// </summary>
    public abstract class DataType
    {
        [NotNull] public static UnknownType Unknown = UnknownType.Instance;
        [NotNull] public static VoidType Void = VoidType.Instance;
        [NotNull] public static NeverType Never = NeverType.Instance;
        [NotNull] public static BoolType Bool = BoolType.Instance;

        /// <summary>
        /// A resolved type is one that has no unknown or unresolved parts
        /// </summary>
        public abstract bool IsResolved { get; }

        [NotNull]
        public DataType AssertResolved()
        {
            if (!IsResolved)
                throw new ArgumentException($"Type {this} not resolved");

            return this;
        }

        [NotNull]
        public abstract override string ToString();
    }
}
