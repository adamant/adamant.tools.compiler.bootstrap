using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions
{
    public class InvocationSyntax : ExpressionSyntax
    {
        public ExpressionSyntax Callee { get; set; }
        public Token OpenParen { get; }
        public SeparatedListSyntax<ExpressionSyntax> Arguments { get; }
        public Token CloseParen { get; }

        public InvocationSyntax(ExpressionSyntax callee, Token openParen, SeparatedListSyntax<ExpressionSyntax> arguments, Token closeParen)
            : base(callee, openParen, arguments, closeParen)
        {
            Callee = callee;
            OpenParen = openParen;
            Arguments = arguments;
            CloseParen = closeParen;
        }
    }
}
