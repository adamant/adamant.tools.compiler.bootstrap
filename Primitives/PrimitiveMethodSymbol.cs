using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Primitives
{
    internal class PrimitiveMethodSymbol : PrimitiveFunctionSymbol, IMethodSymbol
    {
        public IBindingSymbol SelfParameterSymbol { get; }

        private PrimitiveMethodSymbol(
            Name fullName,
            PrimitiveParameterSymbol selfParameter,
            FixedList<PrimitiveParameterSymbol> parameters,
            DataType returnType)
            : base(fullName, parameters, returnType)
        {
            SelfParameterSymbol = selfParameter;
        }

        public static PrimitiveMethodSymbol New(
            Name fullName,
            DataType returnType,
            DataType selfType,
            params (string name, DataType type)[] parameters)
        {
            var selfParameter = new PrimitiveParameterSymbol(fullName.Qualify(SpecialName.Self), selfType);
            return new PrimitiveMethodSymbol(fullName, selfParameter, ConvertParameters(fullName, parameters), returnType);
        }
    }
}
