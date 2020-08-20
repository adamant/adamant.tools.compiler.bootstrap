using System;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols
{
    public class PrimitiveTypeSymbol : TypeSymbol
    {
        public new SpecialTypeName Name { get; }

        public PrimitiveTypeSymbol(SimpleType declaresDataType)
            : base(null, declaresDataType.Name, declaresDataType)
        {
            Name = declaresDataType.Name;
        }

        public PrimitiveTypeSymbol(EmptyType declaresDataType)
            : base(null, declaresDataType.Name, declaresDataType)
        {
            Name = declaresDataType.Name;
        }

        public override bool Equals(Symbol? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return other is PrimitiveTypeSymbol otherType
                   && Name == otherType.Name
                   && DeclaresDataType == otherType.DeclaresDataType;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, DeclaresDataType);
        }

        public override string ToILString()
        {
            return Name.ToString();
        }
    }
}
