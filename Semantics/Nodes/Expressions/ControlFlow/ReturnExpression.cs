using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.ControlFlow;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Expressions.ControlFlow
{
    public class ReturnExpression : Expression
    {
        public new ReturnExpressionSyntax Syntax { get; }
        public Expression Expression { get; }

        public ReturnExpression(
            ReturnExpressionSyntax syntax,
            IEnumerable<Diagnostic> diagnostics,
            Expression expression)
        // TODO take in the type rather than assuming never here (let that be semantic analysis
            : base(diagnostics, PrimitiveType.Never)
        {
            Syntax = syntax;
            Expression = expression;
        }

        protected override SyntaxNode GetSyntax()
        {
            return Syntax;
        }

        public override void AllDiagnostics(List<Diagnostic> list)
        {
            base.AllDiagnostics(list);
            Expression.AllDiagnostics(list);
        }
    }
}
