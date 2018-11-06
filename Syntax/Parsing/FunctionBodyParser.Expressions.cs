using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;
using static Adamant.Tools.Compiler.Bootstrap.Framework.TypeOperations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public partial class FunctionBodyParser : IExpressionParser
    {
        [CanBeNull]
        public ExpressionSyntax AcceptExpression(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            switch (tokens.Current)
            {
                case CloseParenToken _:
                case CloseBracketToken _:
                case CloseBraceToken _:
                case SemicolonToken _:
                case CommaToken _:
                    return null;
                default:
                    return ParseExpression(tokens, diagnostics);
            }
        }

        [NotNull]
        [MustUseReturnValue]
        public ExpressionSyntax ParseExpression(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            return ParseExpression(tokens, diagnostics, OperatorPrecedence.Min);
        }

        [MustUseReturnValue]
        [NotNull]
        public ExpressionSyntax ParseExpression(
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
                    case QuestionQuestionToken _:
                        if (minPrecedence <= OperatorPrecedence.Coalesce)
                        {
                            precedence = OperatorPrecedence.Coalesce;
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
                    case LessThanColonToken _: // Subtype operator
                    case AsKeywordToken _:
                        if (minPrecedence <= OperatorPrecedence.Relational)
                        {
                            precedence = OperatorPrecedence.Relational;
                            @operator = tokens.TakeOperator();
                        }
                        break;
                    case ColonToken _: // type kind
                        if (minPrecedence <= OperatorPrecedence.Relational)
                        {
                            var colon = tokens.Take<ColonToken>();
                            var typeKind = tokens.Expect<ITypeKindKeywordToken>();
                            expression = new TypeKindExpressionSyntax(colon, typeKind);
                            continue;
                        }
                        break;
                    case DotDotToken _:
                    case LessThanDotDotToken _:
                    case DotDotLessThanToken _:
                    case LessThanDotDotLessThanToken _:
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
                    case DollarToken _:
                    case DollarLessThanToken _:
                    case DollarLessThanNotEqualToken _:
                    case DollarGreaterThanToken _:
                    case DollarGreaterThanNotEqualToken _:
                        if (minPrecedence <= OperatorPrecedence.Lifetime)
                        {
                            var leftOperand = expression;
                            var lifetimeOperator = tokens.Take<ILifetimeOperatorToken>();
                            var name = tokens.Expect<ILifetimeNameToken>();
                            expression = new LifetimeTypeSyntax(leftOperand, lifetimeOperator, name);
                            continue;
                        }
                        break;
                    case QuestionToken _:
                        if (minPrecedence <= OperatorPrecedence.Unary)
                        {
                            @operator = tokens.TakeOperator();
                            expression = new UnaryOperatorExpressionSyntax(@operator, expression);
                            continue;
                        }
                        break;
                    case OpenParenToken _:
                        if (minPrecedence <= OperatorPrecedence.Primary)
                        {
                            var callee = expression;
                            var openParen = tokens.Expect<IOpenParenToken>();
                            var arguments = ParseArgumentList(tokens, diagnostics);
                            var closeParen = tokens.Expect<ICloseParenToken>();
                            expression = new InvocationSyntax(callee, openParen, arguments, closeParen);
                            continue;
                        }
                        break;
                    case OpenBracketToken _:
                        if (minPrecedence <= OperatorPrecedence.Primary)
                        {
                            var callee = expression;
                            var openBracket = tokens.Expect<IOpenBracketToken>();
                            var arguments = ParseArgumentList(tokens, diagnostics);
                            var closeBracket = tokens.Expect<ICloseBracketToken>();
                            expression = new GenericsInvocationSyntax(callee, openBracket, arguments, closeBracket);
                            continue;
                        }
                        break;
                    case DotToken _:
                    case CaretDotToken _:
                    case QuestionDotToken _:
                        if (minPrecedence <= OperatorPrecedence.Primary)
                        {
                            // Member Access
                            var accessOperator = tokens.TakeOperator();
                            var member = tokens.Expect<IMemberNameToken>();
                            expression = new MemberAccessExpressionSyntax(expression, accessOperator, member);
                            continue;
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

                    var rightOperand = ParseExpression(tokens, diagnostics, operatorPrecedence);
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
                case SelfTypeKeywordToken selfTypeKeyword:
                    tokens.MoveNext();
                    return new SelfTypeExpressionSyntax(selfTypeKeyword);
                case UninitializedKeywordToken uninitializedKeyword:
                    tokens.MoveNext();
                    return new UninitializedExpressionSyntax(uninitializedKeyword);
                case NoneKeywordToken noneKeyword:
                    tokens.MoveNext();
                    return new NoneExpressionSyntax(noneKeyword);
                case SelfKeywordToken selfKeyword:
                    tokens.MoveNext();
                    return new SelfExpressionSyntax(selfKeyword);
                case BaseKeywordToken baseKeyword:
                    tokens.MoveNext();
                    return new BaseExpressionSyntax(baseKeyword);
                case NewKeywordToken _:
                    {
                        var newKeyword = tokens.Expect<INewKeywordToken>();
                        var type = ParseName(tokens, diagnostics);
                        var openParen = tokens.Expect<IOpenParenToken>();
                        var arguments = ParseArgumentList(tokens, diagnostics);
                        var closeParen = tokens.Expect<ICloseParenToken>();
                        return new NewObjectExpressionSyntax(newKeyword, type, openParen, arguments, closeParen);
                    }
                case InitKeywordToken _:
                    {
                        var initKeyword = tokens.Expect<IInitKeywordToken>();
                        var openParen = tokens.Expect<IOpenParenToken>();
                        var placeExpression = ParseExpression(tokens, diagnostics);
                        var closeParen = tokens.Expect<ICloseParenToken>();
                        var initializer = ParseName(tokens, diagnostics);
                        var argumentsOpenParen = tokens.Expect<IOpenParenToken>();
                        var arguments = ParseArgumentList(tokens, diagnostics);
                        var argumentsCloseParen = tokens.Expect<ICloseParenToken>();
                        return new PlacementInitExpressionSyntax(initKeyword, openParen, placeExpression,
                            closeParen, initializer, argumentsOpenParen, arguments, argumentsCloseParen);
                    }
                case DeleteKeywordToken deleteKeyword:
                    {
                        tokens.MoveNext();
                        var expression = ParseExpression(tokens, diagnostics);
                        return new DeleteExpressionSyntax(deleteKeyword, expression);
                    }
                case ReturnKeywordToken _:
                    {
                        var returnKeyword = tokens.Expect<IReturnKeywordToken>();
                        var expression = tokens.AtTerminator<SemicolonToken>() ? null : ParseExpression(tokens, diagnostics);
                        return new ReturnExpressionSyntax(returnKeyword, expression);
                    }
                case EqualsGreaterThanToken _:
                    return ParseExpressionBlock(tokens, diagnostics);
                case OpenParenToken _:
                    return ParseParenthesizedExpression(tokens, diagnostics);
                case MinusToken _:
                case PlusToken _:
                case AtSignToken _:
                case CaretToken _:
                case NotKeywordToken _:
                    {
                        var @operator = tokens.TakeOperator();
                        var operand = ParseExpression(tokens, diagnostics, OperatorPrecedence.Unary);
                        return new UnaryOperatorExpressionSyntax(@operator, operand);
                    }
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
                case AnyKeywordToken _:
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
                            var lifetime = tokens.Expect<ILifetimeNameToken>();
                            return new LifetimeTypeSyntax(name, dollar, lifetime);
                        }

                        return name;
                    }
                case ForeachKeywordToken _:
                    return ParseForeach(tokens, diagnostics);
                case WhileKeywordToken _:
                    return ParseWhile(tokens, diagnostics);
                case UnsafeKeywordToken unsafeKeyword:
                    {
                        tokens.MoveNext();
                        var expression = tokens.Current is OpenBraceToken ?
                            ParseBlock(tokens, diagnostics)
                            : ParseParenthesizedExpression(tokens, diagnostics);

                        return new UnsafeExpressionSyntax(unsafeKeyword, expression);
                    }
                case RefKeywordToken refKeyword:
                    {
                        tokens.MoveNext();
                        var varKeyword = tokens.Accept<VarKeywordToken>();
                        var referencedType = ParseExpression(tokens, diagnostics);
                        return new RefTypeSyntax(refKeyword, varKeyword, referencedType);
                    }
                case MutableKeywordToken mutableKeyword:
                    {
                        tokens.MoveNext();
                        var referencedType = ParseExpression(tokens, diagnostics);
                        return new MutableTypeSyntax(mutableKeyword, referencedType);
                    }
                case IfKeywordToken _:
                    return ParseIf(tokens, diagnostics);
                case MatchKeywordToken _:
                    return ParseMatch(tokens, diagnostics);
                case DotToken _:
                    {
                        // implicit self etc.
                        var @operator = tokens.TakeOperator();
                        var operand = ParseExpression(tokens, diagnostics, OperatorPrecedence.Unary);
                        return new UnaryOperatorExpressionSyntax(@operator, operand);
                    }
                case AsteriskToken _:
                case SlashToken _:
                case QuestionToken _:
                case SemicolonToken _:
                case CloseParenToken _:
                    // If it is one of these, we assume there is a missing identifier
                    return new IdentifierNameSyntax(tokens.ExpectIdentifier());
                default:
                    throw NonExhaustiveMatchException.For(tokens.Current);
            }
        }

        [MustUseReturnValue]
        [NotNull]
        private ExpressionSyntax ParseForeach(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var foreachKeyword = tokens.Expect<ForeachKeywordToken>();
            var varKeyword = tokens.Accept<VarKeywordToken>();
            var identifier = tokens.ExpectIdentifier();
            var colon = tokens.Accept<ColonToken>();
            ExpressionSyntax typeExpression = null;
            if (colon != null)
                typeExpression = ParseExpression(tokens, diagnostics);
            var inKeyword = tokens.Expect<InKeywordToken>();
            var expression = ParseExpression(tokens, diagnostics);
            var block = ParseBlock(tokens, diagnostics);
            return new ForeachExpressionSyntax(foreachKeyword, varKeyword, identifier,
                colon, typeExpression, inKeyword, expression, block);
        }


        [MustUseReturnValue]
        [NotNull]
        private WhileExpressionSyntax ParseWhile(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var whileKeyword = tokens.Expect<WhileKeywordToken>();
            var condition = ParseExpression(tokens, diagnostics);
            var block = ParseBlock(tokens, diagnostics);
            return new WhileExpressionSyntax(whileKeyword, condition, block);
        }

        [MustUseReturnValue]
        [NotNull]
        private ExpressionSyntax ParseIf(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var ifKeyword = tokens.Take<IfKeywordToken>();
            var condition = ParseExpression(tokens, diagnostics);
            var thenBlock = ParseExpressionBlock(tokens, diagnostics);
            var elseClause = AcceptElse(tokens, diagnostics);
            return new IfExpressionSyntax(ifKeyword, condition, thenBlock, elseClause);
        }

        [CanBeNull]
        private ElseClauseSyntax AcceptElse(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var elseKeyword = tokens.Accept<ElseKeywordToken>();
            if (elseKeyword == null) return null;
            var expression = tokens.Current is IfKeywordToken
                ? ParseIf(tokens, diagnostics)
                : ParseExpressionBlock(tokens, diagnostics);
            return new ElseClauseSyntax(elseKeyword, expression);
        }

        [MustUseReturnValue]
        [NotNull]
        private ExpressionSyntax ParseMatch(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var matchKeyword = tokens.Take<MatchKeywordToken>();
            var value = ParseExpression(tokens, diagnostics);
            var openBrace = tokens.Expect<IOpenBraceToken>();
            var arms = listParser.ParseList(tokens, ParseMatchArm, TypeOf<CloseBraceToken>(), diagnostics);
            var closeBrace = tokens.Expect<ICloseBraceToken>();
            return new MatchExpressionSyntax(matchKeyword, value, openBrace, arms, closeBrace);
        }

        [MustUseReturnValue]
        [NotNull]
        private MatchArmSyntax ParseMatchArm(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var pattern = ParsePattern(tokens, diagnostics);
            var expression = ParseExpressionBlock(tokens, diagnostics);
            var comma = tokens.Accept<CommaToken>();
            return new MatchArmSyntax(pattern, expression, comma);
        }

        [MustUseReturnValue]
        [NotNull]
        private ExpressionSyntax ParseParenthesizedExpression(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var openParen = tokens.Expect<IOpenParenToken>();
            var expression = ParseExpression(tokens, diagnostics);
            var closeParen = tokens.Expect<ICloseParenToken>();
            return new ParenthesizedExpressionSyntax(openParen, expression, closeParen);
        }

        [MustUseReturnValue]
        [NotNull]
        public SeparatedListSyntax<ArgumentSyntax> ParseArgumentList(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var arguments = listParser.ParseSeparatedList(tokens, AcceptArgument, TypeOf<CommaToken>(), diagnostics);
            return new SeparatedListSyntax<ArgumentSyntax>(arguments);
        }

        [MustUseReturnValue]
        [CanBeNull]
        private ArgumentSyntax AcceptArgument(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var paramsKeyword = tokens.Accept<ParamsKeywordToken>();
            var value = AcceptExpression(tokens, diagnostics);
            if (paramsKeyword == null && value == null) return null;
            return new ArgumentSyntax(paramsKeyword, value);
        }
    }
}
