using System;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols
{
    /// <summary>
    /// A symbol for a variable or parameter. Both of which are bindings using `let` or `var`
    /// </summary>
    public sealed class VariableSymbol : NamedBindingSymbol
    {
        public new InvocableSymbol ContainingSymbol { get; }
        public int? DeclarationNumber { get; }

        public VariableSymbol(
            InvocableSymbol containingSymbol,
            Name name,
            int? declarationNumber,
            bool isMutableBinding,
            DataType dataType)
            : base(containingSymbol, name, isMutableBinding, dataType)
        {
            ContainingSymbol = containingSymbol;
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

        public override string ToILString()
        {
            var mutable = IsMutableBinding ? "mut " : "";
            var declarationNumber = DeclarationNumber is null ? "" : "#" + DeclarationNumber;
            return $"{ContainingSymbol} {{{mutable}{Name}{declarationNumber}: {DataType}}}";
        }
    }
}
