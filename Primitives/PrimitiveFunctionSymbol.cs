using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Primitives
{
    internal class PrimitiveFunctionSymbol : PrimitiveSymbol, IFunctionSymbol
    {
        public FixedList<PrimitiveParameterSymbol> Parameters { get; }

        IEnumerable<IBindingSymbol> IFunctionSymbol.Parameters => Parameters;

        public DataType ReturnType { get; }

        protected PrimitiveFunctionSymbol(Name fullName, FixedList<PrimitiveParameterSymbol> parameters, DataType returnType)
            : base(fullName, new SymbolSet(parameters))
        {
            Parameters = parameters;
            ReturnType = returnType;
        }

        public static PrimitiveFunctionSymbol New(
            Name fullName,
            params (string name, DataType type)[] parameters)
        {
            return new PrimitiveFunctionSymbol(fullName, ConvertParameters(fullName, parameters), DataType.Void);
        }

        public static PrimitiveFunctionSymbol New(
            Name fullName,
            DataType returnType,
            params (string name, DataType type)[] parameters)
        {
            return new PrimitiveFunctionSymbol(fullName, ConvertParameters(fullName, parameters), returnType);
        }

        protected static FixedList<PrimitiveParameterSymbol> ConvertParameters(
            Name fullName,
            params (string name, DataType type)[] parameters)
        {
            return parameters.Select(p => new PrimitiveParameterSymbol(fullName.Qualify(p.name), p.type))
                .ToFixedList();
        }
    }
}
