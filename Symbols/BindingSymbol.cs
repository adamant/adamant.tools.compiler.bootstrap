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
    }
}
