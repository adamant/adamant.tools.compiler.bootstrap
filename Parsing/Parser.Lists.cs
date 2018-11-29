using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class Parser
    {
        [MustUseReturnValue]
        [NotNull, ItemNotNull]
        public FixedList<T> AcceptMany<T>([NotNull] Func<T> acceptItem)
            where T : class
        {
            return new Generator<T>(acceptItem)
                .TakeWhile(t => t != null).ToFixedList();
        }

        [MustUseReturnValue]
        [NotNull, ItemNotNull]
        public FixedList<T> ParseMany<T, TTerminator>([NotNull] Func<T> parseItem)
            where T : class
            where TTerminator : IToken
        {
            var items = new List<T>();
            while (!Tokens.AtEnd<TTerminator>())
                items.Add(parseItem());

            return items.ToFixedList();
        }

        [MustUseReturnValue]
        [NotNull, ItemNotNull]
        public FixedList<T> AcceptSeparatedList<T, TSeparator>([NotNull] Func<T> acceptItem)
            where T : class
            where TSeparator : class, IToken
        {
            var items = new List<T>();
            var item = acceptItem();
            while (item != null)
            {
                items.Add(item);
                if (!Tokens.Accept<TSeparator>())
                    break;

                item = acceptItem();
            }
            return items.ToFixedList();
        }

        [MustUseReturnValue]
        [NotNull, ItemNotNull]
        public FixedList<T> AcceptOneOrMore<T, TSeparator>([NotNull] Func<T> acceptItem)
            where T : class
            where TSeparator : class, IToken
        {
            var item = acceptItem();
            if (item == null)
                throw new ParseFailedException();
            var items = new List<T>();
            while (item != null)
            {
                items.Add(item);
                if (!Tokens.Accept<TSeparator>())
                    break;

                item = acceptItem();
            }
            return items.ToFixedList();
        }

        [MustUseReturnValue]
        [NotNull]
        public FixedList<T> ParseMany<T, TSeparator, TTerminator>([NotNull] Func<T> parseItem)
            where T : class
            where TSeparator : class, IToken
            where TTerminator : IToken
        {
            var items = new List<T>();
            while (!Tokens.AtEnd<TTerminator>())
            {
                items.Add(parseItem());
                if (!Tokens.Accept<TSeparator>())
                    break;
            }

            return items.ToFixedList();
        }
    }
}
