using System;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Instance;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Literals;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Operators;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;
using static Adamant.Tools.Compiler.Bootstrap.Framework.TypeOperations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public class ExpressionParser : IExpressionParser
    {
        [NotNull] private readonly IListParser listParser;
        [NotNull] private readonly IParser<NameSyntax> qualifiedNameParser;
        [NotNull]
        private IParser<BlockExpressionSyntax> BlockParser
        {
            get
            {
                if (blockParser != null) return blockParser;

                blockParser = blockParserProvider.AssertNotNull()();
                blockParserProvider = null;
                return blockParser.AssertNotNull();
            }
        }
        [CanBeNull] private IParser<BlockExpressionSyntax> blockParser;
        [CanBeNull] private Func<IParser<BlockExpressionSyntax>> blockParserProvider;

        public ExpressionParser(
            [NotNull] IListParser listParser,
            [NotNull] IParser<NameSyntax> qualifiedNameParser,
            [NotNull] Func<IParser<BlockExpressionSyntax>> blockParserProvider)
        {
            this.listParser = listParser;
            this.qualifiedNameParser = qualifiedNameParser;
            this.blockParserProvider = blockParserProvider;
        }

        [NotNull]
        [MustUseReturnValue]
        public ExpressionSyntax Parse([NotNull] ITokenStream tokens, [NotNull] IDiagnosticsCollector diagnostics)
        {
            return Parse(tokens, diagnostics, OperatorPrecedence.Min);
        }

        [MustUseReturnValue]
        [NotNull]
        public ExpressionSyntax Parse(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics,
            OperatorPrecedence minPrecedence)
        {
            var expression = ParseAtom(tokens, diagnostics);

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
                            @operator = tokens.TakeOperator();
                        }
                        break;
                    case OrKeywordToken _:
                    case XorKeywordToken _:
                        if (minPrecedence <= OperatorPrecedence.LogicalOr)
                        {
                            precedence = OperatorPrecedence.LogicalOr;
                            @operator = tokens.TakeOperator();
                        }
                        break;
                    case AndKeywordToken _:
                        if (minPrecedence <= OperatorPrecedence.LogicalAnd)
                        {
                            precedence = OperatorPrecedence.LogicalAnd;
                            @operator = tokens.TakeOperator();
                        }
                        break;
                    case EqualsEqualsToken _:
                    case NotEqualToken _:
                        if (minPrecedence <= OperatorPrecedence.Equality)
                        {
                            precedence = OperatorPrecedence.Equality;
                            @operator = tokens.TakeOperator();
                        }
                        break;
                    case LessThanToken _:
                    case LessThanOrEqualToken _:
                    case GreaterThanToken _:
                    case GreaterThanOrEqualToken _:
                        if (minPrecedence <= OperatorPrecedence.Relational)
                        {
                            precedence = OperatorPrecedence.Relational;
                            @operator = tokens.TakeOperator();
                        }
                        break;
                    case DotDotToken _:
                        if (minPrecedence <= OperatorPrecedence.Range)
                        {
                            precedence = OperatorPrecedence.Range;
                            @operator = tokens.TakeOperator();
                        }
                        break;
                    case PlusToken _:
                    case MinusToken _:
                        if (minPrecedence <= OperatorPrecedence.Additive)
                        {
                            precedence = OperatorPrecedence.Additive;
                            @operator = tokens.TakeOperator();
                        }
                        break;
                    case AsteriskToken _:
                    case SlashToken _:
                        if (minPrecedence <= OperatorPrecedence.Multiplicative)
                        {
                            precedence = OperatorPrecedence.Multiplicative;
                            @operator = tokens.TakeOperator();
                        }
                        break;
                    case OpenParenToken _:
                        if (minPrecedence <= OperatorPrecedence.Primary)
                        {
                            var callee = expression;
                            var openParen = tokens.Expect<IOpenParenToken>();
                            var arguments = ParseArguments(tokens, diagnostics);
                            var closeParen = tokens.Expect<ICloseParenToken>();
                            expression = new InvocationSyntax(callee, openParen, arguments, closeParen);
                            continue;
                        }
                        break;
                    case DotToken _:
                        if (minPrecedence <= OperatorPrecedence.Primary)
                        {
                            // Member Access
                            precedence = OperatorPrecedence.Primary;
                            @operator = tokens.TakeOperator();
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

                    var rightOperand = Parse(tokens, diagnostics, operatorPrecedence);
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
        private ExpressionSyntax ParseAtom([NotNull] ITokenStream tokens, [NotNull] IDiagnosticsCollector diagnostics)
        {
            switch (tokens.Current)
            {
                case SelfKeywordToken selfKeyword:
                    return new SelfExpressionSyntax(selfKeyword);
                case BaseKeywordToken baseKeyword:
                    return new BaseExpressionSyntax(baseKeyword);
                case NewKeywordToken _:
                    {
                        var newKeyword = tokens.Expect<INewKeywordToken>();
                        var type = qualifiedNameParser.Parse(tokens, diagnostics);
                        var openParen = tokens.Expect<IOpenParenToken>();
                        var arguments = ParseArguments(tokens, diagnostics);
                        var closeParen = tokens.Expect<ICloseParenToken>();
                        return new NewObjectExpressionSyntax(newKeyword, type, openParen, arguments, closeParen);
                    }
                case ReturnKeywordToken _:
                    {
                        var returnKeyword = tokens.Expect<IReturnKeywordToken>();
                        var expression = tokens.AtTerminator<SemicolonToken>() ? null : Parse(tokens, diagnostics);
                        return new ReturnExpressionSyntax(returnKeyword, expression);
                    }
                case OpenParenToken _:
                    {
                        var openParen = tokens.Expect<IOpenParenToken>();
                        var expression = Parse(tokens, diagnostics);
                        var closeParen = tokens.Expect<ICloseParenToken>();
                        return new ParenthesizedExpressionSyntax(openParen, expression, closeParen);
                    }
                case MinusToken _:
                case PlusToken _:
                case AtSignToken _:
                case CaretToken _:
                case NotKeywordToken _:
                    var @operator = tokens.TakeOperator();
                    var operand = Parse(tokens, diagnostics, OperatorPrecedence.Unary);
                    return new UnaryOperatorExpressionSyntax(@operator, operand);
                case IntegerLiteralToken literal:
                    {
                        tokens.MoveNext();
                        return new IntegerLiteralExpressionSyntax(literal);
                    }
                case StringLiteralToken literal:
                    {
                        tokens.MoveNext();
                        return new StringLiteralExpressionSyntax(literal);
                    }
                case BooleanLiteralToken literal:
                    {
                        tokens.MoveNext();
                        return new BooleanLiteralExpressionSyntax(literal);
                    }
                case VoidKeywordToken _:
                case IntKeywordToken _:
                case UIntKeywordToken _:
                case BoolKeywordToken _:
                case ByteKeywordToken _:
                case StringKeywordToken _:
                case NeverKeywordToken _:
                case SizeKeywordToken _:
                case TypeKeywordToken _:
                    {
                        var keyword = tokens.Take<IPrimitiveTypeToken>();
                        return new PrimitiveTypeSyntax(keyword);
                    }
                case IdentifierToken _:
                    {
                        var identifier = tokens.ExpectIdentifier();
                        var name = new IdentifierNameSyntax(identifier);
                        if (tokens.Current is DollarToken)
                        {
                            var dollar = tokens.Take<DollarToken>();
                            var lifetime = tokens.Current is IdentifierToken
                                ? (IToken)tokens.ExpectIdentifier()
                                : tokens.Expect<IOwnedKeywordToken>();
                            return new LifetimeTypeSyntax(name, dollar, lifetime);
                        }

                        return name;
                    }
                case ForeachKeywordToken _:
                    {
                        var foreachKeyword = tokens.Expect<ForeachKeywordToken>();
                        var varKeyword = tokens.Accept<VarKeywordToken>();
                        var identifier = tokens.ExpectIdentifier();
                        var inKeyword = tokens.Expect<InKeywordToken>();
                        var expression = Parse(tokens, diagnostics);
                        var block = BlockParser.Parse(tokens, diagnostics);
                        return new ForeachExpressionSyntax(foreachKeyword, varKeyword, identifier, inKeyword, expression, block);
                    }
                case AsteriskToken _:
                case SlashToken _:
                case QuestionToken _:
                case SemicolonToken _:
                    // If it is one of these, we assume there is a missing identifier
                    return new IdentifierNameSyntax(tokens.ExpectIdentifier());
                default:
                    throw NonExhaustiveMatchException.For(tokens.Current);
            }
        }

        [MustUseReturnValue]
        [NotNull]
        private SeparatedListSyntax<ExpressionSyntax> ParseArguments([NotNull] ITokenStream tokens, [NotNull] IDiagnosticsCollector diagnostics)
        {
            var arguments = listParser.ParseSeparatedList(tokens, t => Parse(t, diagnostics), TypeOf<CommaToken>(), TypeOf<CloseParenToken>(), diagnostics);
            return new SeparatedListSyntax<ExpressionSyntax>(arguments);
        }
    }
}
