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
            // At one time it was thought two binding symbols should never differ
            // only by type or mutability. However, parameters of overloaded
            // functions differ by only that and are valid.
            return other is BindingSymbol otherBinding
                && FullName == otherBinding.FullName
                && IsMutableBinding == otherBinding.IsMutableBinding
                && Type == otherBinding.Type;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FullName, IsMutableBinding, Type);
        }
    }
}
