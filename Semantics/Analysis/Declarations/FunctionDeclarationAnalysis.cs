using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
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
    public class FunctionDeclarationAnalysis : DeclarationAnalysis
    {
        [NotNull] public FunctionDeclarationSyntax Syntax { get; }
        [NotNull] [ItemNotNull] public IReadOnlyList<ParameterAnalysis> Parameters { get; }
        public int Arity => Parameters.Count;
        [NotNull] public ExpressionAnalysis ReturnTypeExpression { get; }
        [CanBeNull] public DataType ReturnType { get; set; }
        [NotNull] [ItemNotNull] public IReadOnlyList<StatementAnalysis> Statements { get; }
        [CanBeNull] public ControlFlowGraph ControlFlow { get; set; }

        public FunctionDeclarationAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] FunctionDeclarationSyntax syntax,
            [NotNull] QualifiedName qualifiedName,
            [NotNull] [ItemNotNull] IEnumerable<ParameterAnalysis> parameters,
            [NotNull] ExpressionAnalysis returnTypeExpression,
            [NotNull] [ItemNotNull] IEnumerable<StatementAnalysis> statements)
            : base(context, qualifiedName)
        {
            Requires.NotNull(nameof(syntax), syntax);
            Requires.NotNull(nameof(returnTypeExpression), returnTypeExpression);
            Syntax = syntax;
            Parameters = parameters.ToReadOnlyList();
            ReturnTypeExpression = returnTypeExpression;
            Statements = statements.ToReadOnlyList();
        }

        [NotNull]
        public override Declaration Complete()
        {
            return new FunctionDeclaration(
                Context.File,
                QualifiedName,
                Parameters.Select(p => p.Complete()),
                ReturnType.AssertNotNull(),
                ControlFlow);
        }
    }
}
