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
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
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
                            @operator = tokens.TakeOperator();
                        }
                        break;
                    case IQuestionQuestionToken _:
                        if (minPrecedence <= OperatorPrecedence.Coalesce)
                        {
                            precedence = OperatorPrecedence.Coalesce;
                            @operator = tokens.TakeOperator();
                        }
                        break;
                    case IOrKeywordToken _:
                    case IXorKeywordToken _:
                        if (minPrecedence <= OperatorPrecedence.LogicalOr)
                        {
                            precedence = OperatorPrecedence.LogicalOr;
                            @operator = tokens.TakeOperator();
                        }
                        break;
                    case IAndKeywordToken _:
                        if (minPrecedence <= OperatorPrecedence.LogicalAnd)
                        {
                            precedence = OperatorPrecedence.LogicalAnd;
                            @operator = tokens.TakeOperator();
                        }
                        break;
                    case IEqualsEqualsToken _:
                    case INotEqualToken _:
                        if (minPrecedence <= OperatorPrecedence.Equality)
                        {
                            precedence = OperatorPrecedence.Equality;
                            @operator = tokens.TakeOperator();
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
                            @operator = tokens.TakeOperator();
                        }
                        break;
                    case IColonToken _: // type kind
                        if (minPrecedence <= OperatorPrecedence.Relational)
                        {
                            var colon = tokens.Take<IColonToken>();
                            var typeKind = tokens.Expect<ITypeKindKeywordTokenPlace>();
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
                            @operator = tokens.TakeOperator();
                        }
                        break;
                    case IPlusToken _:
                    case IMinusToken _:
                        if (minPrecedence <= OperatorPrecedence.Additive)
                        {
                            precedence = OperatorPrecedence.Additive;
                            @operator = tokens.TakeOperator();
                        }
                        break;
                    case IAsteriskToken _:
                    case ISlashToken _:
                        if (minPrecedence <= OperatorPrecedence.Multiplicative)
                        {
                            precedence = OperatorPrecedence.Multiplicative;
                            @operator = tokens.TakeOperator();
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
                            var name = tokens.Expect<ILifetimeNameTokenPlace>();
                            expression = new LifetimeTypeSyntax(leftOperand, lifetimeOperator, name);
                            continue;
                        }
                        break;
                    case IQuestionToken _:
                        if (minPrecedence <= OperatorPrecedence.Unary)
                        {
                            @operator = tokens.TakeOperator();
                            expression = new UnaryOperatorExpressionSyntax(@operator, expression);
                            continue;
                        }
                        break;
                    case IOpenParenToken _:
                        if (minPrecedence <= OperatorPrecedence.Primary)
                        {
                            var callee = expression;
                            var openParen = tokens.Expect<IOpenParenTokenPlace>();
                            var arguments = ParseArgumentList(tokens, diagnostics);
                            var closeParen = tokens.Expect<ICloseParenTokenPlace>();
                            expression = new InvocationSyntax(callee, openParen, arguments, closeParen);
                            continue;
                        }
                        break;
                    case IOpenBracketToken _:
                        if (minPrecedence <= OperatorPrecedence.Primary)
                        {
                            var callee = expression;
                            var openBracket = tokens.Expect<IOpenBracketTokenPlace>();
                            var arguments = ParseArgumentList(tokens, diagnostics);
                            var closeBracket = tokens.Expect<ICloseBracketTokenPlace>();
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
                            var accessOperator = tokens.TakeOperator();
                            var member = tokens.Expect<IMemberNameTokenPlace>();
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
                case ISelfTypeKeywordToken selfTypeKeyword:
                    tokens.MoveNext();
                    return new SelfTypeExpressionSyntax(selfTypeKeyword);
                case IUninitializedKeywordToken uninitializedKeyword:
                    tokens.MoveNext();
                    return new UninitializedExpressionSyntax(uninitializedKeyword);
                case INoneKeywordToken noneKeyword:
                    tokens.MoveNext();
                    return new NoneExpressionSyntax(noneKeyword);
                case ISelfKeywordToken selfKeyword:
                    tokens.MoveNext();
                    return new SelfExpressionSyntax(selfKeyword);
                case IBaseKeywordToken baseKeyword:
                    tokens.MoveNext();
                    return new BaseExpressionSyntax(baseKeyword);
                case INewKeywordToken _:
                {
                    var newKeyword = tokens.Expect<INewKeywordTokenPlace>();
                    var type = ParseName(tokens, diagnostics);
                    var openParen = tokens.Expect<IOpenParenTokenPlace>();
                    var arguments = ParseArgumentList(tokens, diagnostics);
                    var closeParen = tokens.Expect<ICloseParenTokenPlace>();
                    return new NewObjectExpressionSyntax(newKeyword, type, openParen, arguments, closeParen);
                }
                case IInitKeywordToken _:
                {
                    var initKeyword = tokens.Expect<IInitKeywordTokenPlace>();
                    var openParen = tokens.Expect<IOpenParenTokenPlace>();
                    var placeExpression = ParseExpression(tokens, diagnostics);
                    var closeParen = tokens.Expect<ICloseParenTokenPlace>();
                    var initializer = ParseName(tokens, diagnostics);
                    var argumentsOpenParen = tokens.Expect<IOpenParenTokenPlace>();
                    var arguments = ParseArgumentList(tokens, diagnostics);
                    var argumentsCloseParen = tokens.Expect<ICloseParenTokenPlace>();
                    return new PlacementInitExpressionSyntax(initKeyword, openParen, placeExpression,
                        closeParen, initializer, argumentsOpenParen, arguments, argumentsCloseParen);
                }
                case IDeleteKeywordToken deleteKeyword:
                {
                    tokens.MoveNext();
                    var expression = ParseExpression(tokens, diagnostics);
                    return new DeleteExpressionSyntax(deleteKeyword, expression);
                }
                case IReturnKeywordToken _:
                {
                    var returnKeyword = tokens.Expect<IReturnKeywordTokenPlace>();
                    var expression = tokens.AtTerminator<ISemicolonToken>() ? null : ParseExpression(tokens, diagnostics);
                    return new ReturnExpressionSyntax(returnKeyword, expression);
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
                    return new UnaryOperatorExpressionSyntax(@operator, operand);
                }
                case IIntegerLiteralToken literal:
                {
                    tokens.MoveNext();
                    return new IntegerLiteralExpressionSyntax(literal);
                }
                case IStringLiteralToken literal:
                {
                    tokens.MoveNext();
                    return new StringLiteralExpressionSyntax(literal);
                }
                case IBooleanLiteralToken literal:
                {
                    tokens.MoveNext();
                    return new BooleanLiteralExpressionSyntax(literal);
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
                        var lifetime = tokens.Expect<ILifetimeNameTokenPlace>();
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
                    var breakKeyword = tokens.Expect<IBreakKeywordTokenPlace>();
                    // TODO parse label
                    var expression = AcceptExpression(tokens, diagnostics);
                    return new BreakExpressionSyntax(breakKeyword, expression);
                }
                case IUnsafeKeywordToken unsafeKeyword:
                {
                    tokens.MoveNext();
                    var expression = tokens.Current is IOpenBraceToken ?
                        ParseBlock(tokens, diagnostics)
                        : ParseParenthesizedExpression(tokens, diagnostics);

                    return new UnsafeExpressionSyntax(unsafeKeyword, expression);
                }
                case IRefKeywordToken refKeyword:
                {
                    tokens.MoveNext();
                    var varKeyword = tokens.Accept<IVarKeywordToken>();
                    var referencedType = ParseExpression(tokens, diagnostics);
                    return new RefTypeSyntax(refKeyword, varKeyword, referencedType);
                }
                case IMutableKeywordToken mutableKeyword:
                {
                    tokens.MoveNext();
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
                    return new UnaryOperatorExpressionSyntax(@operator, operand);
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
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var foreachKeyword = tokens.Expect<IForeachKeywordToken>();
            var varKeyword = tokens.Accept<IVarKeywordToken>();
            var identifier = tokens.ExpectIdentifier();
            var colon = tokens.Accept<IColonToken>();
            ExpressionSyntax typeExpression = null;
            if (colon != null)
                typeExpression = ParseExpression(tokens, diagnostics);
            var inKeyword = tokens.Expect<IInKeywordToken>();
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
            var whileKeyword = tokens.Expect<IWhileKeywordToken>();
            var condition = ParseExpression(tokens, diagnostics);
            var block = ParseBlock(tokens, diagnostics);
            return new WhileExpressionSyntax(whileKeyword, condition, block);
        }

        [MustUseReturnValue]
        [NotNull]
        private LoopExpressionSyntax ParseLoop(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var loopKeyword = tokens.Expect<ILoopKeywordToken>();
            var block = ParseBlock(tokens, diagnostics);
            return new LoopExpressionSyntax(loopKeyword, block);
        }

        [MustUseReturnValue]
        [NotNull]
        private ExpressionSyntax ParseIf(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var ifKeyword = tokens.Take<IIfKeywordToken>();
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
            var elseKeyword = tokens.Accept<IElseKeywordToken>();
            if (elseKeyword == null) return null;
            var expression = tokens.Current is IIfKeywordToken
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
            var matchKeyword = tokens.Take<IMatchKeywordToken>();
            var value = ParseExpression(tokens, diagnostics);
            var openBrace = tokens.Expect<IOpenBraceTokenPlace>();
            var arms = listParser.ParseList(tokens, ParseMatchArm, TypeOf<ICloseBraceToken>(), diagnostics);
            var closeBrace = tokens.Expect<ICloseBraceTokenPlace>();
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
            var comma = tokens.Accept<ICommaToken>();
            return new MatchArmSyntax(pattern, expression, comma);
        }

        [MustUseReturnValue]
        [NotNull]
        private ExpressionSyntax ParseParenthesizedExpression(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var openParen = tokens.Expect<IOpenParenTokenPlace>();
            var expression = ParseExpression(tokens, diagnostics);
            var closeParen = tokens.Expect<ICloseParenTokenPlace>();
            return new ParenthesizedExpressionSyntax(openParen, expression, closeParen);
        }

        [MustUseReturnValue]
        [NotNull]
        public SeparatedListSyntax<ArgumentSyntax> ParseArgumentList(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var arguments = listParser.ParseSeparatedList(tokens, AcceptArgument, TypeOf<ICommaToken>(), diagnostics);
            return new SeparatedListSyntax<ArgumentSyntax>(arguments);
        }

        [MustUseReturnValue]
        [CanBeNull]
        private ArgumentSyntax AcceptArgument(
            [NotNull] ITokenStream tokens,
            [NotNull] IDiagnosticsCollector diagnostics)
        {
            var paramsKeyword = tokens.Accept<IParamsKeywordToken>();
            var value = AcceptExpression(tokens, diagnostics);
            if (paramsKeyword == null && value == null) return null;
            return new ArgumentSyntax(paramsKeyword, value);
        }
    }
}
