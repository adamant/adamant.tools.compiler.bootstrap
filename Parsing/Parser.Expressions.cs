using System;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Operators;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.Parsing.Tree;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using ExhaustiveMatching;
using UnaryOperator = Adamant.Tools.Compiler.Bootstrap.Core.Operators.UnaryOperator;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class Parser
    {
        public IExpressionSyntax? AcceptExpression()
        {
            try
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
            catch (ParseFailedException)
            {
                return null;
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
                            var assignmentOperator = BuildAssignmentOperator(Tokens.RequiredToken<IAssignmentToken>());
                            var rightOperand = ParseExpression();
                            if (expression is IAssignableExpressionSyntax assignableExpression)
                                expression = new AssignmentExpressionSyntax(assignableExpression, assignmentOperator, rightOperand);
                            else
                                // Don't assign expression, so it is just the right hand side of the assignment
                                Add(ParseError.CantAssignIntoExpression(File, expression.Span));
                            continue;
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
                    case IDotToken _:
                    case IQuestionDotToken _:
                        if (minPrecedence <= OperatorPrecedence.Primary)
                        {
                            // Member Access
                            var accessOperator = BuildAccessOperator(Tokens.RequiredToken<IAccessOperatorToken>());
                            var nameSyntax = ParseName();
                            if (!(Tokens.Current is IOpenParenToken))
                            {
                                var memberAccessSpan = TextSpan.Covering(expression.Span, nameSyntax.Span);
                                expression = new FieldAccessExpressionSyntax(memberAccessSpan, expression, accessOperator, nameSyntax.ToExpression());
                            }
                            else
                            {
                                Tokens.RequiredToken<IOpenParenToken>();
                                var arguments = ParseArguments();
                                var closeParenSpan = Tokens.Expect<ICloseParenToken>();
                                var invocationSpan = TextSpan.Covering(expression.Span, closeParenSpan);
                                expression = new MethodInvocationExpressionSyntax(invocationSpan, expression, nameSyntax.Name, nameSyntax.ToInvocable(), arguments);
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

        private static AssignmentOperator BuildAssignmentOperator(IAssignmentToken assignmentToken)
        {
            return assignmentToken switch
            {
                IEqualsToken _ => AssignmentOperator.Simple,
                IPlusEqualsToken _ => AssignmentOperator.Plus,
                IMinusEqualsToken _ => AssignmentOperator.Minus,
                IAsteriskEqualsToken _ => AssignmentOperator.Asterisk,
                ISlashEqualsToken _ => AssignmentOperator.Slash,
                _ => throw ExhaustiveMatch.Failed(assignmentToken)
            };
        }

        private static AccessOperator BuildAccessOperator(IAccessOperatorToken accessOperatorToken)
        {
            return accessOperatorToken switch
            {
                IDotToken _ => AccessOperator.Standard,
                IQuestionDotToken _ => AccessOperator.Conditional,
                _ => throw ExhaustiveMatch.Failed(accessOperatorToken)
            };
        }

        private static IExpressionSyntax BuildBinaryOperatorExpression(
             IExpressionSyntax left,
             IBinaryOperatorToken operatorToken,
             IExpressionSyntax right)
        {
            BinaryOperator binaryOperator = operatorToken switch
            {
                IPlusToken _ => BinaryOperator.Plus,
                IMinusToken _ => BinaryOperator.Minus,
                IAsteriskToken _ => BinaryOperator.Asterisk,
                ISlashToken _ => BinaryOperator.Slash,
                IEqualsEqualsToken _ => BinaryOperator.EqualsEquals,
                INotEqualToken _ => BinaryOperator.NotEqual,
                ILessThanToken _ => BinaryOperator.LessThan,
                ILessThanOrEqualToken _ => BinaryOperator.LessThanOrEqual,
                IGreaterThanToken _ => BinaryOperator.GreaterThan,
                IGreaterThanOrEqualToken _ => BinaryOperator.GreaterThanOrEqual,
                IAndKeywordToken _ => BinaryOperator.And,
                IOrKeywordToken _ => BinaryOperator.Or,
                IDotDotToken _ => BinaryOperator.DotDot,
                ILessThanDotDotToken _ => BinaryOperator.LessThanDotDot,
                IDotDotLessThanToken _ => BinaryOperator.DotDotLessThan,
                ILessThanDotDotLessThanToken _ => BinaryOperator.LessThanDotDotLessThan,
                IQuestionQuestionToken _ => throw new NotImplementedException(),
                _ => throw ExhaustiveMatch.Failed(operatorToken)
            };
            return new BinaryOperatorExpressionSyntax(left, binaryOperator, right);
        }

        // An atom is the unit of an expression that occurs between infix operators, i.e. an identifier, literal, group, or new
        private IExpressionSyntax ParseAtom()
        {
            switch (Tokens.Current)
            {
                default:
                    throw ExhaustiveMatch.Failed(Tokens.Current);
                case ISelfKeywordToken _:
                    var selfKeyword = Tokens.Expect<ISelfKeywordToken>();
                    return new SelfExpressionSyntax(selfKeyword, false);
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
                case IPlusToken _:
                    return ParsePrefixUnaryOperator(UnaryOperator.Plus);
                case IMinusToken _:
                    return ParsePrefixUnaryOperator(UnaryOperator.Minus);
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
                    var nameSyntax = ParseName();
                    if (!(Tokens.Current is IOpenParenToken))
                        return nameSyntax.ToExpression();
                    Tokens.RequiredToken<IOpenParenToken>();
                    var arguments = ParseArguments();
                    var closeParenSpan = Tokens.Expect<ICloseParenToken>();
                    var span = TextSpan.Covering(nameSyntax.Span, closeParenSpan);
                    return new FunctionInvocationExpressionSyntax(span, nameSyntax.Name, nameSyntax.ToInvocable(), arguments);
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
                case IIfKeywordToken _:
                    return ParseIf();
                case IDotToken _:
                {
                    // implicit self etc.
                    var dot = Tokens.Required<IDotToken>();
                    var self = new SelfExpressionSyntax(dot.AtStart(), true);
                    var member = ParseName();
                    var span = TextSpan.Covering(dot, member.Span);
                    // TODO we need to check for method call
                    return new FieldAccessExpressionSyntax(span, self, AccessOperator.Standard, member.ToExpression());
                }
                case IMutableKeywordToken _:
                {
                    var mut = Tokens.Required<IMutableKeywordToken>();
                    // `mut` is like a unary operator
                    var expression = ParseExpression(OperatorPrecedence.Unary);
                    var span = TextSpan.Covering(mut, expression.Span);
                    return new BorrowExpressionSyntax(span, expression);
                }
                case IMoveKeywordToken _:
                {
                    var move = Tokens.Required<IMoveKeywordToken>();
                    // `move` is like a unary operator
                    var expression = ParseExpression(OperatorPrecedence.Unary);
                    var span = TextSpan.Covering(move, expression.Span);
                    if (expression is INameExpressionSyntax name)
                        return new MoveExpressionSyntax(span, name);
                    Add(ParseError.CantMoveOutOfExpression(File, span));
                    return expression;
                }
                case IBinaryOperatorToken _:
                case IAssignmentToken _:
                case IQuestionDotToken _:
                case ISemicolonToken _:
                case ICloseParenToken _:
                {
                    // If it is one of these, we assume there is a missing identifier
                    var identifierSpan = Tokens.Expect<IIdentifierToken>();
                    return new NameExpressionSyntax(identifierSpan, null);
                }
                case IOpenBraceToken _:
                case ICloseBraceToken _:
                case IColonToken _:
                case IColonColonDotToken _:
                case ILessThanColonToken _:
                case ICommaToken _:
                case IRightArrowToken _:
                case IQuestionToken _:
                case IKeywordToken _:
                case IEndOfFileToken _:
                    Add(ParseError.UnexpectedEndOfExpression(File, Tokens.Current.Span.AtStart()));
                    throw new ParseFailedException("Unexpected end of expression");
                case ILeftWaveArrowToken _:
                case IRightWaveArrowToken _:
                    throw new ParseFailedException("Reachability operator in expression");
                case IRightDoubleArrowToken _:
                    throw new NotImplementedException($"`{Tokens.Current.Text(File.Code)}` in expression position");
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

        private IForeachExpressionSyntax ParseForeach()
        {
            var foreachKeyword = Tokens.Expect<IForeachKeywordToken>();
            var mutableBinding = Tokens.Accept<IVarKeywordToken>();
            var identifier = Tokens.RequiredToken<IIdentifierToken>();
            var variableName = identifier.Value;
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
                && elseClause is null
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
        /// Parse an expression that is required to have parenthesis around it.
        /// for example `unsafe(x);`.
        /// </summary>
        private IExpressionSyntax ParseParenthesizedExpression()
        {
            Tokens.Expect<IOpenParenToken>();
            var expression = ParseExpression();
            Tokens.Expect<ICloseParenToken>();
            return expression;
        }

        public FixedList<IArgumentSyntax> ParseArguments()
        {
            return AcceptManySeparated<IArgumentSyntax, ICommaToken>(AcceptArgument);
        }

        private IArgumentSyntax? AcceptArgument()
        {
            var expression = AcceptExpression();
            if (expression is null) return null;
            return new ArgumentSyntax(expression);
        }

        public IBlockOrResultSyntax ParseBlockOrResult()
        {
            if (Tokens.Current is IOpenBraceToken)
                return ParseBlock();

            var rightDoubleArrow = Tokens.Expect<IRightDoubleArrowToken>();
            var expression = ParseExpression();
            var span = TextSpan.Covering(rightDoubleArrow, expression.Span);
            return new ResultStatementSyntax(span, expression);
        }
    }
}
