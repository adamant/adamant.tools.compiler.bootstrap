using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols
{
    /// <summary>
    /// A symbol for a variable or field. Both of which are bindings using `let` or `var`
    /// </summary>
    public interface IBindingSymbol : ISymbol
    {
        DataType Type { get; }
        bool IsMutableBinding { get; }
    }
}
