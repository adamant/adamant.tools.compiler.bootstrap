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
                    return new ExpressionStatementSyntax(ParseBlock(tokens, diagnostics));
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
                    tokens.Expect<ISemicolonToken>();
                    return new ExpressionStatementSyntax(expression);
                }
            }
        }

        [MustUseReturnValue]
        [CanBeNull]
        public BlockSyntax AcceptBlock()
        {
            var openBrace = Tokens.Current.Span;
            if (!Tokens.Accept<IOpenBraceToken>()) return null;
            var statements = listParser.ParseList(Tokens, ParseStatement, TypeOf<ICloseBraceToken>(), Tokens.Context.Diagnostics);
            var closeBrace = Tokens.Expect<ICloseBraceToken>();
            openBrace = TextSpan.Covering(openBrace, closeBrace);
            return new BlockSyntax(openBrace, statements);
        }

        [MustUseReturnValue]
        [NotNull]
        public BlockSyntax ParseBlock(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var openBrace = tokens.Expect<IOpenBraceToken>();
            var statements = listParser.ParseList(tokens, ParseStatement, TypeOf<ICloseBraceToken>(), diagnostics);
            var closeBrace = tokens.Expect<ICloseBraceToken>();
            var span = TextSpan.Covering(openBrace, closeBrace);
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
