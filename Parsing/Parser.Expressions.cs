using System;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Parsing.Tree;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class Parser
    {
        public IExpressionSyntax? AcceptExpression()
        {
            switch (Tokens.Current)
            {
                case ICloseParenToken _:
                case ICloseBraceToken _:
                case ISemicolonToken _:
                case ICommaToken _:
                case IRightArrowToken _:
                    return null;
                default:
                    return ParseExpression();
            }
        }

        public IExpressionSyntax ParseExpression()
        {
            return ParseExpression(OperatorPrecedence.Min);
        }

        /// <summary>
        /// For expressions, we switch to a precedence climbing parser.
        /// </summary>
        public IExpressionSyntax ParseExpression(OperatorPrecedence minPrecedence)
        {
            var expression = ParseAtom();

            for (; ; )
            {
                IBinaryOperatorToken? @operator = null;
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
                            @operator = Tokens.RequiredToken<IBinaryOperatorToken>();
                        }
                        break;
                    case IQuestionQuestionToken _:
                        if (minPrecedence <= OperatorPrecedence.Coalesce)
                        {
                            precedence = OperatorPrecedence.Coalesce;
                            @operator = Tokens.RequiredToken<IBinaryOperatorToken>();
                        }
                        break;
                    case IOrKeywordToken _:
                        if (minPrecedence <= OperatorPrecedence.LogicalOr)
                        {
                            precedence = OperatorPrecedence.LogicalOr;
                            @operator = Tokens.RequiredToken<IBinaryOperatorToken>();
                        }
                        break;
                    case IAndKeywordToken _:
                        if (minPrecedence <= OperatorPrecedence.LogicalAnd)
                        {
                            precedence = OperatorPrecedence.LogicalAnd;
                            @operator = Tokens.RequiredToken<IBinaryOperatorToken>();
                        }
                        break;
                    case IEqualsEqualsToken _:
                    case INotEqualToken _:
                        if (minPrecedence <= OperatorPrecedence.Equality)
                        {
                            precedence = OperatorPrecedence.Equality;
                            @operator = Tokens.RequiredToken<IBinaryOperatorToken>();
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
                            @operator = Tokens.RequiredToken<IBinaryOperatorToken>();
                        }
                        break;
                    case IDotDotToken _:
                    case ILessThanDotDotToken _:
                    case IDotDotLessThanToken _:
                    case ILessThanDotDotLessThanToken _:
                        if (minPrecedence <= OperatorPrecedence.Range)
                        {
                            precedence = OperatorPrecedence.Range;
                            @operator = Tokens.RequiredToken<IBinaryOperatorToken>();
                        }
                        break;
                    case IPlusToken _:
                    case IMinusToken _:
                        if (minPrecedence <= OperatorPrecedence.Additive)
                        {
                            precedence = OperatorPrecedence.Additive;
                            @operator = Tokens.RequiredToken<IBinaryOperatorToken>();
                        }
                        break;
                    case IAsteriskToken _:
                    case ISlashToken _:
                        if (minPrecedence <= OperatorPrecedence.Multiplicative)
                        {
                            precedence = OperatorPrecedence.Multiplicative;
                            @operator = Tokens.RequiredToken<IBinaryOperatorToken>();
                        }
                        break;
                    case IQuestionToken _: // TODO this be a type
                        if (minPrecedence <= OperatorPrecedence.Unary)
                        {
                            var question = Tokens.Required<IQuestionToken>();
                            var span = TextSpan.Covering(expression.Span, question);
                            expression = new UnaryOperatorExpressionSyntax(span, UnaryOperatorFixity.Postfix, UnaryOperator.Question, expression);
                            continue;
                        }
                        break;
                    //case IOpenParenToken _:
                    //    if (minPrecedence <= OperatorPrecedence.Primary)
                    //    {
                    //        var callee = expression;
                    //        Tokens.RequiredToken<IOpenParenToken>();
                    //        var arguments = ParseArguments();
                    //        var closeParenSpan = Tokens.Expect<ICloseParenToken>();
                    //        var span = TextSpan.Covering(callee.Span, closeParenSpan);
                    //        expression = new MethodInvocationSyntax(span, callee, null, arguments);
                    //        continue;
                    //    }
                    //    break;
                    case IDotToken _:
                    case IQuestionDotToken _:
                        if (minPrecedence <= OperatorPrecedence.Primary)
                        {
                            // Member Access
                            var accessOperator = BuildAccessOperator(Tokens.RequiredToken<IAccessOperatorToken>());
                            var member = ParseNameExpression();
                            if (!(Tokens.Current is IOpenParenToken))
                            {
                                var memberAccessSpan = TextSpan.Covering(expression.Span, member.Span);
                                expression = new MemberAccessExpressionSyntax(memberAccessSpan, expression, accessOperator, member);
                            }
                            else
                            {
                                Tokens.RequiredToken<IOpenParenToken>();
                                var arguments = ParseArguments();
                                var closeParenSpan = Tokens.Expect<ICloseParenToken>();
                                var invocationSpan = TextSpan.Covering(expression.Span, closeParenSpan);
                                expression = new MethodInvocationExpressionSyntax(invocationSpan, expression, member, arguments);
                            }
                            continue;
                        }
                        break;
                    default:
                        return expression;
                }

                if (!(@operator is null) &&
                    precedence is OperatorPrecedence operatorPrecedence)
                {
                    if (leftAssociative)
                        operatorPrecedence += 1;

                    var rightOperand = ParseExpression(operatorPrecedence);
                    expression = BuildBinaryOperatorExpression(expression, @operator, rightOperand);
                }
                else
                {
                    // if we didn't match any operator
                    return expression;
                }
            }
        }

        private static AccessOperator BuildAccessOperator(IAccessOperatorToken accessOperatorToken)
        {
            switch (accessOperatorToken)
            {
                default:
                    throw NonExhaustiveMatchException.For(accessOperatorToken);
                case IDotToken _:
                    return AccessOperator.Standard;
                case IQuestionDotToken _:
                    return AccessOperator.Conditional;
            }
        }

        private static IExpressionSyntax BuildBinaryOperatorExpression(
             IExpressionSyntax left,
             IBinaryOperatorToken operatorToken,
             IExpressionSyntax right)
        {
            BinaryOperator binaryOperator;
            switch (operatorToken)
            {
                default:
                    throw ExhaustiveMatch.Failed(operatorToken);
                case IEqualsToken _:
                    return new AssignmentExpressionSyntax(left, AssignmentOperator.Simple, right);
                case IPlusEqualsToken _:
                    return new AssignmentExpressionSyntax(left, AssignmentOperator.Plus, right);
                case IMinusEqualsToken _:
                    return new AssignmentExpressionSyntax(left, AssignmentOperator.Minus, right);
                case IAsteriskEqualsToken _:
                    return new AssignmentExpressionSyntax(left, AssignmentOperator.Asterisk, right);
                case ISlashEqualsToken _:
                    return new AssignmentExpressionSyntax(left, AssignmentOperator.Slash, right);
                case IPlusToken _:
                    binaryOperator = BinaryOperator.Plus;
                    break;
                case IMinusToken _:
                    binaryOperator = BinaryOperator.Minus;
                    break;
                case IAsteriskToken _:
                    binaryOperator = BinaryOperator.Asterisk;
                    break;
                case ISlashToken _:
                    binaryOperator = BinaryOperator.Slash;
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
                case IDotDotToken _:
                    binaryOperator = BinaryOperator.DotDot;
                    break;
                case ILessThanDotDotToken _:
                case IDotDotLessThanToken _:
                case ILessThanDotDotLessThanToken _:
                case IQuestionQuestionToken _:
                case IDollarToken _:
                    throw new NotImplementedException();
            }
            return new BinaryOperatorExpressionSyntax(left, binaryOperator, right);
        }

        // An atom is the unit of an expression that occurs between infix operators, i.e. an identifier, literal, group, or new
        private IExpressionSyntax ParseAtom()
        {
            switch (Tokens.Current)
            {
                default:
                    throw new NotImplementedException();
                    throw ExhaustiveMatch.Failed(Tokens.Current);
                case ISelfKeywordToken _:
                    var selfKeyword = Tokens.Expect<ISelfKeywordToken>();
                    return new SelfExpressionSyntax(selfKeyword);
                case INewKeywordToken _:
                {
                    var newKeyword = Tokens.Expect<INewKeywordToken>();
                    var type = ParseTypeName();
                    Tokens.Expect<IOpenParenToken>();
                    var arguments = ParseArguments();
                    var closeParen = Tokens.Expect<ICloseParenToken>();
                    var span = TextSpan.Covering(newKeyword, closeParen);
                    return new NewObjectExpressionSyntax(span, type, null, arguments);
                }
                case IReturnKeywordToken _:
                {
                    var returnKeyword = Tokens.Expect<IReturnKeywordToken>();
                    var expression = Tokens.AtEnd<ISemicolonToken>() ? null : ParseExpression();
                    var span = TextSpan.Covering(returnKeyword, expression?.Span);
                    return new ReturnExpressionSyntax(span, expression);
                }
                case IOpenParenToken _:
                    return ParseParenthesizedExpression();
                case IMinusToken _:
                    return ParsePrefixUnaryOperator(UnaryOperator.Minus);
                case IPlusToken _:
                    return ParsePrefixUnaryOperator(UnaryOperator.Plus);
                case INotKeywordToken _:
                    return ParsePrefixUnaryOperator(UnaryOperator.Not);
                case IBooleanLiteralToken _:
                {
                    var literal = Tokens.RequiredToken<IBooleanLiteralToken>();
                    return new BoolLiteralExpressionSyntax(literal.Span, literal.Value);
                }
                case IIntegerLiteralToken _:
                {
                    var literal = Tokens.RequiredToken<IIntegerLiteralToken>();
                    return new IntegerLiteralExpressionSyntax(literal.Span, literal.Value);
                }
                case IStringLiteralToken _:
                {
                    var literal = Tokens.RequiredToken<IStringLiteralToken>();
                    return new StringLiteralExpressionSyntax(literal.Span, literal.Value);
                }
                case INoneKeywordToken _:
                {
                    var literal = Tokens.Required<INoneKeywordToken>();
                    return new NoneLiteralExpressionSyntax(literal);
                }
                case IIdentifierToken _:
                {
                    var name = ParseNameExpression();
                    if (!(Tokens.Current is IOpenParenToken))
                        return name;
                    Tokens.RequiredToken<IOpenParenToken>();
                    var arguments = ParseArguments();
                    var closeParenSpan = Tokens.Expect<ICloseParenToken>();
                    var span = TextSpan.Covering(name.Span, closeParenSpan);
                    return new FunctionInvocationExpressionSyntax(span, name, arguments);
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
                case INextKeywordToken _:
                {
                    var span = Tokens.Required<INextKeywordToken>();
                    return new NextExpressionSyntax(span);
                }
                case IUnsafeKeywordToken _:
                    return ParseUnsafeExpression();
                case IMoveKeywordToken _:
                {
                    var moveKeyword = Tokens.Expect<IMoveKeywordToken>();
                    var expression = ParseExpression();
                    var span = TextSpan.Covering(moveKeyword, expression.Span);
                    return new MoveExpressionSyntax(span, expression);
                }
                case IIfKeywordToken _:
                    return ParseIf();
                case IDotToken _:
                {
                    // implicit self etc.
                    var dot = Tokens.Required<IDotToken>();
                    var member = ParseNameExpression();
                    var span = TextSpan.Covering(dot, member.Span);
                    return new MemberAccessExpressionSyntax(span, null, AccessOperator.Standard, member);
                }
                case IDollarToken _:
                {
                    var dollar = Tokens.Required<IDollarToken>();
                    var (span, lifetime) = ParseLifetimeName();
                    if (lifetime is null)
                        throw new NotImplementedException();
                    span = TextSpan.Covering(dollar, span);
                    return new LifetimeExpressionSyntax(span, lifetime);
                }
                case IAsteriskToken _:
                case ISlashToken _:
                case IQuestionToken _:
                case ISemicolonToken _:
                case ICloseParenToken _:
                {
                    // If it is one of these, we assume there is a missing identifier
                    var identifierSpan = Tokens.Expect<IIdentifierToken>();
                    throw new NotImplementedException();
                    //return new IdentifierNameSyntax(identifierSpan, SpecialName.Underscore);
                }
                case IEqualsGreaterThanToken _:
                    throw new NotImplementedException("`=>` in expression position");
            }
        }

        private IUnsafeExpressionSyntax ParseUnsafeExpression()
        {
            var unsafeKeyword = Tokens.Expect<IUnsafeKeywordToken>();
            var isBlock = Tokens.Current is IOpenBraceToken;
            var expression = isBlock
                ? ParseBlock()
                : ParseParenthesizedExpression();
            var span = TextSpan.Covering(unsafeKeyword, expression.Span);
            return new UnsafeExpressionSyntax(span, expression);
        }

        private IUnaryOperatorExpressionSyntax ParsePrefixUnaryOperator(UnaryOperator @operator)
        {
            var operatorSpan = Tokens.Required<IOperatorToken>();
            var operand = ParseExpression(OperatorPrecedence.Unary);
            var span = TextSpan.Covering(operatorSpan, operand.Span);
            return new UnaryOperatorExpressionSyntax(span, UnaryOperatorFixity.Prefix, @operator, operand);
        }

        private (TextSpan, SimpleName?) ParseLifetimeName()
        {
            switch (Tokens.Current)
            {
                case IIdentifierToken _:
                    var identifier = Tokens.RequiredToken<IIdentifierToken>();
                    return (identifier.Span, new SimpleName(identifier.Value));
                case IOwnedKeywordToken _:
                    var ownedKeyword = Tokens.RequiredToken<IOwnedKeywordToken>();
                    return (ownedKeyword.Span, SpecialName.Owned);
                case IForeverKeywordToken _:
                    var foreverKeyword = Tokens.RequiredToken<IForeverKeywordToken>();
                    return (foreverKeyword.Span, SpecialName.Forever);
                default:
                    var span = Tokens.Expect<IIdentifierToken>();
                    return (span, null);
            }
        }

        private IForeachExpressionSyntax ParseForeach()
        {
            var foreachKeyword = Tokens.Expect<IForeachKeywordToken>();
            var mutableBinding = Tokens.Accept<IVarKeywordToken>();
            var identifier = Tokens.RequiredToken<IIdentifierToken>();
            var variableName = nameContext.Qualify(variableNumbers.VariableName(identifier.Value));
            ITypeSyntax? type = null;
            if (Tokens.Accept<IColonToken>())
                type = ParseType();
            Tokens.Expect<IInKeywordToken>();
            var expression = ParseExpression();
            var block = ParseBlock();
            var span = TextSpan.Covering(foreachKeyword, block.Span);
            return new ForeachExpressionSyntax(span, mutableBinding, variableName, type, expression, block);
        }

        private IWhileExpressionSyntax ParseWhile()
        {
            var whileKeyword = Tokens.Expect<IWhileKeywordToken>();
            var condition = ParseExpression();
            var block = ParseBlock();
            var span = TextSpan.Covering(whileKeyword, block.Span);
            return new WhileExpressionSyntax(span, condition, block);
        }

        private ILoopExpressionSyntax ParseLoop()
        {
            var loopKeyword = Tokens.Expect<ILoopKeywordToken>();
            var block = ParseBlock();
            var span = TextSpan.Covering(loopKeyword, block.Span);
            return new LoopExpressionSyntax(span, block);
        }

        private IIfExpressionSyntax ParseIf(ParseAs parseAs = ParseAs.Expression)
        {
            var @if = Tokens.Expect<IIfKeywordToken>();
            var condition = ParseExpression();
            var thenBlock = ParseBlockOrResult();
            var elseClause = AcceptElse(parseAs);
            var span = TextSpan.Covering(@if, thenBlock.Span, elseClause?.Span);
            if (parseAs == ParseAs.Statement
                && elseClause == null
                && thenBlock is IResultStatementSyntax)
            {
                var semicolon = Tokens.Expect<ISemicolonToken>();
                span = TextSpan.Covering(span, semicolon);
            }
            return new IfExpressionSyntax(span, condition, thenBlock, elseClause);
        }

        private IElseClauseSyntax? AcceptElse(ParseAs parseAs)
        {
            if (!Tokens.Accept<IElseKeywordToken>())
                return null;
            var expression = Tokens.Current is IIfKeywordToken
                ? (IElseClauseSyntax)ParseIf(parseAs)
                : ParseBlockOrResult();
            if (parseAs == ParseAs.Statement
                && expression is IResultStatementSyntax)
                Tokens.Expect<ISemicolonToken>();
            return expression;
        }

        /// <summary>
        /// Parse and expression that is required to have parenthesis around it.
        /// for example `unsafe(x);`.
        /// </summary>
        /// <returns></returns>
        private IExpressionSyntax ParseParenthesizedExpression()
        {
            Tokens.Expect<IOpenParenToken>();
            var expression = ParseExpression();
            Tokens.Expect<ICloseParenToken>();
            return expression;
        }

        public FixedList<IArgumentSyntax> ParseArguments()
        {
            return AcceptSeparatedList<IArgumentSyntax, ICommaToken>(AcceptArgument);
        }

        private IArgumentSyntax? AcceptArgument()
        {
            var value = AcceptExpression();
            return value == null ? null : new ArgumentSyntax(value.Span, value);
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
