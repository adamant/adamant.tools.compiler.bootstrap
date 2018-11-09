using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;
using static Adamant.Tools.Compiler.Bootstrap.Framework.TypeOperations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class FunctionBodyParser : IBlockParser
    {
        [MustUseReturnValue]
        [NotNull]
        public StatementSyntax ParseStatement(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
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
                    IColonToken colon = null;
                    ExpressionSyntax typeExpression = null;
                    if (tokens.Current is ColonToken)
                    {
                        colon = tokens.Expect<IColonToken>();
                        // Need to not consume the assignment that separates the type from the initializer,
                        // hence the min operator precedence.
                        typeExpression = ParseExpression(tokens, diagnostics, OperatorPrecedence.LogicalOr);
                    }
                    EqualsToken equals = null;
                    ExpressionSyntax initializer = null;
                    if (tokens.Current is EqualsToken)
                    {
                        equals = tokens.Take<EqualsToken>();
                        initializer = ParseExpression(tokens, diagnostics);
                    }
                    var semicolon = tokens.Expect<ISemicolonToken>();
                    return new VariableDeclarationStatementSyntax(binding, name, colon, typeExpression, equals, initializer, semicolon);
                }
                default:
                {
                    var expression = ParseExpression(tokens, diagnostics);
                    var semicolon = tokens.Expect<ISemicolonToken>();
                    return new ExpressionStatementSyntax(expression, semicolon);
                }
            }
        }

        [MustUseReturnValue]
        [CanBeNull]
        public BlockSyntax AcceptBlock(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var openBrace = tokens.Accept<OpenBraceToken>();
            if (openBrace == null) return null;
            var statements = listParser.ParseList(tokens, ParseStatement, TypeOf<CloseBraceToken>(), diagnostics);
            var closeBrace = tokens.Expect<ICloseBraceToken>();
            return new BlockSyntax(openBrace, statements, closeBrace);
        }

        [MustUseReturnValue]
        [NotNull]
        public BlockSyntax ParseBlock(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var openBrace = tokens.Expect<IOpenBraceToken>();
            var statements = listParser.ParseList(tokens, ParseStatement, TypeOf<CloseBraceToken>(), diagnostics);
            var closeBrace = tokens.Expect<ICloseBraceToken>();
            return new BlockSyntax(openBrace, statements, closeBrace);
        }

        [MustUseReturnValue]
        [NotNull]
        public ExpressionBlockSyntax ParseExpressionBlock(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            switch (tokens.Current)
            {
                case EqualsGreaterThanToken equalsGreaterThan:
                    tokens.MoveNext();
                    var expression = ParseExpression(tokens, diagnostics);
                    return new ResultExpressionSyntax(equalsGreaterThan, expression);
                case OpenBraceToken _:
                default:
                    return ParseBlock(tokens, diagnostics);
            }
        }
    }
}
