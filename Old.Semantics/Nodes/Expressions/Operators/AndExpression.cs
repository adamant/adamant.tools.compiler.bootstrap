using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Operators;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Nodes.Expressions.Operators
{
    public class AndExpression : BinaryOperatorExpression
    {
        public AndExpression(
            [NotNull] BinaryOperatorExpressionSyntax syntax,
            [NotNull][ItemNotNull] IEnumerable<Diagnostic> diagnostics,
            [NotNull] Expression leftOperand,
            [NotNull] Expression rightOperand,
            [NotNull] DataType type)
            : base(syntax, diagnostics, leftOperand, rightOperand, type)
        {
        }
    }
}
