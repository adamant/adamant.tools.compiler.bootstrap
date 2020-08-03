using System;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols
{
    public sealed class FieldSymbol : BindingSymbol
    {
        public new TypeSymbol ContainingSymbol { get; }

        public FieldSymbol(
            TypeSymbol containingSymbol,
            SimpleName name,
            bool isMutableBinding,
            DataType dataType)
            : base(containingSymbol, name, isMutableBinding, dataType)
        {
            ContainingSymbol = containingSymbol;
        }

        public override bool Equals(Symbol? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return other is FieldSymbol otherField
                   && ContainingSymbol == otherField.ContainingSymbol
                   && Name == otherField.Name
                   && IsMutableBinding == otherField.IsMutableBinding
                   && DataType == otherField.DataType;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ContainingSymbol, Name, IsMutableBinding, DataType);
        }
    }
}
