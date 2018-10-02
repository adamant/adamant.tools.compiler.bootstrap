using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Operators;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Expressions.Operators
{
    public class AsteriskAssignmentExpression : BinaryOperatorExpression
    {
        public AsteriskAssignmentExpression(
            BinaryOperatorExpressionSyntax syntax,
            IEnumerable<DiagnosticInfo> diagnostics,
            Expression leftOperand,
            Expression rightOperand,
            DataType type)
            : base(syntax, diagnostics, leftOperand, rightOperand, type)
        {
        }
    }
}
