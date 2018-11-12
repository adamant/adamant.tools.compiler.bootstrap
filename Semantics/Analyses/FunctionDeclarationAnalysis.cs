using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    [NotNull]
    public class FunctionDeclarationAnalysis : MemberDeclarationAnalysis
    {
        [NotNull] public new NamedFunctionDeclarationSyntax Syntax { get; }
        [NotNull, ItemNotNull] public FixedList<ParameterAnalysis> Parameters { get; }
        public int? Arity => Parameters?.Count;
        [CanBeNull] public ExpressionAnalysis ReturnTypeExpression { get; }
        [NotNull] public TypeAnalysis ReturnType { get; } = new TypeAnalysis();
        [NotNull, ItemNotNull] public IReadOnlyList<StatementAnalysis> Statements { get; }
        [CanBeNull] public ControlFlowGraph ControlFlow { get; private set; }

        public FunctionDeclarationAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] NamedFunctionDeclarationSyntax syntax,
            [NotNull] Name name,
            [CanBeNull, ItemNotNull] IEnumerable<GenericParameterAnalysis> genericParameters,
            [NotNull, ItemNotNull] IEnumerable<ParameterAnalysis> parameters,
            [CanBeNull] ExpressionAnalysis returnTypeExpression,
            [CanBeNull] [ItemNotNull] IEnumerable<StatementAnalysis> statements)
            : base(context, syntax, name, genericParameters)
        {
            Syntax = syntax;
            Parameters = parameters?.ToFixedList();
            ReturnTypeExpression = returnTypeExpression;
            Statements = (statements ?? Enumerable.Empty<StatementAnalysis>()).ToFixedList();
        }

        [NotNull]
        public ControlFlowGraph CreateControlFlowGraph()
        {
            Requires.Null(nameof(ControlFlow), ControlFlow);
            return ControlFlow = new ControlFlowGraph();
        }

        [CanBeNull]
        public override Declaration Complete([NotNull] Diagnostics diagnostics)
        {
            if (CompleteDiagnostics(diagnostics)) return null;
            return new FunctionDeclaration(
                Context.File,
                Name,
                Type.AssertResolved(),
                Parameters?.Select(p => p.Complete()),
                ReturnType.AssertResolved(),
                ControlFlow);
        }
    }
}
