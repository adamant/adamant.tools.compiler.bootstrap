using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Primitives
{
    internal class Symbol : ISymbol
    {
        public Name FullName { get; }
        public DataType Type { get; internal set; }
        public SymbolSet ChildSymbols { get; }

        protected Symbol(
            Name fullName,
            DataType type,
            IEnumerable<ISymbol> childSymbols = null)
        {
            FullName = fullName;
            Type = type;
            ChildSymbols = new SymbolSet(childSymbols ?? Enumerable.Empty<ISymbol>());
        }

        public static Symbol NewSimpleType(
            SimpleType type,
            IEnumerable<ISymbol> childSymbols = null)
        {
            return new Symbol(type.Name, new Metatype(type), childSymbols);
        }

        public static Symbol NewType(Name fullName, IEnumerable<ISymbol> childSymbols)
        {
            return new Symbol(fullName, null, childSymbols);
        }

        public static Symbol NewMember(Name fullName, DataType type = null)
        {
            return new Symbol(fullName, type);
        }

        public static AccessorSymbol NewGetter(Name propertyName, DataType type = null)
        {
            var getterName = ((QualifiedName)propertyName).Qualifier.Qualify(SimpleName.Special("get_" + propertyName.UnqualifiedName.Text));
            return new AccessorSymbol(getterName, propertyName, type);
        }
    }
}
