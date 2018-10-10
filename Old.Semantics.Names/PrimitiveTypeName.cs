using System.Text;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Names
{
    public class PrimitiveTypeName : ObjectTypeName
    {
        [NotNull] public static readonly PrimitiveTypeName Int = new PrimitiveTypeName("int", PrimitiveTypeKind.Int);
        [NotNull] public static readonly PrimitiveTypeName UInt = new PrimitiveTypeName("uint", PrimitiveTypeKind.UInt);
        [NotNull] public static readonly PrimitiveTypeName Byte = new PrimitiveTypeName("byte", PrimitiveTypeKind.Byte);
        [NotNull] public static readonly PrimitiveTypeName Size = new PrimitiveTypeName("size", PrimitiveTypeKind.Size);
        [NotNull] public static readonly PrimitiveTypeName Bool = new PrimitiveTypeName("bool", PrimitiveTypeKind.Bool);
        [NotNull] public static readonly PrimitiveTypeName Void = new PrimitiveTypeName("void", PrimitiveTypeKind.Void);
        [NotNull] public static readonly PrimitiveTypeName Never = new PrimitiveTypeName("never", PrimitiveTypeKind.Never);
        [NotNull] public static readonly PrimitiveTypeName String = new PrimitiveTypeName("string", PrimitiveTypeKind.String);

        public PrimitiveTypeKind Kind { get; }

        private PrimitiveTypeName([NotNull] string name, PrimitiveTypeKind kind)
            : base(name)
        {
            Kind = kind;
        }

        public override void GetFullName([NotNull] StringBuilder builder)
        {
            Requires.NotNull(nameof(builder), builder);
            builder.Append("primitive:"); // The single colon distinguishes this from a package named `primitive`
            builder.Append(EntityName);
        }
    }
}
