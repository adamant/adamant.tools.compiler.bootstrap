using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class Parser
    {
        public static FixedList<T> AcceptMany<T>(Func<T?> acceptItem)
            where T : class
        {
            return new Generator<T?>(acceptItem)
                .TakeWhile(t => t != null).ToFixedList()!;
        }

        public FixedList<T> ParseMany<T, TTerminator>(Func<T> parseItem)
            where T : class
            where TTerminator : IToken
        {
            var items = new List<T>();
            while (!Tokens.AtEnd<TTerminator>())
                items.Add(parseItem());

            return items.ToFixedList();
        }

        public FixedList<T> AcceptSeparatedList<T, TSeparator>(Func<T?> acceptItem)
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

        public FixedList<T> AcceptOneOrMore<T, TSeparator>(Func<T?> acceptItem)
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

        public FixedList<T> ParseMany<T, TSeparator, TTerminator>(Func<T> parseItem)
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
