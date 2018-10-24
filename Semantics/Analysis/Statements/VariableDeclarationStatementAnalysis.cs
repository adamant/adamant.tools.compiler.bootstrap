using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Statements
{
    public class VariableDeclarationStatementAnalysis : StatementAnalysis, IDeclarationAnalysis
    {
        [NotNull] public VariableDeclarationStatementSyntax Syntax { get; }
        [NotNull] public QualifiedName Name { get; }
        [CanBeNull] public ExpressionAnalysis TypeExpression { get; }
        [CanBeNull] public ExpressionAnalysis Initializer { get; }
        [CanBeNull] public DataType Type { get; set; }

        public VariableDeclarationStatementAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] VariableDeclarationStatementSyntax syntax,
            [NotNull] QualifiedName name,
            [CanBeNull] ExpressionAnalysis typeExpression,
            [CanBeNull] ExpressionAnalysis initializer)
            : base(context)
        {
            Requires.NotNull(nameof(syntax), syntax);
            Requires.NotNull(nameof(name), name);
            Syntax = syntax;
            Name = name;
            TypeExpression = typeExpression;
            Initializer = initializer;
        }
    }
}
