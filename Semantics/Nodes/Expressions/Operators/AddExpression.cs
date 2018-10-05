using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Operators;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Expressions.Operators
{
    /// <summary>
    /// This is an actual add between primitive types, a user defined plus operator
    /// should be a function call
    /// </summary>
    public class AddExpression : BinaryOperatorExpression
    {
        public AddExpression(
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
