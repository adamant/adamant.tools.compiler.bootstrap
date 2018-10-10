using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Nodes.Statements
{
    public class ExpressionStatement : Statement
    {
        public new ExpressionStatementSyntax Syntax { get; }
        public Expression Expression { get; }

        public ExpressionStatement(
            ExpressionStatementSyntax syntax,
            IEnumerable<Diagnostic> diagnostics,
            Expression expression)
            : base(diagnostics)
        {
            Syntax = syntax;
            Expression = expression;
        }

        protected override SyntaxNode GetSyntax() => Syntax;

        public override void AllDiagnostics(List<Diagnostic> list)
        {
            base.AllDiagnostics(list);
            Expression.AllDiagnostics(list);
        }
    }
}
