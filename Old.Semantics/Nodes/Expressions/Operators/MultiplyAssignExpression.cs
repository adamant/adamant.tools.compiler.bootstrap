using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Operators;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Expressions.Operators
{
    /// <summary>
    /// Multiply assign of primitive types
    /// </summary>
    public class MultiplyAssignExpression : BinaryOperatorExpression
    {
        public MultiplyAssignExpression(
            BinaryOperatorExpressionSyntax syntax,
            IEnumerable<Diagnostic> diagnostics,
            Expression leftOperand,
            Expression rightOperand,
            DataType type)
            : base(syntax, diagnostics, leftOperand, rightOperand, type)
        {
        }
    }
}
