using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols
{
    public class MethodSymbol : FunctionSymbol
    {
        public BindingSymbol SelfParameter { get; }

        public MethodSymbol(
            Name fullName,
            BindingSymbol selfParameter,
            FixedList<BindingSymbol> parameters,
            DataType returnType,
            SymbolSet childSymbols)
            : base(fullName, parameters, returnType, childSymbols)
        {
            SelfParameter = selfParameter;
        }
    }
}
