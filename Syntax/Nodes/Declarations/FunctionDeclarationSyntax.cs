using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations
{
    public class FunctionDeclarationSyntax : MemberDeclarationSyntax
    {
        public SimpleToken AccessModifier { get; }
        public SimpleToken FunctionKeyword { get; }
        public override IdentifierToken Name { get; }
        public SimpleToken OpenParen { get; }
        public SeparatedListSyntax<ParameterSyntax> Parameters { get; }
        public SimpleToken CloseParen { get; }
        public SimpleToken Arrow { get; }
        public ExpressionSyntax ReturnTypeExpression { get; }
        public BlockStatementSyntax Body { get; }

        public FunctionDeclarationSyntax(
            SimpleToken accessModifier,
            SimpleToken functionKeyword,
            IdentifierToken name,
            SimpleToken openParen,
            SeparatedListSyntax<ParameterSyntax> parameters,
            SimpleToken closeParen,
            SimpleToken arrow,
            ExpressionSyntax returnTypeExpression,
            BlockStatementSyntax body)
        {
            AccessModifier = accessModifier;
            FunctionKeyword = functionKeyword;
            Name = name;
            OpenParen = openParen;
            Parameters = parameters;
            CloseParen = closeParen;
            Arrow = arrow;
            ReturnTypeExpression = returnTypeExpression;
            Body = body;
        }
    }
}
