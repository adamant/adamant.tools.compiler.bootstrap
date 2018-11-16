using System;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    // TODO list parsing based on a terminator is problematic, it would be better to have the parse function decide if the next token was a start
    public interface IListParser
    {
        [MustUseReturnValue]
        [NotNull, ItemNotNull]
        FixedList<T> AcceptList<T>([NotNull] Func<T> acceptItem)
            where T : class;

        [MustUseReturnValue]
        [NotNull, ItemNotNull]
        FixedList<T> ParseList<T, TTerminator>([NotNull] Func<T> parseItem)
            where T : class
            where TTerminator : IToken;

        [MustUseReturnValue]
        [NotNull, ItemNotNull]
        FixedList<T> AcceptSeparatedList<T, TSeparator>([NotNull] Func<T> acceptItem)
            where T : class
            where TSeparator : class, IToken;

        [MustUseReturnValue]
        [NotNull, ItemNotNull]
        FixedList<T> ParseSeparatedList<T, TSeparator>(Func<T> acceptItem)
                    where T : class
                    where TSeparator : class, IToken;

        [MustUseReturnValue]
        [NotNull]
        FixedList<T> ParseSeparatedList<T, TSeparator, TTerminator>([NotNull] Func<T> parseItem)
            where T : class where TSeparator : class, IToken where TTerminator : IToken;
    }
}
