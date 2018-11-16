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
        public ExpressionSyntax AcceptExpression()
        {
            switch (Tokens.Current)
            {
                case ICloseParenToken _:
                case ICloseBracketToken _:
                case ICloseBraceToken _:
                case ISemicolonToken _:
                case ICommaToken _:
                    return null;
                default:
                    return ParseExpression();
            }
        }

        [NotNull]
        [MustUseReturnValue]
        public ExpressionSyntax ParseExpression()
        {
            return ParseExpression(OperatorPrecedence.Min);
        }

        [MustUseReturnValue]
        [NotNull]
        public ExpressionSyntax ParseExpression(OperatorPrecedence minPrecedence)
        {
            var expression = ParseAtom();

            for (; ; )
            {
                IOperatorToken @operator = null;
                OperatorPrecedence? precedence = null;
                var leftAssociative = true;
                switch (Tokens.Current)
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
                            @operator = Tokens.RequiredToken<IOperatorToken>();
                        }
                        break;
                    case IQuestionQuestionToken _:
                        if (minPrecedence <= OperatorPrecedence.Coalesce)
                        {
                            precedence = OperatorPrecedence.Coalesce;
                            @operator = Tokens.RequiredToken<IOperatorToken>();
                        }
                        break;
                    case IOrKeywordToken _:
                    case IXorKeywordToken _:
                        if (minPrecedence <= OperatorPrecedence.LogicalOr)
                        {
                            precedence = OperatorPrecedence.LogicalOr;
                            @operator = Tokens.RequiredToken<IOperatorToken>();
                        }
                        break;
                    case IAndKeywordToken _:
                        if (minPrecedence <= OperatorPrecedence.LogicalAnd)
                        {
                            precedence = OperatorPrecedence.LogicalAnd;
                            @operator = Tokens.RequiredToken<IOperatorToken>();
                        }
                        break;
                    case IEqualsEqualsToken _:
                    case INotEqualToken _:
                        if (minPrecedence <= OperatorPrecedence.Equality)
                        {
                            precedence = OperatorPrecedence.Equality;
                            @operator = Tokens.RequiredToken<IOperatorToken>();
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
                            @operator = Tokens.RequiredToken<IOperatorToken>();
                        }
                        break;
                    case IColonToken _: // type kind
                        if (minPrecedence <= OperatorPrecedence.Relational)
                        {
                            var colon = Tokens.RequiredToken<IColonToken>();
                            var typeKind = Tokens.Consume<ITypeKindKeywordTokenPlace>();
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
                            @operator = Tokens.RequiredToken<IOperatorToken>();
                        }
                        break;
                    case IPlusToken _:
                    case IMinusToken _:
                        if (minPrecedence <= OperatorPrecedence.Additive)
                        {
                            precedence = OperatorPrecedence.Additive;
                            @operator = Tokens.RequiredToken<IOperatorToken>();
                        }
                        break;
                    case IAsteriskToken _:
                    case ISlashToken _:
                        if (minPrecedence <= OperatorPrecedence.Multiplicative)
                        {
                            precedence = OperatorPrecedence.Multiplicative;
                            @operator = Tokens.RequiredToken<IOperatorToken>();
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
                            var lifetimeOperator = Tokens.Take<ILifetimeOperatorToken>();
                            var name = Tokens.Consume<ILifetimeNameTokenPlace>();
                            expression = new LifetimeTypeSyntax(leftOperand, lifetimeOperator, name);
                            continue;
                        }
                        break;
                    case IQuestionToken _:
                        if (minPrecedence <= OperatorPrecedence.Unary)
                        {
                            @operator = Tokens.TakeOperator();
                            expression = new UnaryExpressionSyntax(@operator, expression);
                            continue;
                        }
                        break;
                    case IOpenParenToken _:
                        if (minPrecedence <= OperatorPrecedence.Primary)
                        {
                            var callee = expression;
                            Tokens.Expect<IOpenParenToken>();
                            var arguments = ParseArguments();
                            var closeParenSpan = Tokens.Expect<ICloseParenToken>();
                            var span = TextSpan.Covering(callee.Span, closeParenSpan);
                            expression = new InvocationSyntax(span, callee, arguments);
                            continue;
                        }
                        break;
                    case IOpenBracketToken _:
                        if (minPrecedence <= OperatorPrecedence.Primary)
                        {
                            var callee = expression;
                            var openBracket = Tokens.Consume<IOpenBracketTokenPlace>();
                            var arguments = ParseArgumentList();
                            var closeBracket = Tokens.Consume<ICloseBracketTokenPlace>();
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
                            var accessOperator = BuildAccessOperator(Tokens.RequiredToken<IAccessOperatorToken>());
                            var member = Tokens.RequiredToken<IMemberNameToken>();
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

                    var rightOperand = ParseExpression(operatorPrecedence);
                    expression = BuildOperatorExpression(expression, operatorToken, rightOperand);
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
        private ExpressionSyntax ParseAtom()
        {
            switch (Tokens.Current)
            {
                case ISelfTypeKeywordToken selfTypeKeyword:
                    Tokens.Next();
                    return new SelfTypeExpressionSyntax(selfTypeKeyword);
                case ISelfKeywordToken selfKeyword:
                    Tokens.Next();
                    return new SelfExpressionSyntax(selfKeyword);
                case IBaseKeywordToken baseKeyword:
                    Tokens.Next();
                    return new BaseExpressionSyntax(baseKeyword);
                case INewKeywordToken _:
                {
                    var newKeyword = Tokens.Consume<INewKeywordTokenPlace>();
                    var type = ParseName();
                    var openParen = Tokens.Consume<IOpenParenTokenPlace>();
                    var arguments = ParseArgumentList();
                    var closeParen = Tokens.Consume<ICloseParenTokenPlace>();
                    return new NewObjectExpressionSyntax(newKeyword, type, openParen, arguments, closeParen);
                }
                case IInitKeywordToken _:
                {
                    var initKeyword = Tokens.Consume<IInitKeywordTokenPlace>();
                    var openParen = Tokens.Consume<IOpenParenTokenPlace>();
                    var placeExpression = ParseExpression();
                    var closeParen = Tokens.Consume<ICloseParenTokenPlace>();
                    var initializer = ParseName();
                    var argumentsOpenParen = Tokens.Consume<IOpenParenTokenPlace>();
                    var arguments = ParseArgumentList();
                    var argumentsCloseParen = Tokens.Consume<ICloseParenTokenPlace>();
                    return new PlacementInitExpressionSyntax(initKeyword, openParen, placeExpression,
                        closeParen, initializer, argumentsOpenParen, arguments, argumentsCloseParen);
                }
                case IDeleteKeywordToken deleteKeyword:
                {
                    Tokens.Next();
                    var expression = ParseExpression();
                    return new DeleteExpressionSyntax(deleteKeyword, expression);
                }
                case IReturnKeywordToken _:
                {
                    var returnKeywordSpan = Tokens.Expect<IReturnKeywordToken>();
                    var expression = Tokens.AtEnd<ISemicolonToken>() ? null : ParseExpression();
                    var span = TextSpan.Covering(returnKeywordSpan, expression?.Span);
                    return new ReturnExpressionSyntax(span, expression);
                }
                case IEqualsGreaterThanToken _:
                    return ParseExpressionBlock();
                case IOpenParenToken _:
                    return ParseParenthesizedExpression();
                case IMinusToken _:
                case IPlusToken _:
                case IAtSignToken _:
                case ICaretToken _:
                case INotKeywordToken _:
                {
                    var @operator = Tokens.TakeOperator();
                    var operand = ParseExpression(OperatorPrecedence.Unary);
                    return new UnaryExpressionSyntax(@operator, operand);
                }
                case IIntegerLiteralToken _:
                case IStringLiteralToken _:
                case IBooleanLiteralToken _:
                case IUninitializedKeywordToken _:
                case INoneKeywordToken _:
                {
                    var literal = Tokens.Consume<ILiteralToken>();
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
                    var keyword = Tokens.Take<IPrimitiveTypeToken>();
                    return new PrimitiveTypeSyntax(keyword);
                }
                case IIdentifierToken _:
                {
                    var identifier = Tokens.ExpectIdentifier();
                    var name = new IdentifierNameSyntax(identifier);
                    if (Tokens.Current is IDollarToken)
                    {
                        var dollar = Tokens.Take<IDollarToken>();
                        var lifetime = Tokens.Consume<ILifetimeNameTokenPlace>();
                        return new LifetimeTypeSyntax(name, dollar, lifetime);
                    }

                    return name;
                }
                case IForeachKeywordToken _:
                    return ParseForeach();
                case IWhileKeywordToken _:
                    return ParseWhile();
                case ILoopKeywordToken _:
                    return ParseLoop();
                case IBreakKeywordToken _:
                {
                    var breakKeyword = Tokens.Expect<IBreakKeywordToken>();
                    // TODO parse label
                    var expression = AcceptExpression();
                    var span = TextSpan.Covering(breakKeyword, expression?.Span);
                    return new BreakExpressionSyntax(span, expression);
                }
                case IUnsafeKeywordToken unsafeKeyword:
                {
                    Tokens.Next();
                    var expression = Tokens.Current is IOpenBraceToken ?
                        ParseBlock()
                        : ParseParenthesizedExpression();

                    return new UnsafeExpressionSyntax(unsafeKeyword, expression);
                }
                case IRefKeywordToken refKeyword:
                {
                    Tokens.Next();
                    var varKeyword = Tokens.AcceptToken<IVarKeywordToken>();
                    var referencedType = ParseExpression();
                    return new RefTypeSyntax(refKeyword, varKeyword, referencedType);
                }
                case IMutableKeywordToken mutableKeyword:
                {
                    Tokens.Next();
                    var referencedType = ParseExpression();
                    return new MutableTypeSyntax(mutableKeyword, referencedType);
                }
                case IIfKeywordToken _:
                    return ParseIf();
                case IMatchKeywordToken _:
                    return ParseMatch();
                case IDotToken _:
                {
                    // implicit self etc.
                    var @operator = Tokens.TakeOperator();
                    var operand = ParseExpression(OperatorPrecedence.Unary);
                    return new UnaryExpressionSyntax(@operator, operand);
                }
                case IAsteriskToken _:
                case ISlashToken _:
                case IQuestionToken _:
                case ISemicolonToken _:
                case ICloseParenToken _:
                    // If it is one of these, we assume there is a missing identifier
                    return new IdentifierNameSyntax(Tokens.ExpectIdentifier());
                default:
                    throw NonExhaustiveMatchException.For(Tokens.Current);
            }
        }

        [MustUseReturnValue]
        [NotNull]
        private ExpressionSyntax ParseForeach()
        {
            var foreachKeyword = Tokens.Consume<IForeachKeywordToken>();
            var varKeyword = Tokens.AcceptToken<IVarKeywordToken>();
            var identifier = Tokens.ExpectIdentifier();
            var colon = Tokens.AcceptToken<IColonToken>();
            ExpressionSyntax typeExpression = null;
            if (colon != null)
                typeExpression = ParseExpression();
            var inKeyword = Tokens.Consume<IInKeywordToken>();
            var expression = ParseExpression();
            var block = ParseBlock();
            return new ForeachExpressionSyntax(foreachKeyword, varKeyword, identifier,
                colon, typeExpression, inKeyword, expression, block);
        }

        [MustUseReturnValue]
        [NotNull]
        private WhileExpressionSyntax ParseWhile()
        {
            var whileKeyword = Tokens.Consume<IWhileKeywordToken>();
            var condition = ParseExpression();
            var block = ParseBlock();
            return new WhileExpressionSyntax(whileKeyword, condition, block);
        }

        [MustUseReturnValue]
        [NotNull]
        private LoopExpressionSyntax ParseLoop()
        {
            var loopKeyword = Tokens.Consume<ILoopKeywordToken>();
            var block = ParseBlock();
            return new LoopExpressionSyntax(loopKeyword, block);
        }

        [MustUseReturnValue]
        [NotNull]
        private ExpressionSyntax ParseIf()
        {
            var span = Tokens.Expect<IIfKeywordToken>();
            var condition = ParseExpression();
            var thenBlock = ParseExpressionBlock();
            var elseClause = AcceptElse();
            span = TextSpan.Covering(span, thenBlock.Span, elseClause?.Span);
            return new IfExpressionSyntax(span, condition, thenBlock, elseClause);
        }

        [CanBeNull]
        private ExpressionSyntax AcceptElse()
        {
            if (!Tokens.Accept<IElseKeywordToken>()) return null;
            var expression = Tokens.Current is IIfKeywordToken
                ? ParseIf()
                : ParseExpressionBlock();
            return expression;
        }

        [MustUseReturnValue]
        [NotNull]
        private ExpressionSyntax ParseMatch()
        {
            var matchKeyword = Tokens.Take<IMatchKeywordToken>();
            var value = ParseExpression();
            var openBrace = Tokens.Consume<IOpenBraceTokenPlace>();
            var arms = listParser.ParseList(Tokens, (t, d) => ParseMatchArm(), TypeOf<ICloseBraceToken>(), Tokens.Context.Diagnostics);
            var closeBrace = Tokens.Consume<ICloseBraceTokenPlace>();
            return new MatchExpressionSyntax(matchKeyword, value, openBrace, arms, closeBrace);
        }

        [MustUseReturnValue]
        [NotNull]
        private MatchArmSyntax ParseMatchArm()
        {
            var pattern = ParsePattern();
            var expression = ParseExpressionBlock();
            // TODO the comma is only optional on the last one
            Tokens.Accept<ICommaToken>();
            return new MatchArmSyntax(pattern, expression);
        }

        [MustUseReturnValue]
        [NotNull]
        private ExpressionSyntax ParseParenthesizedExpression()
        {
            Tokens.Expect<IOpenParenToken>();
            var expression = ParseExpression();
            Tokens.Expect<ICloseParenToken>();
            return expression;
        }

        [MustUseReturnValue]
        [NotNull]
        public FixedList<ArgumentSyntax> ParseArguments()
        {
            return listParser.AcceptSeparatedList<ArgumentSyntax, ICommaToken>(AcceptArgument);
        }

        [MustUseReturnValue]
        [NotNull]
        public SeparatedListSyntax<ArgumentSyntax> ParseArgumentList()
        {
            var arguments = listParser.ParseSeparatedList(Tokens, (t, d) => AcceptArgument(), TypeOf<ICommaToken>(), Tokens.Context.Diagnostics);
            return new SeparatedListSyntax<ArgumentSyntax>(arguments);
        }

        [MustUseReturnValue]
        [CanBeNull]
        private ArgumentSyntax AcceptArgument()
        {
            var isParams = Tokens.Accept<IParamsKeywordToken>();
            var value = AcceptExpression();
            if (!isParams && value == null) return null;
            return new ArgumentSyntax(isParams, value);
        }
    }
}
