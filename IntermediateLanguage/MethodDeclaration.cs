using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public class MethodDeclaration : Declaration, ICallableDeclaration, IMethodSymbol
    {
        public bool IsExternal => false;
        [SuppressMessage("Design", "CA1033:Interface methods should be callable by child types",
            Justification = "NA")]
        bool ICallableDeclaration.IsConstructor => false;
        public Parameter SelfParameter { get; }

        [SuppressMessage("Design", "CA1033:Interface methods should be callable by child types",
            Justification = "NA")]
        IBindingSymbol IMethodSymbol.SelfParameterSymbol => SelfParameter;
        public FixedList<Parameter> Parameters { get; }
        public int Arity => Parameters.Count;
        public DataType ReturnType { get; }
        public ControlFlowGraph? IL { get; }

        IEnumerable<IBindingSymbol> IFunctionSymbol.Parameters => Parameters;

        public MethodDeclaration(
            Name name,
            Parameter selfParameter,
            FixedList<Parameter> parameters,
            DataType returnType,
            ControlFlowGraph? il)
            : base(true, name, SymbolSet.Empty)
        {
            SelfParameter = selfParameter;
            Parameters = parameters;
            ReturnType = returnType;
            IL = il;
        }
    }
}
