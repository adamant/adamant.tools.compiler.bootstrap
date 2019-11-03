namespace Adamant.Tools.Compiler.Bootstrap.Names
{
    public static class SpecialName
    {
        // The name of the `UnknownSymbol`
        public static readonly SimpleName Unknown = SimpleName.Special("unknown");

        public static readonly SimpleName New = SimpleName.Special("new");
        public static readonly SimpleName Self = SimpleName.Special("self");
        public static readonly SimpleName Owned = SimpleName.Special("owned");
        public static readonly SimpleName Forever = SimpleName.Special("forever");

        public static readonly SimpleName Void = SimpleName.Special("void");
        public static readonly SimpleName Never = SimpleName.Special("never");
        public static readonly SimpleName Bool = SimpleName.Special("bool");
        public static readonly SimpleName Any = SimpleName.Special("Any");
        public static readonly SimpleName Byte = SimpleName.Special("byte");
#pragma warning disable CA1720
        public static readonly SimpleName Int = SimpleName.Special("int");
        public static readonly SimpleName UInt = SimpleName.Special("uint");
#pragma warning restore CA1720
        public static readonly SimpleName Size = SimpleName.Special("size");
        public static readonly SimpleName Offset = SimpleName.Special("offset");

        public static SimpleName Constructor(string? name = null)
        {
            return name == null ? New : SimpleName.Special("new_" + name);
        }
    }
}
