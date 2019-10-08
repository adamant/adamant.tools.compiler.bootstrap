using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class MutableTransferSyntax : TransferSyntax, IMutableTransferSyntax
    {
        public MutableTransferSyntax(TextSpan span, IExpressionSyntax expression)
            : base(span, expression)
        {
        }

        public override string ToString()
        {
            return $"mut {Expression}";
        }
    }
}
