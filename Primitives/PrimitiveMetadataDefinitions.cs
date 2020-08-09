using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Primitives
{
    /// <summary>
    /// Provides a declaration of the primitives for use when name binding etc.
    /// </summary>
    public static class PrimitiveMetadataDefinitions
    {
        public static readonly FixedList<IMetadata> Instance = DefinePrimitiveSymbols();

        private static FixedList<IMetadata> DefinePrimitiveSymbols()
        {
            var stringType = new ObjectType(NamespaceName.Global, "String", false, ReferenceCapability.Shared);
            return new List<IMetadata>
            {
                // Simple Types
                BuildBoolMetadata(),

                BuildIntegerTypeMetadata(DataType.Byte, stringType),
                BuildIntegerTypeMetadata(DataType.Int, stringType),
                BuildIntegerTypeMetadata(DataType.UInt, stringType),

                BuildIntegerTypeMetadata(DataType.Size, stringType),
                BuildIntegerTypeMetadata(DataType.Offset, stringType),

                PrimitiveTypeMetadata.NewEmptyType(DataType.Void),
                PrimitiveTypeMetadata.NewEmptyType(DataType.Never),

                // Intrinsic functions used by the standard library
                PrimitiveFunctionMetadata.New(MaybeQualifiedName.From("intrinsics", "mem_allocate"),
                    DataType.Size, ("length", DataType.Size)),

                PrimitiveFunctionMetadata.New(MaybeQualifiedName.From("intrinsics", "mem_deallocate"),
                    ("ptr", DataType.Size)),

                PrimitiveFunctionMetadata.New(MaybeQualifiedName.From("intrinsics", "mem_copy"),
                    ("from_ptr", DataType.Size), ("to_ptr", DataType.Size), ("length", DataType.Size)),

                PrimitiveFunctionMetadata.New(MaybeQualifiedName.From("intrinsics", "mem_set_byte"),
                    ("ptr", DataType.Size), ("value", DataType.Byte)),

                PrimitiveFunctionMetadata.New(MaybeQualifiedName.From("intrinsics", "mem_get_byte"),
                    DataType.Byte, ("ptr", DataType.Size)),

                PrimitiveFunctionMetadata.New(MaybeQualifiedName.From("intrinsics", "print_utf8"),
                    ("ptr", DataType.Size), ("length", DataType.Size)),

                PrimitiveFunctionMetadata.New(MaybeQualifiedName.From("intrinsics", "read_utf8_line"),
                    DataType.Size, ("ptr", DataType.Size), ("length", DataType.Size)),
            }.ToFixedList();
        }

        private static IMetadata BuildBoolMetadata()
        {
            return PrimitiveTypeMetadata.NewSimpleType(DataType.Bool);
        }

        private static IMetadata BuildIntegerTypeMetadata(IntegerType integerType, DataType stringType)
        {
            var typeName = integerType.Name.ToSimpleName();
            var symbols = new List<IMetadata>
            {
                PrimitiveMethodMetadata.New(typeName.Qualify("remainder"), integerType, integerType, ("divisor", integerType)),
                PrimitiveMethodMetadata.New(typeName.Qualify("to_display_string"), stringType, integerType),
            };
            return PrimitiveTypeMetadata.NewSimpleType(integerType, symbols);
        }
    }
}
