using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public class StatementParser : IParser<StatementSyntax>, IParser<BlockStatementSyntax>
    {
        private readonly IListParser listParser;
        private readonly IParser<ExpressionSyntax> expressionParser;

        public StatementParser(IListParser listParser, IParser<ExpressionSyntax> expressionParser)
        {
            this.listParser = listParser;
            this.expressionParser = expressionParser;
        }

        [MustUseReturnValue]
        public StatementSyntax Parse(ITokenStream tokens)
        {
            switch (tokens.Current?.Kind)
            {
                case TokenKind.OpenBrace:
                    return ParseStatementBlock(tokens);
                case TokenKind.LetKeyword:
                case TokenKind.VarKeyword:
                    {
                        var binding = tokens.ExpectSimple();
                        var name = tokens.ExpectIdentifier();
                        var colon = tokens.ExpectSimple(TokenKind.Colon);
                        var typeExpression = expressionParser.Parse(tokens);
                        SimpleToken? equals = null;
                        ExpressionSyntax initializer = null;
                        if (tokens.Current?.Kind == TokenKind.Equals)
                        {
                            equals = tokens.ExpectSimple(TokenKind.Equals);
                            initializer = expressionParser.Parse(tokens);
                        }
                        var semicolon = tokens.ExpectSimple(TokenKind.Semicolon);
                        return new VariableDeclarationStatementSyntax(binding, name, colon, typeExpression, equals, initializer, semicolon);
                    }
                default:
                    {
                        var expression = expressionParser.Parse(tokens);
                        var semicolon = tokens.ExpectSimple(TokenKind.Semicolon);
                        return new ExpressionStatementSyntax(expression, semicolon);
                    }
            }
        }

        BlockStatementSyntax IParser<BlockStatementSyntax>.Parse(ITokenStream tokens)
        {
            return ParseStatementBlock(tokens);
        }

        [MustUseReturnValue]
        private BlockStatementSyntax ParseStatementBlock(ITokenStream tokens)
        {
            var openBrace = tokens.ExpectSimple(TokenKind.OpenBrace);
            var statements = listParser.ParseList<StatementSyntax>(tokens, Parse, TokenKind.CloseBrace);
            var closeBrace = tokens.ExpectSimple(TokenKind.CloseBrace);
            return new BlockStatementSyntax(openBrace, statements, closeBrace);
        }
    }
}
