using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Primitives
{
    internal class PrimitiveTypeSymbol : PrimitiveSymbol, ITypeSymbol
    {
        private DataType? declaresType;

        protected PrimitiveTypeSymbol(
            Name fullName,
            DataType? declaresType,
            IEnumerable<ISymbol>? childSymbols = null)
            : base(fullName, new SymbolSet(childSymbols ?? Enumerable.Empty<ISymbol>()))
        {
            this.declaresType = declaresType;
        }

        public DataType DeclaresType
        {
            get => declaresType ?? throw new InvalidOperationException();
            internal set => declaresType = value;
        }

        public static PrimitiveTypeSymbol NewType(Name fullName, IEnumerable<ISymbol> childSymbols)
        {
            return new PrimitiveTypeSymbol(fullName, null, childSymbols);
        }

        public static PrimitiveTypeSymbol NewSimpleType(
            SimpleType type,
            IEnumerable<ISymbol>? childSymbols = null)
        {
            return new PrimitiveTypeSymbol(type.Name, type, childSymbols);
        }

        public static PrimitiveTypeSymbol NewEmptyType(EmptyType type)
        {
            return new PrimitiveTypeSymbol(type.Name, type);
        }

        public static PrimitiveTypeSymbol New(Name fullName, DataType? type = null)
        {
            return new PrimitiveTypeSymbol(fullName, type);
        }
    }
}
