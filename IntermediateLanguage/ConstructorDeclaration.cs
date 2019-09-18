using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public class ConstructorDeclaration : Declaration
    {
        public FixedList<Parameter> Parameters { get; }
        public int Arity => Parameters.Count;
        public DataType ReturnType { get; }
        public ControlFlowGraph ControlFlow { get; }

        public ConstructorDeclaration(
            Name fullName,
            //DataType type,
            FixedList<Parameter> parameters,
            DataType returnType,
            ControlFlowGraph controlFlow)
            : base(true, fullName, /*type,*/null, SymbolSet.Empty)
        {
            ReturnType = returnType;
            ControlFlow = controlFlow;
            Parameters = parameters;
        }
    }
}
