using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;
using static Adamant.Tools.Compiler.Bootstrap.Framework.TypeOperations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class FunctionBodyParser : IExpressionParser
    {
        [CanBeNull]
        public ExpressionSyntax AcceptExpression(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            switch (tokens.Current)
            {
                case ICloseParenToken _:
                case ICloseBracketToken _:
                case ICloseBraceToken _:
                case ISemicolonToken _:
                case ICommaToken _:
                    return null;
                default:
                    return ParseExpression(tokens, diagnostics);
            }
        }

        [NotNull]
        [MustUseReturnValue]
        public ExpressionSyntax ParseExpression(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            return ParseExpression(tokens, diagnostics, OperatorPrecedence.Min);
        }

        [MustUseReturnValue]
        [NotNull]
        public ExpressionSyntax ParseExpression(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics,
            OperatorPrecedence minPrecedence)
        {
            var expression = ParseAtom(tokens, diagnostics);

            for (; ; )
            {
                IOperatorToken @operator = null;
                OperatorPrecedence? precedence = null;
                var leftAssociative = true;
                switch (tokens.Current)
                {
                    case IEqualsToken _:
                    case IPlusEqualsToken _:
                    case IMinusEqualsToken _:
                    case IAsteriskEqualsToken _:
                    case ISlashEqualsToken _:
                        if (minPrecedence <= OperatorPrecedence.Assignment)
                        {
                            precedence = OperatorPrecedence.Assignment;
                            leftAssociative = false;
                            @operator = tokens.RequiredToken<IOperatorToken>();
                        }
                        break;
                    case IQuestionQuestionToken _:
                        if (minPrecedence <= OperatorPrecedence.Coalesce)
                        {
                            precedence = OperatorPrecedence.Coalesce;
                            @operator = tokens.RequiredToken<IOperatorToken>();
                        }
                        break;
                    case IOrKeywordToken _:
                    case IXorKeywordToken _:
                        if (minPrecedence <= OperatorPrecedence.LogicalOr)
                        {
                            precedence = OperatorPrecedence.LogicalOr;
                            @operator = tokens.RequiredToken<IOperatorToken>();
                        }
                        break;
                    case IAndKeywordToken _:
                        if (minPrecedence <= OperatorPrecedence.LogicalAnd)
                        {
                            precedence = OperatorPrecedence.LogicalAnd;
                            @operator = tokens.RequiredToken<IOperatorToken>();
                        }
                        break;
                    case IEqualsEqualsToken _:
                    case INotEqualToken _:
                        if (minPrecedence <= OperatorPrecedence.Equality)
                        {
                            precedence = OperatorPrecedence.Equality;
                            @operator = tokens.RequiredToken<IOperatorToken>();
                        }
                        break;
                    case ILessThanToken _:
                    case ILessThanOrEqualToken _:
                    case IGreaterThanToken _:
                    case IGreaterThanOrEqualToken _:
                    case ILessThanColonToken _: // Subtype operator
                    case IAsKeywordToken _:
                        if (minPrecedence <= OperatorPrecedence.Relational)
                        {
                            precedence = OperatorPrecedence.Relational;
                            @operator = tokens.RequiredToken<IOperatorToken>();
                        }
                        break;
                    case IColonToken _: // type kind
                        if (minPrecedence <= OperatorPrecedence.Relational)
                        {
                            var colon = tokens.RequiredToken<IColonToken>();
                            var typeKind = tokens.Consume<ITypeKindKeywordTokenPlace>();
                            expression = new TypeKindExpressionSyntax(colon, typeKind);
                            continue;
                        }
                        break;
                    case IDotDotToken _:
                    case ILessThanDotDotToken _:
                    case IDotDotLessThanToken _:
                    case ILessThanDotDotLessThanToken _:
                        if (minPrecedence <= OperatorPrecedence.Range)
                        {
                            precedence = OperatorPrecedence.Range;
                            @operator = tokens.RequiredToken<IOperatorToken>();
                        }
                        break;
                    case IPlusToken _:
                    case IMinusToken _:
                        if (minPrecedence <= OperatorPrecedence.Additive)
                        {
                            precedence = OperatorPrecedence.Additive;
                            @operator = tokens.RequiredToken<IOperatorToken>();
                        }
                        break;
                    case IAsteriskToken _:
                    case ISlashToken _:
                        if (minPrecedence <= OperatorPrecedence.Multiplicative)
                        {
                            precedence = OperatorPrecedence.Multiplicative;
                            @operator = tokens.RequiredToken<IOperatorToken>();
                        }
                        break;
                    case IDollarToken _:
                    case IDollarLessThanToken _:
                    case IDollarLessThanNotEqualToken _:
                    case IDollarGreaterThanToken _:
                    case IDollarGreaterThanNotEqualToken _:
                        if (minPrecedence <= OperatorPrecedence.Lifetime)
                        {
                            var leftOperand = expression;
                            var lifetimeOperator = tokens.Take<ILifetimeOperatorToken>();
                            var name = tokens.Consume<ILifetimeNameTokenPlace>();
                            expression = new LifetimeTypeSyntax(leftOperand, lifetimeOperator, name);
                            continue;
                        }
                        break;
                    case IQuestionToken _:
                        if (minPrecedence <= OperatorPrecedence.Unary)
                        {
                            @operator = tokens.TakeOperator();
                            expression = new UnaryExpressionSyntax(@operator, expression);
                            continue;
                        }
                        break;
                    case IOpenParenToken _:
                        if (minPrecedence <= OperatorPrecedence.Primary)
                        {
                            var callee = expression;
                            tokens.Expect<IOpenParenToken>();
                            var arguments = ParseArguments(tokens, diagnostics);
                            var closeParenSpan = tokens.Expect<ICloseParenToken>();
                            var span = TextSpan.Covering(callee.Span, closeParenSpan);
                            expression = new InvocationSyntax(span, callee, arguments);
                            continue;
                        }
                        break;
                    case IOpenBracketToken _:
                        if (minPrecedence <= OperatorPrecedence.Primary)
                        {
                            var callee = expression;
                            var openBracket = tokens.Consume<IOpenBracketTokenPlace>();
                            var arguments = ParseArgumentList(tokens, diagnostics);
                            var closeBracket = tokens.Consume<ICloseBracketTokenPlace>();
                            expression = new GenericsInvocationSyntax(callee, openBracket, arguments, closeBracket);
                            continue;
                        }
                        break;
                    case IDotToken _:
                    case ICaretDotToken _:
                    case IQuestionDotToken _:
                        if (minPrecedence <= OperatorPrecedence.Primary)
                        {
                            // Member Access
                            var accessOperator = BuildAccessOperator(tokens.RequiredToken<IAccessOperatorToken>());
                            var member = tokens.RequiredToken<IMemberNameToken>();
                            expression = new MemberAccessExpressionSyntax(expression, accessOperator, member);
                            continue;
                        }
                        break;
                    default:
                        return expression;
                }

                if (@operator is IOperatorToken operatorToken &&
                    precedence is OperatorPrecedence operatorPrecedence)
                {
                    if (leftAssociative)
                        operatorPrecedence += 1;

                    var rightOperand = ParseExpression(tokens, diagnostics, operatorPrecedence);
                    BuildOperatorExpression(expression, operatorToken, rightOperand);
                }
                else
                {
                    // if we didn't match any operator
                    return expression;
                }
            }
        }

        private static AccessOperator BuildAccessOperator([NotNull] IAccessOperatorToken accessOperatorToken)
        {
            switch (accessOperatorToken)
            {
                case IDotToken _:
                    return AccessOperator.Standard;
                case IQuestionDotToken _:
                    return AccessOperator.Conditional;
                case ICaretDotToken _:
                    return AccessOperator.Dereference;
                default:
                    throw NonExhaustiveMatchException.For(accessOperatorToken);
            }
        }

        [NotNull]
        private static ExpressionSyntax BuildOperatorExpression(
            [NotNull] ExpressionSyntax left,
            [NotNull] IOperatorToken operatorToken,
            [NotNull] ExpressionSyntax right)
        {
            BinaryOperator binaryOperator;
            switch (operatorToken)
            {
                case IEqualsToken _:
                    return new AssignmentExpressionSyntax(left, AssignmentOperation.Direct, right);
                case IPlusEqualsToken _:
                    return new AssignmentExpressionSyntax(left, AssignmentOperation.Plus, right);
                case IMinusEqualsToken _:
                    return new AssignmentExpressionSyntax(left, AssignmentOperation.Minus, right);
                case IAsteriskEqualsToken _:
                    return new AssignmentExpressionSyntax(left, AssignmentOperation.Asterisk, right);
                case ISlashEqualsToken _:
                    return new AssignmentExpressionSyntax(left, AssignmentOperation.Slash, right);
                case IPlusToken _:
                    binaryOperator = BinaryOperator.Plus;
                    break;
                case IEqualsEqualsToken _:
                    binaryOperator = BinaryOperator.EqualsEquals;
                    break;
                case INotEqualToken _:
                    binaryOperator = BinaryOperator.NotEqual;
                    break;
                case ILessThanToken _:
                    binaryOperator = BinaryOperator.LessThan;
                    break;
                case ILessThanOrEqualToken _:
                    binaryOperator = BinaryOperator.LessThanOrEqual;
                    break;
                case IGreaterThanToken _:
                    binaryOperator = BinaryOperator.GreaterThan;
                    break;
                case IGreaterThanOrEqualToken _:
                    binaryOperator = BinaryOperator.GreaterThanOrEqual;
                    break;
                case IAndKeywordToken _:
                    binaryOperator = BinaryOperator.And;
                    break;
                case IOrKeywordToken _:
                    binaryOperator = BinaryOperator.Or;
                    break;
                case IXorKeywordToken _:
                    binaryOperator = BinaryOperator.Xor;
                    break;
                default:
                    throw NonExhaustiveMatchException.For(operatorToken);
            }
            return new BinaryExpressionSyntax(left, binaryOperator, right);
        }

        // An atom is the unit of an expression that occurs between infix operators, i.e. an identifier, literal, group, or new
        [MustUseReturnValue]
        [NotNull]
        private ExpressionSyntax ParseAtom([NotNull] ITokenIterator tokens, [NotNull] Diagnostics diagnostics)
        {
            switch (tokens.Current)
            {
                case ISelfTypeKeywordToken selfTypeKeyword:
                    tokens.Next();
                    return new SelfTypeExpressionSyntax(selfTypeKeyword);
                case ISelfKeywordToken selfKeyword:
                    tokens.Next();
                    return new SelfExpressionSyntax(selfKeyword);
                case IBaseKeywordToken baseKeyword:
                    tokens.Next();
                    return new BaseExpressionSyntax(baseKeyword);
                case INewKeywordToken _:
                {
                    var newKeyword = tokens.Consume<INewKeywordTokenPlace>();
                    var type = ParseName(tokens, diagnostics);
                    var openParen = tokens.Consume<IOpenParenTokenPlace>();
                    var arguments = ParseArgumentList(tokens, diagnostics);
                    var closeParen = tokens.Consume<ICloseParenTokenPlace>();
                    return new NewObjectExpressionSyntax(newKeyword, type, openParen, arguments, closeParen);
                }
                case IInitKeywordToken _:
                {
                    var initKeyword = tokens.Consume<IInitKeywordTokenPlace>();
                    var openParen = tokens.Consume<IOpenParenTokenPlace>();
                    var placeExpression = ParseExpression(tokens, diagnostics);
                    var closeParen = tokens.Consume<ICloseParenTokenPlace>();
                    var initializer = ParseName(tokens, diagnostics);
                    var argumentsOpenParen = tokens.Consume<IOpenParenTokenPlace>();
                    var arguments = ParseArgumentList(tokens, diagnostics);
                    var argumentsCloseParen = tokens.Consume<ICloseParenTokenPlace>();
                    return new PlacementInitExpressionSyntax(initKeyword, openParen, placeExpression,
                        closeParen, initializer, argumentsOpenParen, arguments, argumentsCloseParen);
                }
                case IDeleteKeywordToken deleteKeyword:
                {
                    tokens.Next();
                    var expression = ParseExpression(tokens, diagnostics);
                    return new DeleteExpressionSyntax(deleteKeyword, expression);
                }
                case IReturnKeywordToken _:
                {
                    var returnKeywordSpan = tokens.Expect<IReturnKeywordToken>();
                    var expression = tokens.AtEnd<ISemicolonToken>() ? null : ParseExpression(tokens, diagnostics);
                    var span = TextSpan.Covering(returnKeywordSpan, expression?.Span);
                    return new ReturnExpressionSyntax(span, expression);
                }
                case IEqualsGreaterThanToken _:
                    return ParseExpressionBlock(tokens, diagnostics);
                case IOpenParenToken _:
                    return ParseParenthesizedExpression(tokens, diagnostics);
                case IMinusToken _:
                case IPlusToken _:
                case IAtSignToken _:
                case ICaretToken _:
                case INotKeywordToken _:
                {
                    var @operator = tokens.TakeOperator();
                    var operand = ParseExpression(tokens, diagnostics, OperatorPrecedence.Unary);
                    return new UnaryExpressionSyntax(@operator, operand);
                }
                case IIntegerLiteralToken _:
                case IStringLiteralToken _:
                case IBooleanLiteralToken _:
                case IUninitializedKeywordToken _:
                case INoneKeywordToken noneKeyword:
                {
                    var literal = tokens.Consume<ILiteralToken>();
                    return new LiteralExpressionSyntax(literal);
                }
                case IVoidKeywordToken _:
                case IIntKeywordToken _:
                case IUIntKeywordToken _:
                case IBoolKeywordToken _:
                case IByteKeywordToken _:
                case IStringKeywordToken _:
                case INeverKeywordToken _:
                case ISizeKeywordToken _:
                case ITypeKeywordToken _:
                case IAnyKeywordToken _:
                {
                    var keyword = tokens.Take<IPrimitiveTypeToken>();
                    return new PrimitiveTypeSyntax(keyword);
                }
                case IIdentifierToken _:
                {
                    var identifier = tokens.ExpectIdentifier();
                    var name = new IdentifierNameSyntax(identifier);
                    if (tokens.Current is IDollarToken)
                    {
                        var dollar = tokens.Take<IDollarToken>();
                        var lifetime = tokens.Consume<ILifetimeNameTokenPlace>();
                        return new LifetimeTypeSyntax(name, dollar, lifetime);
                    }

                    return name;
                }
                case IForeachKeywordToken _:
                    return ParseForeach(tokens, diagnostics);
                case IWhileKeywordToken _:
                    return ParseWhile(tokens, diagnostics);
                case ILoopKeywordToken _:
                    return ParseLoop(tokens, diagnostics);
                case IBreakKeywordToken _:
                {
                    var breakKeyword = tokens.Consume<IBreakKeywordTokenPlace>();
                    // TODO parse label
                    var expression = AcceptExpression(tokens, diagnostics);
                    return new BreakExpressionSyntax(breakKeyword, expression);
                }
                case IUnsafeKeywordToken unsafeKeyword:
                {
                    tokens.Next();
                    var expression = tokens.Current is IOpenBraceToken ?
                        ParseBlock(tokens, diagnostics)
                        : ParseParenthesizedExpression(tokens, diagnostics);

                    return new UnsafeExpressionSyntax(unsafeKeyword, expression);
                }
                case IRefKeywordToken refKeyword:
                {
                    tokens.Next();
                    var varKeyword = tokens.AcceptToken<IVarKeywordToken>();
                    var referencedType = ParseExpression(tokens, diagnostics);
                    return new RefTypeSyntax(refKeyword, varKeyword, referencedType);
                }
                case IMutableKeywordToken mutableKeyword:
                {
                    tokens.Next();
                    var referencedType = ParseExpression(tokens, diagnostics);
                    return new MutableTypeSyntax(mutableKeyword, referencedType);
                }
                case IIfKeywordToken _:
                    return ParseIf(tokens, diagnostics);
                case IMatchKeywordToken _:
                    return ParseMatch(tokens, diagnostics);
                case IDotToken _:
                {
                    // implicit self etc.
                    var @operator = tokens.TakeOperator();
                    var operand = ParseExpression(tokens, diagnostics, OperatorPrecedence.Unary);
                    return new UnaryExpressionSyntax(@operator, operand);
                }
                case IAsteriskToken _:
                case ISlashToken _:
                case IQuestionToken _:
                case ISemicolonToken _:
                case ICloseParenToken _:
                    // If it is one of these, we assume there is a missing identifier
                    return new IdentifierNameSyntax(tokens.ExpectIdentifier());
                default:
                    throw NonExhaustiveMatchException.For(tokens.Current);
            }
        }

        [MustUseReturnValue]
        [NotNull]
        private ExpressionSyntax ParseForeach(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var foreachKeyword = tokens.Consume<IForeachKeywordToken>();
            var varKeyword = tokens.AcceptToken<IVarKeywordToken>();
            var identifier = tokens.ExpectIdentifier();
            var colon = tokens.AcceptToken<IColonToken>();
            ExpressionSyntax typeExpression = null;
            if (colon != null)
                typeExpression = ParseExpression(tokens, diagnostics);
            var inKeyword = tokens.Consume<IInKeywordToken>();
            var expression = ParseExpression(tokens, diagnostics);
            var block = ParseBlock(tokens, diagnostics);
            return new ForeachExpressionSyntax(foreachKeyword, varKeyword, identifier,
                colon, typeExpression, inKeyword, expression, block);
        }

        [MustUseReturnValue]
        [NotNull]
        private WhileExpressionSyntax ParseWhile(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var whileKeyword = tokens.Consume<IWhileKeywordToken>();
            var condition = ParseExpression(tokens, diagnostics);
            var block = ParseBlock(tokens, diagnostics);
            return new WhileExpressionSyntax(whileKeyword, condition, block);
        }

        [MustUseReturnValue]
        [NotNull]
        private LoopExpressionSyntax ParseLoop(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var loopKeyword = tokens.Consume<ILoopKeywordToken>();
            var block = ParseBlock(tokens, diagnostics);
            return new LoopExpressionSyntax(loopKeyword, block);
        }

        [MustUseReturnValue]
        [NotNull]
        private ExpressionSyntax ParseIf(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var span = tokens.Expect<IIfKeywordToken>();
            var condition = ParseExpression(tokens, diagnostics);
            var thenBlock = ParseExpressionBlock(tokens, diagnostics);
            var elseClause = AcceptElse();
            span = TextSpan.Covering(span, thenBlock.Span, elseClause?.Span);
            return new IfExpressionSyntax(span, condition, thenBlock, elseClause);
        }

        [CanBeNull]
        private ExpressionSyntax AcceptElse()
        {
            if (!Tokens.Accept<IElseKeywordToken>()) return null;
            var expression = Tokens.Current is IIfKeywordToken
                ? ParseIf(Tokens, Tokens.Context.Diagnostics)
                : ParseExpressionBlock(Tokens, Tokens.Context.Diagnostics);
            return expression;
        }

        [MustUseReturnValue]
        [NotNull]
        private ExpressionSyntax ParseMatch(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var matchKeyword = tokens.Take<IMatchKeywordToken>();
            var value = ParseExpression(tokens, diagnostics);
            var openBrace = tokens.Consume<IOpenBraceTokenPlace>();
            var arms = listParser.ParseList(tokens, ParseMatchArm, TypeOf<ICloseBraceToken>(), diagnostics);
            var closeBrace = tokens.Consume<ICloseBraceTokenPlace>();
            return new MatchExpressionSyntax(matchKeyword, value, openBrace, arms, closeBrace);
        }

        [MustUseReturnValue]
        [NotNull]
        private MatchArmSyntax ParseMatchArm(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var pattern = ParsePattern(tokens, diagnostics);
            var expression = ParseExpressionBlock(tokens, diagnostics);
            var comma = tokens.AcceptToken<ICommaToken>();
            return new MatchArmSyntax(pattern, expression, comma);
        }

        [MustUseReturnValue]
        [NotNull]
        private ExpressionSyntax ParseParenthesizedExpression(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            tokens.Expect<IOpenParenToken>();
            var expression = ParseExpression(tokens, diagnostics);
            tokens.Expect<ICloseParenToken>();
            return expression;
        }

        [MustUseReturnValue]
        [NotNull]
        public FixedList<ArgumentSyntax> ParseArguments(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            return listParser.AcceptSeparatedList<ArgumentSyntax, ICommaToken>(AcceptArgument);
        }

        [MustUseReturnValue]
        [NotNull]
        public SeparatedListSyntax<ArgumentSyntax> ParseArgumentList(
            [NotNull] ITokenIterator tokens,
            [NotNull] Diagnostics diagnostics)
        {
            var arguments = listParser.ParseSeparatedList(tokens, (tokens1, diagnostics1) => AcceptArgument(), TypeOf<ICommaToken>(), diagnostics);
            return new SeparatedListSyntax<ArgumentSyntax>(arguments);
        }

        [MustUseReturnValue]
        [CanBeNull]
        private ArgumentSyntax AcceptArgument()
        {
            var isParams = Tokens.Accept<IParamsKeywordToken>();
            var value = AcceptExpression(Tokens, Tokens.Context.Diagnostics);
            if (!isParams && value == null) return null;
            return new ArgumentSyntax(isParams, value);
        }
    }
}
