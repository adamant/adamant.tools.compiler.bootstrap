using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
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
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class Parser
    {
        [MustUseReturnValue]
        public CompilationUnitSyntax Parse(CodeFile file, IEnumerable<Token> tokens)
        {
            return Parse(new TokenStream(file, tokens));
        }

        [MustUseReturnValue]
        public CompilationUnitSyntax Parse(ITokenStream tokens)
        {
            return ParseCompilationUnit(tokens);
        }

        #region Parse Syntax Functions
        [MustUseReturnValue]
        public CompilationUnitSyntax ParseCompilationUnit(ITokenStream tokens)
        {
            var @namespace = ParseCompilationUnitNamespace(tokens);
            var usingDirectives = ParseUsingDirectives(tokens).ToList().AsReadOnly();
            var declarations = ParseDeclarations(tokens).ToList().AsReadOnly();
            var endOfFile = tokens.ExpectEndOfFile();

            return new CompilationUnitSyntax(@namespace, usingDirectives, declarations, endOfFile);
        }

        [MustUseReturnValue]
        private IEnumerable<DeclarationSyntax> ParseDeclarations(ITokenStream tokens)
        {
            while (!tokens.AtEndOfFile())
            {
                yield return ParseDeclaration(tokens);
            }
        }

        [MustUseReturnValue]
        private static CompilationUnitNamespaceSyntax ParseCompilationUnitNamespace(ITokenStream tokens)
        {
            if (!tokens.CurrentIs(TokenKind.NamespaceKeyword)) return null;

            var namespaceKeyword = tokens.ExpectSimple(TokenKind.NamespaceKeyword);
            var name = ParseQualifiedName(tokens);
            var semicolon = tokens.ExpectSimple(TokenKind.Semicolon);
            return new CompilationUnitNamespaceSyntax(namespaceKeyword, name, semicolon);
        }

        [MustUseReturnValue]
        public IEnumerable<UsingDirectiveSyntax> ParseUsingDirectives(ITokenStream tokens)
        {
            while (tokens.CurrentIs(TokenKind.UsingKeyword))
            {
                var usingKeyword = tokens.ExpectSimple(TokenKind.UsingKeyword);
                var name = ParseQualifiedName(tokens);
                var semicolon = tokens.ExpectSimple(TokenKind.Semicolon);
                yield return new UsingDirectiveSyntax(usingKeyword, name, semicolon);
            }
        }

        [MustUseReturnValue]
        public DeclarationSyntax ParseDeclaration(ITokenStream tokens)
        {
            var accessModifier = ParseAccessModifier(tokens);

            switch (tokens.Current?.Kind)
            {
                case TokenKind.ClassKeyword:
                    {
                        var classKeyword = tokens.ExpectSimple(TokenKind.ClassKeyword);
                        var name = tokens.ExpectIdentifier();
                        var openBrace = tokens.ExpectSimple(TokenKind.OpenBrace);
                        var closeBrace = tokens.ExpectSimple(TokenKind.CloseBrace);
                        return new ClassDeclarationSyntax(accessModifier, classKeyword, name, openBrace, Enumerable.Empty<MemberDeclarationSyntax>(), closeBrace);
                    }
                case TokenKind.EnumKeyword:
                    {
                        var enumKeyword = tokens.ExpectSimple(TokenKind.EnumKeyword);
                        switch (tokens.Current?.Kind)
                        {
                            case TokenKind.StructKeyword:
                                var structKeyword = tokens.ExpectSimple(TokenKind.StructKeyword);
                                var name = tokens.ExpectIdentifier();
                                var openBrace = tokens.ExpectSimple(TokenKind.OpenBrace);
                                var closeBrace = tokens.ExpectSimple(TokenKind.CloseBrace);
                                return new EnumStructDeclarationSyntax(accessModifier, enumKeyword, structKeyword, name, openBrace, Enumerable.Empty<MemberDeclarationSyntax>(), closeBrace);
                            case TokenKind.ClassKeyword:
                                throw new NotImplementedException(
                                    "Parsing enum classes not implemented");
                            default:
                                return ParseIncompleteDeclaration(tokens, accessModifier);
                        }
                    }
                case TokenKind.FunctionKeyword:
                    {
                        var functionKeyword = tokens.ExpectSimple(TokenKind.FunctionKeyword);
                        var name = tokens.ExpectIdentifier();
                        var openParen = tokens.ExpectSimple(TokenKind.OpenParen);
                        var parameters = ParseParameters(tokens);
                        var closeParen = tokens.ExpectSimple(TokenKind.CloseParen);
                        var arrow = tokens.ExpectSimple(TokenKind.RightArrow);
                        var returnType = ParseType(tokens);
                        var body = ParseStatementBlock(tokens);
                        return new FunctionDeclarationSyntax(accessModifier, functionKeyword, name,
                            openParen, parameters, closeParen, arrow, returnType, body);
                    }
                default:
                    return ParseIncompleteDeclaration(tokens, accessModifier);
                case null:
                    throw new InvalidOperationException("Can't parse past end of file");
            }
        }

        [MustUseReturnValue]
        private static IncompleteDeclarationSyntax ParseIncompleteDeclaration(ITokenStream tokens, SimpleToken accessModifier)
        {
            var skipped = new List<Token>() { accessModifier };
            var startToken = tokens.Current;
            var name = tokens.ExpectIdentifier();
            skipped.Add(name);
            throw new NotImplementedException();
            //if (tokens.Current == start)
            //    // We have not advanced at all when trying to parse a declaration.
            //    // Skip a token to try to see if we can find the start of a declaration.
            //    children.Add(new SkippedTokensSyntax(tokens.ConsumeAny()));
            //EnsureAdvance(tokens, startToken, children);

            //return new IncompleteDeclarationSyntax(accessModifier, name, );
        }

        [MustUseReturnValue]
        private static NameSyntax ParseQualifiedName(ITokenStream tokens)
        {
            NameSyntax qualifiedName = new IdentifierNameSyntax(tokens.ExpectIdentifier());
            while (tokens.CurrentIs(TokenKind.Dot))
            {
                var dot = tokens.ExpectSimple(TokenKind.Dot);
                var name = new IdentifierNameSyntax(tokens.ExpectIdentifier());
                qualifiedName = new QualifiedNameSyntax(qualifiedName, dot, name);
            }
            return qualifiedName;
        }

        [MustUseReturnValue]
        private static SeparatedListSyntax<ParameterSyntax> ParseParameters(ITokenStream tokens)
        {
            // What if there isn't a current token?

            var emptySpan = tokens.MissingToken().Span;
            var parameters = ParseSyntaxList(tokens, ParseParameter, TokenKind.Comma, TokenKind.CloseParen).ToList();
            var span = parameters.Any() ? default : emptySpan;
            return new SeparatedListSyntax<ParameterSyntax>(parameters);
        }

        [MustUseReturnValue]
        private static ParameterSyntax ParseParameter(ITokenStream tokens)
        {
            var children = new List<SyntaxNode>();
            switch (tokens.Current?.Kind)
            {
                //case TokenKind.MutableKeyword:
                //     `mut self`
                //    throw new NotImplementedException();
                //case TokenKind.SelfKeyword:
                //      `self`
                //    throw new NotImplementedException();
                default:
                    var varKeyword = tokens.AcceptSimple(TokenKind.VarKeyword);
                    var name = tokens.ExpectIdentifier();
                    var colon = tokens.ExpectSimple(TokenKind.Colon);
                    var type = ParseType(tokens);
                    return new ParameterSyntax(varKeyword, name, colon, type);
            }
        }

        [MustUseReturnValue]
        private static IEnumerable<T> ParseSyntaxList<T>(
            ITokenStream tokens,
            Func<ITokenStream, T> parseItem,
            TokenKind separator,
            TokenKind terminator)
            where T : SyntaxNode
        {
            //if (tokens.Finished || tokens.CurrentIs(terminator))
            //    yield break;

            //var start = tokens.Current;
            //yield return parseItem(tokens);
            //if (tokens.Current == start)
            //    yield return new SkippedTokensSyntax(tokens.ConsumeAny());
            //while (!tokens.CurrentIs(terminator) && !tokens.AtEndOfFile())
            //{
            //    start = tokens.Current;
            //    yield return tokens.Expect(separator);
            //    yield return parseItem(tokens);
            //    if (tokens.Current == start)
            //        yield return new SkippedTokensSyntax(tokens.ConsumeAny());
            //}
            throw new NotImplementedException();
        }

        [MustUseReturnValue]
        private static IEnumerable<T> ParseSyntaxList<T>(
            ITokenStream tokens,
            Func<ITokenStream, SyntaxNode> parseItem,
            TokenKind terminator)
            where T : SyntaxNode
        {
            //if (tokens.Finished || tokens.CurrentIs(terminator))
            //    yield break;

            //var start = tokens.Current;
            //yield return parseItem(tokens);
            //if (tokens.Current == start)
            //    yield return new SkippedTokensSyntax(tokens.ConsumeAny());
            //while (!tokens.CurrentIs(terminator) && !tokens.AtEndOfFile())
            //{
            //    start = tokens.Current;
            //    yield return parseItem(tokens);
            //    if (tokens.Current == start)
            //        yield return new SkippedTokensSyntax(tokens.ConsumeAny());
            //}
            throw new NotImplementedException();
        }

        [MustUseReturnValue]
        private BlockSyntax ParseStatementBlock(ITokenStream tokens)
        {
            var openBrace = tokens.ExpectSimple(TokenKind.OpenBrace);
            var statements = ParseSyntaxList<StatementSyntax>(tokens, ParseStatement, TokenKind.CloseBrace).ToList();
            var closeBrace = tokens.ExpectSimple(TokenKind.CloseBrace);
            return new BlockSyntax(openBrace, statements, closeBrace);
        }

        [MustUseReturnValue]
        private StatementSyntax ParseStatement(ITokenStream tokens)
        {
            var children = new List<SyntaxNode>();
            switch (tokens.Current?.Kind)
            {
                case TokenKind.OpenBrace:
                    return ParseStatementBlock(tokens);
                case TokenKind.LetKeyword:
                case TokenKind.VarKeyword:
                    //children.Add(tokens.ConsumeAny());
                    //children.Add(tokens.Expect(TokenKind.Identifier));
                    //children.Add(tokens.Expect(TokenKind.Colon));
                    //var type = ParseType(tokens);
                    //children.Add(type);
                    //ExpressionSyntax initializer = null;
                    //if (tokens.CurrentIs(TokenKind.Equals))
                    //{
                    //    children.Add(tokens.Expect(TokenKind.Equals));
                    //    initializer = ParseExpression(tokens);
                    //    children.Add(initializer);
                    //}
                    //children.Add(tokens.Expect(TokenKind.Semicolon));
                    //return new VariableDeclarationStatementSyntax(children, type, initializer);
                    throw new NotImplementedException();
                default:
                    //children.Add(ParseExpression(tokens));
                    //children.Add(tokens.Expect(TokenKind.Semicolon));
                    //return new ExpressionStatementSyntax(children);
                    throw new NotImplementedException();
            }
        }

        [MustUseReturnValue]
        private ExpressionSyntax ParseExpression(ITokenStream tokens, OperatorPrecedence minPrecedence = OperatorPrecedence.Min)
        {
            var expression = ParseAtom(tokens);

            for (; ; )
            {
                SimpleToken? @operator = null;
                OperatorPrecedence? precedence = null;
                var leftAssociative = true;
                switch (tokens.Current?.Kind)
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
                            @operator = tokens.ExpectSimple();
                        }
                        break;
                    case TokenKind.OrKeyword:
                    case TokenKind.XorKeyword:
                        if (minPrecedence <= OperatorPrecedence.LogicalOr)
                        {
                            precedence = OperatorPrecedence.LogicalOr;
                            @operator = tokens.ExpectSimple();
                        }
                        break;
                    case TokenKind.AndKeyword:
                        if (minPrecedence <= OperatorPrecedence.LogicalAnd)
                        {
                            precedence = OperatorPrecedence.LogicalAnd;
                            @operator = tokens.ExpectSimple();
                        }
                        break;
                    case TokenKind.EqualsEquals:
                    case TokenKind.NotEqual:
                        if (minPrecedence <= OperatorPrecedence.Equality)
                        {
                            precedence = OperatorPrecedence.Equality;
                            @operator = tokens.ExpectSimple();
                        }
                        break;
                    case TokenKind.LessThan:
                    case TokenKind.LessThanOrEqual:
                    case TokenKind.GreaterThan:
                    case TokenKind.GreaterThanOrEqual:
                        if (minPrecedence <= OperatorPrecedence.Relational)
                        {
                            precedence = OperatorPrecedence.Relational;
                            @operator = tokens.ExpectSimple();
                        }
                        break;
                    case TokenKind.DotDot:
                        if (minPrecedence <= OperatorPrecedence.Range)
                        {
                            precedence = OperatorPrecedence.Range;
                            @operator = tokens.ExpectSimple();
                        }
                        break;
                    case TokenKind.Plus:
                    case TokenKind.Minus:
                        if (minPrecedence <= OperatorPrecedence.Additive)
                        {
                            precedence = OperatorPrecedence.Additive;
                            @operator = tokens.ExpectSimple();
                        }
                        break;
                    case TokenKind.Asterisk:
                    case TokenKind.Slash:
                        if (minPrecedence <= OperatorPrecedence.Multiplicative)
                        {
                            precedence = OperatorPrecedence.Multiplicative;
                            @operator = tokens.ExpectSimple();
                        }
                        break;
                    case TokenKind.OpenParen:
                        if (minPrecedence <= OperatorPrecedence.Primary)
                        {
                            var callee = expression;
                            var openParen = tokens.ExpectSimple(TokenKind.OpenParen);
                            var arguments = ParseArguments(tokens);
                            var closeParen = tokens.ExpectSimple(TokenKind.CloseParen);
                            expression = new InvocationSyntax(callee, openParen, arguments, closeParen);
                            continue;
                        }
                        break;
                    case TokenKind.Dot:
                        if (minPrecedence <= OperatorPrecedence.Primary)
                        {
                            // Member Access
                            precedence = OperatorPrecedence.Primary;
                            @operator = tokens.ExpectSimple();
                        }
                        break;
                    default:
                        return expression;
                }

                if (@operator is SimpleToken @operatorToken &&
                    precedence is OperatorPrecedence operatorPrecedence)
                {
                    if (leftAssociative)
                        operatorPrecedence += 1;

                    var rightOperand = ParseExpression(tokens, operatorPrecedence);
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
        private ExpressionSyntax ParseAtom(ITokenStream tokens)
        {
            switch (tokens.Current?.Kind)
            {
                case TokenKind.NewKeyword:
                    {
                        var newKeyword = tokens.ExpectSimple(TokenKind.NewKeyword);
                        var type = ParseType(tokens);
                        var openParen = tokens.ExpectSimple(TokenKind.OpenParen);
                        var arguments = ParseArguments(tokens);
                        var closeParen = tokens.ExpectSimple(TokenKind.CloseParen);
                        return new NewObjectExpressionSyntax(newKeyword, type, openParen, arguments,
                            closeParen);
                    }
                case TokenKind.ReturnKeyword:
                    {
                        var returnKeyword = tokens.ExpectSimple(TokenKind.ReturnKeyword);
                        var expression = tokens.CurrentIs(TokenKind.Semicolon) ? null : ParseExpression(tokens);
                        return new ReturnExpressionSyntax(returnKeyword, expression);
                    }
                case TokenKind.OpenParen:
                    {
                        var openParen = tokens.ExpectSimple(TokenKind.OpenParen);
                        var expression = ParseExpression(tokens);
                        var closeParen = tokens.ExpectSimple(TokenKind.CloseParen);
                        return new ParenthesizedExpressionSyntax(openParen, expression, closeParen);
                    }
                case TokenKind.Minus:
                case TokenKind.Plus:
                case TokenKind.AtSign:
                case TokenKind.Caret:
                    var @operator = tokens.ExpectSimple();
                    var operand = ParseExpression(tokens, OperatorPrecedence.Unary);
                    return new UnaryOperatorExpressionSyntax(@operator, operand);
                case TokenKind.StringLiteral:
                    return new StringLiteralExpressionSyntax(tokens.ExpectStringLiteral());
                case TokenKind.Identifier:
                    return new IdentifierNameSyntax(tokens.ExpectIdentifier());
                default:
                    return new IdentifierNameSyntax(tokens.ExpectIdentifier());
            }
        }

        [MustUseReturnValue]
        private static TypeSyntax ParseType(ITokenStream tokens)
        {
            switch (tokens.Current?.Kind)
            {
                case TokenKind.VoidKeyword:
                case TokenKind.IntKeyword:
                case TokenKind.UIntKeyword:
                case TokenKind.BoolKeyword:
                case TokenKind.ByteKeyword:
                case TokenKind.StringKeyword:
                    {
                        var keyword = tokens.ExpectSimple();
                        return new PrimitiveTypeSyntax(keyword);
                    }
                case TokenKind.Identifier:
                default: // If it is something else, we assume it should be an identifier name
                    {
                        var identifier = tokens.ExpectIdentifier();
                        var name = new IdentifierNameSyntax(identifier);
                        if (!tokens.CurrentIs(TokenKind.Dollar)) return name;

                        //var children = NewChildList();
                        //children.Add(name);
                        //var dollar = tokens.Expect(TokenKind.Dollar);
                        //AnyToken
                        //if (tokens.Current?.Kind.IsIdentifier() ?? false)
                        //    ? tokens.Accept(TokenKind.Identifier)
                        //    : tokens.Expect(TokenKind.OwnedKeyword));
                        //return new LifetimeTypeSyntax(children);
                        throw new NotImplementedException();
                    }
            }
        }

        [MustUseReturnValue]
        private static SimpleToken ParseAccessModifier(ITokenStream tokens)
        {
            switch (tokens.Current?.Kind)
            {
                case TokenKind.PublicKeyword:
                case TokenKind.ProtectedKeyword:
                case TokenKind.PrivateKeyword:
                    return tokens.ExpectSimple();
                default:
                    return (SimpleToken)tokens.MissingToken();
            }
        }

        [MustUseReturnValue]
        private SeparatedListSyntax<ExpressionSyntax> ParseArguments(ITokenStream tokens)
        {
            // What if there isn't a current token?
            var arguments = ParseSyntaxList(tokens, t => ParseExpression(t), TokenKind.Comma, TokenKind.CloseParen).ToList();
            return new SeparatedListSyntax<ExpressionSyntax>(arguments);
        }
        #endregion
    }
}
