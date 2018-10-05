using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions
{
    public class NewObjectExpressionSyntax : ExpressionSyntax
    {
        public SimpleToken NewKeyword { get; }
        public TypeSyntax Type { get; }
        public SimpleToken OpenParen { get; }
        public SeparatedListSyntax<ExpressionSyntax> Arguments { get; }
        public SimpleToken CloseParen { get; }

        public NewObjectExpressionSyntax(SimpleToken newKeyword, TypeSyntax type, SimpleToken openParen, SeparatedListSyntax<ExpressionSyntax> arguments, SimpleToken closeParen)
        {
            NewKeyword = newKeyword;
            Type = type;
            OpenParen = openParen;
            Arguments = arguments;
            CloseParen = closeParen;
        }
    }
}
