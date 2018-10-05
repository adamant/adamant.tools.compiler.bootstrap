using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions
{
    public class InvocationSyntax : ExpressionSyntax
    {
        public ExpressionSyntax Callee { get; set; }
        public SimpleToken OpenParen { get; }
        public SeparatedListSyntax<ExpressionSyntax> Arguments { get; }
        public SimpleToken CloseParen { get; }

        public InvocationSyntax(ExpressionSyntax callee, SimpleToken openParen, SeparatedListSyntax<ExpressionSyntax> arguments, SimpleToken closeParen)
        {
            Callee = callee;
            OpenParen = openParen;
            Arguments = arguments;
            CloseParen = closeParen;
        }
    }
}
