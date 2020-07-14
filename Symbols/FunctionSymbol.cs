using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols
{
    /// <summary>
    /// A symbol for a function
    /// </summary>
    public class FunctionSymbol : ParentSymbol
    {
        public FixedList<BindingSymbol> Parameters { get; }
        public DataType ReturnType { get; }

        // TODO isn't there overlap between the parameters and the child symbols?
        public FunctionSymbol(
            Name fullName,
            FixedList<BindingSymbol> parameters,
            DataType returnType,
            SymbolSet childSymbols)
            : base(fullName, childSymbols)
        {
            Parameters = parameters;
            ReturnType = returnType;
        }
    }
}
