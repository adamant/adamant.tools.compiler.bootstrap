using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public class ListParser : IListParser
    {
        [MustUseReturnValue]
        [NotNull]
        public SeparatedListSyntax<T> ParseSeparatedList<T, TSeparator, TTerminator>(
            [NotNull] ITokenStream tokens,
            [NotNull] ParseFunction<T> parseItem,
            TypeOf<TSeparator> separatorType,
            TypeOf<TTerminator> terminatorType)
            where T : SyntaxNode
            where TSeparator : Token
            where TTerminator : Token
        {
            return new SeparatedListSyntax<T>(ParseSeparatedSyntaxEnumerable(tokens, parseItem, separatorType, terminatorType));
        }

        [MustUseReturnValue]
        [NotNull]
        private static IEnumerable<ISyntaxNodeOrToken> ParseSeparatedSyntaxEnumerable<TSeparator, TTerminator>(
            [NotNull] ITokenStream tokens,
            [NotNull] ParseFunction<SyntaxNode> parseItem,
            TypeOf<TSeparator> separatorType,
            TypeOf<TTerminator> terminatorType)
            where TSeparator : Token
            where TTerminator : Token
        {
            while (!(tokens.Current is TTerminator || tokens.Current is EndOfFileToken))
            {
                var start = tokens.Current;
                yield return parseItem(tokens);
                if (tokens.Current == start)
                {
                    tokens.MoveNext();
                    throw new NotImplementedException("Error for skipped token");
                }
                if (tokens.Current is TSeparator)

                    yield return tokens.Expect<TSeparator>();
                else
                    yield break;
            }
        }

        [MustUseReturnValue]
        [NotNull]
        public SyntaxList<T> ParseList<T, TTerminator>(
            [NotNull] ITokenStream tokens,
            [NotNull] ParseFunction<T> parseItem,
            TypeOf<TTerminator> terminatorType)
            where T : SyntaxNode
            where TTerminator : Token
        {
            return ParseSyntaxEnumerable(tokens, parseItem, terminatorType).ToSyntaxList();
        }

        [MustUseReturnValue]
        [NotNull]
        private static IEnumerable<T> ParseSyntaxEnumerable<T, TTerminator>(
            [NotNull] ITokenStream tokens,
            [NotNull] ParseFunction<T> parseItem,
            TypeOf<TTerminator> terminatorType)
            where T : SyntaxNode
            where TTerminator : Token
        {
            while (!(tokens.Current is TTerminator || tokens.Current is EndOfFileToken))
            {
                var start = tokens.Current;
                yield return parseItem(tokens);
                if (tokens.Current != start) continue;

                tokens.MoveNext();
                throw new NotImplementedException("Error for skipped token");
            }
        }
    }
}
