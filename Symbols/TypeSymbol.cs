using System;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols
{
    /// <summary>
    /// A symbol for a type declaration (i.e. a class)
    /// </summary>
    public sealed class TypeSymbol : Symbol
    {
        public override PackageSymbol Package { get; }
        public new NamespaceOrPackageSymbol ContainingSymbol { get; }
        public new TypeName Name { get; }
        public ObjectType DeclaresDataType { get; }

        public TypeSymbol(
            NamespaceOrPackageSymbol containingSymbol,
            TypeName name,
            ObjectType declaresDataType)
            : base(containingSymbol, name)
        {
            // TODO check the name correctly (i.e. containing namespace too)
            if (name != declaresDataType.Name)
                throw new ArgumentException("Declared type must have the same name as symbol", nameof(declaresDataType));

            Package = containingSymbol.Package;
            ContainingSymbol = containingSymbol;
            Name = name;
            DeclaresDataType = declaresDataType;
        }

        public override bool Equals(Symbol? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return other is TypeSymbol otherType
                   && ContainingSymbol == otherType.ContainingSymbol
                   && Name == otherType.Name
                   && DeclaresDataType == otherType.DeclaresDataType;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ContainingSymbol, Name, DeclaresDataType);
        }

        public override string ToString()
        {
            // TODO include generics
            return $"{ContainingSymbol}.{Name}";
        }
    }
}
