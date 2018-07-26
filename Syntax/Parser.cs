using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Language.Tests.Parse.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Literals;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class Parser
    {
        public SyntaxTree<CompilationUnitSyntax> Parse(CodeReference codeRef, CodeText code, IEnumerable<Token> tokens)
        {
            return Parse(new TokenStream(codeRef, code, tokens));
        }

        public SyntaxTree<CompilationUnitSyntax> Parse(ITokenStream tokens)
        {
            return new SyntaxTree<CompilationUnitSyntax>(tokens.CodeReference, tokens.Code, ParseCompilationUnit(tokens));
        }

        #region Parse Syntax Functions
        public CompilationUnitSyntax ParseCompilationUnit(ITokenStream tokens)
        {
            var children = NewChildList();
            while (!tokens.AtEndOfFile())
            {
                var start = tokens.Current;
                children.Add(ParseDeclaration(tokens));
                EnsureAdvance(tokens, start, children);
            }
            children.Add(tokens.Expect(TokenKind.EndOfFile));

            return new CompilationUnitSyntax(children);
        }

        public DeclarationSyntax ParseDeclaration(ITokenStream tokens)
        {
            var children = NewChildList();
            children.Add(ParseAccessModifier(tokens));

            // Function Declaration
            children.Add(tokens.Expect(TokenKind.Identifier));
            children.Add(ParseParameterList(tokens));
            children.Add(tokens.Expect(TokenKind.RightArrow));
            children.Add(ParseType(tokens));
            children.Add(ParseBlock(tokens));
            return new FunctionDeclarationSyntax(children);
        }

        private ParameterListSyntax ParseParameterList(ITokenStream tokens)
        {
            var children = NewChildList();
            children.Add(tokens.Expect(TokenKind.LeftParen));
            children.AddRange(ParseSyntaxList(tokens, ParseParameter, TokenKind.Comma, TokenKind.RightParen));
            children.Add(tokens.Expect(TokenKind.RightParen));
            return new ParameterListSyntax(children);
        }

        private ParameterSyntax ParseParameter(ITokenStream tokens)
        {
            var children = NewChildList();
            switch (tokens.Current.Kind)
            {
                //case TokenKind.MutableKeyword:
                //     `mut self`
                //    throw new NotImplementedException();
                //case TokenKind.SelfKeyword:
                //      `self`
                //    throw new NotImplementedException();
                default:
                    if (tokens.CurrentIs(TokenKind.VarKeyword))
                        children.Add(tokens.Expect(TokenKind.VarKeyword));
                    children.Add(tokens.Expect(TokenKind.Identifier));
                    children.Add(tokens.Expect(TokenKind.Colon));
                    children.Add(ParseType(tokens));
                    break;
            }
            return new ParameterSyntax(children);
        }

        private IEnumerable<SyntaxNode> ParseSyntaxList(
            ITokenStream tokens,
            Func<ITokenStream, SyntaxNode> parseItem,
            TokenKind separator,
            TokenKind terminator)
        {
            if (tokens.Finished || tokens.CurrentIs(terminator))
                yield break;

            yield return parseItem(tokens);
            while (!tokens.CurrentIs(terminator))
            {
                yield return tokens.Expect(separator);
                yield return parseItem(tokens);
            }
        }

        private IEnumerable<SyntaxNode> ParseSyntaxList(
            ITokenStream tokens,
            Func<ITokenStream, SyntaxNode> parseItem,
            TokenKind terminator)
        {
            if (tokens.Finished || tokens.CurrentIs(terminator))
                yield break;

            yield return parseItem(tokens);
            while (!tokens.CurrentIs(terminator))
            {
                yield return parseItem(tokens);
            }
        }

        private BlockSyntax ParseBlock(ITokenStream tokens)
        {
            var children = NewChildList();
            children.Add(tokens.Expect(TokenKind.LeftBrace));
            children.AddRange(ParseSyntaxList(tokens, ParseStatement, TokenKind.RightBrace));
            children.Add(tokens.Expect(TokenKind.RightBrace));
            return new BlockSyntax(children);
        }

        private StatementSyntax ParseStatement(ITokenStream tokens)
        {
            var children = NewChildList();
            children.Add(tokens.Expect(TokenKind.ReturnKeyword));
            if (!tokens.CurrentIs(TokenKind.Semicolon))
                children.Add(ParseExpression(tokens));
            children.Add(tokens.Expect(TokenKind.Semicolon));
            return new ReturnStatementSyntax(children);
        }

        private ExpressionSyntax ParseExpression(ITokenStream tokens, OperatorPrecedence minPrecendence = OperatorPrecedence.Min)
        {
            var expression = ParseAtom(tokens);

            for (; ; )
            {
                var children = NewChildList();
                children.Add(expression);

                OperatorPrecedence? precedence = null;
                var leftAssociative = true;
                //var suffixOperator = false;
                switch (tokens.Current.Kind)
                {
                    case TokenKind.Equals:
                    case TokenKind.PlusEquals:
                    case TokenKind.MinusEquals:
                    case TokenKind.AsteriskEquals:
                    case TokenKind.SlashEquals:
                        if (minPrecendence <= OperatorPrecedence.Assignment)
                        {
                            precedence = OperatorPrecedence.Assignment;
                            leftAssociative = false;
                            children.Add(tokens.Consume());
                        }
                        break;
                    case TokenKind.EqualsEquals:
                    case TokenKind.NotEqual:
                        if (minPrecendence <= OperatorPrecedence.Equality)
                        {
                            precedence = OperatorPrecedence.Equality;
                            children.Add(tokens.Consume());
                        }
                        break;
                    case TokenKind.LessThan:
                    case TokenKind.LessThanOrEqual:
                    case TokenKind.GreaterThan:
                    case TokenKind.GreaterThanOrEqual:
                        if (minPrecendence <= OperatorPrecedence.Relational)
                        {
                            precedence = OperatorPrecedence.Relational;
                            children.Add(tokens.Consume());
                        }
                        break;
                    case TokenKind.Plus:
                    case TokenKind.Minus:
                        if (minPrecendence <= OperatorPrecedence.Additive)
                        {
                            precedence = OperatorPrecedence.Additive;
                            children.Add(tokens.Consume());
                        }
                        break;
                    case TokenKind.Asterisk:
                    case TokenKind.Slash:
                        if (minPrecendence <= OperatorPrecedence.Multiplicative)
                        {
                            precedence = OperatorPrecedence.Multiplicative;
                            children.Add(tokens.Consume());
                        }
                        break;
                    case TokenKind.LeftParen:
                        if (minPrecendence <= OperatorPrecedence.Primary)
                        {
                            // Invocation
                            precedence = OperatorPrecedence.Primary;
                            //suffixOperator = true;
                            throw new NotImplementedException();
                            // TODO children.Add(ParseCallArguments(tokens));
                        }
                        break;
                    case TokenKind.Dot:
                        if (minPrecendence <= OperatorPrecedence.Primary)
                        {
                            // Member Access
                            precedence = OperatorPrecedence.Primary;
                            children.Add(tokens.Consume());
                        }
                        break;
                    default:
                        return expression;
                }

                // if we did't match any operator
                if (precedence == null)
                    return expression;

                if (leftAssociative)
                    precedence += 1;
                children.Add(ParseExpression(tokens, precedence.Value));
                expression = new BinaryOperatorExpressionSyntax(children);
            }
        }

        // An atom is the unit of an expression that occurs between infix operators, i.e. an identifier, literal, group, or new
        private ExpressionSyntax ParseAtom(ITokenStream tokens)
        {
            var children = NewChildList();
            switch (tokens.Current.Kind)
            {
                case TokenKind.LeftParen:
                    children.Add(tokens.Expect(TokenKind.LeftParen));
                    children.Add(ParseExpression(tokens));
                    children.Add(tokens.Expect(TokenKind.RightParen));
                    return new ParenthesizedExpressionSyntax(children);
                case TokenKind.Minus:
                case TokenKind.Plus:
                case TokenKind.AtSign:
                case TokenKind.Caret:
                    children.Add(tokens.Consume());
                    children.Add(ParseExpression(tokens, OperatorPrecedence.Unary));
                    return new UnaryOperatorExpressionSyntax(children);
                case TokenKind.StringLiteral:
                    return new StringLiteralExpressionSyntax(tokens.Consume());
                case TokenKind.Identifier:
                    return new IdentifierNameSyntax(tokens.Expect(TokenKind.Identifier));
                default:
                    return new IdentifierNameSyntax(tokens.MissingToken(TokenKind.Identifier));
            }
            throw new NotImplementedException();
        }

        private TypeSyntax ParseType(ITokenStream tokens)
        {
            switch (tokens.Current.Kind)
            {
                case TokenKind.VoidKeyword:
                case TokenKind.IntKeyword:
                case TokenKind.BoolKeyword:
                    var keyword = tokens.Consume();
                    return new PrimitiveTypeSyntax(keyword);
                default:
                    throw new NotImplementedException(tokens.Current.Kind.ToString());
            }
        }

        private Token ParseAccessModifier(ITokenStream tokens)
        {
            switch (tokens.Current.Kind)
            {
                case TokenKind.PublicKeyword:
                    return tokens.Consume();
                default:
                    return tokens.MissingToken(TokenKind.PublicKeyword);
            }
        }

        #endregion

        #region Helper Functions
        private List<SyntaxNode> NewChildList()
        {
            return new List<SyntaxNode>();
        }

        private void EnsureAdvance(ITokenStream tokens, Token start, IList<SyntaxNode> children)
        {
            if (tokens.Current == start)
                // We have not advanced at all when trying to parse a declaration.
                // Skip a token to try to see if we can find the start of a declaration.
                children.Add(new SkippedTokensSyntax(tokens.Consume()));
        }
        #endregion
    }
}
