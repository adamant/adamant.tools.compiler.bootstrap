using System;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols
{
    public sealed class MethodSymbol : FunctionOrMethodSymbol
    {
        public new TypeSymbol ContainingSymbol { get; }
        public new Name Name { get; }
        public DataType SelfDataType { get; }

        public MethodSymbol(
            TypeSymbol containingSymbol,
            Name name,
            DataType selfDataType,
            FixedList<DataType> parameterDataTypes,
            DataType returnDataType)
            : base(containingSymbol, name, parameterDataTypes, returnDataType)
        {
            ContainingSymbol = containingSymbol;
            Name = name;
            SelfDataType = selfDataType;
        }

        public override bool Equals(Symbol? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return other is MethodSymbol otherMethod
                   && ContainingSymbol == otherMethod.ContainingSymbol
                   && Name == otherMethod.Name
                   && SelfDataType == otherMethod.SelfDataType
                   && ParameterDataTypes.SequenceEqual(otherMethod.ParameterDataTypes)
                   && ReturnDataType == otherMethod.ReturnDataType;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, SelfDataType, ParameterDataTypes, ReturnDataType);
        }

        public override string ToString()
        {
            return $"{ContainingSymbol}::{Name}({string.Join(", ", ParameterDataTypes)}) -> {ReturnDataType}";
        }
    }
}
