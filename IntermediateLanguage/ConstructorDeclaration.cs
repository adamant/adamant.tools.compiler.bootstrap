using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public class ConstructorDeclaration : Declaration, ICallableDeclaration, IFunctionSymbol
    {
        public FixedList<Parameter> Parameters { get; }
        public int Arity => Parameters.Count;
        public DataType ReturnType { get; }
        public ControlFlowGraph ControlFlow { get; }

        IEnumerable<IBindingSymbol> IFunctionSymbol.Parameters => Parameters;

        public ConstructorDeclaration(
            Name fullName,
            FixedList<Parameter> parameters,
            DataType returnType,
            ControlFlowGraph controlFlow)
            : base(true, fullName, SymbolSet.Empty)
        {
            ReturnType = returnType;
            ControlFlow = controlFlow;
            Parameters = parameters;
        }
    }
}
