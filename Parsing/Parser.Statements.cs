using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class Parser
    {
        public StatementSyntax ParseStatement()
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
                    var expression = ParseExpression();
                    var semicolon = Tokens.Expect<ISemicolonToken>();
                    return new ExpressionStatementSyntax(
                        TextSpan.Covering(expression.Span, semicolon),
                        expression);
            }
        }

        // Requires the binding has already been consumed
        private StatementSyntax ParseRestOfVariableDeclaration(
            TextSpan binding,
            bool mutableBinding)
        {
            var identifier = Tokens.RequiredToken<IIdentifierToken>();
            var name = nameContext.Qualify(variableNumbers.VariableName(identifier.Value));
            TypeSyntax type = null;
            if (Tokens.Accept<IColonToken>())
                type = ParseType();

            ExpressionSyntax initializer = null;
            if (Tokens.Accept<IEqualsToken>())
                initializer = ParseExpression();

            var semicolon = Tokens.Expect<ISemicolonToken>();
            var span = TextSpan.Covering(binding, semicolon);
            return new VariableDeclarationStatementSyntax(span,
                mutableBinding, name, identifier.Span, type, initializer);
        }

        private ExpressionStatementSyntax ParseUnsafeStatement()
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

        public BlockSyntax AcceptBlock()
        {
            var openBrace = Tokens.AcceptToken<IOpenBraceToken>();
            if (openBrace == null)
                return null;
            return ParseRestOfBlock(openBrace.Span);
        }

        public BlockSyntax ParseBlock()
        {
            var openBrace = Tokens.Expect<IOpenBraceToken>();
            return ParseRestOfBlock(openBrace);
        }

        // Requires the open brace has already been consumed
        private BlockSyntax ParseRestOfBlock(TextSpan openBrace)
        {
            var statements = ParseMany<StatementSyntax, ICloseBraceToken>(ParseStatement);
            var closeBrace = Tokens.Expect<ICloseBraceToken>();
            var span = TextSpan.Covering(openBrace, closeBrace);
            return new BlockSyntax(span, statements);
        }

        public IBlockOrResultSyntax ParseBlockOrResult()
        {
            if (Tokens.Current is IOpenBraceToken)
                return ParseBlock();

            var equalsGreaterThan = Tokens.Expect<IEqualsGreaterThanToken>();
            var expression = ParseExpression();
            var span = TextSpan.Covering(equalsGreaterThan, expression.Span);
            return new ResultStatementSyntax(span, expression);
        }
    }
}
