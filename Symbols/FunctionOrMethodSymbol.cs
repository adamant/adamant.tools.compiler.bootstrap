using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols.Reachability;
using Adamant.Tools.Compiler.Bootstrap.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols
{
    [Closed(
        typeof(FunctionSymbol),
        typeof(MethodSymbol))]
    public abstract class FunctionOrMethodSymbol : InvocableSymbol
    {
        public new Name Name { get; }

        protected FunctionOrMethodSymbol(
            Symbol containingSymbol,
            Name name,
            FixedList<DataType> parameterDataTypes,
            DataType returnDataType,
            FixedSet<ReachabilityAnnotation> reachabilityAnnotations)
            : base(containingSymbol, name, parameterDataTypes, returnDataType, reachabilityAnnotations)
        {
            Name = name;
        }
    }
}
