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
        public ControlFlowGraphOld ControlFlowOld { get; }

        IEnumerable<IBindingSymbol> IFunctionSymbol.Parameters => Parameters;

        public ConstructorDeclaration(
            Name fullName,
            FixedList<Parameter> parameters,
            DataType returnType,
            ControlFlowGraphOld controlFlowOld)
            : base(true, fullName, SymbolSet.Empty)
        {
            ReturnType = returnType;
            ControlFlowOld = controlFlowOld;
            Parameters = parameters;
        }
    }
}
