using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public class StatementParser : IParser<StatementSyntax>, IParser<BlockStatementSyntax>
    {
        [NotNull]
        private readonly IListParser listParser;

        [NotNull]
        private readonly IParser<ExpressionSyntax> expressionParser;

        public StatementParser([NotNull] IListParser listParser, [NotNull] IParser<ExpressionSyntax> expressionParser)
        {
            this.listParser = listParser;
            this.expressionParser = expressionParser;
        }

        [MustUseReturnValue]
        [NotNull]
        public StatementSyntax Parse([NotNull] ITokenStream tokens)
        {
            switch (tokens.Current)
            {
                case OpenBraceToken _:
                    return ParseStatementBlock(tokens);
                case LetKeywordToken _:
                case VarKeywordToken _:
                    {
                        var binding = tokens.ExpectKeyword();
                        var name = tokens.ExpectIdentifier();
                        var colon = tokens.Expect<ColonToken>();
                        var typeExpression = expressionParser.Parse(tokens);
                        EqualsToken equals = null;
                        ExpressionSyntax initializer = null;
                        if (tokens.Current is EqualsToken)
                        {
                            equals = tokens.Expect<EqualsToken>();
                            initializer = expressionParser.Parse(tokens);
                        }
                        var semicolon = tokens.Expect<SemicolonToken>();
                        return new VariableDeclarationStatementSyntax(binding, name, colon, typeExpression, equals, initializer, semicolon);
                    }
                default:
                    {
                        var expression = expressionParser.Parse(tokens);
                        var semicolon = tokens.Expect<SemicolonToken>();
                        return new ExpressionStatementSyntax(expression, semicolon);
                    }
            }
        }

        [NotNull]
        BlockStatementSyntax IParser<BlockStatementSyntax>.Parse([NotNull] ITokenStream tokens)
        {
            return ParseStatementBlock(tokens);
        }

        [MustUseReturnValue]
        [NotNull]
        private BlockStatementSyntax ParseStatementBlock([NotNull] ITokenStream tokens)
        {
            var openBrace = tokens.Expect<OpenBraceToken>();
            var statements = listParser.ParseList(tokens, Parse, TypeOf<CloseBraceToken>._);
            var closeBrace = tokens.Expect<CloseBraceToken>();
            return new BlockStatementSyntax(openBrace, statements, closeBrace);
        }
    }
}
