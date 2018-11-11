using System;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    // Delegate needed so we can declare the arg as not null
    [NotNull]
    public delegate T ParseFunction<out T>([NotNull] ITokenIterator tokens, [NotNull] Diagnostics diagnostics);

    [CanBeNull]
    public delegate T AcceptFunction<out T>([NotNull] ITokenIterator tokens, [NotNull] Diagnostics diagnostics);

    // TODO list parsing based on a terminator is problematic, it would be better to have the parse function decide if the next token was a start
    public interface IListParser
    {
        [MustUseReturnValue]
        [NotNull, ItemNotNull]
        FixedList<T> ParseList<T>([NotNull] Func<T> acceptItem)
            where T : class;

        [MustUseReturnValue]
        [NotNull, ItemNotNull]
        FixedList<T> ParseSeparatedList<T, TSeparator>(Func<T> acceptItem)
            where T : class
            where TSeparator : class, IToken;

        [MustUseReturnValue]
        [NotNull]
        SeparatedListSyntax<T> ParseSeparatedList<T, TSeparator>(
            [NotNull] ITokenIterator tokens,
            [NotNull] AcceptFunction<T> acceptItem,
            Type<TSeparator> separatorType,
            [NotNull] Diagnostics diagnostics)
            where T : NonTerminal
            where TSeparator : class, IToken;

        [Obsolete("Use ParseList() taking an AcceptFunction instead")]
        [MustUseReturnValue]
        [NotNull]
        FixedList<T> ParseList<T, TTerminator>(
            [NotNull] ITokenIterator tokens,
            [NotNull] ParseFunction<T> parseItem,
            Type<TTerminator> terminatorType,
            [NotNull] Diagnostics diagnostics)
            where T : NonTerminal
            where TTerminator : class, IToken;

        [Obsolete("Use ParseSeparatedList() taking an AcceptFunction instead")]
        [MustUseReturnValue]
        [NotNull]
        SeparatedListSyntax<T> ParseSeparatedList<T, TSeparator, TTerminator>(
            [NotNull] ITokenIterator tokens,
            [NotNull] ParseFunction<T> parseItem,
            Type<TSeparator> separatorType,
            Type<TTerminator> terminatorType,
            [NotNull] Diagnostics diagnostics)
            where T : NonTerminal
            where TSeparator : class, IToken
            where TTerminator : class, IToken;
    }
}
