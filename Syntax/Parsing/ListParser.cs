using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public class ListParser : IListParser
    {
        [MustUseReturnValue]
        public SeparatedListSyntax<T> ParseSeparatedList<T>(
            ITokenStream tokens,
            Func<ITokenStream, T> parseItem,
            TokenKind separator,
            TokenKind terminator)
            where T : SyntaxNode
        {
            return new SeparatedListSyntax<T>(ParseSeparatedSyntaxEnumerable(tokens, parseItem, separator, terminator));
        }

        [MustUseReturnValue]
        private static IEnumerable<ISyntaxNodeOrToken> ParseSeparatedSyntaxEnumerable(
            ITokenStream tokens,
            Func<ITokenStream, SyntaxNode> parseItem,
            TokenKind separator,
            TokenKind terminator)
        {
            while (tokens.Current?.Kind != terminator && !tokens.AtEndOfFile())
            {
                var start = tokens.Current;
                yield return parseItem(tokens);
                if (tokens.Current == start)
                {
                    tokens.Next();
                    throw new NotImplementedException("Error for skipped token");
                }
                if (tokens.Current?.Kind == separator)

                    yield return (Token)tokens.ExpectSimple(separator);
                else
                    yield break;
            }
        }

        [MustUseReturnValue]
        public SyntaxList<T> ParseList<T>(
            ITokenStream tokens,
            Func<ITokenStream, T> parseItem,
            TokenKind terminator)
            where T : SyntaxNode
        {
            return ParseSyntaxEnumerable(tokens, parseItem, terminator).ToSyntaxList();
        }

        [MustUseReturnValue]
        private static IEnumerable<T> ParseSyntaxEnumerable<T>(
            ITokenStream tokens,
            Func<ITokenStream, T> parseItem,
            TokenKind terminator)
            where T : SyntaxNode
        {
            while (tokens.Current?.Kind != terminator && !tokens.AtEndOfFile())
            {
                var start = tokens.Current;
                yield return parseItem(tokens);
                if (tokens.Current != start) continue;

                tokens.Next();
                throw new NotImplementedException("Error for skipped token");
            }
        }
    }
}
