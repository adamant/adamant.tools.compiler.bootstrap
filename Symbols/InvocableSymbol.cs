using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols
{
    [Closed(
        typeof(FunctionOrMethodSymbol),
        typeof(ConstructorSymbol))]
    public abstract class InvocableSymbol : Symbol
    {
        public new Symbol ContainingSymbol { get; }
        public new Name? Name { get; }
        public FixedList<DataType> ParameterDataTypes { get; }
        public int Arity => ParameterDataTypes.Count;
        public DataType ReturnDataType { get; }

        protected InvocableSymbol(Symbol containingSymbol, Name? name, FixedList<DataType> parameterDataTypes, DataType returnDataType)
            : base(containingSymbol, name)
        {
            ContainingSymbol = containingSymbol;
            Name = name;
            ParameterDataTypes = parameterDataTypes;
            ReturnDataType = returnDataType;
        }
    }
}
