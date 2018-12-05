using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage
{
    public class ConstructorDeclaration : Declaration
    {
        [NotNull, ItemNotNull] public FixedList<Parameter> Parameters { get; }
        public int Arity => Parameters.Count;
        [NotNull] public DataType ReturnType { get; }
        [CanBeNull] public ControlFlowGraph ControlFlow { get; }

        public ConstructorDeclaration(
            [NotNull] Name fullName,
            [NotNull] DataType type,
            [NotNull] [ItemNotNull] FixedList<Parameter> parameters,
            [NotNull] DataType returnType,
            [CanBeNull] ControlFlowGraph controlFlow)
            : base(fullName, type, SymbolSet.Empty)
        {
            ReturnType = returnType;
            ControlFlow = controlFlow;
            Parameters = parameters;
        }
    }
}