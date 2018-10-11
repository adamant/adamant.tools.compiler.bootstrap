using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations
{
    public class FunctionDeclaration : Declaration
    {
        [NotNull] [ItemNotNull] public IReadOnlyList<Parameter> Parameters { get; }
        [NotNull] [ItemNotNull] private readonly List<Parameter> parameters = new List<Parameter>();
        public int Arity => Parameters.Count;
        [NotNull] public DataType ReturnType { get; internal set; }
        //[NotNull] public Block Body { get; }

        public FunctionDeclaration([NotNull] CodeFile file, [NotNull] QualifiedName qualifiedName)
            : base(file, qualifiedName)
        {
            Parameters = parameters.AsReadOnly().AssertNotNull();
            ReturnType = DataType.Unknown;
        }
    }
}
