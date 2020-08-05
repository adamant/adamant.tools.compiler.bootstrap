using System;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols
{
    public sealed class ConstructorSymbol : InvokableSymbol
    {
        public new TypeSymbol ContainingSymbol { get; }

        public ConstructorSymbol(
            TypeSymbol containingSymbol,
            SimpleName? name,
            FixedList<DataType> parameterDataTypes)
            : base(containingSymbol, name, parameterDataTypes)
        {
            ContainingSymbol = containingSymbol;
        }

        public static ConstructorSymbol CreateDefault(TypeSymbol containingSymbol)
        {
            return new ConstructorSymbol(containingSymbol, null, FixedList<DataType>.Empty);
        }

        public override bool Equals(Symbol? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return other is ConstructorSymbol otherConstructor
                   && ContainingSymbol == otherConstructor.ContainingSymbol
                   && Name == otherConstructor.Name
                   && ParameterDataTypes.SequenceEqual(otherConstructor.ParameterDataTypes);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ContainingSymbol, Name, ParameterDataTypes);
        }
    }
}