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
        public new Name? Name { get; }
        public FixedList<DataType> ParameterDataTypes { get; }
        public int Arity => ParameterDataTypes.Count;

        protected InvocableSymbol(Symbol containingSymbol, Name? name, FixedList<DataType> parameterDataTypes)
            : base(containingSymbol, name)
        {
            Name = name;
            ParameterDataTypes = parameterDataTypes;
        }
    }
}
