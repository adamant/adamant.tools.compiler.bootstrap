namespace Adamant.Tools.Compiler.Bootstrap.Names
{
    public static class SpecialName
    {
        // The name of the `UnknownSymbol`
        public static readonly SimpleName Unknown = SimpleName.Special("unknown");

        public static readonly SimpleName New = SimpleName.Special("new");
        public static readonly SimpleName Self = SimpleName.Special("self");
        //public static readonly SimpleName Base = SimpleName.Special("base");
        //public static readonly SimpleName Ref = SimpleName.Special("ref");
        public static readonly SimpleName Owned = SimpleName.Special("owned");
        public static readonly SimpleName Forever = SimpleName.Special("forever");
        //public static readonly SimpleName Delete = SimpleName.Special("delete");
        //public static readonly SimpleName Underscore = SimpleName.Special("_");

        public static readonly SimpleName Void = SimpleName.Special("void");
        public static readonly SimpleName Never = SimpleName.Special("never");
        public static readonly SimpleName Bool = SimpleName.Special("bool");
        public static readonly SimpleName Any = SimpleName.Special("Any");
        //public static readonly SimpleName Type = SimpleName.Special("Type");
        //public static readonly SimpleName Int8 = SimpleName.Special("int8");
        public static readonly SimpleName Byte = SimpleName.Special("byte");
        //public static readonly SimpleName Int16 = SimpleName.Special("int16");
        //public static readonly SimpleName UInt16 = SimpleName.Special("uint16");
        public static readonly SimpleName Int = SimpleName.Special("int");
        public static readonly SimpleName UInt = SimpleName.Special("uint");
        //public static readonly SimpleName Int64 = SimpleName.Special("int64");
        //public static readonly SimpleName UInt64 = SimpleName.Special("uint64");
        public static readonly SimpleName Size = SimpleName.Special("size");
        public static readonly SimpleName Offset = SimpleName.Special("offset");
        //public static readonly SimpleName Float32 = SimpleName.Special("float32");
        //public static readonly SimpleName Float = SimpleName.Special("float");

        //public static readonly SimpleName OperatorStringLiteral = SimpleName.Special("op_string_literal");
        //public static readonly SimpleName OperatorEquals = SimpleName.Special("op_equals");

        public static SimpleName Constructor(string name = null)
        {
            return name == null ? New : SimpleName.Special("new_" + name);
        }
    }
}
