using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Operators;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Nodes.Expressions.Operators
{
    public class MemberAccessExpression : BinaryOperatorExpression
    {
        public MemberAccessExpression(
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
