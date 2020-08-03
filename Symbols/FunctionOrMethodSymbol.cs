using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols
{
    [Closed(
        typeof(FunctionSymbol),
        typeof(MethodSymbol))]
    public abstract class FunctionOrMethodSymbol : InvokableSymbol
    {
        public new SimpleName Name { get; }
        public DataType ReturnDataType { get; }
        protected FunctionOrMethodSymbol(Symbol containingSymbol, SimpleName name, FixedList<DataType> parameterDataTypes, DataType returnDataType)
            : base(containingSymbol, name, parameterDataTypes)
        {
            Name = name;
            ReturnDataType = returnDataType;
        }
    }
}
