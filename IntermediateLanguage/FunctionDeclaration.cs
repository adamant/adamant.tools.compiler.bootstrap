using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Model
{
    public class FunctionDeclaration : Declaration
    {
        [NotNull, ItemNotNull] public FixedList<Parameter> Parameters { get; }
        public int Arity => Parameters.Count;
        [NotNull] public DataType ReturnType { get; }
        [CanBeNull] public ControlFlowGraph ControlFlow { get; }

        public FunctionDeclaration(
            [NotNull] Name name,
            [NotNull] DataType type,
            [NotNull, ItemNotNull] FixedList<Parameter> parameters,
            [NotNull] DataType returnType,
            [CanBeNull] ControlFlowGraph controlFlow)
            : base(name, type, SymbolSet.Empty)
        {
            Parameters = parameters;
            ReturnType = returnType;
            ControlFlow = controlFlow;
        }
    }
}
