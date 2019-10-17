using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class MoveTransferSyntax : TransferSyntax, IMoveTransferSyntax
    {
        public MoveTransferSyntax(TextSpan span, IExpressionSyntax expression)
            : base(span, expression)
        {
        }

        public override string ToString()
        {
            return $"move {Expression}";
        }
    }
}