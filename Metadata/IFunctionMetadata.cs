using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols
{
    /// <summary>
    /// A symbol for a function
    /// </summary>
    public interface IFunctionSymbol : IParentSymbol
    {
        IEnumerable<IBindingSymbol> Parameters { get; }
        DataType ReturnType { get; }
    }
}
