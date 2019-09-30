using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Primitives
{
    internal class Type : Primitive, ITypeSymbol
    {
        private DataType? declaresType;

        protected Type(
            Name fullName,
            DataType? declaresType,
            IEnumerable<ISymbol>? childSymbols = null)
            : base(fullName)
        {
            this.declaresType = declaresType;
            ChildSymbols = new SymbolSet(childSymbols ?? Enumerable.Empty<ISymbol>());
        }

        public DataType DeclaresType
        {
            get => declaresType ?? throw new InvalidOperationException();
            internal set => declaresType = value;
        }

        public static Type NewType(Name fullName, IEnumerable<ISymbol> childSymbols)
        {
            return new Type(fullName, null, childSymbols);
        }

        public static Type NewSimpleType(
            SimpleType type,
            IEnumerable<ISymbol>? childSymbols = null)
        {
            return new Type(type.Name, type, childSymbols);
        }

        public static Type New(Name fullName, DataType? type = null)
        {
            return new Type(fullName, type);
        }
    }
}