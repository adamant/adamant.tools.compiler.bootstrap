using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations.Function;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Statements;
using Adamant.Tools.Compiler.Bootstrap.Semantics.ControlFlow.Graph;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations
{
    public class FunctionDeclarationAnalysis : MemberDeclarationAnalysis
    {
        [CanBeNull] public new FunctionType Type { get; set; }
        [NotNull] public new FunctionDeclarationSyntax Syntax { get; }
        [NotNull] [ItemNotNull] public IReadOnlyList<ParameterAnalysis> Parameters { get; }
        public int Arity => Parameters.Count;
        [NotNull] public ExpressionAnalysis ReturnTypeExpression { get; }
        [CanBeNull] public DataType ReturnType { get; set; }
        [NotNull] [ItemNotNull] public IReadOnlyList<StatementAnalysis> Statements { get; }
        [CanBeNull] public ControlFlowGraph ControlFlow { get; set; }


        public FunctionDeclarationAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] FunctionDeclarationSyntax syntax,
            [NotNull] QualifiedName name,
            [NotNull] [ItemNotNull] IEnumerable<ParameterAnalysis> parameters,
            [NotNull] ExpressionAnalysis returnTypeExpression,
            [NotNull] [ItemNotNull] IEnumerable<StatementAnalysis> statements)
            : base(context, syntax, name)
        {
            Requires.NotNull(nameof(syntax), syntax);
            Requires.NotNull(nameof(returnTypeExpression), returnTypeExpression);
            Type = (FunctionType)base.Type;
            Syntax = syntax;
            Parameters = parameters.ToReadOnlyList();
            ReturnTypeExpression = returnTypeExpression;
            Statements = statements.ToReadOnlyList();
        }

        [CanBeNull]
        protected override DataType GetDataType()
        {
            return Type;
        }

        [CanBeNull]
        public override Declaration Complete([NotNull] DiagnosticsBuilder diagnostics)
        {
            if (CompleteDiagnostics(diagnostics)) return null;
            return new FunctionDeclaration(
                Context.File,
                Name,
                Parameters.Select(p => p.Complete()),
                ReturnType.AssertNotNull(),
                ControlFlow);
        }
    }
}
