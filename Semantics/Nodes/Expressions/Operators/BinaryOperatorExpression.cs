using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Operators;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Expressions.Operators
{
    public abstract class BinaryOperatorExpression : OperatorExpression
    {
        public new BinaryOperatorExpressionSyntax Syntax { get; }
        public Expression LeftOperand { get; }
        public Expression RightOperand { get; }

        protected BinaryOperatorExpression(
            BinaryOperatorExpressionSyntax syntax,
            IEnumerable<Diagnostic> diagnostics,
            Expression leftOperand,
            Expression rightOperand,
            DataType type)
            : base(diagnostics, type)
        {
            Syntax = syntax;
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
        }

        protected override SyntaxNode GetSyntax()
        {
            return Syntax;
        }

        public override void AllDiagnostics(List<Diagnostic> list)
        {
            base.AllDiagnostics(list);
            LeftOperand.AllDiagnostics(list);
            RightOperand.AllDiagnostics(list);
        }
    }
}
