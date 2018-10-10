using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Operators;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Nodes.Expressions.Operators
{
    public abstract class BinaryOperatorExpression : OperatorExpression
    {
        [NotNull] public new BinaryOperatorExpressionSyntax Syntax { get; }
        [NotNull] public Expression LeftOperand { get; }
        [NotNull] public Expression RightOperand { get; }

        protected BinaryOperatorExpression(
            [NotNull] BinaryOperatorExpressionSyntax syntax,
            [NotNull] [ItemNotNull] IEnumerable<Diagnostic> diagnostics,
            [NotNull] Expression leftOperand,
            [NotNull] Expression rightOperand,
            [NotNull] DataType type)
            : base(diagnostics, type)
        {
            Requires.NotNull(nameof(syntax), syntax);
            Requires.NotNull(nameof(leftOperand), leftOperand);
            Requires.NotNull(nameof(rightOperand), rightOperand);
            Syntax = syntax;
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
        }

        [NotNull]
        protected override SyntaxNode GetSyntax()
        {
            return Syntax;
        }

        public override void AllDiagnostics([NotNull][ItemNotNull] List<Diagnostic> list)
        {
            Requires.NotNull(nameof(list), list);
            base.AllDiagnostics(list);
            LeftOperand.AllDiagnostics(list);
            RightOperand.AllDiagnostics(list);
        }
    }
}
