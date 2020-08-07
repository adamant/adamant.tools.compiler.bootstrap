using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols
{
    [Closed(
        typeof(FunctionOrMethodSymbol),
        typeof(ConstructorSymbol))]
    public abstract class InvokableSymbol : Symbol
    {
        public override PackageSymbol Package { get; }
        public FixedList<DataType> ParameterDataTypes { get; }
        public int Arity => ParameterDataTypes.Count;

        protected InvokableSymbol(Symbol containingSymbol, Name? name, FixedList<DataType> parameterDataTypes)
            : base(containingSymbol, name)
        {
            Package = containingSymbol.Package;
            ParameterDataTypes = parameterDataTypes;
        }
    }
}
