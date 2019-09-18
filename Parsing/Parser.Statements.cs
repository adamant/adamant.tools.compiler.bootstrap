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
                case IForeachKeywordToken _:
                    return ParseForeach();
                case IWhileKeywordToken _:
                    return ParseWhile();
                case ILoopKeywordToken _:
                    return ParseLoop();
                case IIfKeywordToken _:
                    return ParseIf(ParseAs.Statement);
                //case IMatchKeywordToken _:
                //    return ParseMatch();
                case IUnsafeKeywordToken _:
                    return ParseUnsafeExpression(ParseAs.Statement);
                default:
                {
                    var expression = ParseExpression();
                    Tokens.Expect<ISemicolonToken>();
                    return expression;
                }
            }
        }

        // Requires the binding has already been consumed
        private StatementSyntax ParseRestOfVariableDeclaration(bool mutableBinding)
        {
            var identifier = Tokens.RequiredToken<IIdentifierToken>();
            var name = nameContext.Qualify(variableNumbers.VariableName(identifier.Value));
            ExpressionSyntax type = null;
            if (Tokens.Accept<IColonToken>())
                // Need to not consume the assignment that separates the type from the initializer,
                // hence the min operator precedence.
                type = ParseExpression(OperatorPrecedence.AboveAssignment);

            ExpressionSyntax initializer = null;
            if (Tokens.Accept<IEqualsToken>())
                initializer = ParseExpression();

            Tokens.Expect<ISemicolonToken>();
            return new VariableDeclarationStatementSyntax(mutableBinding, name, identifier.Span, type,
                initializer);
        }

        public BlockSyntax AcceptBlock()
        {
            var openBrace = Tokens.Current.Span;
            if (!Tokens.Accept<IOpenBraceToken>())
                return null;
            return ParseRestOfBlock(openBrace);
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
