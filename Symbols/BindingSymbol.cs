using System;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols
{
    /// <summary>
    /// A symbol for a variable or field. Both of which are bindings using `let` or `var`
    /// </summary>
    public sealed class BindingSymbol : Symbol
    {
        public DataType Type { get; }
        public bool IsMutableBinding { get; }

        internal BindingSymbol(Name fullName, bool isMutableBinding, DataType type)
            : base(fullName)
        {
            IsMutableBinding = isMutableBinding;
            Type = type;
        }

        public override bool Equals(Symbol? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (!(other is BindingSymbol otherBinding)) return false;
            if (FullName != otherBinding.FullName) return false;
            if (IsMutableBinding != otherBinding.IsMutableBinding)
                throw new Exception($"Two {nameof(BindingSymbol)}s with the name {FullName} differ in whether the binding is mutable.");
            if (Type != otherBinding.Type)
                throw new Exception($"Two {nameof(BindingSymbol)}s with the name {FullName} differ in in type. One is {Type}, the other {otherBinding.Type}.");
            return true;
        }

        public override int GetHashCode()
        {
            // Can't include type and is mutable, because we *want* to be compared
            // against a binding that only differs in those respects so we detect
            // the error.
            return HashCode.Combine(FullName);
        }
    }
}
