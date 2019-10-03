using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public class FunctionDeclaration : Declaration, IFunctionSymbol
    {
        public bool IsExternal { get; }
        public FixedList<Parameter> Parameters { get; }
        public int Arity => Parameters.Count;
        public DataType ReturnType { get; }
        public ControlFlowGraph ControlFlow { get; }

        IEnumerable<IBindingSymbol> IFunctionSymbol.Parameters => Parameters;

        public FunctionDeclaration(
            bool isExternal,
            bool isMember,
            Name name,
            FixedList<Parameter> parameters,
            DataType returnType,
            ControlFlowGraph controlFlow)
            : base(isMember, name, SymbolSet.Empty)
        {
            Parameters = parameters;
            ReturnType = returnType;
            ControlFlow = controlFlow;
            IsExternal = isExternal;
        }
    }
}
