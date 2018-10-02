using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions
{
    public class NewObjectExpressionSyntax : ExpressionSyntax
    {
        public Token NewKeyword { get; }
        public TypeSyntax Type { get; }
        public Token OpenParen { get; }
        public SeparatedListSyntax<ExpressionSyntax> Arguments { get; }
        public Token CloseParen { get; }

        public NewObjectExpressionSyntax(Token newKeyword, TypeSyntax type, Token openParen, SeparatedListSyntax<ExpressionSyntax> arguments, Token closeParen)
            : base(newKeyword, type, openParen, arguments, closeParen)
        {
            NewKeyword = newKeyword;
            Type = type;
            OpenParen = openParen;
            Arguments = arguments;
            CloseParen = closeParen;
        }
    }
}
