using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public class StatementParser : IParser<StatementSyntax>, IParser<BlockExpressionSyntax>
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
        public StatementSyntax Parse([NotNull] ITokenStream tokens, [NotNull] IDiagnosticsCollector diagnostics)
        {
            switch (tokens.Current)
            {
                case OpenBraceToken _:
                    // To simplfy things later, we wrap blocks in an expression statement syntax w/o a semicolon
                    return new ExpressionStatementSyntax(ParseBlock(tokens, diagnostics), null);
                case LetKeywordToken _:
                case VarKeywordToken _:
                    {
                        var binding = tokens.ExpectKeyword();
                        var name = tokens.ExpectIdentifier();
                        var colon = tokens.Expect<ColonToken>();
                        var typeExpression = expressionParser.Parse(tokens, diagnostics);
                        EqualsToken equals = null;
                        ExpressionSyntax initializer = null;
                        if (tokens.Current is EqualsToken)
                        {
                            equals = tokens.Expect<EqualsToken>();
                            initializer = expressionParser.Parse(tokens, diagnostics);
                        }
                        var semicolon = tokens.Expect<SemicolonToken>();
                        return new VariableDeclarationStatementSyntax(binding, name, colon, typeExpression, equals, initializer, semicolon);
                    }
                default:
                    {
                        var expression = expressionParser.Parse(tokens, diagnostics);
                        var semicolon = tokens.Expect<SemicolonToken>();
                        return new ExpressionStatementSyntax(expression, semicolon);
                    }
            }
        }

        [NotNull]
        BlockExpressionSyntax IParser<BlockExpressionSyntax>.Parse(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            return ParseBlock(tokens, diagnostics);
        }

        [MustUseReturnValue]
        [NotNull]
        public BlockExpressionSyntax ParseBlock([NotNull] ITokenStream tokens, [NotNull] IDiagnosticsCollector diagnostics)
        {
            var openBrace = tokens.Expect<OpenBraceToken>();
            var statements = listParser.ParseList(tokens, t => Parse(t, diagnostics), TypeOf<CloseBraceToken>._, diagnostics);
            var closeBrace = tokens.Expect<CloseBraceToken>();
            return new BlockExpressionSyntax(openBrace, statements, closeBrace);
        }
    }
}
