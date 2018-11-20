using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;
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
            [NotNull, ItemNotNull] IEnumerable<Parameter> parameters,
            [NotNull] DataType returnType,
            [CanBeNull] ControlFlowGraph controlFlow)
            : base(name, type)
        {
            Parameters = parameters.ToFixedList();
            ReturnType = returnType;
            ControlFlow = controlFlow;
        }

        [CanBeNull]
        public override ISymbol Lookup([NotNull] SimpleName name)
        {
            throw new System.NotImplementedException();
        }
    }
}
