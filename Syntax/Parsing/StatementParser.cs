using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;
using static Adamant.Tools.Compiler.Bootstrap.Framework.TypeOperations;

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
                    // To simplify things later, we wrap blocks in an expression statement syntax w/o a semicolon
                    return new ExpressionStatementSyntax(ParseBlock(tokens, diagnostics), null);
                case LetKeywordToken _:
                case VarKeywordToken _:
                    {
                        var binding = tokens.Take<IBindingKeywordToken>();
                        var name = tokens.ExpectIdentifier();
                        var colon = tokens.Expect<IColonToken>();
                        var typeExpression = expressionParser.Parse(tokens, diagnostics);
                        EqualsToken equals = null;
                        ExpressionSyntax initializer = null;
                        if (tokens.Current is EqualsToken)
                        {
                            equals = tokens.Take<EqualsToken>();
                            initializer = expressionParser.Parse(tokens, diagnostics);
                        }
                        var semicolon = tokens.Expect<ISemicolonToken>();
                        return new VariableDeclarationStatementSyntax(binding, name, colon, typeExpression, equals, initializer, semicolon);
                    }
                default:
                    {
                        var expression = expressionParser.Parse(tokens, diagnostics);
                        var semicolon = tokens.Expect<ISemicolonToken>();
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
            var openBrace = tokens.Expect<IOpenBraceToken>();
            var statements = listParser.ParseList(tokens, t => Parse(t, diagnostics), TypeOf<CloseBraceToken>(), diagnostics);
            var closeBrace = tokens.Expect<ICloseBraceToken>();
            return new BlockExpressionSyntax(openBrace, statements, closeBrace);
        }
    }
}
