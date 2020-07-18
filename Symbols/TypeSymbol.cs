using System;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols
{
    /// <summary>
    /// A symbol for a type declaration (i.e. a class)
    /// </summary>
    public sealed class TypeSymbol : ParentSymbol
    {
        public ObjectType DeclaresType { get; }

        public TypeSymbol(Name fullName, ObjectType declaresType, SymbolSet childSymbols)
            : base(fullName, childSymbols)
        {
            if (fullName != declaresType.FullName)
                throw new ArgumentException("Declared type must have the same name as symbol", nameof(declaresType));

            DeclaresType = declaresType;
        }

        public override bool Equals(Symbol? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return other is TypeSymbol otherType
                   && FullName == otherType.FullName
                   && DeclaresType == otherType.DeclaresType;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FullName, DeclaresType);
        }
    }
}
