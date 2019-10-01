using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Parsing.Tree;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class Parser
    {
        public IStatementSyntax ParseStatement()
        {
            switch (Tokens.Current)
            {
                case IOpenBraceToken _:
                    var block = ParseBlock();
                    return new ExpressionStatementSyntax(block.Span, block);
                case ILetKeywordToken _:
                    var let = Tokens.Expect<IBindingToken>();
                    return ParseRestOfVariableDeclaration(let, false);
                case IVarKeywordToken _:
                    var @var = Tokens.Expect<IBindingToken>();
                    return ParseRestOfVariableDeclaration(@var, true);
                case IForeachKeywordToken _:
                    var @foreach = ParseForeach();
                    return new ExpressionStatementSyntax(@foreach.Span, @foreach);
                case IWhileKeywordToken _:
                    var @while = ParseWhile();
                    return new ExpressionStatementSyntax(@while.Span, @while);
                case ILoopKeywordToken _:
                    var loop = ParseLoop();
                    return new ExpressionStatementSyntax(loop.Span, loop);
                case IIfKeywordToken _:
                    var @if = ParseIf(ParseAs.Statement);
                    return new ExpressionStatementSyntax(@if.Span, @if);
                case IUnsafeKeywordToken _:
                    return ParseUnsafeStatement();
                default:
                    try
                    {
                        var expression = ParseExpression();
                        var semicolon = Tokens.Expect<ISemicolonToken>();
                        return new ExpressionStatementSyntax(
                            TextSpan.Covering(expression.Span, semicolon), expression);
                    }
                    catch (ParseFailedException)
                    {
                        SkipToEndOfStatement();
                        throw;
                    }
            }
        }

        // Requires the binding has already been consumed
        private IStatementSyntax ParseRestOfVariableDeclaration(
            TextSpan binding,
            bool mutableBinding)
        {
            var identifier = Tokens.RequiredToken<IIdentifierToken>();
            var name = nameContext.Qualify(variableNumbers.VariableName(identifier.Value));
            ITypeSyntax type = null;
            if (Tokens.Accept<IColonToken>())
                type = ParseType();

            IExpressionSyntax initializer = null;
            if (Tokens.Accept<IEqualsToken>())
                initializer = ParseExpression();

            var semicolon = Tokens.Expect<ISemicolonToken>();
            var span = TextSpan.Covering(binding, semicolon);
            return new VariableDeclarationStatementSyntax(span,
                mutableBinding, name, identifier.Span, type, initializer);
        }

        private IExpressionStatementSyntax ParseUnsafeStatement()
        {
            var unsafeKeyword = Tokens.Expect<IUnsafeKeywordToken>();
            var isBlock = Tokens.Current is IOpenBraceToken;
            var expression = isBlock ? ParseBlock() : ParseParenthesizedExpression();
            var span = TextSpan.Covering(unsafeKeyword, expression.Span);
            var unsafeExpression = new UnsafeExpressionSyntax(span, expression);
            if (!isBlock)
            {
                var semicolon = Tokens.Expect<ISemicolonToken>();
                span = TextSpan.Covering(span, semicolon);
            }
            return new ExpressionStatementSyntax(span, unsafeExpression);
        }

        public IBlockExpressionSyntax ParseBlock()
        {
            var openBrace = Tokens.Expect<IOpenBraceToken>();
            var statements = ParseMany<IStatementSyntax, ICloseBraceToken>(ParseStatement);
            var closeBrace = Tokens.Expect<ICloseBraceToken>();
            var span = TextSpan.Covering(openBrace, closeBrace);
            return new BlockExpressionSyntax(span, statements);
        }

        /// <summary>
        /// Skip tokens until we reach what we assume to be the end of a statement
        /// </summary>
        private void SkipToEndOfStatement()
        {
            while (!Tokens.AtEnd<ISemicolonToken>())
                Tokens.Next();

            // Consume the semicolon if we aren't at the end of the file.
            var _ = Tokens.Accept<ISemicolonToken>();
        }
    }
}
