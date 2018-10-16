using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
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
        public StatementSyntax Parse([NotNull] ITokenStream tokens, [NotNull] IDiagnosticsCollector diagnostics)
        {
            switch (tokens.Current)
            {
                case OpenBraceToken _:
                    return ParseBlockStatement(tokens, diagnostics);
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
        BlockStatementSyntax IParser<BlockStatementSyntax>.Parse(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            return ParseBlockStatement(tokens, diagnostics);
        }

        [MustUseReturnValue]
        [NotNull]
        public BlockStatementSyntax ParseBlockStatement([NotNull] ITokenStream tokens, [NotNull] IDiagnosticsCollector diagnostics)
        {
            var openBrace = tokens.Expect<OpenBraceToken>();
            var statements = listParser.ParseList(tokens, t => Parse(t, diagnostics), TypeOf<CloseBraceToken>._, diagnostics);
            var closeBrace = tokens.Expect<CloseBraceToken>();
            return new BlockStatementSyntax(openBrace, statements, closeBrace);
        }
    }
}
