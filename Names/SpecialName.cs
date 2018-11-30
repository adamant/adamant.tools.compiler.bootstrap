using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Names
{
    public static class SpecialName
    {
        // The name of the `UnknownSymbol`
        [NotNull] public static readonly SimpleName Unknown = SimpleName.Special("unknown");

        [NotNull] public static readonly SimpleName Self = SimpleName.Special("self");
        [NotNull] public static readonly SimpleName Base = SimpleName.Special("base");
        [NotNull] public static readonly SimpleName Ref = SimpleName.Special("ref");
        [NotNull] public static readonly SimpleName Owned = SimpleName.Special("owned");
        [NotNull] public static readonly SimpleName Forever = SimpleName.Special("forever");
        [NotNull] public static readonly SimpleName Delete = SimpleName.Special("delete");
        [NotNull] public static readonly SimpleName Underscore = SimpleName.Special("_");

        [NotNull] public static readonly SimpleName Void = SimpleName.Special("void");
        [NotNull] public static readonly SimpleName Never = SimpleName.Special("never");
        [NotNull] public static readonly SimpleName Bool = SimpleName.Special("bool");
        [NotNull] public static readonly SimpleName Any = SimpleName.Special("Any");
        [NotNull] public static readonly SimpleName Type = SimpleName.Special("Type");
        [NotNull] public static readonly SimpleName Int8 = SimpleName.Special("int8");
        [NotNull] public static readonly SimpleName Byte = SimpleName.Special("byte");
        [NotNull] public static readonly SimpleName Int16 = SimpleName.Special("int16");
        [NotNull] public static readonly SimpleName UInt16 = SimpleName.Special("uint16");
        [NotNull] public static readonly SimpleName Int = SimpleName.Special("int");
        [NotNull] public static readonly SimpleName UInt = SimpleName.Special("uint");
        [NotNull] public static readonly SimpleName Int64 = SimpleName.Special("int64");
        [NotNull] public static readonly SimpleName UInt64 = SimpleName.Special("uint64");
        [NotNull] public static readonly SimpleName Size = SimpleName.Special("size");
        [NotNull] public static readonly SimpleName Offset = SimpleName.Special("offset");
        [NotNull] public static readonly SimpleName Float32 = SimpleName.Special("float32");
        [NotNull] public static readonly SimpleName Float = SimpleName.Special("float");

        [NotNull] public static readonly SimpleName OperatorStringLiteral = SimpleName.Special("operator_string_literal");
    }
}
