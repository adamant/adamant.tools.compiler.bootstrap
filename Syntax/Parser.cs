using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Directives;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Literals;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Operators;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class Parser
    {
        public CompilationUnitSyntax Parse(CodeFile file, IEnumerable<Token> tokens)
        {
            return Parse(new TokenStream(file, tokens));
        }

        public CompilationUnitSyntax Parse(ITokenStream tokens)
        {
            return ParseCompilationUnit(tokens);
        }

        #region Parse Syntax Functions
        public CompilationUnitSyntax ParseCompilationUnit(ITokenStream tokens)
        {
            var children = NewChildList();

            // Namespace Declaration
            if (tokens.CurrentIs(TokenKind.NamespaceKeyword))
            {
                var namespaceChildren = NewChildList();
                namespaceChildren.Add(tokens.Expect(TokenKind.NamespaceKeyword));
                namespaceChildren.Add(ParseQualifiedName(tokens));
                namespaceChildren.Add(tokens.Expect(TokenKind.Semicolon));
                children.Add(new CompilationUnitNamespaceSyntax(namespaceChildren));
            }

            // Child Declarations
            while (!tokens.AtEndOfFile())
            {
                var start = tokens.Current;
                children.Add(ParseDeclaration(tokens));
                EnsureAdvance(tokens, start, children);
            }
            children.Add(tokens.Expect(TokenKind.EndOfFile));

            return new CompilationUnitSyntax(children);
        }

        public SyntaxBranchNode ParseDeclaration(ITokenStream tokens)
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
                case TokenKind.EnumKeyword:
                    children.Add(tokens.Expect(TokenKind.EnumKeyword));
                    switch (tokens.Current.Kind)
                    {
                        case TokenKind.StructKeyword:
                            children.Add(tokens.Expect(TokenKind.StructKeyword));
                            children.Add(tokens.Expect(TokenKind.Identifier));
                            children.Add(tokens.Expect(TokenKind.OpenBrace));
                            children.Add(tokens.Expect(TokenKind.CloseBrace));
                            return new EnumStructDeclarationSyntax(children);
                        case TokenKind.ClassKeyword:
                            throw new NotImplementedException("Parsing enum classes not implemented");
                        default:
                            throw NonExhaustiveMatchException.ForEnum(tokens.Current.Kind);
                    }
                case TokenKind.UsingKeyword:
                    children.Add(tokens.Expect(TokenKind.UsingKeyword));
                    children.Add(ParseQualifiedName(tokens));
                    children.Add(tokens.Expect(TokenKind.Semicolon));
                    return new UsingSyntax(children);
                case TokenKind.FunctionKeyword:
                    children.Add(tokens.Expect(TokenKind.FunctionKeyword));
                    children.Add(tokens.Expect(TokenKind.Identifier));
                    children.Add(ParseParameterList(tokens));
                    children.Add(tokens.Expect(TokenKind.RightArrow));
                    children.Add(ParseType(tokens));
                    children.Add(ParseBlock(tokens));
                    return new FunctionDeclarationSyntax(children);
                default:
                    children.Add(tokens.Expect(TokenKind.Identifier));
                    if (tokens.CurrentIs(TokenKind.OpenParen))
                        children.Add(ParseParameterList(tokens));
                    if (tokens.CurrentIs(TokenKind.RightArrow))
                    {
                        children.Add(tokens.Expect(TokenKind.RightArrow));
                        children.Add(ParseType(tokens));
                    }
                    if (tokens.CurrentIs(TokenKind.OpenBrace))
                    {
                        // TODO Read in everything until next close brace
                    }
                    return new IncompleteDeclarationSyntax(children);

            }
        }

        private static NameSyntax ParseQualifiedName(ITokenStream tokens)
        {
            NameSyntax name = new IdentifierNameSyntax((IdentifierToken)tokens.Expect(TokenKind.Identifier));
            while (tokens.CurrentIs(TokenKind.Dot))
            {
                var children = NewChildList();
                children.Add(name);
                children.Add(tokens.Expect(TokenKind.Dot));
                children.Add(new IdentifierNameSyntax((IdentifierToken)tokens.Expect(TokenKind.Identifier)));
                name = new QualifiedNameSyntax(children);
            }
            return name;
        }

        private static ParameterListSyntax ParseParameterList(ITokenStream tokens)
        {
            var children = NewChildList();
            children.Add(tokens.Expect(TokenKind.OpenParen));
            children.AddRange(ParseSyntaxList(tokens, ParseParameter, TokenKind.Comma, TokenKind.CloseParen));
            children.Add(tokens.Expect(TokenKind.CloseParen));
            return new ParameterListSyntax(children);
        }

        private static ParameterSyntax ParseParameter(ITokenStream tokens)
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

        private static IEnumerable<SyntaxNode> ParseSyntaxList(
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
            while (!tokens.CurrentIs(terminator) && !tokens.AtEndOfFile())
            {
                start = tokens.Current;
                yield return tokens.Expect(separator);
                yield return parseItem(tokens);
                if (tokens.Current == start)
                    yield return new SkippedTokensSyntax(tokens.Consume());
            }
        }

        private static IEnumerable<SyntaxNode> ParseSyntaxList(
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
            while (!tokens.CurrentIs(terminator) && !tokens.AtEndOfFile())
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
                    var type = ParseType(tokens);
                    children.Add(type);
                    ExpressionSyntax initializer = null;
                    if (tokens.CurrentIs(TokenKind.Equals))
                    {
                        children.Add(tokens.Expect(TokenKind.Equals));
                        initializer = ParseExpression(tokens);
                        children.Add(initializer);
                    }
                    children.Add(tokens.Expect(TokenKind.Semicolon));
                    return new VariableDeclarationStatementSyntax(children, type, initializer);
                default:
                    children.Add(ParseExpression(tokens));
                    children.Add(tokens.Expect(TokenKind.Semicolon));
                    return new ExpressionStatementSyntax(children);
            }
        }

        private ExpressionSyntax ParseExpression(ITokenStream tokens, OperatorPrecedence minPrecedence = OperatorPrecedence.Min)
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
                        if (minPrecedence <= OperatorPrecedence.Assignment)
                        {
                            precedence = OperatorPrecedence.Assignment;
                            leftAssociative = false;
                            children.Add(tokens.Consume());
                        }
                        break;
                    case TokenKind.OrKeyword:
                    case TokenKind.XorKeyword:
                        if (minPrecedence <= OperatorPrecedence.LogicalOr)
                        {
                            precedence = OperatorPrecedence.LogicalOr;
                            children.Add(tokens.Consume());
                        }
                        break;
                    case TokenKind.AndKeyword:
                        if (minPrecedence <= OperatorPrecedence.LogicalAnd)
                        {
                            precedence = OperatorPrecedence.LogicalAnd;
                            children.Add(tokens.Consume());
                        }
                        break;
                    case TokenKind.EqualsEquals:
                    case TokenKind.NotEqual:
                        if (minPrecedence <= OperatorPrecedence.Equality)
                        {
                            precedence = OperatorPrecedence.Equality;
                            children.Add(tokens.Consume());
                        }
                        break;
                    case TokenKind.LessThan:
                    case TokenKind.LessThanOrEqual:
                    case TokenKind.GreaterThan:
                    case TokenKind.GreaterThanOrEqual:
                        if (minPrecedence <= OperatorPrecedence.Relational)
                        {
                            precedence = OperatorPrecedence.Relational;
                            children.Add(tokens.Consume());
                        }
                        break;
                    case TokenKind.DotDot:
                        if (minPrecedence <= OperatorPrecedence.Range)
                        {
                            precedence = OperatorPrecedence.Range;
                            children.Add(tokens.Consume());
                        }
                        break;
                    case TokenKind.Plus:
                    case TokenKind.Minus:
                        if (minPrecedence <= OperatorPrecedence.Additive)
                        {
                            precedence = OperatorPrecedence.Additive;
                            children.Add(tokens.Consume());
                        }
                        break;
                    case TokenKind.Asterisk:
                    case TokenKind.Slash:
                        if (minPrecedence <= OperatorPrecedence.Multiplicative)
                        {
                            precedence = OperatorPrecedence.Multiplicative;
                            children.Add(tokens.Consume());
                        }
                        break;
                    case TokenKind.OpenParen:
                        if (minPrecedence <= OperatorPrecedence.Primary)
                        {
                            children.Add(ParseArgumentList(tokens));
                            expression = new InvocationSyntax(children);
                            continue;
                        }
                        break;
                    case TokenKind.Dot:
                        if (minPrecedence <= OperatorPrecedence.Primary)
                        {
                            // Member Access
                            precedence = OperatorPrecedence.Primary;
                            children.Add(tokens.Consume());
                        }
                        break;
                    default:
                        return expression;
                }

                // if we didn't match any operator
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
                case TokenKind.UIntKeyword:
                case TokenKind.BoolKeyword:
                case TokenKind.StringKeyword:
                    {
                        var keyword = tokens.Consume();
                        return new PrimitiveTypeSyntax(keyword);
                    }
                case TokenKind.Identifier:
                default: // If it is something else, we assume it should be an identifier name
                    {
                        var identifier = (IdentifierToken)tokens.Expect(TokenKind.Identifier);
                        var name = new IdentifierNameSyntax(identifier);
                        if (!tokens.CurrentIs(TokenKind.Dollar)) return name;

                        var children = NewChildList();
                        children.Add(name);
                        children.Add(tokens.Accept(TokenKind.Dollar));
                        children.Add(tokens.CurrentIs(TokenKind.Identifier)
                            ? tokens.Accept(TokenKind.Identifier)
                            : tokens.Expect(TokenKind.OwnedKeyword));
                        return new LifetimeTypeSyntax(children);
                    }
            }
        }

        private static Token ParseAccessModifier(ITokenStream tokens)
        {
            switch (tokens.Current.Kind)
            {
                case TokenKind.PublicKeyword:
                case TokenKind.PrivateKeyword:
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
