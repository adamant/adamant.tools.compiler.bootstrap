using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    public class PrimitiveType : ObjectType
    {
        public static readonly PrimitiveType Void = new PrimitiveType(PrimitiveTypeName.Void);
        public static readonly PrimitiveType Never = new PrimitiveType(PrimitiveTypeName.Never);
        public static readonly PrimitiveType Int = new PrimitiveType(PrimitiveTypeName.Int);
        public static readonly PrimitiveType UInt = new PrimitiveType(PrimitiveTypeName.UInt);
        public static readonly PrimitiveType Byte = new PrimitiveType(PrimitiveTypeName.Byte);
        public static readonly PrimitiveType Size = new PrimitiveType(PrimitiveTypeName.Size);
        public static readonly PrimitiveType Bool = new PrimitiveType(PrimitiveTypeName.Bool);
        public static readonly PrimitiveType String = new PrimitiveType(PrimitiveTypeName.String);

        public PrimitiveTypeKind Kind { get; }

        private PrimitiveType(PrimitiveTypeName name)
          : base(name, false)
        {
            Kind = name.Kind;
        }
    }
}
