using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Primitives
{
    internal class PrimitiveSymbol : ISymbol
    {
        // These are all declarations not local variables
        bool ISymbol.MutableBinding => false;
        public Name FullName { get; }
        public DataType Type { get; internal set; }
        public DataType DeclaresType { get; internal set; }
        public SymbolSet ChildSymbols { get; }

        protected PrimitiveSymbol(
            Name fullName,
            DataType type,
            DataType declaresType,
            IEnumerable<ISymbol> childSymbols = null)
        {
            FullName = fullName;
            Type = type;
            DeclaresType = declaresType;
            ChildSymbols = new SymbolSet(childSymbols ?? Enumerable.Empty<ISymbol>());
        }

        public static PrimitiveSymbol NewSimpleType(
            SimpleType type,
            IEnumerable<ISymbol> childSymbols = null)
        {
            return new PrimitiveSymbol(type.Name, null, type, childSymbols);
        }

        public static PrimitiveSymbol NewType(Name fullName, IEnumerable<ISymbol> childSymbols)
        {
            return new PrimitiveSymbol(fullName, null, null, childSymbols);
        }

        public static PrimitiveSymbol New(Name fullName, DataType type = null)
        {
            return new PrimitiveSymbol(fullName, type, null);
        }

        //public static AccessorSymbol NewGetter(Name propertyName, DataType type = null)
        //{
        //    var getterName = ((QualifiedName)propertyName).Qualifier.Qualify(SimpleName.Special("get_" + propertyName.UnqualifiedName.Text));
        //    return new AccessorSymbol(getterName, propertyName, type);
        //}

        public static PrimitiveFunctionSymbol NewFunction(Name fullName/*, FunctionType type = null*/)
        {
            return new PrimitiveFunctionSymbol(fullName/*, type*/);
        }
    }
}
