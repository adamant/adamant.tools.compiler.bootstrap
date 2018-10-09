using Adamant.Tools.Compiler.Bootstrap.Framework;
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
        [NotNull]
        private readonly IListParser listParser;

        [NotNull]
        private readonly IParser<NameSyntax> qualifiedNameParser;

        public ExpressionParser([NotNull] IListParser listParser, [NotNull] IParser<NameSyntax> qualifiedNameParser)
        {
            this.listParser = listParser;
            this.qualifiedNameParser = qualifiedNameParser;
        }

        [NotNull]
        public ExpressionSyntax Parse([NotNull] ITokenStream tokens)
        {
            return ParseExpression(tokens);
        }

        [MustUseReturnValue]
        [NotNull]
        private ExpressionSyntax ParseExpression([NotNull] ITokenStream tokens, OperatorPrecedence minPrecedence = OperatorPrecedence.Min)
        {
            var expression = ParseAtom(tokens);

            for (; ; )
            {
                OperatorToken @operator = null;
                OperatorPrecedence? precedence = null;
                var leftAssociative = true;
                switch (tokens.Current)
                {
                    case EqualsToken _:
                    case PlusEqualsToken _:
                    case MinusEqualsToken _:
                    case AsteriskEqualsToken _:
                    case SlashEqualsToken _:
                        if (minPrecedence <= OperatorPrecedence.Assignment)
                        {
                            precedence = OperatorPrecedence.Assignment;
                            leftAssociative = false;
                            @operator = tokens.ExpectOperator();
                        }
                        break;
                    case OrKeywordToken _:
                    case XorKeywordToken _:
                        if (minPrecedence <= OperatorPrecedence.LogicalOr)
                        {
                            precedence = OperatorPrecedence.LogicalOr;
                            @operator = tokens.ExpectOperator();
                        }
                        break;
                    case AndKeywordToken _:
                        if (minPrecedence <= OperatorPrecedence.LogicalAnd)
                        {
                            precedence = OperatorPrecedence.LogicalAnd;
                            @operator = tokens.ExpectOperator();
                        }
                        break;
                    case EqualsEqualsToken _:
                    case NotEqualToken _:
                        if (minPrecedence <= OperatorPrecedence.Equality)
                        {
                            precedence = OperatorPrecedence.Equality;
                            @operator = tokens.ExpectOperator();
                        }
                        break;
                    case LessThanToken _:
                    case LessThanOrEqualToken _:
                    case GreaterThanToken _:
                    case GreaterThanOrEqualToken _:
                        if (minPrecedence <= OperatorPrecedence.Relational)
                        {
                            precedence = OperatorPrecedence.Relational;
                            @operator = tokens.ExpectOperator();
                        }
                        break;
                    case DotDotToken _:
                        if (minPrecedence <= OperatorPrecedence.Range)
                        {
                            precedence = OperatorPrecedence.Range;
                            @operator = tokens.ExpectOperator();
                        }
                        break;
                    case PlusToken _:
                    case MinusToken _:
                        if (minPrecedence <= OperatorPrecedence.Additive)
                        {
                            precedence = OperatorPrecedence.Additive;
                            @operator = tokens.ExpectOperator();
                        }
                        break;
                    case AsteriskToken _:
                    case SlashToken _:
                        if (minPrecedence <= OperatorPrecedence.Multiplicative)
                        {
                            precedence = OperatorPrecedence.Multiplicative;
                            @operator = tokens.ExpectOperator();
                        }
                        break;
                    case OpenParenToken _:
                        if (minPrecedence <= OperatorPrecedence.Primary)
                        {
                            var callee = expression;
                            var openParen = tokens.Expect<OpenParenToken>();
                            var arguments = ParseArguments(tokens);
                            var closeParen = tokens.Expect<CloseParenToken>();
                            expression = new InvocationSyntax(callee, openParen, arguments, closeParen);
                            continue;
                        }
                        break;
                    case DotToken _:
                        if (minPrecedence <= OperatorPrecedence.Primary)
                        {
                            // Member Access
                            precedence = OperatorPrecedence.Primary;
                            @operator = tokens.ExpectOperator();
                        }
                        break;
                    default:
                        return expression;
                }

                if (@operator is OperatorToken @operatorToken &&
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
        [NotNull]
        private ExpressionSyntax ParseAtom([NotNull] ITokenStream tokens)
        {
            switch (tokens.Current)
            {
                case NewKeywordToken _:
                    {
                        var newKeyword = tokens.Expect<NewKeywordToken>();
                        var type = qualifiedNameParser.Parse(tokens);
                        var openParen = tokens.Expect<OpenParenToken>();
                        var arguments = ParseArguments(tokens);
                        var closeParen = tokens.Expect<CloseParenToken>();
                        return new NewObjectExpressionSyntax(newKeyword, type, openParen, arguments,
                            closeParen);
                    }
                case ReturnKeywordToken _:
                    {
                        var returnKeyword = tokens.Expect<ReturnKeywordToken>();
                        var expression = tokens.AtTerminator<SemicolonToken>() ? null : ParseExpression(tokens);
                        return new ReturnExpressionSyntax(returnKeyword, expression);
                    }
                case OpenParenToken _:
                    {
                        var openParen = tokens.Expect<OpenParenToken>();
                        var expression = ParseExpression(tokens);
                        var closeParen = tokens.Expect<CloseParenToken>();
                        return new ParenthesizedExpressionSyntax(openParen, expression, closeParen);
                    }
                case MinusToken _:
                case PlusToken _:
                case AtSignToken _:
                case CaretToken _:
                    var @operator = tokens.ExpectOperator().AssertNotNull();
                    var operand = ParseExpression(tokens, OperatorPrecedence.Unary);
                    return new UnaryOperatorExpressionSyntax(@operator, operand);
                case StringLiteralToken _:
                    return new StringLiteralExpressionSyntax(tokens.ExpectStringLiteral());
                case VoidKeywordToken _:
                case IntKeywordToken _:
                case UIntKeywordToken _:
                case BoolKeywordToken _:
                case ByteKeywordToken _:
                case StringKeywordToken _:
                    {
                        var keyword = tokens.ExpectKeyword().AssertNotNull();
                        return new PrimitiveTypeSyntax(keyword);
                    }
                case IdentifierToken _:
                    {
                        var identifier = tokens.ExpectIdentifier();
                        var name = new IdentifierNameSyntax(identifier);
                        if (tokens.Current is DollarToken)
                        {
                            var dollar = tokens.Expect<DollarToken>();
                            var lifetime = tokens.Current is IdentifierToken
                                ? (Token)tokens.ExpectIdentifier()
                                : tokens.Expect<OwnedKeywordToken>();
                            return new LifetimeTypeSyntax(name, dollar, lifetime);
                        }

                        return name;
                    }
                default:// If it is something else, we assume it should be an identifier name
                    return new IdentifierNameSyntax(tokens.ExpectIdentifier());
            }
        }

        [MustUseReturnValue]
        [NotNull]
        private SeparatedListSyntax<ExpressionSyntax> ParseArguments([NotNull] ITokenStream tokens)
        {
            var arguments = listParser.ParseSeparatedList(tokens, t => ParseExpression(t), TypeOf<CommaToken>._, TypeOf<CloseParenToken>._);
            return new SeparatedListSyntax<ExpressionSyntax>(arguments);
        }
    }
}