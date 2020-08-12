using System;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols
{
    /// <summary>
    /// A symbol for a variable or parameter. Both of which are bindings using `let` or `var`
    /// </summary>
    public sealed class VariableSymbol : BindingSymbol
    {
        public new InvokableSymbol ContainingSymbol { get; }
        public new Name Name { get; }
        public int? DeclarationNumber { get; }

        public VariableSymbol(
            InvokableSymbol containingSymbol,
            Name name,
            int? declarationNumber,
            bool isMutableBinding,
            DataType dataType)
            : base(containingSymbol, name, isMutableBinding, dataType)
        {
            ContainingSymbol = containingSymbol;
            Name = name;
            DeclarationNumber = declarationNumber;
        }

        public override bool Equals(Symbol? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return other is VariableSymbol otherVariable
                   && ContainingSymbol == otherVariable.ContainingSymbol
                   && Name == otherVariable.Name
                   && DeclarationNumber == otherVariable.DeclarationNumber
                   && IsMutableBinding == otherVariable.IsMutableBinding
                   && DataType == otherVariable.DataType;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, DeclarationNumber, IsMutableBinding, DataType);
        }

        public override string ToString()
        {
            var mutable = IsMutableBinding ? "mut " : "";
            var declarationNumber = DeclarationNumber is null ? "" : "#" + DeclarationNumber;
            return $"{ContainingSymbol} {{{mutable}{Name}{declarationNumber}: {DataType}}}";
        }
    }
}
