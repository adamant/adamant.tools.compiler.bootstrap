using System;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols
{
    public sealed class MethodSymbol : FunctionSymbol
    {
        public BindingSymbol SelfParameter { get; }

        public MethodSymbol(
            Name fullName,
            BindingSymbol selfParameter,
            FixedList<BindingSymbol> parameters,
            DataType returnType)
            : base(fullName, parameters, returnType)
        {
            SelfParameter = selfParameter;
        }

        public override bool Equals(Symbol? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return other is MethodSymbol otherMethod
                   && FullName == otherMethod.FullName
                   && SelfParameter == otherMethod.SelfParameter
                   && Parameters.SequenceEqual(otherMethod.Parameters)
                   && ReturnType == otherMethod.ReturnType;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FullName, SelfParameter, Parameters, ReturnType);
        }
    }
}
