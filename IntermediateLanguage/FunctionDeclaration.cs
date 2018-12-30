using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public class FunctionDeclaration : Declaration
    {
        public bool IsExternal { get; }
        public FixedList<Parameter> Parameters { get; }
        public int Arity => Parameters.Count;
        public DataType ReturnType { get; }
        public ControlFlowGraph ControlFlow { get; }

        public FunctionDeclaration(
            bool isExternal,
            bool isMember,
            Name name,
            DataType type,
            FixedList<Parameter> parameters,
            DataType returnType,
            ControlFlowGraph controlFlow)
            : base(isMember, name, type, SymbolSet.Empty)
        {
            Parameters = parameters;
            ReturnType = returnType;
            ControlFlow = controlFlow;
            IsExternal = isExternal;
        }
    }
}
