using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Statements
{
    public class ExpressionStatement : Statement
    {
        public new ExpressionStatementSyntax Syntax { get; }
        public Expression Expression { get; }

        public ExpressionStatement(
            ExpressionStatementSyntax syntax,
            IEnumerable<DiagnosticInfo> diagnostics,
            Expression expression)
            : base(diagnostics)
        {
            Syntax = syntax;
            Expression = expression;
        }

        protected override SyntaxNode GetSyntax() => Syntax;
    }
}
