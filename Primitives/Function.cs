using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Primitives
{
    internal class Function : Primitive, IFunctionSymbol
    {
        public FixedList<Parameter> Parameters { get; internal set; }

        IEnumerable<IBindingSymbol> IFunctionSymbol.Parameters => Parameters;

        public DataType ReturnType { get; internal set; } = DataType.Void;

        private Function(Name fullName)
            : base(fullName)
        {
        }

        public static Function New(Name fullName, params (string name, DataType type)[] parameters)
        {
            var func = new Function(fullName);
            func.SetParameters(parameters);
            return func;
        }

        public static Function New(
            Name fullName,
            DataType returnType,
            params (string name, DataType type)[] parameters)
        {
            var func = new Function(fullName);
            func.SetParameters(parameters);
            func.ReturnType = returnType;
            return func;
        }

        public void SetParameters(params (string name, DataType type)[] parameters)
        {
            Parameters = parameters.Select(p => new Parameter(FullName.Qualify(p.name), p.type))
                .ToFixedList();
            ChildSymbols = new SymbolSet(Parameters);
        }
    }
}
