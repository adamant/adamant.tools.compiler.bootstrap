using System.Collections.Generic;
using System.Linq;
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
        [NotNull] public DataType Type { get; }
        [NotNull] public SymbolSet ChildSymbols { get; }

        private PrimitiveSymbol(
            [NotNull] Name fullName,
            [NotNull] DataType type,
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
            bool isReferenceType,
            bool declaredMutable,
            [NotNull] SimpleName lookupByName,
            [CanBeNull, ItemNotNull] IEnumerable<ISymbol> childSymbols = null)
        {
            FullName = fullName;
            Type = new Metatype(new ObjectType(this, isReferenceType, declaredMutable));
            LookupByName = lookupByName;
            ChildSymbols = new SymbolSet(childSymbols ?? Enumerable.Empty<ISymbol>());
        }

        [NotNull]
        public static PrimitiveSymbol SimpleType(
            [NotNull] SimpleType type,
            [CanBeNull, ItemNotNull] IEnumerable<ISymbol> childSymbols = null)
        {
            return new PrimitiveSymbol(type.Name, new Metatype(type), type.Name.UnqualifiedName, childSymbols);
        }

        [NotNull]
        public static PrimitiveSymbol ReferenceType(
            [NotNull] Name fullName,
            bool declaredMutable,
            [CanBeNull, ItemNotNull] IEnumerable<ISymbol> childSymbols = null)
        {
            return new PrimitiveSymbol(fullName, isReferenceType: true, declaredMutable, fullName.UnqualifiedName, childSymbols);
        }

        [NotNull]
        public static PrimitiveSymbol Member([NotNull] Name fullName, [NotNull] DataType type)
        {
            return new PrimitiveSymbol(fullName, type, fullName.UnqualifiedName);
        }

        [NotNull]
        public static PrimitiveSymbol Getter([NotNull] Name propertyName, [NotNull] DataType type)
        {
            var getterName = ((QualifiedName)propertyName).Qualifier.Qualify(SimpleName.Special("get_" + propertyName.UnqualifiedName.Text));
            return new PrimitiveSymbol(getterName, type, propertyName.UnqualifiedName);
        }
    }
}
