using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public class FunctionDeclaration : Declaration, ICallableDeclaration, IFunctionSymbol
    {
        public bool IsExternal { get; }

        [SuppressMessage("Design", "CA1033:Interface methods should be callable by child types", Justification = "NA")]
        bool ICallableDeclaration.IsConstructor => false;

        public FixedList<Parameter> Parameters { get; }
        public int Arity => Parameters.Count;
        public DataType ReturnType { get; }
        public ControlFlowGraph? IL { get; }

        IEnumerable<IBindingSymbol> IFunctionSymbol.Parameters => Parameters;

        public FunctionDeclaration(
            bool isExternal,
            bool isMember,
            Name name,
            FixedList<Parameter> parameters,
            DataType returnType,
            ControlFlowGraph? il)
            : base(isMember, name, SymbolSet.Empty)
        {
            Parameters = parameters;
            ReturnType = returnType;
            IL = il;
            IsExternal = isExternal;
        }
    }
}
