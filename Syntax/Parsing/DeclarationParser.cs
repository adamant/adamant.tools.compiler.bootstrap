using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public class DeclarationParser : IParser<DeclarationSyntax>
    {
        private readonly IListParser listParser;
        private readonly IParser<ExpressionSyntax> expressionParser;
        private readonly IParser<BlockStatementSyntax> blockStatementParser;

        public DeclarationParser(IListParser listParser, IParser<ExpressionSyntax> expressionParser, IParser<BlockStatementSyntax> blockStatementParser)
        {
            this.listParser = listParser;
            this.expressionParser = expressionParser;
            this.blockStatementParser = blockStatementParser;
        }

        [MustUseReturnValue]
        public DeclarationSyntax Parse(ITokenStream tokens)
        {
            var accessModifier = ParseAccessModifier(tokens);

            switch (tokens.Current?.Kind)
            {
                case TokenKind.ClassKeyword:
                    return ParseClass(accessModifier, tokens);
                case TokenKind.EnumKeyword:
                    return ParseEnum(accessModifier, tokens);
                case TokenKind.FunctionKeyword:
                    return ParseFunction(accessModifier, tokens);
                default:
                    return ParseIncompleteDeclaration(tokens, accessModifier);
                case null:
                    throw new InvalidOperationException("Can't parse past end of file");
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
        private DeclarationSyntax ParseClass(SimpleToken accessModifier, ITokenStream tokens)
        {
            var classKeyword = tokens.ExpectSimple(TokenKind.ClassKeyword);
            var name = tokens.ExpectIdentifier();
            var openBrace = tokens.ExpectSimple(TokenKind.OpenBrace);
            var closeBrace = tokens.ExpectSimple(TokenKind.CloseBrace);
            return new ClassDeclarationSyntax(accessModifier, classKeyword, name, openBrace,
                SyntaxList<MemberDeclarationSyntax>.Empty, closeBrace);
        }

        [MustUseReturnValue]
        private static DeclarationSyntax ParseEnum(SimpleToken accessModifier, ITokenStream tokens)
        {
            var enumKeyword = tokens.ExpectSimple(TokenKind.EnumKeyword);
            switch (tokens.Current?.Kind)
            {
                case TokenKind.StructKeyword:
                    var structKeyword = tokens.ExpectSimple(TokenKind.StructKeyword);
                    var name = tokens.ExpectIdentifier();
                    var openBrace = tokens.ExpectSimple(TokenKind.OpenBrace);
                    var closeBrace = tokens.ExpectSimple(TokenKind.CloseBrace);
                    return new EnumStructDeclarationSyntax(accessModifier, enumKeyword, structKeyword, name,
                        openBrace, SyntaxList<MemberDeclarationSyntax>.Empty, closeBrace);
                case TokenKind.ClassKeyword:
                    throw new NotImplementedException(
                        "Parsing enum classes not implemented");
                default:
                    return ParseIncompleteDeclaration(tokens, accessModifier);
            }
        }

        #region Parse Function
        [MustUseReturnValue]
        public FunctionDeclarationSyntax ParseFunction(
            SimpleToken accessModifier,
            ITokenStream tokens)
        {
            var functionKeyword = tokens.ExpectSimple(TokenKind.FunctionKeyword);
            var name = tokens.ExpectIdentifier();
            var openParen = tokens.ExpectSimple(TokenKind.OpenParen);
            var parameters = ParseParameters(tokens);
            var closeParen = tokens.ExpectSimple(TokenKind.CloseParen);
            var arrow = tokens.ExpectSimple(TokenKind.RightArrow);
            var returnTypeExpression = expressionParser.Parse(tokens);
            var body = blockStatementParser.Parse(tokens);
            return new FunctionDeclarationSyntax(accessModifier, functionKeyword, name,
                openParen, parameters, closeParen, arrow, returnTypeExpression, body);
        }

        [MustUseReturnValue]
        private SeparatedListSyntax<ParameterSyntax> ParseParameters(ITokenStream tokens)
        {
            return listParser.ParseSeparatedList(tokens, ParseParameter, TokenKind.Comma, TokenKind.CloseParen);
        }

        [MustUseReturnValue]
        private ParameterSyntax ParseParameter(ITokenStream tokens)
        {
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
                    var typeExpression = expressionParser.Parse(tokens);
                    return new ParameterSyntax(varKeyword, name, colon, typeExpression);
            }
        }
        #endregion

        [MustUseReturnValue]
        private static IncompleteDeclarationSyntax ParseIncompleteDeclaration(ITokenStream tokens, SimpleToken accessModifier)
        {
            var skipped = new List<ISyntaxNodeOrToken>() { (Token)accessModifier };
            var startToken = tokens.Current;
            var name = tokens.ExpectIdentifier();
            skipped.Add((Token)name);
            throw new NotImplementedException();
            //if (tokens.Current == start)
            //    // We have not advanced at all when trying to parse a declaration.
            //    // Skip a token to try to see if we can find the start of a declaration.
            //    children.Add(new SkippedTokensSyntax(tokens.ConsumeAny()));
            //EnsureAdvance(tokens, startToken, children);

            //return new IncompleteDeclarationSyntax(accessModifier, name, );
        }
    }
}
