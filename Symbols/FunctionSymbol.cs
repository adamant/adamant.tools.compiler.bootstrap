using System;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols.Reachability;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols
{
    /// <summary>
    /// A symbol for a function
    /// </summary>
    public sealed class FunctionSymbol : FunctionOrMethodSymbol
    {
        public new Name Name { get; }

        public FunctionSymbol(
            Symbol containingSymbol,
            Name name,
            FixedList<DataType> parameterDataTypes,
            DataType returnDataType,
            FixedSet<ReachabilityAnnotation>? reachabilityAnnotations = null)
            : base(containingSymbol, name, parameterDataTypes, returnDataType, reachabilityAnnotations ?? FixedSet<ReachabilityAnnotation>.Empty)
        {
            Name = name;
        }

        public FunctionSymbol(
            Symbol containingSymbol,
            Name name,
            FixedList<DataType> parameterDataTypes,
            FixedSet<ReachabilityAnnotation>? reachabilityAnnotations = null)
            : this(containingSymbol, name, parameterDataTypes, DataType.Void, reachabilityAnnotations ?? FixedSet<ReachabilityAnnotation>.Empty)
        {
        }

        public override bool Equals(Symbol? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return other is FunctionSymbol otherFunction
                   && ContainingSymbol == otherFunction.ContainingSymbol
                   && Name == otherFunction.Name
                   && ParameterDataTypes.SequenceEqual(otherFunction.ParameterDataTypes)
                   && ReturnDataType == otherFunction.ReturnDataType;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ContainingSymbol, Name, ParameterDataTypes, ReturnDataType);
        }

        public override string ToILString()
        {
            return $"{ContainingSymbol}.{Name}({string.Join(", ", ParameterDataTypes)}) -> {ReturnDataType}";
        }
    }
}
