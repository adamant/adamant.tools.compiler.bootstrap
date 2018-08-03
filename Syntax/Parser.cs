using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Literals;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Operators;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Core.Syntax;

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

            switch (tokens.Current.Kind)
            {
                case TokenKind.ClassKeyword:
                    children.Add(tokens.Expect(TokenKind.ClassKeyword));
                    children.Add(tokens.Expect(TokenKind.Identifier));
                    children.Add(tokens.Expect(TokenKind.OpenBrace));
                    children.Add(tokens.Expect(TokenKind.CloseBrace));
                    return new ClassDeclarationSyntax(children);
                default:
                    // Function Declaration
                    children.Add(tokens.Expect(TokenKind.Identifier));
                    children.Add(ParseParameterList(tokens));
                    children.Add(tokens.Expect(TokenKind.RightArrow));
                    children.Add(ParseType(tokens));
                    children.Add(ParseBlock(tokens));
                    return new FunctionDeclarationSyntax(children);
            }
        }

        private ParameterListSyntax ParseParameterList(ITokenStream tokens)
        {
            var children = NewChildList();
            children.Add(tokens.Expect(TokenKind.OpenParen));
            children.AddRange(ParseSyntaxList(tokens, ParseParameter, TokenKind.Comma, TokenKind.CloseParen));
            children.Add(tokens.Expect(TokenKind.CloseParen));
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

            var start = tokens.Current;
            yield return parseItem(tokens);
            if (tokens.Current == start)
                yield return new SkippedTokensSyntax(tokens.Consume());
            while (!tokens.CurrentIs(terminator))
            {
                start = tokens.Current;
                yield return tokens.Expect(separator);
                yield return parseItem(tokens);
                if (tokens.Current == start)
                    yield return new SkippedTokensSyntax(tokens.Consume());
            }
        }

        private IEnumerable<SyntaxNode> ParseSyntaxList(
            ITokenStream tokens,
            Func<ITokenStream, SyntaxNode> parseItem,
            TokenKind terminator)
        {
            if (tokens.Finished || tokens.CurrentIs(terminator))
                yield break;

            var start = tokens.Current;
            yield return parseItem(tokens);
            if (tokens.Current == start)
                yield return new SkippedTokensSyntax(tokens.Consume());
            while (!tokens.CurrentIs(terminator))
            {
                start = tokens.Current;
                yield return parseItem(tokens);
                if (tokens.Current == start)
                    yield return new SkippedTokensSyntax(tokens.Consume());
            }
        }

        private BlockSyntax ParseBlock(ITokenStream tokens)
        {
            var children = NewChildList();
            children.Add(tokens.Expect(TokenKind.OpenBrace));
            children.AddRange(ParseSyntaxList(tokens, ParseStatement, TokenKind.CloseBrace));
            children.Add(tokens.Expect(TokenKind.CloseBrace));
            return new BlockSyntax(children);
        }

        private StatementSyntax ParseStatement(ITokenStream tokens)
        {
            var children = NewChildList();
            switch (tokens.Current.Kind)
            {
                case TokenKind.OpenBrace:
                    return ParseBlock(tokens);
                case TokenKind.LetKeyword:
                case TokenKind.VarKeyword:
                    children.Add(tokens.Consume());
                    children.Add(tokens.Expect(TokenKind.Identifier));
                    children.Add(tokens.Expect(TokenKind.Colon));
                    children.Add(ParseType(tokens));
                    if (tokens.CurrentIs(TokenKind.Equals))
                    {
                        children.Add(tokens.Expect(TokenKind.Equals));
                        children.Add(ParseExpression(tokens));
                    }
                    children.Add(tokens.Expect(TokenKind.Semicolon));
                    return new VariableDeclarationStatementSyntax(children);
                default:
                    children.Add(ParseExpression(tokens));
                    children.Add(tokens.Expect(TokenKind.Semicolon));
                    return new ExpressionStatementSyntax(children);
            }
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
                    case TokenKind.OpenParen:
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
                case TokenKind.NewKeyword:
                    children.Add(tokens.Expect(TokenKind.NewKeyword));
                    children.Add(ParseType(tokens));
                    children.Add(ParseArgumentList(tokens));
                    return new NewObjectExpressionSyntax(children);
                case TokenKind.ReturnKeyword:
                    children.Add(tokens.Expect(TokenKind.ReturnKeyword));
                    if (!tokens.CurrentIs(TokenKind.Semicolon))
                        children.Add(ParseExpression(tokens));
                    return new ReturnExpressionSyntax(children);
                case TokenKind.OpenParen:
                    children.Add(tokens.Expect(TokenKind.OpenParen));
                    children.Add(ParseExpression(tokens));
                    children.Add(tokens.Expect(TokenKind.CloseParen));
                    return new ParenthesizedExpressionSyntax(children);
                case TokenKind.Minus:
                case TokenKind.Plus:
                case TokenKind.AtSign:
                case TokenKind.Caret:
                    children.Add(tokens.Consume());
                    children.Add(ParseExpression(tokens, OperatorPrecedence.Unary));
                    return new UnaryOperatorExpressionSyntax(children);
                case TokenKind.StringLiteral:
                    return new StringLiteralExpressionSyntax((StringLiteralToken)tokens.Consume());
                case TokenKind.Identifier:
                    return new IdentifierNameSyntax((IdentifierToken)tokens.Expect(TokenKind.Identifier));
                default:
                    return new IdentifierNameSyntax((IdentifierToken)tokens.MissingToken(TokenKind.Identifier));
            }
        }

        private static TypeSyntax ParseType(ITokenStream tokens)
        {
            switch (tokens.Current.Kind)
            {
                case TokenKind.VoidKeyword:
                case TokenKind.IntKeyword:
                case TokenKind.BoolKeyword:
                    var keyword = tokens.Consume();
                    return new PrimitiveTypeSyntax(keyword);
                case TokenKind.Identifier:
                    var name = (IdentifierToken)tokens.Expect(TokenKind.Identifier);
                    return new IdentifierNameSyntax(name);
                default:
                    throw NonExhaustiveMatchException.For(tokens.Current);
            }
        }

        private static Token ParseAccessModifier(ITokenStream tokens)
        {
            switch (tokens.Current.Kind)
            {
                case TokenKind.PublicKeyword:
                    return tokens.Consume();
                default:
                    return tokens.MissingToken(TokenKind.PublicKeyword);
            }
        }

        private ArgumentListSyntax ParseArgumentList(ITokenStream tokens)
        {
            var children = NewChildList();
            children.Add(tokens.Expect(TokenKind.OpenParen));
            children.AddRange(ParseSyntaxList(tokens, t => ParseExpression(t), TokenKind.Comma, TokenKind.CloseParen));
            children.Add(tokens.Expect(TokenKind.CloseParen));
            return new ArgumentListSyntax(children);
        }
        #endregion

        #region Helper Functions
        private static List<SyntaxNode> NewChildList()
        {
            return new List<SyntaxNode>();
        }

        private static void EnsureAdvance(ITokenStream tokens, Token start, IList<SyntaxNode> children)
        {
            if (tokens.Current == start)
                // We have not advanced at all when trying to parse a declaration.
                // Skip a token to try to see if we can find the start of a declaration.
                children.Add(new SkippedTokensSyntax(tokens.Consume()));
        }
        #endregion
    }
}
