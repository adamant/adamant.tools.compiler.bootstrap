using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations
{
    public class FunctionDeclaration : Declaration
    {
        [NotNull] [ItemNotNull] public IReadOnlyList<Parameter> Parameters { get; }
        public int Arity => Parameters.Count;
        [NotNull] public KnownType ReturnType { get; }
        [CanBeNull] public ControlFlowGraph ControlFlow { get; }

        public FunctionDeclaration(
            [NotNull] CodeFile file,
            [NotNull] Name name,
            [NotNull] KnownType type,
            [NotNull] [ItemNotNull] IEnumerable<Parameter> parameters,
            [NotNull] KnownType returnType,
            [CanBeNull] ControlFlowGraph controlFlow)
            : base(file, name, type)
        {
            Requires.NotNull(nameof(parameters), parameters);
            Requires.NotNull(nameof(returnType), returnType);
            Parameters = parameters.ToReadOnlyList();
            ReturnType = returnType;
            ControlFlow = controlFlow;
        }
    }
}
