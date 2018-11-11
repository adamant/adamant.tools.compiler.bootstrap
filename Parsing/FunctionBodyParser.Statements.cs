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
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            switch (tokens.Current)
            {
                case IOpenBraceToken _:
                    // To simplify things later, we wrap blocks in an expression statement syntax w/o a semicolon
                    return new ExpressionStatementSyntax(ParseBlock(tokens, diagnostics), null);
                case ILetKeywordToken _:
                case IVarKeywordToken _:
                {
                    var binding = tokens.Take<IBindingToken>();
                    var name = tokens.ExpectIdentifier();
                    IColonTokenPlace colon = null;
                    ExpressionSyntax typeExpression = null;
                    if (tokens.Current is IColonToken)
                    {
                        colon = tokens.Consume<IColonTokenPlace>();
                        // Need to not consume the assignment that separates the type from the initializer,
                        // hence the min operator precedence.
                        typeExpression = ParseExpression(tokens, diagnostics, OperatorPrecedence.LogicalOr);
                    }
                    IEqualsToken equals = null;
                    ExpressionSyntax initializer = null;
                    if (tokens.Current is IEqualsToken)
                    {
                        equals = tokens.Take<IEqualsToken>();
                        initializer = ParseExpression(tokens, diagnostics);
                    }
                    var semicolon = tokens.Consume<ISemicolonTokenPlace>();
                    return new VariableDeclarationStatementSyntax(binding, name, colon, typeExpression, equals, initializer, semicolon);
                }
                default:
                {
                    var expression = ParseExpression(tokens, diagnostics);
                    var semicolon = tokens.Consume<ISemicolonTokenPlace>();
                    return new ExpressionStatementSyntax(expression, semicolon);
                }
            }
        }

        [MustUseReturnValue]
        [CanBeNull]
        public BlockSyntax AcceptBlock(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var openBrace = tokens.Accept<IOpenBraceToken>();
            if (openBrace == null) return null;
            var statements = listParser.ParseList(tokens, ParseStatement, TypeOf<ICloseBraceToken>(), diagnostics);
            var closeBrace = tokens.Consume<ICloseBraceTokenPlace>();
            var span = TextSpan.Covering(openBrace.Span, closeBrace.Span);
            return new BlockSyntax(span, statements);
        }

        [MustUseReturnValue]
        [NotNull]
        public BlockSyntax ParseBlock(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var openBrace = tokens.Consume<IOpenBraceTokenPlace>();
            var statements = listParser.ParseList(tokens, ParseStatement, TypeOf<ICloseBraceToken>(), diagnostics);
            var closeBrace = tokens.Consume<ICloseBraceTokenPlace>();
            var span = TextSpan.Covering(openBrace.Span, closeBrace.Span);
            return new BlockSyntax(span, statements);
        }

        [MustUseReturnValue]
        [NotNull]
        public ExpressionBlockSyntax ParseExpressionBlock(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            switch (tokens.Current)
            {
                case IEqualsGreaterThanToken equalsGreaterThan:
                    tokens.Next();
                    var expression = ParseExpression(tokens, diagnostics);
                    return new ResultExpressionSyntax(equalsGreaterThan, expression);
                case IOpenBraceToken _:
                default:
                    return ParseBlock(tokens, diagnostics);
            }
        }
    }
}
