using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Primitives
{
    public class PrimitiveSymbol : ISymbol
    {
        [NotNull] public Name FullName { get; }
        [NotNull] public SimpleName LookupByName { get; }
        [CanBeNull] public DataType Type { get; internal set; }
        [NotNull] DataType ISymbol.Type => Type.NotNull();
        [NotNull] public SymbolSet ChildSymbols { get; }

        private PrimitiveSymbol(
            [NotNull] Name fullName,
            [CanBeNull] DataType type,
            [NotNull] SimpleName lookupByName,
            [CanBeNull, ItemNotNull] IEnumerable<ISymbol> childSymbols = null)
        {
            FullName = fullName;
            Type = type;
            LookupByName = lookupByName;
            ChildSymbols = new SymbolSet(childSymbols ?? Enumerable.Empty<ISymbol>());
        }

        private PrimitiveSymbol(
            [NotNull] Name fullName,
            bool declaredMutable,
            [NotNull] SimpleName lookupByName,
            [CanBeNull] [ItemNotNull] IEnumerable<ISymbol> childSymbols = null)
        {
            FullName = fullName;
            Type = new Metatype(new ObjectType(this, declaredMutable));
            LookupByName = lookupByName;
            ChildSymbols = new SymbolSet(childSymbols ?? Enumerable.Empty<ISymbol>());
        }

        [NotNull]
        public static PrimitiveSymbol NewSimpleType(
            [NotNull] SimpleType type,
            [CanBeNull, ItemNotNull] IEnumerable<ISymbol> childSymbols = null)
        {
            return new PrimitiveSymbol(type.Name, new Metatype(type), type.Name.UnqualifiedName, childSymbols);
        }

        [NotNull]
        public static PrimitiveSymbol NewType([NotNull] Name fullName, [ItemNotNull] [NotNull] IEnumerable<ISymbol> childSymbols)
        {
            return new PrimitiveSymbol(fullName, null, fullName.UnqualifiedName, childSymbols);
        }

        [NotNull]
        public static PrimitiveSymbol NewMember([NotNull] Name fullName, [CanBeNull] DataType type = null)
        {
            return new PrimitiveSymbol(fullName, type, fullName.UnqualifiedName);
        }

        [NotNull]
        public static PrimitiveSymbol NewGetter([NotNull] Name propertyName, [CanBeNull] DataType type = null)
        {
            var getterName = ((QualifiedName)propertyName).Qualifier.Qualify(SimpleName.Special("get_" + propertyName.UnqualifiedName.Text));
            return new PrimitiveSymbol(getterName, type, propertyName.UnqualifiedName);
        }
    }
}
