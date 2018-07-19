using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Language.Tests.Parse.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class Parser
    {
        public SyntaxTree Parse(CodeReference codeRef, CodeText code, IEnumerable<Token> tokens)
        {
            return Parse(new TokenStream(codeRef, code, tokens));
        }

        public SyntaxTree Parse(ITokenStream tokens)
        {
            return new SyntaxTree(tokens.CodeReference, tokens.Code, ParseCompilationUnit(tokens));
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

        public SyntaxNode ParseDeclaration(ITokenStream tokens)
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

        private SyntaxNode ParseParameterList(ITokenStream tokens)
        {
            var children = NewChildList();
            children.Add(tokens.Expect(TokenKind.LeftParen));
            children.AddRange(ParseSyntaxList(tokens, ParseParameter, TokenKind.Comma, TokenKind.RightParen));
            children.Add(tokens.Expect(TokenKind.RightParen));
            return new ParameterListSyntax(children);
        }

        private SyntaxNode ParseParameter(ITokenStream tokens)
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

        private SyntaxNode ParseBlock(ITokenStream tokens)
        {
            var children = NewChildList();
            children.Add(tokens.Expect(TokenKind.LeftBrace));
            children.Add(tokens.Expect(TokenKind.RightBrace));
            return new BlockSyntax(children);
        }

        private SyntaxNode ParseType(ITokenStream tokens)
        {
            switch (tokens.Current.Kind)
            {
                case TokenKind.VoidKeyword:
                case TokenKind.IntKeyword:
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
