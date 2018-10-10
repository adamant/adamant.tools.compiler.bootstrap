using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    public class PrimitiveType : ObjectType
    {
        [NotNull] public static readonly PrimitiveType Void = new PrimitiveType(PrimitiveTypeName.Void);
        [NotNull] public static readonly PrimitiveType Never = new PrimitiveType(PrimitiveTypeName.Never);
        [NotNull] public static readonly PrimitiveType Int = new PrimitiveType(PrimitiveTypeName.Int);
        [NotNull] public static readonly PrimitiveType UInt = new PrimitiveType(PrimitiveTypeName.UInt);
        [NotNull] public static readonly PrimitiveType Byte = new PrimitiveType(PrimitiveTypeName.Byte);
        [NotNull] public static readonly PrimitiveType Size = new PrimitiveType(PrimitiveTypeName.Size);
        [NotNull] public static readonly PrimitiveType Bool = new PrimitiveType(PrimitiveTypeName.Bool);
        [NotNull] public static readonly PrimitiveType String = new PrimitiveType(PrimitiveTypeName.String);

        public PrimitiveTypeKind Kind { get; }

        private PrimitiveType([NotNull] PrimitiveTypeName name)
          : base(name, false)
        {
            Kind = name.Kind;
        }
    }
}
