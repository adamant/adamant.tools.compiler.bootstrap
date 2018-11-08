using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    [NotNull]
    public class FunctionDeclarationAnalysis : MemberDeclarationAnalysis
    {
        [NotNull] public new NamedFunctionDeclarationSyntax Syntax { get; }
        [NotNull, ItemNotNull] public IReadOnlyList<ParameterAnalysis> Parameters { get; }
        public int Arity => Parameters.Count;
        [NotNull] public ExpressionAnalysis ReturnTypeExpression { get; }
        [NotNull] public TypeAnalysis ReturnType { get; } = new TypeAnalysis();
        [NotNull, ItemNotNull] public IReadOnlyList<StatementAnalysis> Statements { get; }
        [CanBeNull] public ControlFlowGraph ControlFlow { get; set; }

        public FunctionDeclarationAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] NamedFunctionDeclarationSyntax syntax,
            [NotNull] Name name,
            [CanBeNull, ItemNotNull] IEnumerable<GenericParameterAnalysis> genericParameters,
            [NotNull, ItemNotNull] IEnumerable<ParameterAnalysis> parameters,
            [NotNull] ExpressionAnalysis returnTypeExpression,
            [CanBeNull] [ItemNotNull] IEnumerable<StatementAnalysis> statements)
            : base(context, syntax, name, genericParameters)
        {
            Requires.NotNull(nameof(syntax), syntax);
            Requires.NotNull(nameof(returnTypeExpression), returnTypeExpression);
            Syntax = syntax;
            Parameters = parameters.ToReadOnlyList();
            ReturnTypeExpression = returnTypeExpression;
            Statements = (statements ?? Enumerable.Empty<StatementAnalysis>()).ToReadOnlyList();
        }

        [CanBeNull]
        public override Declaration Complete([NotNull] DiagnosticsBuilder diagnostics)
        {
            if (CompleteDiagnostics(diagnostics)) return null;
            return new FunctionDeclaration(
                Context.File,
                Name,
                Type.AssertResolved(),
                Parameters.Select(p => p.Complete()),
                ReturnType.AssertResolved(),
                ControlFlow);
        }
    }
}
