using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Errors;
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
            Type<TSeparator> separatorType,
            Type<TTerminator> terminatorType,
            [NotNull] IDiagnosticsCollector diagnostics)
            where T : SyntaxNode
            where TSeparator : Token
            where TTerminator : Token
        {
            return new SeparatedListSyntax<T>(ParseSeparatedSyntaxEnumerable(tokens, parseItem, separatorType, terminatorType, diagnostics));
        }

        [MustUseReturnValue]
        [NotNull]
        private static IEnumerable<ISyntaxNodeOrToken> ParseSeparatedSyntaxEnumerable<TSeparator, TTerminator>(
            [NotNull] ITokenStream tokens,
            [NotNull] ParseFunction<SyntaxNode> parseItem,
            Type<TSeparator> separatorType,
            Type<TTerminator> terminatorType,
            [NotNull] IDiagnosticsCollector diagnostics)
            where TSeparator : Token
            where TTerminator : Token
        {
            while (!(tokens.Current is TTerminator || tokens.Current is EndOfFileToken))
            {
                var start = tokens.Current;
                yield return parseItem(tokens, diagnostics);
                if (tokens.Current == start)
                {
                    diagnostics.Publish(ParseError.UnexpectedToken(tokens.File, tokens.Current.Span));
                    tokens.MoveNext();
                }

                if (tokens.Current is TSeparator)
                    yield return tokens.Expect<TSeparator>();
                else
                    yield break;
            }
        }

        [MustUseReturnValue]
        [NotNull]
        public SyntaxList<T> ParseList<T>(
            [NotNull] ITokenStream tokens,
            [NotNull] AcceptFunction<T> acceptItem,
            [NotNull] IDiagnosticsCollector diagnostics)
            where T : SyntaxNode
        {
            return new Generator<T>(() => acceptItem(tokens, diagnostics))
                .TakeWhile(t => t != null).ToSyntaxList();
        }

        [MustUseReturnValue]
        [NotNull]
        public SyntaxList<T> ParseList<T, TTerminator>(
            [NotNull] ITokenStream tokens,
            [NotNull] ParseFunction<T> parseItem,
            Type<TTerminator> terminatorType,
            [NotNull] IDiagnosticsCollector diagnostics)
            where T : SyntaxNode
            where TTerminator : Token
        {
            return ParseSyntaxEnumerable(tokens, parseItem, terminatorType, diagnostics).ToSyntaxList();
        }

        [MustUseReturnValue]
        [NotNull]
        private static IEnumerable<T> ParseSyntaxEnumerable<T, TTerminator>(
            [NotNull] ITokenStream tokens,
            [NotNull] ParseFunction<T> parseItem,
            Type<TTerminator> terminatorType,
            [NotNull] IDiagnosticsCollector diagnostics)
            where T : SyntaxNode
            where TTerminator : Token
        {
            while (!(tokens.Current is TTerminator || tokens.Current is EndOfFileToken))
            {
                var start = tokens.Current;
                yield return parseItem(tokens, diagnostics);
                if (tokens.Current == start)
                {
                    diagnostics.Publish(ParseError.UnexpectedToken(tokens.File, tokens.Current.Span));
                    tokens.MoveNext();
                }
            }
        }
    }
}
