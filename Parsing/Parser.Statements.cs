using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class Parser
    {
        [MustUseReturnValue]
        [NotNull]
        public StatementSyntax ParseStatement()
        {
            switch (Tokens.Current)
            {
                // TODO handle cases like if, loop, foreach etc.
                case IOpenBraceToken _:
                    return ParseBlock();
                case ILetKeywordToken _:
                {
                    Tokens.Expect<IBindingToken>();
                    return ParseRestOfVariableDeclaration(false);
                }
                case IVarKeywordToken _:
                {
                    Tokens.Expect<IBindingToken>();
                    return ParseRestOfVariableDeclaration(true);
                }
                default:
                {
                    var expression = ParseExpression();
                    Tokens.Expect<ISemicolonToken>();
                    return expression;
                }
            }
        }

        // Requires the binding has already been consumed
        [MustUseReturnValue]
        [NotNull]
        private StatementSyntax ParseRestOfVariableDeclaration(bool mutableBinding)
        {
            var name = Tokens.RequiredToken<IIdentifierToken>();
            ExpressionSyntax type = null;
            if (Tokens.Accept<IColonToken>())
                // Need to not consume the assignment that separates the type from the initializer,
                // hence the min operator precedence.
                type = ParseExpression(OperatorPrecedence.LogicalOr);

            ExpressionSyntax initializer = null;
            if (Tokens.Accept<IEqualsToken>()) initializer = ParseExpression();

            Tokens.Expect<ISemicolonToken>();
            return new VariableDeclarationStatementSyntax(mutableBinding, name.Value, name.Span, type,
                initializer);
        }

        [MustUseReturnValue]
        [CanBeNull]
        public BlockSyntax AcceptBlock()
        {
            var openBrace = Tokens.Current.Span;
            if (!Tokens.Accept<IOpenBraceToken>()) return null;
            return ParseRestOfBlock(openBrace);
        }

        [MustUseReturnValue]
        [NotNull]
        public BlockSyntax ParseBlock()
        {
            var openBrace = Tokens.Expect<IOpenBraceToken>();
            return ParseRestOfBlock(openBrace);
        }

        // Requires the open brace has already been consumed
        [NotNull]
        private BlockSyntax ParseRestOfBlock(TextSpan openBrace)
        {
            var statements = ParseMany<StatementSyntax, ICloseBraceToken>(ParseStatement);
            var closeBrace = Tokens.Expect<ICloseBraceToken>();
            openBrace = TextSpan.Covering(openBrace, closeBrace);
            return new BlockSyntax(openBrace, statements);
        }

        [MustUseReturnValue]
        [NotNull]
        public ExpressionBlockSyntax ParseExpressionBlock()
        {
            switch (Tokens.Current)
            {
                case IOpenBraceToken _:
                    return ParseBlock();
                case IEqualsGreaterThanToken _:
                default:
                    var equalsGreaterThan = Tokens.Expect<IEqualsGreaterThanToken>();
                    var expression = ParseExpression();
                    var span = TextSpan.Covering(equalsGreaterThan, expression.Span);
                    return new ResultExpressionSyntax(span, expression);
            }
        }
    }
}
