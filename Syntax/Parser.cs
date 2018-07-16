using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tree;

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
            throw new NotImplementedException();
        }

        #endregion

        #region Helper Functions
        private IList<SyntaxNode> NewChildList()
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
