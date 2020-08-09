using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Primitives
{
    internal class PrimitiveTypeMetadata : PrimitiveMetadata, ITypeMetadata
    {
        private DataType? declaresType;

        protected PrimitiveTypeMetadata(
            MaybeQualifiedName fullName,
            DataType? declaresType,
            IEnumerable<IMetadata>? childSymbols = null)
            : base(fullName, new MetadataSet(childSymbols ?? Enumerable.Empty<IMetadata>()))
        {
            this.declaresType = declaresType;
        }

        public DataType DeclaresDataType
        {
            get => declaresType ?? throw new InvalidOperationException();
            internal set => declaresType = value;
        }

        public static PrimitiveTypeMetadata NewType(MaybeQualifiedName fullName, IEnumerable<IMetadata> childSymbols)
        {
            return new PrimitiveTypeMetadata(fullName, null, childSymbols);
        }

        public static PrimitiveTypeMetadata NewSimpleType(
            SimpleType type,
            IEnumerable<IMetadata>? childSymbols = null)
        {
            return new PrimitiveTypeMetadata(type.Name.ToSimpleName(), type, childSymbols);
        }

        public static PrimitiveTypeMetadata NewEmptyType(EmptyType type)
        {
            return new PrimitiveTypeMetadata(type.Name, type);
        }

        public static PrimitiveTypeMetadata New(MaybeQualifiedName fullName, DataType? type = null)
        {
            return new PrimitiveTypeMetadata(fullName, type);
        }
    }
}
