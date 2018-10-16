using System.Collections.Generic;
using System.Linq;
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
        [NotNull] [ItemNotNull] private readonly List<Parameter> parameters = new List<Parameter>();
        public int Arity => Parameters.Count;
        [NotNull] public DataType ReturnType { get; internal set; }
        [NotNull] public ControlFlowGraph ControlFlow { get; internal set; } = new ControlFlowGraph();

        public FunctionDeclaration([NotNull] CodeFile file, [NotNull] QualifiedName qualifiedName)
            : base(file, qualifiedName)
        {
            Parameters = parameters.AsReadOnly().AssertNotNull();
            ReturnType = DataType.Unknown;
        }

        // Full constructor needed for testing etc.
        public FunctionDeclaration(
            [NotNull] CodeFile file,
            [NotNull] QualifiedName qualifiedName,
            [NotNull][ItemNotNull] IEnumerable<Parameter> parameters,
            [NotNull] DataType returnType)
            : base(file, qualifiedName)
        {
            Requires.NotNull(nameof(parameters), parameters);
            Requires.NotNull(nameof(returnType), returnType);
            Parameters = parameters.ToList();
            ReturnType = returnType;
        }
    }
}
