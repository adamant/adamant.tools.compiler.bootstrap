using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Operators;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Nodes.Expressions.Operators
{
    // TODO this shouldn't exist, it should just be a function call to the correct operator overload
    public class DotDotExpression : BinaryOperatorExpression
    {
        public DotDotExpression(
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
