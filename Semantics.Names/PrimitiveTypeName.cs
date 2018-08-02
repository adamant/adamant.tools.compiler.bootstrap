using System.Text;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Names
{
    public class PrimitiveTypeName : ObjectTypeName
    {
        public static readonly PrimitiveTypeName Int = new PrimitiveTypeName("int", PrimitiveTypeKind.Int);
        public static readonly PrimitiveTypeName Bool = new PrimitiveTypeName("bool", PrimitiveTypeKind.Bool);
        public static readonly PrimitiveTypeName Void = new PrimitiveTypeName("void", PrimitiveTypeKind.Void);
        public static readonly PrimitiveTypeName Never = new PrimitiveTypeName("never", PrimitiveTypeKind.Never);

        public PrimitiveTypeKind Kind { get; }

        private PrimitiveTypeName(string name, PrimitiveTypeKind kind)
            : base(name)
        {
            Kind = kind;
        }

        public override void GetFullName(StringBuilder builder)
        {
            builder.Append("primitive:"); // The single colon distinguishes this from a package named `primitive`
            builder.Append(EntityName);
        }
    }
}
