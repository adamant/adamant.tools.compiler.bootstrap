using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

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
