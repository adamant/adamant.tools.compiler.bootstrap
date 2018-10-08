using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Literals;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Operators;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public class ExpressionParser : IParser<ExpressionSyntax>
    {
        private readonly IListParser listParser;
        private readonly IParser<NameSyntax> qualifiedNameParser;

        public ExpressionParser(IListParser listParser, IParser<NameSyntax> qualifiedNameParser)
        {
            this.listParser = listParser;
            this.qualifiedNameParser = qualifiedNameParser;
        }

        public ExpressionSyntax Parse(ITokenStream tokens)
        {
            return ParseExpression(tokens);
        }

        [MustUseReturnValue]
        private ExpressionSyntax ParseExpression(ITokenStream tokens, OperatorPrecedence minPrecedence = OperatorPrecedence.Min)
        {
            var expression = ParseAtom(tokens);

            for (; ; )
            {
                SimpleToken? @operator = null;
                OperatorPrecedence? precedence = null;
                var leftAssociative = true;
                switch (tokens.Current?.Kind)
                {
                    case TokenKind.Equals:
                    case TokenKind.PlusEquals:
                    case TokenKind.MinusEquals:
                    case TokenKind.AsteriskEquals:
                    case TokenKind.SlashEquals:
                        if (minPrecedence <= OperatorPrecedence.Assignment)
                        {
                            precedence = OperatorPrecedence.Assignment;
                            leftAssociative = false;
                            @operator = tokens.ExpectSimple();
                        }
                        break;
                    case TokenKind.OrKeyword:
                    case TokenKind.XorKeyword:
                        if (minPrecedence <= OperatorPrecedence.LogicalOr)
                        {
                            precedence = OperatorPrecedence.LogicalOr;
                            @operator = tokens.ExpectSimple();
                        }
                        break;
                    case TokenKind.AndKeyword:
                        if (minPrecedence <= OperatorPrecedence.LogicalAnd)
                        {
                            precedence = OperatorPrecedence.LogicalAnd;
                            @operator = tokens.ExpectSimple();
                        }
                        break;
                    case TokenKind.EqualsEquals:
                    case TokenKind.NotEqual:
                        if (minPrecedence <= OperatorPrecedence.Equality)
                        {
                            precedence = OperatorPrecedence.Equality;
                            @operator = tokens.ExpectSimple();
                        }
                        break;
                    case TokenKind.LessThan:
                    case TokenKind.LessThanOrEqual:
                    case TokenKind.GreaterThan:
                    case TokenKind.GreaterThanOrEqual:
                        if (minPrecedence <= OperatorPrecedence.Relational)
                        {
                            precedence = OperatorPrecedence.Relational;
                            @operator = tokens.ExpectSimple();
                        }
                        break;
                    case TokenKind.DotDot:
                        if (minPrecedence <= OperatorPrecedence.Range)
                        {
                            precedence = OperatorPrecedence.Range;
                            @operator = tokens.ExpectSimple();
                        }
                        break;
                    case TokenKind.Plus:
                    case TokenKind.Minus:
                        if (minPrecedence <= OperatorPrecedence.Additive)
                        {
                            precedence = OperatorPrecedence.Additive;
                            @operator = tokens.ExpectSimple();
                        }
                        break;
                    case TokenKind.Asterisk:
                    case TokenKind.Slash:
                        if (minPrecedence <= OperatorPrecedence.Multiplicative)
                        {
                            precedence = OperatorPrecedence.Multiplicative;
                            @operator = tokens.ExpectSimple();
                        }
                        break;
                    case TokenKind.OpenParen:
                        if (minPrecedence <= OperatorPrecedence.Primary)
                        {
                            var callee = expression;
                            var openParen = tokens.ExpectSimple(TokenKind.OpenParen);
                            var arguments = ParseArguments(tokens);
                            var closeParen = tokens.ExpectSimple(TokenKind.CloseParen);
                            expression = new InvocationSyntax(callee, openParen, arguments, closeParen);
                            continue;
                        }
                        break;
                    case TokenKind.Dot:
                        if (minPrecedence <= OperatorPrecedence.Primary)
                        {
                            // Member Access
                            precedence = OperatorPrecedence.Primary;
                            @operator = tokens.ExpectSimple();
                        }
                        break;
                    default:
                        return expression;
                }

                if (@operator is SimpleToken @operatorToken &&
                    precedence is OperatorPrecedence operatorPrecedence)
                {
                    if (leftAssociative)
                        operatorPrecedence += 1;

                    var rightOperand = ParseExpression(tokens, operatorPrecedence);
                    expression = new BinaryOperatorExpressionSyntax(expression, @operatorToken, rightOperand);
                }
                else
                {
                    // if we didn't match any operator
                    return expression;
                }
            }
        }

        // An atom is the unit of an expression that occurs between infix operators, i.e. an identifier, literal, group, or new
        [MustUseReturnValue]
        private ExpressionSyntax ParseAtom(ITokenStream tokens)
        {
            switch (tokens.Current?.Kind)
            {
                case TokenKind.NewKeyword:
                    {
                        var newKeyword = tokens.ExpectSimple(TokenKind.NewKeyword);
                        var type = qualifiedNameParser.Parse(tokens);
                        var openParen = tokens.ExpectSimple(TokenKind.OpenParen);
                        var arguments = ParseArguments(tokens);
                        var closeParen = tokens.ExpectSimple(TokenKind.CloseParen);
                        return new NewObjectExpressionSyntax(newKeyword, type, openParen, arguments,
                            closeParen);
                    }
                case TokenKind.ReturnKeyword:
                    {
                        var returnKeyword = tokens.ExpectSimple(TokenKind.ReturnKeyword);
                        var expression = tokens.CurrentIs(TokenKind.Semicolon) ? null : ParseExpression(tokens);
                        return new ReturnExpressionSyntax(returnKeyword, expression);
                    }
                case TokenKind.OpenParen:
                    {
                        var openParen = tokens.ExpectSimple(TokenKind.OpenParen);
                        var expression = ParseExpression(tokens);
                        var closeParen = tokens.ExpectSimple(TokenKind.CloseParen);
                        return new ParenthesizedExpressionSyntax(openParen, expression, closeParen);
                    }
                case TokenKind.Minus:
                case TokenKind.Plus:
                case TokenKind.AtSign:
                case TokenKind.Caret:
                    var @operator = tokens.ExpectSimple();
                    var operand = ParseExpression(tokens, OperatorPrecedence.Unary);
                    return new UnaryOperatorExpressionSyntax(@operator, operand);
                case TokenKind.StringLiteral:
                    return new StringLiteralExpressionSyntax(tokens.ExpectStringLiteral());
                case TokenKind.VoidKeyword:
                case TokenKind.IntKeyword:
                case TokenKind.UIntKeyword:
                case TokenKind.BoolKeyword:
                case TokenKind.ByteKeyword:
                case TokenKind.StringKeyword:
                    {
                        var keyword = tokens.ExpectSimple();
                        return new PrimitiveTypeSyntax(keyword);
                    }
                case TokenKind.Identifier:
                    {
                        var identifier = tokens.ExpectIdentifier();
                        var name = new IdentifierNameSyntax(identifier);
                        if (!tokens.CurrentIs(TokenKind.Dollar)) return name;

                        var dollar = tokens.ExpectSimple(TokenKind.Dollar);
                        var lifetime = (tokens.Current?.Kind.IsIdentifier() ?? false)
                            ? (Token)tokens.ExpectIdentifier()
                            : tokens.ExpectSimple(TokenKind.OwnedKeyword);
                        return new LifetimeTypeSyntax(name, dollar, lifetime);
                    }
                default:// If it is something else, we assume it should be an identifier name
                    return new IdentifierNameSyntax(tokens.ExpectIdentifier());
            }
        }

        [MustUseReturnValue]
        private SeparatedListSyntax<ExpressionSyntax> ParseArguments(ITokenStream tokens)
        {
            // What if there isn't a current token?
            var arguments = listParser.ParseSeparatedList(tokens, t => ParseExpression(t), TokenKind.Comma, TokenKind.CloseParen);
            return new SeparatedListSyntax<ExpressionSyntax>(arguments);
        }
    }
}
