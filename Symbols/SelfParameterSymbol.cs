using System;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols
{
    public class SelfParameterSymbol : BindingSymbol
    {
        public new InvokableSymbol ContainingSymbol { get; }

        public SelfParameterSymbol(InvokableSymbol containingSymbol, bool isMutableBinding, DataType dataType)
            : base(containingSymbol, null, isMutableBinding, dataType)
        {
            ContainingSymbol = containingSymbol;
        }

        public override bool Equals(Symbol? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return other is VariableSymbol otherVariable
                   && ContainingSymbol == otherVariable.ContainingSymbol
                   && Name == otherVariable.Name
                   && IsMutableBinding == otherVariable.IsMutableBinding
                   && DataType == otherVariable.DataType;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, IsMutableBinding, DataType);
        }

        public override string ToString()
        {
            var mutable = IsMutableBinding ? "mut " : "";
            return $"{ContainingSymbol} {{{mutable}{Name}: {DataType}}}";
        }
    }
}