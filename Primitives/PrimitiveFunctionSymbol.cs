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
        public FixedList<PrimitiveParameterSymbol> Parameters { get; internal set; }

        IEnumerable<IBindingSymbol> IFunctionSymbol.Parameters => Parameters;

        public DataType ReturnType { get; internal set; } = DataType.Void;

        private PrimitiveFunctionSymbol(Name fullName)
            : base(fullName)
        {
            Parameters = FixedList<PrimitiveParameterSymbol>.Empty;
        }

        public static PrimitiveFunctionSymbol New(Name fullName, params (string name, DataType type)[] parameters)
        {
            var func = new PrimitiveFunctionSymbol(fullName);
            func.SetParameters(parameters);
            return func;
        }

        public static PrimitiveFunctionSymbol New(
            Name fullName,
            DataType returnType,
            params (string name, DataType type)[] parameters)
        {
            var func = new PrimitiveFunctionSymbol(fullName);
            func.SetParameters(parameters);
            func.ReturnType = returnType;
            return func;
        }

        public void SetParameters(params (string name, DataType type)[] parameters)
        {
            Parameters = parameters.Select(p => new PrimitiveParameterSymbol(FullName.Qualify(p.name), p.type))
                .ToFixedList();
            ChildSymbols = new SymbolSet(Parameters);
        }
    }
}
