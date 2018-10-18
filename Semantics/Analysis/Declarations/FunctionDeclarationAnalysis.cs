using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.ControlFlow.Graph;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations
{
    public class FunctionDeclarationAnalysis : DeclarationAnalysis
    {
        [NotNull] public FunctionDeclarationSyntax Syntax { get; }
        [NotNull] [ItemNotNull] public IReadOnlyList<ParameterAnalysis> Parameters { get; }
        public int Arity => Parameters.Count;
        [NotNull] public DataType ReturnType { get; set; }
        [NotNull] public ControlFlowGraph ControlFlow { get; } = new ControlFlowGraph();

        public FunctionDeclarationAnalysis(
            [NotNull] CodeFile file,
            [NotNull] LexicalScope scope,
            [NotNull] FunctionDeclarationSyntax syntax,
            [NotNull] QualifiedName qualifiedName)
            : base(file, scope, qualifiedName)
        {
            Requires.NotNull(nameof(syntax), syntax);
            Syntax = syntax;
            Parameters = syntax.Parameters.Select(p => new ParameterAnalysis(p)).ToReadOnlyList();
            ReturnType = DataType.Unknown;
        }

        [NotNull]
        public override Declaration Complete()
        {
            return new FunctionDeclaration(File, QualifiedName, Parameters.Select(p => p.Complete()), ReturnType, ControlFlow);
        }
    }
}
